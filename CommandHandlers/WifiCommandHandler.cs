using System;
using System.Collections.Generic;
using iznakurnoz.Bot.Services.RouterService;
using iznakurnoz.Bot.Extensions;
using Iznakurnoz.Bot.Interfaces;
using Telegram.Bot.Types;

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
        private readonly IDictionary<string, Action<Message, IEnumerator<string>>> _optionHandlers;
        private readonly RouterRequestService _routerRequestService;

        public WifiCommandHandler(
            IBotTelegramClient botTelegramClient,
            RouterRequestService routerRequestService)
            : base(botTelegramClient)
        {
            _routerRequestService = routerRequestService;
            _optionHandlers = new Dictionary<string, Action<Message, IEnumerator<string>>>()
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

        public IEnumerable<string> SupportedCommands => _supportedCommands;

        public void HandleCommand(Message message, string command, IReadOnlyCollection<string> arguments)
        {
            var argumentEnumerator = arguments.GetEnumerator();

            if (!argumentEnumerator.MoveNext())
            {
                StateOptionHandler(message, null);
                return;
            }

            if (_optionHandlers.TryGetValue(argumentEnumerator.Current, out var handler))
            {
                handler(message, argumentEnumerator);
            }
        }

        async private void StateOptionHandler(Message message, IEnumerator<string> parameters)
        {
            var result = await _routerRequestService.GetWirelessFilterPage();

            if (string.IsNullOrWhiteSpace(result))
            {
                result = "Не удалось получить информацию о состоянии фильтра.";
            }

            BotClient.SendTextMessage(message.Chat, result);
        }

        async private void AddOptionHandler(Message message, IEnumerator<string> parameters)
        {
            if (!parameters.MoveNext())
            {
                BotClient.SendTextMessage(message.Chat, InvalidParameterCount);
                return;
            }

            var parameter1 = parameters.Current;

            if (!parameters.MoveNext())
            {
                BotClient.SendTextMessage(message.Chat, InvalidParameterCount);
                return;
            }

            var parameter2 = parameters.Current;

            var parameter1IsMacAddress = parameter1.TryConvertToMacAddress(out var macAddress1);
            var parameter2IsMacAddress = parameter2.TryConvertToMacAddress(out var macAddress2);

            if (!parameter1IsMacAddress && !parameter2IsMacAddress)
            {
                BotClient.SendTextMessage(message.Chat, InvalidParameters);
                return;
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
                BotClient.SendTextMessage(message.Chat, InvalidParameterCount);
                return;
            }

            var resultMessage = await _routerRequestService.AddDeviceToWirelessFilter(macAddress, deviceName);
            BotClient.SendTextMessage(message.Chat, resultMessage);
        }

        async private void DeleteOptionHandler(Message message, IEnumerator<string> parameters)
        {
            if (!parameters.MoveNext())
            {
                BotClient.SendTextMessage(message.Chat, InvalidParameterCount);
                return;
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

            string resultMessage;
            if (macAddresses.Count + deviceNames.Count == 0)
            {
                resultMessage = InvalidParameters;
            }
            else
            {
                resultMessage = await _routerRequestService.RemoveDeviceFromWirelessFilter(macAddresses, deviceNames);
            }

            BotClient.SendTextMessage(message.Chat, resultMessage);
        }

        private void OffOptionHandler(Message message, IEnumerator<string> parameters)
        {
            EnableWifiForDevice(message, parameters, false);
        }

        private void OnOptionHandler(Message message, IEnumerator<string> parameters)
        {
            EnableWifiForDevice(message, parameters, true);
        }

        async private void EnableWifiForDevice(Message message, IEnumerator<string> parameters, bool enabled)
        {
            if (!parameters.MoveNext())
            {
                BotClient.SendTextMessage(message.Chat, InvalidParameterCount);
                return;
            }
            
            var isParameterMacAddress = parameters.Current.TryConvertToMacAddress(out var macAddress);
            var isParameterDeviceName = parameters.Current.TryConvertToDeviceName(out var deviceName);

            if (!isParameterMacAddress && !isParameterDeviceName)
            {
                BotClient.SendTextMessage(message.Chat, InvalidParameters);
                return;
            }

            var resultMessage = await _routerRequestService.EnableWiFiByMacAddressOrDeviceName(macAddress, deviceName, enabled);

            BotClient.SendTextMessage(message.Chat, resultMessage);
        }
    }
}