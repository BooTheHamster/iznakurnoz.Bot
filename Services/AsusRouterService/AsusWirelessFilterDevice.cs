namespace iznakurnoz.Bot.Services.RouterService
{
    internal partial class AsusRouterRequestService
    {
        /// <summary>
        /// Описание устройства для WiFi фильтра.
        /// </summary>
        internal class WirelessFilterDevice
        {
            /// <summary>
            /// Наменование устройства.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// MAC адрес устройства.
            /// </summary>
            /// <value></value>
            public string MacAddress { get; private set; }

            public WirelessFilterDevice(string name, string macAddress)
            {
                Name = name;
                MacAddress = macAddress;
            }

            public override string ToString()
            {
                return $"{MacAddress} {Name}";
            }
        }
    }
}