using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Ecclesia.MessageQueue.RabbitMQ
{
    public class RmqMessageQueueParams
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class RmqMessageQueue<TMessage> : IMessageQueue<TMessage>, IDisposable
    {
        private const string DelayedTypeArgKey = "x-delayed-type";
        private const string ExchangeType = "direct";
        private const string DelayedTypeName = "x-delayed-message";
        private const string DelayedHeader = "x-delay";
        private const string ExchangeName = "ecclesia-delayed-exchange";
        private readonly string QueueName = typeof(TMessage).FullName;
        private readonly string RoutingKey = typeof(TMessage).FullName;

        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly AsyncEventingBasicConsumer _consumer;

        public event Func<TMessage, Task> MessageReceived;

        public bool Persistent { get; set; } = true;

        public RmqMessageQueue(RmqMessageQueueParams queueParams) : this(queueParams.HostName,
                                                                         queueParams.UserName,
                                                                         queueParams.Password) 
        {

        }

        public RmqMessageQueue(string hostName, string userName, string password)
        {
            var factory = new ConnectionFactory
            {
                HostName = hostName, 
                UserName = userName,
                Password = password,
                DispatchConsumersAsync = true
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            _channel.BasicQos(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false
            );

            var args = new Dictionary<string, object>()
            {
                { DelayedTypeArgKey, ExchangeType }
            };

            _channel.ExchangeDeclare(
                exchange: ExchangeName, 
                type: DelayedTypeName, 
                durable: true,
                autoDelete: false,
                arguments: args
            );

            _channel.QueueBind(
                queue: QueueName,
                exchange: ExchangeName,
                routingKey: RoutingKey
            );

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.Received += async (model, eventArgs) =>
            {
                var str = Encoding.UTF8.GetString(eventArgs.Body);
                var message = JsonConvert.DeserializeObject<TMessage>(str);
                await MessageReceived?.Invoke(message);
                _channel.BasicAck(
                    deliveryTag: eventArgs.DeliveryTag,
                    multiple: false
                );
            };

            _channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: _consumer
            );
        }

        public void Push(TMessage message)
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = true;
            Push(message, TimeSpan.Zero);
        }

        public void Push(TMessage message, TimeSpan delay)
        {
            var properties = _channel.CreateBasicProperties();
            properties.Persistent = Persistent;

            var headers = new Dictionary<string, object>
            {
                { DelayedHeader, delay.Milliseconds }
            };
            properties.Headers = headers;

            var msg = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(msg);
            _channel.BasicPublish(
                exchange: ExchangeName,
                routingKey: RoutingKey,
                basicProperties: properties,
                body: body
            );
        }

        public void Dispose()
        {
            using (_connection)
            using (_channel)
            {
                _channel.BasicCancel(_consumer.ConsumerTag);
            }
        }
    }
}