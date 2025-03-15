using Newtonsoft.Json;

namespace MessengerBot.Telegram
{
    class UpdateMessageChat
    {
        public int Id { get; set; }
    }

    class UpdateMessage
    {
        public UpdateMessageChat Chat { get; set; }
        public string Text { get; set; }
    }

    class Update
    {
        [JsonProperty("update_id")]
        public int UpdateId { get; set; }

        public UpdateMessage Message { get; set; }
    }
}