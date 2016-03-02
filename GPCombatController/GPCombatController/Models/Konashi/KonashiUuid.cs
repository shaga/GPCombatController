using System;
using System.Collections.Generic;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace GPCombatController.Models.Konashi
{
    public class KonashiUuid
    {
        private const string UuidBase = "-03fb-40da-98a7-b0def65c2d4b";

        public static readonly Guid CharacteristicConfigDescriptorUuid =
            Guid.Parse("00002902-0000-1000-8000-00805f9b34fb");

        public static readonly Guid BatteryServiceUuid = Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb");
        public static readonly Guid BatteryLevelUuid = Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb");

        public static readonly string BatteryServiceSelector =
            GattDeviceService.GetDeviceSelectorFromUuid(BatteryServiceUuid);

        public static readonly Guid KonashiServiceUuid = Guid.Parse("229bff00" + UuidBase);

        public static readonly string KonashiServiceSelector =
            GattDeviceService.GetDeviceSelectorFromUuid(KonashiServiceUuid);

        public static readonly Guid KonashiUartConfigUuid = Guid.Parse("229b3010" + UuidBase);
        public static readonly Guid KonashiUartBaudRateUuid = Guid.Parse("229b3011" + UuidBase);
        public static readonly Guid KonashiUartTxUuid = Guid.Parse("229b3012" + UuidBase);
        public static readonly Guid KonashiUartRxNotifyUuid = Guid.Parse("229b3013" + UuidBase);
        public const string ClientCharacteristicConfigUuidStr = "00002902-0000-1000-8000-00805f9b34fb";

    }
}
