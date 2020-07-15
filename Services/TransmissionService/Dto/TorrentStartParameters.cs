namespace iznakurnoz.Bot.Services.TransmissionService.Dto
{
    /// <summary>
    /// Параметры запуска загрузки всех торрентов.
    /// </summary>
    internal class TorrentStartAllParameters : MethodCallParameters<TorrentEmptyRequestArguments>
    {
        public TorrentStartAllParameters()
            : base("torrent-start", new TorrentEmptyRequestArguments())
        {
        }
    }

    /// <summary>
    /// Параметры запуска загрузки определенных торрентов.
    /// </summary>
    internal class TorrentStartParameters : MethodCallParameters<TorrentIdsRequestArguments>
    {
        public TorrentStartParameters(int[] torrentIds)
            : base("torrent-start", new TorrentIdsRequestArguments(torrentIds))
        {
        }
    }

}