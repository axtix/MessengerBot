using System.Collections.Generic;
using System.Threading.Tasks;

namespace MessengerBot.Abstractions
{
    public delegate void MessageReceivedEventHandler(ReceivedMessage message);

    public interface IMessengerBot
    {
        event MessageReceivedEventHandler MessageReceived;
        IEnumerable<Chat> Chats { get; }
        void Start();
        void Stop();
        Task SendMessageAsync(Message message);
    }
}
