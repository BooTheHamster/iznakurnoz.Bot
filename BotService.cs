using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace Iznakurnoz.Bot
{
    internal class BotService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IOptions<BotConfig> _config;
        private static TelegramBotClient _client;

        public BotService(
            ILogger<BotService> logger,
            IOptions<BotConfig> config)
        {
            _logger = logger;
            _config = config;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting bot.");

            _client = new TelegramBotClient(_config.Value.AuthToken);
            _client.OnMessage += BotOnMessageReceived;
            _client.OnMessageEdited += BotOnMessageReceived;
            _client.StartReceiving();

            return Task.CompletedTask;
        }

        private void BotOnMessageReceived(object sender, MessageEventArgs e)
        {

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping bot.");
            _client.StopReceiving();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _client = null;
            _logger.LogInformation("Disposing ...");
        }
    }
}