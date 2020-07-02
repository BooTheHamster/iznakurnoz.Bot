using System.Linq;
using System.Text.Json.Serialization;

namespace iznakurnoz.Bot.Services.TransmissionService.Dto
{
    /// <summary>
    /// Ответ торрент сервера на запрос информации о торрентах.
    /// </summary>
    internal class TorrentGetResponse : MethodCallResponse<TorrentGetResponseArguments>
    {
    }

    /// <summary>
    /// Данные ответа торрент сервера на запрос информации о торрентах.
    /// </summary>
    internal class TorrentGetResponseArguments
    {
        [JsonPropertyName("torrents")]
        public TorrentInformation[] Torrents { get; set; }
    }

    /// <summary>
    /// Параметры запроса информации о торрентах.
    /// </summary>
    internal class TorrentGetParameters : MethodCallParameters<TorrentGetArguments>
    {
        public TorrentGetParameters()
            : base("torrent-get", new TorrentGetArguments())
        {
        }
    }

    /// <summary>
    /// Данные запроса информации о торрентах.
    /// </summary>
    internal class TorrentGetArguments
    {
        public string[] Fields { get; private set; }

        public TorrentGetArguments()
        {
            Fields = typeof(TorrentInformation).GetProperties()
                .Select(p =>
                    {
                        var name = p.Name;
                        name = name.Substring(0, 1).ToLower() + name.Substring(1);
                        return name;
                    })
                .ToArray();
        }
    }
}