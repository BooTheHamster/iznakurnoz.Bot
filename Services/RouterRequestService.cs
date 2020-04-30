using Iznakurnoz.Bot.Interfaces;

namespace iznakurnoz.Bot.CommandHandlers
{
    /// <summary>
    /// Сервис для запросов к роутеру.
    /// </summary>
    internal class RouterRequestService
    {
        private readonly IConfigProvider _configProvider;
        public RouterRequestService(IConfigProvider configProvider)
        {
            _configProvider = configProvider;
        }

        public string GetHttpId()
        {
            return null;
        }
    }
}