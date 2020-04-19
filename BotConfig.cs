namespace Iznakurnoz.Bot
{
    internal class ProxySettings
    {
        /// <summary>
        /// Адрес прокси.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Порт прокси.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Имя пользователя прокси.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Пароль пользователя прокси.
        /// </summary>
        public string Password { get; set; }
    }

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