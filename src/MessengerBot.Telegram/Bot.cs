using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessengerBot.Abstractions;
using MihaZupan;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MessengerBot.Telegram
{
    public class Bot : IMessengerBot
    {
        public event MessageReceivedEventHandler MessageReceived;

        private readonly string _token;

        private readonly HttpClient _client;

        private CancellationTokenSource _cancellationTokenSource;

        public bool IsRunning { get; private set; } = false;

        private readonly ConcurrentDictionary<string, Chat> _chats = new ConcurrentDictionary<string, Chat>();

        public IEnumerable<Chat> Chats
        {
            get { return _chats.Values.Select(c => new Chat { Id = c.Id.ToString() }); }
        }

        private int _offset = 0;

        public Bot(string token)
        {
            _token = token;
            var proxy = new HttpToSocks5Proxy("127.0.0.1", 9150);
            var handler = new HttpClientHandler { Proxy = null /*proxy*/ };
            _client = new HttpClient(handler, true);
            _client.BaseAddress = new Uri($"https://api.telegram.org/bot{token}/");
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private async void GetUpdates(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    using (var response = await _client.GetAsync($"getUpdates?offset={_offset}", token))
                    {
                        var json = JObject.Parse(await response.Content.ReadAsStringAsync());

                        var updates = json["result"].ToObject<List<Update>>();
                        foreach (var update in updates)
                        {
                            if (update.Message != null)
                            {
                                var chat = new Chat { Id = update.Message.Chat.Id.ToString() };
                                _chats.TryAdd(chat.Id, chat);
                                await SendMessageAsync(new Message(chat.Id, $"Hello {chat.Id}"));
                                MessageReceived?.Invoke(new ReceivedMessage { Chat = chat, Text = update.Message.Text });
                            }

                            _offset = update.UpdateId + 1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
        }

        public async Task SendMessageAsync(Message message)
        {
            var content = new StringContent(JsonConvert.SerializeObject(new { chat_id = Convert.ToInt32(message.ChatId), text = message.Text }), Encoding.UTF8, "application/json");
            await _client.PostAsync("sendMessage", content);
        }

        public void Start()
        {
            if (!IsRunning)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                var token = _cancellationTokenSource.Token;
                token.Register(() => IsRunning = false);
                Task.Run(() => GetUpdates(token));
                IsRunning = true;
            }
        }

        public void Stop()
        {
            _cancellationTokenSource?.Cancel();
        }
    }
}