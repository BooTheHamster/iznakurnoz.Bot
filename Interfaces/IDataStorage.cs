namespace iznakurnoz.Bot.Interfaces
{
    /// <summary>
    /// Хранилище произвольных данных бота.
    /// </summary>
    public interface IDataStorage
    {
        /// <summary>
        /// Сохранение объекта в хранилище.
        /// </summary>
        /// <param name="key">Уникальный ключ хранилища.</param>
        /// <param name="obj">Объект.</param>
        /// <typeparam name="T">Тип объекта.</typeparam>
         void Set<T>(string key, T obj);

        /// <summary>
        /// Получение объекта из хранилища.
        /// 
        /// Если объекта по ключу в хранилище не будет найдено,
        /// то вернется пустой объект.
        /// </summary>
        /// <param name="key">Уникальный ключ хранилища.</param>
        /// <typeparam name="T">Тип объекта.</typeparam>
        /// <returns>Объект.</returns>
         T Get<T>(string key);
    }
}