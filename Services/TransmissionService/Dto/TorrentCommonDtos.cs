using System.Text.Json.Serialization;

namespace iznakurnoz.Bot.Services.TransmissionService.Dto
{
    /// <summary>
    /// Ответ торрент сервера на запросы которые не предполагают никакой возвращаемой информации.
    /// </summary>
    internal class TorrentEmptyResponse : MethodCallResponse<TorrentEmptyResponseArguments>
    {
    }

    /// <summary>
    /// Данные ответа торрент сервера на запросы которые не предполагают никакой возвращаемой информации.
    /// </summary>
    internal class TorrentEmptyResponseArguments
    {
    }

    /// <summary>
    /// Параметры запроса содержащие Id торрентов.
    /// </summary>
    internal class TorrentIdsRequestArguments
    {
        [JsonPropertyName("ids")]
        public int[] TorrentIds { get; }

        public TorrentIdsRequestArguments(int[] torrentIds)
        {
            TorrentIds = torrentIds;

        }
    }

    /// <summary>
    /// Пустые параметры запроса.
    /// </summary>
    internal class TorrentEmptyRequestArguments
    {
    }
}