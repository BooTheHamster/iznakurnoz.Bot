using System;
using System.Text.Json.Serialization;
using iznakurnoz.Bot.Services.TransmissionService.Interfaces;

namespace iznakurnoz.Bot.Services.TransmissionService
{
    /// <summary>
    /// Информация о торрентах.
    /// </summary>
    internal class TorrentInformation : ITorrentInformation
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("status")]
        public int StatusCode { get; set; }

        public TorrentStatus Status
        {
            get
            {
                if (Enum.IsDefined(typeof(TorrentStatus), StatusCode))
                {
                    return (TorrentStatus) StatusCode;
                }

                return TorrentStatus.UnknownStatusCode;
            }
        }

        public bool IsComplete
        {
            get
            {
                return Math.Abs(1 - PercentDone) < double.Epsilon;
            }
        }

        [JsonPropertyName("eta")]
        public int Eta { get; set; }

        [JsonPropertyName("errorString")]
        public string ErrorString { get; set; }

        [JsonPropertyName("percentDone")]
        public double PercentDone { get; set; }
    }
}