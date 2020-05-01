using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using iznakurnoz.Bot.Extensions;

namespace iznakurnoz.Bot.Services.RouterService
{
    internal partial class RouterRequestService
    {
        /// <summary>
        /// Параметры WiFi фильтра в роутере.
        /// </summary>
        internal class WirelessFilterParameters
        {
            private static readonly Regex _wlMacListRegex = new Regex("'wl_maclist':\\s*'(.*)'", RegexOptions.IgnoreCase);
            private static readonly Regex _wlMacNamesRegex = new Regex("'macnames':\\s*'(.*)'", RegexOptions.IgnoreCase);
            private static readonly Regex _wlMacModeRegex = new Regex("'wl_macmode':\\s*'(.*)'", RegexOptions.IgnoreCase);
            private IDictionary<string, string> _deviceMacToNameMap = new Dictionary<string, string>();

            /// <summary>
            /// Режим работы фильтра.
            /// </summary>
            public string Mode { get; private set; }

            /// <summary>
            /// Список устройств.
            /// </summary>
            public IEnumerable<WirelessFilterDevice> Devices => _deviceMacToNameMap.Select(pair => new WirelessFilterDevice(pair.Value, pair.Key));

            public WirelessFilterParameters(string parameterResponse)
            {
                ParseMode(parameterResponse);
                ParseDevices(parameterResponse);
            }

            private void ParseDevices(string parameterResponse)
            {
                var match = _wlMacListRegex.Match(parameterResponse);

                if (match.Success)
                {
                    var macAddresses = match.Groups[1].Value
                        .Split(" ", StringSplitOptions.RemoveEmptyEntries);

                    foreach (var macAddress in macAddresses)
                    {
                        _deviceMacToNameMap[macAddress] = null;
                    }
                }

                match = _wlMacNamesRegex.Match(parameterResponse);

                if (match.Success)
                {
                    var macNames = match.Groups[1].Value
                        .Split(">", StringSplitOptions.RemoveEmptyEntries)
                        .Select(s =>
                            {
                                var macAndName = s.Split("<");

                                if (StringExtensions.TryConvertToMacAddress(macAndName[0], out var macAddress))
                                {
                                    return new { mac = macAddress, name = macAndName[1].Trim() };
                                }

                                return null;

                            })
                        .Where(t => t != null)
                        .ToArray();

                    foreach (var macName in macNames)
                    {
                        _deviceMacToNameMap[macName.mac] = macName.name;
                    }
                }
            }

            internal void AddDevice(string macAddress, string deviceName)
            {
                _deviceMacToNameMap[macAddress] = deviceName;
            }

            private void ParseMode(string parameterResponse)
            {
                Mode = null;

                var match = _wlMacModeRegex.Match(parameterResponse);
                if (match.Success)
                {
                    Mode = match.Groups[1].Value.ToLower();
                }
            }
        }
    }
}