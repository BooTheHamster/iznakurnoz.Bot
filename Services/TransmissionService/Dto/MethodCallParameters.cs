namespace iznakurnoz.Bot.Services.TransmissionService.Dto
{
    /// <summary>
    /// Параметры для RPC вызова метода торрент сервера.
    /// </summary>
    /// <typeparam name="TArguments"></typeparam>
    internal class MethodCallParameters<TArguments>
    {
        public string Method { get; private set; }

        public TArguments Arguments { get; private set; }

        public MethodCallParameters(string method, TArguments arguments)
        {
            Method = method;
            Arguments = arguments;
        }
    }
}