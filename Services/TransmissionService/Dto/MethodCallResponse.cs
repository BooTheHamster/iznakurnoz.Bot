using System.Text.Json.Serialization;

namespace iznakurnoz.Bot.Services.TransmissionService.Dto
{
    /// <summary>
    /// Ответ торрент сервера на запрос.
    /// </summary>
    /// <typeparam name="TArguments">Тип данных ответа.</typeparam>
    internal class MethodCallResponse<TArguments>
    {
        [JsonPropertyName("result")]
        public string Result { get; set; }

        [JsonPropertyName("arguments")]
        public TArguments Arguments { get; set; }
    }
}