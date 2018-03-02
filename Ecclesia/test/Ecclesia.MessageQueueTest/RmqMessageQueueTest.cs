using System;
using System.Threading;
using System.Threading.Tasks;
using Ecclesia.MessageQueue.RabbitMQ;
using Xunit;

namespace Ecclesia.MessageQueueTest
{
    public class RmqMessageQueueTest
    {
        struct TestMessage
        {
            public int Number { get; set; }
        }

        [Fact]
        public void MessageDelivery_PushWithoutDelay_ReceivePushedMessage()
        {
            using (var queue = new RmqMessageQueue<TestMessage>("ecclesia.ict.nsc.ru", "ecclesia", "Str0ngP@ssw0rd"))
            {
                var expected = 42;
                int actual = 0;

                queue.MessageReceived += message => 
                {
                    actual = message.Number;
                    return Task.CompletedTask;
                };

                queue.Push(new TestMessage { Number = expected });
                
                Thread.Sleep(100);

                Assert.Equal(expected, actual);
            }
        }
    }
}
