namespace Iznakurnoz.Bot.Configuration
{
    /// <summary>
    /// Настройки бота.
    /// </summary>
    internal class BotConfig
    {
        public BotConfig()
        {
            AuthToken = null;
        }

        /// <summary>
        /// Токен авторизации бота.
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// Настройки прокси.
        /// </summary>
        public ProxySettings ProxySettings { get; set; }
    }
}