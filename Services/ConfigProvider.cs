using System;
using Iznakurnoz.Bot.Configuration;
using Iznakurnoz.Bot.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace iznakurnoz.Bot.CommandHandlers
{
    /// <summary>
    /// Провайдер текущей конфигурации бота.
    /// </summary>
    internal class ConfigProvider : IConfigProvider
    {
        private BotConfig _config;
        private readonly ILogger<ConfigProvider> _logger;

        public event EventHandler<BotConfig> Changed;

        public ConfigProvider(
            ILogger<ConfigProvider> logger,
            IOptionsMonitor<BotConfig> configMonitor)
        {
            _logger = logger;
            _config = configMonitor.CurrentValue;
            configMonitor.OnChange(OnOptionChanged);
        }
        public BotConfig CurrentConfig => _config;

        private void OnOptionChanged(BotConfig config, string name)
        {            
            _logger.LogInformation($"Bot configuration reloaded");
            _config = config;

            var handler = Changed;
            handler?.Invoke(this, _config);
        }
    }
}