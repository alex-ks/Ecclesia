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

        RmqMessageQueueParams p = new RmqMessageQueueParams
        {
            HostName = "ecclesia.ict.nsc.ru",
            UserName = "ecclesia",
            Password = "Str0ngP@ssw0rd"
        };

        [Fact]
        public void MessageDelivery_PushWithoutDelay_ReceivePushedMessage()
        {
            using (var queue = new RmqMessageQueue<TestMessage>(p))
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

        [Fact]
        public void MessageDelivery_PushWithDelay_ReceivePushedMessageOnlyAfterDelay()
        {
            using (var queue = new RmqMessageQueue<TestMessage>(p))
            {
                var expected = 42;
                var initial = 0;
                int actual = initial;

                queue.MessageReceived += message => 
                {
                    actual = message.Number;
                    return Task.CompletedTask;
                };

                queue.Push(new TestMessage { Number = expected }, TimeSpan.FromMilliseconds(500));
                
                Thread.Sleep(450);
                Assert.Equal(initial, actual);
                Thread.Sleep(200);
                Assert.Equal(expected, actual);
            }
        }
    }
}
