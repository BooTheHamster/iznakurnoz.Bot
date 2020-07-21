namespace iznakurnoz.Bot.Services.TransmissionService.Dto
{
    /// <summary>
    /// Параметры остановки загрузки всех торрентов.
    /// </summary>
    internal class TorrentStopAllParameters : MethodCallParameters<TorrentEmptyRequestArguments>
    {
        public TorrentStopAllParameters()
            : base("torrent-stop", new TorrentEmptyRequestArguments())
        {
        }
    }

    /// <summary>
    /// Параметры остановки загрузки определенных торрентов.
    /// </summary>
    internal class TorrentStopParameters : MethodCallParameters<TorrentIdsRequestArguments>
    {
        public TorrentStopParameters(int[] torrentIds)
            : base("torrent-stop", new TorrentIdsRequestArguments(torrentIds))
        {
        }
    }
}