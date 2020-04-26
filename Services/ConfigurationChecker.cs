using System.Collections.Generic;
using Iznakurnoz.Bot.Configuration;
using Microsoft.Extensions.Logging;

namespace iznakurnoz.Bot.Services
{
    /// <summary>
    /// Сервис проверки конфигурации на корректность.
    /// </summary>
    internal static class ConfigurationChecker
    {
        /// <summary>
        /// Проверка конфигурации на корректность.
        /// </summary>
        /// <param name="config">Конфигурация.</param>
        /// <param name="logger">Логгер.</param>
        /// <returns>Истина, если конфигурация корректна.</returns>
        public static bool CheckConfig(BotConfig config, ILogger logger)
        {
            var validationErrors = new List<string>();

            if (config == null)
            {
                validationErrors.Add("Configuration not found");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(config.AuthToken))
                {
                    validationErrors.Add($"{nameof(config.AuthToken)} not defined.");
                }

                if (config.AdminId <= 0)
                {
                    validationErrors.Add($"{nameof(config.AdminId)} not defined.");
                }


                if (config.ProxySettings == null)
                {
                    validationErrors.Add($"{nameof(config.ProxySettings)} not defined.");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(config.ProxySettings.Address))
                    {
                        validationErrors.Add($"{nameof(config.ProxySettings.Address)} not defined.");
                    }

                    if (config.ProxySettings.Port <= 0)
                    {
                        validationErrors.Add($"{nameof(config.ProxySettings.Port)} not defined.");
                    }

                    if (string.IsNullOrWhiteSpace(config.ProxySettings.Username))
                    {
                        validationErrors.Add($"{nameof(config.ProxySettings.Username)} not defined.");
                    }

                    if (string.IsNullOrWhiteSpace(config.ProxySettings.Password))
                    {
                        validationErrors.Add($"{nameof(config.ProxySettings.Password)} not defined.");
                    }
                }

                if (config.TorrentServerSettings == null)
                {
                    validationErrors.Add($"{nameof(config.TorrentServerSettings)} not defined.");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(config.TorrentServerSettings.WatchDirectoryPath))
                    {
                        validationErrors.Add($"{nameof(config.TorrentServerSettings.WatchDirectoryPath)} not defined.");
                    }
                }
            }

            if (validationErrors.Count > 0)
            {
                validationErrors.Add("Awaiting configuration ...");

                foreach (var line in validationErrors)
                {
                    logger.LogInformation(line);
                }

                return false;
            }

            return true;
        }
    }
}