using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Iznakurnoz.Bot.Interfaces;
using Microsoft.Extensions.Logging;

namespace iznakurnoz.Bot.CommandHandlers
{
    /// <summary>
    /// Сервис для запросов к роутеру.
    /// </summary>
    internal class RouterRequestService
    {
        private static readonly Regex _httpIdRegex = new Regex("'http_id':\\s*'(.*)'", RegexOptions.IgnoreCase);
        private static readonly Regex _wlMacnamesRegex = new Regex("'macnames':\\s*'(.*)'", RegexOptions.IgnoreCase);
        private static readonly Regex _wlMacModeRegex = new Regex("'wl_macmode':\\s*'(.*)'", RegexOptions.IgnoreCase);
        private static readonly IDictionary<string, string> _filterStateDescription = new Dictionary<string, string>()
        {
            { "disabled", "отключен" },
            { "allow", "заблокировано для всех, кроме списка разрешенных устройств" },
            { "deny", "разрешено для всех, кроме списка заблокированных устройств" },
        };
        private const string WirelessFilterPageUri = "#basic-wfilter.asp";
        private const string StatusPageUri = "#status-home.asp";
        private const string TomatoCgi = "tomato.cgi";
        private const string UpdateCgi = "update.cgi";
        private readonly IConfigProvider _configProvider;
        private readonly HttpClient _client;
        private readonly ILogger<RouterRequestService> _logger;

        public RouterRequestService(
            ILogger<RouterRequestService> logger,
            IConfigProvider configProvider)
        {
            _logger = logger;
            _configProvider = configProvider;
            _client = new HttpClient();
        }

        async public Task<string> GetWirelessFilterPage()
        {
            var httpId = await Login();
            var page = await Request(
                UpdateCgi,
                httpId,
                new NameValueCollection()
                {
                    { "exec", "nvram" },
                    { "arg0", "wl_macmode,wl_maclist,macnames" },
                });
            var builder = new StringBuilder();
            var match = _wlMacModeRegex.Match(page);
            var filterStateMessage = "неизвестно";

            if (match.Success)
            {
                var filterState = match.Groups[1].Value;
                if (_filterStateDescription.TryGetValue(filterState, out var description))
                {
                    filterStateMessage = description;
                }
            } 

            builder
                .AppendLine($"<b>Состояние фильтра WiFi:</b> {filterStateMessage}")
                .AppendLine();

            match = _wlMacnamesRegex.Match(page);


            if (match.Success)
            {
                builder.AppendLine($"<b>Список устройств:</b>");

                var filterDeviceMacAndNames = match.Groups[1].Value
                    .Split(">", StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in filterDeviceMacAndNames)
                {
                    var macNames = item.Split("<");

                    if (macNames.Length == 2)
                    {
                        builder.AppendLine($"{macNames[1]}");
                    }
                }
            }

            return builder.ToString();
        }

        async public Task<string> Login()
        {
            var page = await Request(StatusPageUri);
            var match = _httpIdRegex.Match(page);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
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