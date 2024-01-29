namespace Iznakurnoz.Bot.Configuration
{
    /// <summary>
    /// Настройки бота.
    /// </summary>
    internal class BotConfig
    {
        /// <summary>
        /// Токен авторизации бота.
        /// </summary>
        public string AuthToken { get; set; }

        /// <summary>
        /// Id администратора от которого можно выполнять команды.
        /// </summary>
        public int AdminId { get; set; }

        /// <summary>
        /// Настройки торрент-сервера.
        /// </summary>
        public TorrentServerSettings TorrentServerSettings { get; set; }

        /// <summary>
        /// Настройки прокси.
        /// </summary>
        public ProxySettings ProxySettings { get; set; }
    }
}