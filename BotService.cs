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
        private const int StartWaitDelayInMilliseconds = 10000;
        private readonly ILogger _logger;
        private readonly IOptions<BotConfig> _config;
        private static TelegramBotClient _client;

        public BotService(
            ILogger<BotService> logger,
            IOptionsSnapshot<BotConfig> config)
        {
            _logger = logger;
            _config = config;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting bot.");
            return GetStartTask();

        }

        private Task TryStartBot() 
        {
            if (string.IsNullOrWhiteSpace(_config.Value.AuthToken))
            {
                _logger.LogInformation("AuthToken not defined. Wait configuration ...");
                return GetStartTask();
            }

            try
            {
                _client = new TelegramBotClient(_config.Value.AuthToken);
            }
            catch (Exception error)
            {
                _logger.LogError(null, error, "TelegramBotClient create error");
                return Task.Factory.StartNew(TryStartBot);
            }

            _client.OnMessage += BotOnMessageReceived;
            _client.OnMessageEdited += BotOnMessageReceived;
            _client.StartReceiving();

            return Task.CompletedTask;
        }

        private Task GetStartTask()
        {
            return Task.Delay(StartWaitDelayInMilliseconds).ContinueWith(task => TryStartBot());
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