using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Iznakurnoz.Bot.Interfaces;
using Microsoft.Extensions.Logging;

namespace iznakurnoz.Bot.Services.KeeneticRouterService
{
    /// <summary>
    /// Сервис для взаимодействия с роутером Keenetic.
    /// </summary>
    internal class KeeneticRouterRequestService
    {
        /// <summary>
        /// Url сервиса авторизации роутера.
        /// </summary>
        private const string AuthPageUri = "auth";

        private const string OkMessage = "Ok";

        private readonly IConfigProvider _configProvider;
        private readonly HttpClient _client;
        private readonly ILogger<KeeneticRouterRequestService> _logger;

        public KeeneticRouterRequestService(
            ILogger<KeeneticRouterRequestService> logger,
            IConfigProvider configProvider)
        {
            _logger = logger;
            _configProvider = configProvider;
            _client = new HttpClient();
        }
        
        async public Task<string> GetWirelessStatus()
        {
            var loginResult = await Login();

            if (loginResult)
            {
                return OkMessage;
            }

            return string.Empty;
        }

        async private Task<bool> Login()
        {
            var page = await Request(AuthPageUri);
            return false;
        }

        async private Task<string> Request(
            string relativeUri,
            string httpId = null,
            NameValueCollection queryParameters = null)
        {
            try
            {
                var uri = GetUri(relativeUri);

                if (uri != null)
                {
                    var query = HttpUtility.ParseQueryString(string.Empty);

                    if (!string.IsNullOrWhiteSpace(httpId))
                    {
                        query.Add("_http_id", httpId);
                    }

                    if (queryParameters != null)
                    {
                        query.Add(queryParameters);
                    }

                    var builder = new UriBuilder(uri);
                    builder.Query = query.ToString();

                    _client.DefaultRequestHeaders.Clear();
                    _client.DefaultRequestHeaders.ConnectionClose = true;

                    var authString = $"{_configProvider.CurrentConfig.RouterSettings.Username}:{_configProvider.CurrentConfig.RouterSettings.Password}";
                    authString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authString));

                    var authHeader = new AuthenticationHeaderValue("Basic", authString);
                    _client.DefaultRequestHeaders.Authorization = authHeader;

                    return await _client.GetStringAsync(builder.ToString());
                }
            }
            catch (Exception error)
            {
                _logger.LogError(error, $"Router request error");
            }

            return null;
        }

        private Uri GetUri(string relativeUri)
        {
            var baseUriString = $"http://{_configProvider.CurrentConfig.RouterSettings.Address}";

            if (!Uri.TryCreate(baseUriString, UriKind.Absolute, out var baseUri))
            {
                return null;
            }

            if (!Uri.TryCreate(baseUri, relativeUri, out var fullUri))
            {
                return null;
            }

            return fullUri;
        }
    }
}