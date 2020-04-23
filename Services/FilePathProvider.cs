using System;
using System.IO;

namespace iznakurnoz.Bot.Services
{
    /// <summary>
    /// Поставщик информации о пути к файлам бота.
    /// </summary>
    public class FilePathProvider
    {
        private const string BotFolderName = "iznakurnozbot";

        /// <summary>
        /// Возвращает полный путь к файлу.
        /// </summary>
        /// <param name="filename">Имя файла.</param>
        /// <returns>Полный путь к файлу.</returns>
        public static string GetFilePath(string filename)
        {
            return Path.Combine(GetDirectoryPath(), filename);
        }

        /// <summary>
        /// Возвращает полный путь к каталогу с файлами.
        /// </summary>
        /// <returns>Полный путь к каталогу с файлами.</returns>
        public static string GetDirectoryPath()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            folder = Path.Combine(folder, BotFolderName);
            return folder;
        }
    }
}