using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Iznakurnoz.Bot
{
    internal class BotService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IOptions<BotConfig> _config;

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
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping bot.");
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing ...");
        }
    }
}