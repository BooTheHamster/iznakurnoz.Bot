namespace iznakurnoz.Bot.Services.TransmissionService.Interfaces
{

    /// <summary>
    /// Информация о торренте.
    /// </summary>
    internal interface ITorrentInformation
    {
        /// <summary>
        /// Id торрента.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Наименование торрента.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Статус торрента.
        /// </summary>
        TorrentStatus Status { get; }

        /// <summary>
        /// Оставшееся время загрузки торрента.
        /// </summary>
        int Eta { get; }

        /// <summary>
        /// Строка ошибки загрузки торрента.
        /// </summary>
        string ErrorString { get; }

        /// <summary>
        /// Процент загрузки торрента.
        /// </summary>
        double PercentDone { get; }

        /// <summary>
        /// Торрент скачан полностью.
        /// </summary>
        bool IsComplete { get; }
    }
}