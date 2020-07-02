namespace Iznakurnoz.Bot.Configuration
{
    internal class TorrentServerSettings
    {
        /// <summary>
        /// Полный путь к каталогу.
        /// </summary>
        public string WatchDirectoryPath { get; set; }

        /// <summary>
        /// Адрес Transmission сервера.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Порт Transmission сервера.
        /// </summary>
        public int Port { get; set; }
    }
}