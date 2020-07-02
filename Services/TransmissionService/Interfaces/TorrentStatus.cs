namespace iznakurnoz.Bot.Services.TransmissionService.Interfaces
{
    /// <summary>
    /// Статус торрента.
    /// </summary>
    internal enum TorrentStatus
    {
        /// <summary>
        /// Неизвестный код статуса.
        /// </summary>
        UnknownStatusCode = -1,

        /// <summary>
        /// Остановлен.
        /// </summary>
        Stopped = 0,

        /// <summary>
        /// Ожидание проверки целостности файлов.
        /// </summary>
        CheckFilesWait = 1,

        /// <summary>
        /// Проверка целостности файлов.
        /// </summary>
        CheckFiles = 2,

        /// <summary>
        /// Ожидание загрузки.
        /// </summary>
        DownloadWait  = 3,

        /// <summary>
        /// Загрузка.
        /// </summary>
        Downloading = 4,

        /// <summary>
        /// Ожидание раздачи.
        /// </summary>
        SeedWait = 5,

        /// <summary>
        /// Раздача.
        /// </summary>
        Seed = 6
    }
}