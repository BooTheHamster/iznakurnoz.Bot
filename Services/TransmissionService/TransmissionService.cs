using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using iznakurnoz.Bot.Services.TransmissionService.Dto;
using iznakurnoz.Bot.Services.TransmissionService.Interfaces;
using Iznakurnoz.Bot.Interfaces;
using Microsoft.Extensions.Logging;

namespace iznakurnoz.Bot.Services.TransmissionService
{

    /// <summary>
    /// Служба для работы с торрент-сервером Transmission.
    /// </summary>
    internal partial class TransmissionService
    {
        private const string OkMessage = "Ok";
        private const string StartTorrentsErrorMessage = "Ошибка при запуске закачки торрентов";
        private const string TransmissionRpcUrl = "transmission/rpc";
        private const string HeaderXTransmissionSessionId = "X-Transmission-Session-Id";
        private readonly ILogger<TransmissionService> _logger;
        private readonly IConfigProvider _configProvider;
        private readonly HttpClient _client;
        private JsonSerializerOptions _serializeOptions;

        /// <summary>
        /// Идентификатор сессии для запросов к серверу.
        /// </summary>
        private string _sessionId;

        public TransmissionService(
            ILogger<TransmissionService> logger,
            IConfigProvider configProvider)
        {
            _logger = logger;
            _configProvider = configProvider;
            _client = new HttpClient();
            _serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task<ITorrentInformation[]> GetTorrentsList()
        {
            try
            {
                var parameters = new TorrentGetParameters();
                var result = await CallMethod<TorrentGetParameters, TorrentGetResponse>(parameters);
                return result.Arguments.Torrents.Select(t => t as ITorrentInformation).ToArray();
            }
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return new ITorrentInformation[0];
            }
        }

        public async Task<string> StartTorrents(int[] torrentIds)
        {
            try
            {
                if (torrentIds == null || torrentIds.Length == 0)
                {
                    var startAllResponse = await CallMethod<TorrentStartAllParameters, TorrentEmptyResponse>(new TorrentStartAllParameters());
                    return startAllResponse.GetOkOrResult();
                }

                var startResponse = await CallMethod<TorrentStartParameters, TorrentEmptyResponse>(new TorrentStartParameters(torrentIds));
                return startResponse.GetOkOrResult();

            }                
            catch (Exception error)
            {
                _logger.LogError(error, error.Message);
                return StartTorrentsErrorMessage;
            }
        }

        private Task<HttpResponseMessage> Post<TMethodCallParams>(TMethodCallParams parameters)
        {
            try
            {
                _client.DefaultRequestHeaders.Clear();
                _client.DefaultRequestHeaders.ConnectionClose = true;

                var serializedParameters = JsonSerializer.Serialize<TMethodCallParams>(parameters, _serializeOptions);
                var content = new StringContent(serializedParameters, Encoding.UTF8, "application/json");

                if (!string.IsNullOrWhiteSpace(_sessionId))
                {
                    content.Headers.Add(HeaderXTransmissionSessionId, new string[] { _sessionId });
                }

                return _client.PostAsync(GetUri(), content);
            }
            catch (Exception error)
            {
                _logger.LogError(error, $"Transmission session id request error");
                throw;
            }
        }

        private async Task<TResponse> CallMethod<TMethodCallParams, TResponse>(TMethodCallParams parameters)
        {
            var result = await Post(parameters);

            // Если сервер вернул 409, то нужно обновить идентификатор сессии и вызывать метод повторно.
            if (result.StatusCode == HttpStatusCode.Conflict)
            {
                if (!result.Headers.TryGetValues(HeaderXTransmissionSessionId, out var values) || !values.Any())
                {
                    _sessionId = null;
                    throw new TransmissionServiceException("Transmission not return session id in headers");
                }

                _sessionId = values.First();
                result = await Post(parameters);
            }

            if (result.StatusCode != HttpStatusCode.OK)
            {
                throw new TransmissionServiceException($"Transmission post error {result.StatusCode}: {result.Content?.ToString()}");
            }

            var content = await result.Content.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<TResponse>(content);
            return response;
        }

        private Uri GetUri()
        {
            var baseUriString = $"http://{_configProvider.CurrentConfig.TorrentServerSettings.Address}:{_configProvider.CurrentConfig.TorrentServerSettings.Port}";

            if (!Uri.TryCreate(baseUriString, UriKind.Absolute, out var baseUri))
            {
                return null;
            }

            if (!Uri.TryCreate(baseUri, TransmissionRpcUrl, out var fullUri))
            {
                return null;
            }

            return fullUri;
        }
   }
}