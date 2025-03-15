namespace MessengerBot.Abstractions
{
    public class Message
    {
        public string ChatId { get; set; }

        public string Text { get; set; }

        public Message(string chatId, string text)
        {
            ChatId = chatId;
            Text = text;
        }
    }
}