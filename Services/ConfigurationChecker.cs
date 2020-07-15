using System.Collections.Generic;
using System.Text;
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
            var validationErrors = new StringBuilder();

            if (config == null)
            {
                validationErrors.AppendLine("Configuration not found");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(config.AuthToken))
                {
                    validationErrors.AppendLine($"{nameof(config.AuthToken)} not defined.");
                }

                if (config.AdminId <= 0)
                {
                    validationErrors.AppendLine($"{nameof(config.AdminId)} not defined.");
                }


                if (config.ProxySettings == null)
                {
                    validationErrors.AppendLine($"{nameof(config.ProxySettings)} not defined.");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(config.ProxySettings.Address))
                    {
                        validationErrors.AppendLine($"{nameof(config.ProxySettings)}.{nameof(config.ProxySettings.Address)} not defined.");
                    }

                    if (config.ProxySettings.Port <= 0)
                    {
                        validationErrors.AppendLine($"{nameof(config.ProxySettings)}.{nameof(config.ProxySettings.Port)} not defined.");
                    }

                    if (string.IsNullOrWhiteSpace(config.ProxySettings.Username))
                    {
                        validationErrors.AppendLine($"{nameof(config.ProxySettings)}.{nameof(config.ProxySettings.Username)} not defined.");
                    }

                    if (string.IsNullOrWhiteSpace(config.ProxySettings.Password))
                    {
                        validationErrors.AppendLine($"{nameof(config.ProxySettings)}.{nameof(config.ProxySettings.Password)} not defined.");
                    }
                }

                if (config.TorrentServerSettings == null)
                {
                    validationErrors.AppendLine($"{nameof(config.TorrentServerSettings)} not defined.");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(config.TorrentServerSettings.WatchDirectoryPath))
                    {
                        validationErrors.AppendLine($"{nameof(config.TorrentServerSettings.WatchDirectoryPath)} not defined.");
                    }

                    if (string.IsNullOrWhiteSpace(config.TorrentServerSettings.Address))
                    {
                        validationErrors.AppendLine($"{nameof(config.TorrentServerSettings)}.{nameof(config.TorrentServerSettings.Address)} not defined.");
                    }

                    if (config.TorrentServerSettings.Port <= 0)
                    {
                        validationErrors.AppendLine($"{nameof(config.TorrentServerSettings)}.{nameof(config.TorrentServerSettings.Port)} not defined.");
                    }
                }
            }

            if (validationErrors.Length > 0)
            {
                validationErrors.AppendLine("Awaiting configuration ...");
                logger.LogInformation(validationErrors.ToString());

                return false;
            }

            return true;
        }
    }
}