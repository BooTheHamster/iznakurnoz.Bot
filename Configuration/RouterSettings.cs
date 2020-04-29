namespace Iznakurnoz.Bot.Configuration
{
    /// <summary>
    /// Настройки роутера.
    /// </summary>
    internal class RouterSettings 
    {
        /// <summary>
        /// Адрес роутера.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Имя администратора.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Пароль администратора.
        /// </summary>
        public string Password { get; set; }        
    }
}