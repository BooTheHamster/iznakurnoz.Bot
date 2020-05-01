using System;
using System.Collections.Generic;
using System.Linq;

namespace iznakurnoz.Bot.Services
{
    /// <summary>
    /// Провайдер настроек передаваемх из командной строки.
    /// </summary>
    internal class CommandLineProvider
    {
        private const char CommandPrefix = '-';

        private readonly IDictionary<string, Action<IEnumerator<string>>> _optionHandlers =
            new Dictionary<string, Action<IEnumerator<string>>>();

        /// <summary>
        /// Признак что бот запущен в режиме демона.
        /// </summary>
        public bool IsDaemonMode { get; private set; }

        /// <summary>
        /// Тестовая команда выполняемая ботом при запуске.
        /// </summary>
        public string TestCommand { get; private set; }

        /// <summary>
        /// Признак наличи тестовой команды.
        /// </summary>
        public bool HasTestCommand { get; private set; }

        public CommandLineProvider()
        {
            IsDaemonMode = false;
            TestCommand = null;
            HasTestCommand = false;

            _optionHandlers.Add("test", TestOptionHandler);
            _optionHandlers.Add("daemon", DaemonOptionHandler);

            ParseCommandLine();
        }

        private void TestOptionHandler(IEnumerator<string> argumentEnumerator)
        {
            TestCommand = string.Empty;

            while (argumentEnumerator.MoveNext())
            {
                TestCommand += " " + argumentEnumerator.Current;
            }

            HasTestCommand = !string.IsNullOrWhiteSpace(TestCommand);
        }

        private void DaemonOptionHandler(IEnumerator<string> argumentEnumerator)
        {
            IsDaemonMode = true;
        }

        private void ParseCommandLine()
        {
            var arguments = Environment.GetCommandLineArgs().ToList();

            if (arguments.Count < 2)
            {
                return;
            }

            var argumentEnumerator = arguments.GetEnumerator();

            while (argumentEnumerator.MoveNext())
            {
                var argument = argumentEnumerator.Current;

                if (string.IsNullOrWhiteSpace(argument))
                {
                    continue;
                }

                if (argument.StartsWith(CommandPrefix))
                {
                    argument = argument.TrimStart(CommandPrefix);

                    if (_optionHandlers.TryGetValue(argument, out var handler))
                    {
                        handler(argumentEnumerator);
                    }
                }
            }
        }
    }
}