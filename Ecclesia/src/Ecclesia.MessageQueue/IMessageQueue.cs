using System;
using System.Threading.Tasks;

namespace Ecclesia.MessageQueue
{
    public interface IMessageQueue<TMessage>
    {
        void Push(TMessage message);
        void Push(TMessage message, TimeSpan delay);
        event Func<TMessage, Task> MessageReceived;
    }
}