using System.Collections.Generic;
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

        public WifiCommandHandler(
            IBotTelegramClient botTelegramClient)
            : base(botTelegramClient, _supportedCommands)
        {
            _optionHandlers = new Dictionary<string, OptionHandlerDelegate>()
            {
                { "s", StateOptionHandler },
                { "state", StateOptionHandler },
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

        private Task<string> StateOptionHandler(Message message, IEnumerator<string> parameters)
        {
            return null;
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

            return null;
        }
    }
}