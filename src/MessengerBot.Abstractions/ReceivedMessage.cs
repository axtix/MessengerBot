namespace MessengerBot.Abstractions
{
    public class ReceivedMessage
    {
        public Chat Chat { get; set; }

        public string Text { get; set; }
    }
}