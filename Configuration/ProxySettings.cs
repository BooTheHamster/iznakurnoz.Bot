namespace Iznakurnoz.Bot.Configuration
{
    /// <summary>
    /// Настройки прокси для подключения бота.
    /// </summary>
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
}