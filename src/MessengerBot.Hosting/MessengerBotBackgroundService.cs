using MessengerBot.Abstractions;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MessengerBot.Hosting
{
    public class MessengerBotBackgroundService : IHostedService
    {
        private readonly IMessengerBot _messengerBot;

        public MessengerBotBackgroundService(
            IMessengerBot messengerBot
        )
        {
            if (messengerBot is null)
            {
                throw new ArgumentNullException(nameof(messengerBot));
            }

            _messengerBot = messengerBot;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _messengerBot.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            _messengerBot.Stop();
        }
    }
}
