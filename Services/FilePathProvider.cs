using System;
using System.IO;
using System.Runtime.InteropServices;

namespace iznakurnoz.Bot.Services
{
    /// <summary>
    /// Поставщик информации о пути к файлам бота.
    /// </summary>
    internal class FilePathProvider
    {
        private const string BotFolderName = "iznakurnozbot";
        private readonly CommandLineProvider _commandLineProvider;

        public FilePathProvider(CommandLineProvider commandLineProvider)
        {
            _commandLineProvider = commandLineProvider;
        }

        /// <summary>
        /// Возвращает полный путь к каталогу с файлом конфигурации.
        /// </summary>
        /// <returns>Полный путь к каталогу с файлом конфигурации.</returns>
        public string GetConfigDirectoryPath()
        {
            var folder = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (_commandLineProvider.IsDaemonMode)
                {
                    folder = "/etc";
                }
                else
                {
                    // Для отладки не в режиме демона будет использоваться конфиг из локального каталога пользователя.
                    folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }

            folder = Path.Combine(folder, BotFolderName);
            return folder;
        }

        /// <summary>
        /// Возвращает полный путь к каталогу с файлами данных.
        /// </summary>
        /// <returns>Полный путь к каталогу с файлами данных.</returns>
        public string GetDataDirectoryPath()
        {
            var folder = string.Empty;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (_commandLineProvider.IsDaemonMode)
                {
                    folder = "/var";
                }
                else
                {
                    // Для отладки не в режиме демона будет использоваться локальный каталог пользователя.
                    folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                }
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }

            folder = Path.Combine(folder, BotFolderName);
            return folder;
        }
    }
}