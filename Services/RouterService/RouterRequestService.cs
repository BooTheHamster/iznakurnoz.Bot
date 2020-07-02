using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Iznakurnoz.Bot.Interfaces;
using Microsoft.Extensions.Logging;

namespace iznakurnoz.Bot.Services.RouterService
{
    /// <summary>
    /// Сервис для запросов к роутеру.
    /// </summary>
    internal partial class RouterRequestService
    {
        private const string NoDevice = "Нет устройства с подходящими параметрами.";
        private const string InvalidParameters = "Неверные параметры";
        private static readonly Regex _httpIdRegex = new Regex("'http_id':\\s*'(.*)'", RegexOptions.IgnoreCase);
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
        private const string ShellCgi = "shell.cgi";
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

        async public Task<string> RemoveDeviceFromWirelessFilter(IEnumerable<string> macAddresses, IEnumerable<string> deviceNames)
        {   
            var builder = new StringBuilder();        
            var parameters = await GetWirelessFilterParameters();
            var count = 0;
            var needUpdateCount = 0;

            foreach (var macAddress in macAddresses)
            {
                count++;

                if (!parameters.RemoveByMacAddress(macAddress))
                {
                    builder.AppendLine($"Не найдено устройство с MAC адресом {macAddress}");
                    continue;
                }

                needUpdateCount++;
            }

            foreach (var deviceName in deviceNames)
            {
                count++;

                if (!parameters.RemoveByDeviceName(deviceName))
                {
                    builder.AppendLine($"Не найдено устройство {deviceName}");
                    continue;
                };

                needUpdateCount++;
            }

            if (needUpdateCount == 0)
            {
                if (count == 0)
                {
                    return "Не найдено подходящих устройств для удаления";
                }

                return builder.ToString();
            }

            return await UploadWirelessFilterParameters(parameters);
        }

        async public Task<string> AddDeviceToWirelessFilter(string macAddress, string deviceName)
        {
            var parameters = await GetWirelessFilterParameters();

            parameters.AddDevice(macAddress, deviceName);

            return await UploadWirelessFilterParameters(parameters);
        }

        async public Task<string> GetWirelessFilterPage()
        {
            var parameters = await GetWirelessFilterParameters();
            var builder = new StringBuilder();
            var filterStateMessage = "неизвестно";

            if (_filterStateDescription.TryGetValue(parameters.Mode, out var description))
            {
                filterStateMessage = description;
            }

            builder
                .AppendLine($"<b>Состояние фильтра WiFi:</b> {filterStateMessage}")
                .AppendLine()
                .AppendLine($"<b>Список устройств:</b>");

            foreach (var name in parameters.Devices.Select(d => d.Name))
            {
                    builder.AppendLine(name);
            }

            return builder.ToString();
        }

        async public Task<string> EnableWiFiByMacAddressOrDeviceName(string macAddress, string deviceName, bool enabled)
        {
            if (string.IsNullOrWhiteSpace(macAddress))
            {
                var parameters = await GetWirelessFilterParameters();
                var device = parameters.Devices.Where(d => d.Name == deviceName).FirstOrDefault();

                if (device != null)
                {
                    macAddress = device.MacAddress;
                }
            }

            if (string.IsNullOrWhiteSpace(macAddress))
            {
                return NoDevice;
            }

            var iptablesOperation = enabled
                ? "D"
                : "I";
            var command = $"iptables -{iptablesOperation} FORWARD -m mac --mac-source {macAddress} -j DROP";

            var httpId = await Login();
            var response = await Request(
                ShellCgi,
                httpId,
                new NameValueCollection()
                {
                    { "command", command }
                });

            return "Ok";
        }

        async private Task<string> UploadWirelessFilterParameters(WirelessFilterParameters parameters)
        {
            var wlMacList = string.Empty;
            var wlMacNames = string.Empty;

            foreach (var device in parameters.Devices)
            {
                if (wlMacList.Length > 0)
                {
                    wlMacList += " ";
                }

                wlMacList += device.MacAddress;

                if (wlMacNames.Length > 0)
                {
                    wlMacNames += ">";
                }

                wlMacNames += $"{device.MacAddress.Replace(":", string.Empty)}<{device.Name}";
            }

            var httpId = await Login();
            var response = await Request(
                TomatoCgi,
                httpId,
                new NameValueCollection()
                {
                    { "_ajax", "1" },
                    { "_service", "*" },
                    { "wl_macmode", parameters.Mode },
                    { "wl_maclist", wlMacList },
                    { "macnames", wlMacNames },
                });

            return response;
        }

        async private Task<WirelessFilterParameters> GetWirelessFilterParameters()
        {
            var httpId = await Login();
            var response = await Request(
                UpdateCgi,
                httpId,
                new NameValueCollection()
                {
                    { "exec", "nvram" },
                    { "arg0", "wl_macmode,wl_maclist,macnames" },
                });
            
            return new WirelessFilterParameters(response);
        }

        async private Task<string> Login()
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