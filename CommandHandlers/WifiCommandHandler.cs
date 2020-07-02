using System.Collections.Generic;
using iznakurnoz.Bot.Services.RouterService;
using iznakurnoz.Bot.Extensions;
using Iznakurnoz.Bot.Interfaces;
using Telegram.Bot.Types;
using System.Threading.Tasks;

namespace iznakurnoz.Bot.CommandHandlers
{
    /// <summary>
    /// Обработчик команды wifi.
    /// </summary>
    internal class WifiCommandHandler : BaseCommandHandler, IBotCommandHandler
    {
        private const string InvalidParameters = "Неверные параметры";
        private const string InvalidParameterCount = "Неверное число параметров";
        private static IEnumerable<string> _supportedCommands = new[]
        {
            "wifi"
        };

        /// <summary>
        /// Карта соответствия настроек и методов-обработчиков настроек.
        /// </summary>
        private readonly IDictionary<string, OptionHandlerDelegate> _optionHandlers;
        private readonly RouterRequestService _routerRequestService;

        public WifiCommandHandler(
            IBotTelegramClient botTelegramClient,
            RouterRequestService routerRequestService)
            : base(botTelegramClient, _supportedCommands)
        {
            _routerRequestService = routerRequestService;
            _optionHandlers = new Dictionary<string, OptionHandlerDelegate>()
            {
                { "s", StateOptionHandler },
                { "state", StateOptionHandler },
                { "a", AddOptionHandler },
                { "add", AddOptionHandler },
                { "d", DeleteOptionHandler },
                { "del", DeleteOptionHandler },
                { "delete", DeleteOptionHandler },
                { "off", OffOptionHandler },
                { "on", OnOptionHandler }
            };
        }

        async public Task<string> HandleCommand(Message message, string command, IEnumerable<string> arguments)
        {
            var argumentEnumerator = arguments.GetEnumerator();

            if (!argumentEnumerator.MoveNext())
            {
                return await StateOptionHandler(message, null);
            }

            if (_optionHandlers.TryGetValue(argumentEnumerator.Current, out var handler))
            {
                return await handler(message, argumentEnumerator);
            }

            return null;
        }

        async private Task<string> StateOptionHandler(Message message, IEnumerator<string> parameters)
        {
            var result = await _routerRequestService.GetWirelessFilterPage();

            if (string.IsNullOrWhiteSpace(result))
            {
                return "Не удалось получить информацию о состоянии фильтра";
            }

            return result;
        }

        private Task<string> AddOptionHandler(Message message, IEnumerator<string> parameters)
        {
            if (!parameters.MoveNext())
            {
                return GetAsTextResult(InvalidParameterCount);
            }

            var parameter1 = parameters.Current;

            if (!parameters.MoveNext())
            {
                return GetAsTextResult(InvalidParameterCount);
            }

            var parameter2 = parameters.Current;

            var parameter1IsMacAddress = parameter1.TryConvertToMacAddress(out var macAddress1);
            var parameter2IsMacAddress = parameter2.TryConvertToMacAddress(out var macAddress2);

            if (!parameter1IsMacAddress && !parameter2IsMacAddress)
            {
                return GetAsTextResult(InvalidParameters);
            }

            var parameter1IsDeviceName = parameter1.TryConvertToDeviceName(out var deviceName1);
            var parameter2IsDeviceName = parameter2.TryConvertToDeviceName(out var deviceName2);

            string macAddress;
            string deviceName;

            if (parameter1IsMacAddress && parameter2IsDeviceName)
            {
                macAddress = macAddress1;
                deviceName = deviceName2;
            }
            else if (parameter2IsMacAddress && parameter1IsDeviceName)
            {
                macAddress = macAddress2;
                deviceName = deviceName1;
            }
            else
            {
                return GetAsTextResult(InvalidParameterCount);
            }

            return _routerRequestService.AddDeviceToWirelessFilter(macAddress, deviceName);
        }

        private Task<string> DeleteOptionHandler(Message message, IEnumerator<string> parameters)
        {
            if (!parameters.MoveNext())
            {
                return GetAsTextResult(InvalidParameterCount);
            }

            var macAddresses = new List<string>();
            var deviceNames = new List<string>();

            do
            {
                var isParameterMacAddress = parameters.Current.TryConvertToMacAddress(out var macAddress);
                var isParameterDeviceName = parameters.Current.TryConvertToDeviceName(out var deviceName);

                if (!isParameterMacAddress && !isParameterDeviceName)
                {
                    continue;
                }

                if (isParameterMacAddress)
                {
                    macAddresses.Add(macAddress);
                }

                if (isParameterDeviceName)
                {
                    deviceNames.Add(deviceName);
                }

            } while (parameters.MoveNext());

            if ((macAddresses.Count + deviceNames.Count) == 0)
            {
                return GetAsTextResult(InvalidParameters);
            }
            else
            {
                return _routerRequestService.RemoveDeviceFromWirelessFilter(macAddresses, deviceNames);
            }
        }

        private Task<string> OffOptionHandler(Message message, IEnumerator<string> parameters)
        {
            return EnableWifiForDevice(message, parameters, false);
        }

        private Task<string> OnOptionHandler(Message message, IEnumerator<string> parameters)
        {
            return EnableWifiForDevice(message, parameters, true);
        }

        private Task<string> EnableWifiForDevice(Message message, IEnumerator<string> parameters, bool enabled)
        {
            if (!parameters.MoveNext())
            {
                return GetAsTextResult(InvalidParameterCount);
            }
            
            var isParameterMacAddress = parameters.Current.TryConvertToMacAddress(out var macAddress);
            var isParameterDeviceName = parameters.Current.TryConvertToDeviceName(out var deviceName);

            if (!isParameterMacAddress && !isParameterDeviceName)
            {
                return GetAsTextResult(InvalidParameters);
            }

            return _routerRequestService.EnableWiFiByMacAddressOrDeviceName(macAddress, deviceName, enabled);
        }
    }
}