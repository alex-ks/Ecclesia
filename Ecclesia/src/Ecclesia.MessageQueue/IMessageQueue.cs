using System;

namespace Ecclesia.MessageQueue
{
    public interface IMessageQueue<TMessage>
    {
        void Push(TMessage message);
        void Push(TMessage message, TimeSpan delay);
        event Action<TMessage> MessageReceived;
    }
}