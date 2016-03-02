using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;

namespace GPCombatController.Models.Konashi
{
    public class KonashiManager
    {
        #region property

        private DeviceInformation KonashiInfo { get; set; }

        private GattDeviceService KonashiService { get; set; }

        private DeviceInformation BatteryInfo { get; set; }

        private GattDeviceService BatteryServcie { get; set; }

        private Dictionary<Guid, GattCharacteristic> KonashiCharacteristics { get; set; }

        #endregion

        #region event

        public delegate void KonashiUartReceivedHandler(byte[] data);

        public event KonashiUartReceivedHandler KonashiUartReceived;

        #endregion

        public async Task InitKonashi(DeviceInformation konashiInfo, DeviceInformation batteryInfo)
        {
            KonashiInfo = konashiInfo;

            KonashiService = await GattDeviceService.FromIdAsync(KonashiInfo.Id);

            BatteryInfo = batteryInfo;

            BatteryServcie = await GattDeviceService.FromIdAsync(BatteryInfo.Id);

            if (KonashiService == null || BatteryServcie == null) return;

            var konashiCharacteristics = KonashiService.GetAllCharacteristics();

            var batteryCharacteristics = BatteryServcie.GetAllCharacteristics();

            if ((konashiCharacteristics?.Count ?? 0) == 0 || (batteryCharacteristics?.Count ?? 0) == 0)
            {
                return;
            }

            KonashiCharacteristics = new Dictionary<Guid, GattCharacteristic>();

            foreach (var characteristic in konashiCharacteristics)
            {
                await InitCharacteristic(characteristic);
            }

            foreach (var characteristic in batteryCharacteristics)
            {
                await InitCharacteristic(characteristic);
            }
        }

        public async Task ConfigUart(bool isEnabled, int baudRate)
        {
            var enabledData = new byte[] {(byte) (isEnabled ? 1 : 0)};

            await WriteCharacteristicData(KonashiUuid.KonashiUartConfigUuid, enabledData);

            if (isEnabled)
            {
                var baudRateData = new byte[] {(byte) ((baudRate >> 8) & 0xff), (byte) (baudRate & 0xff)};

                await WriteCharacteristicData(KonashiUuid.KonashiUartBaudRateUuid, baudRateData);
            }
        }

        public async Task ConfigUart(bool isEnabled, EKonashiUartBaudrate baudRate = EKonashiUartBaudrate.UartRate9K6)
        {
            var enableData = new byte[] {(byte) (isEnabled ? 1 : 0)};

            await WriteCharacteristicData(KonashiUuid.KonashiUartConfigUuid, enableData);

            if (isEnabled)
            {
                var rate = (int) baudRate;
                var data = new byte[] {(byte) ((rate >> 8) & 0xff), (byte) (rate & 0xff)};
                await WriteCharacteristicData(KonashiUuid.KonashiUartBaudRateUuid, data);
            }
        }


        public async Task SendUartData(string data)
        {
            await SendUartData(Encoding.ASCII.GetBytes(data));
        }

        public async Task SendUartData(byte[] data)
        {
            var pos = 0;

            while (pos < data.Length)
            {
                var len = Math.Min(data.Length - pos, KonashiConsts.UartDataMaxLength);

                var sendData = new byte[len + 1];

                sendData[0] = (byte) len;

                Array.Copy(data, pos, sendData, 1, len);

                await WriteCharacteristicData(KonashiUuid.KonashiUartTxUuid, sendData);

                pos += len;
            }
        }

        private async Task InitCharacteristic(GattCharacteristic characteristic)
        {
            if (KonashiCharacteristics == null) KonashiCharacteristics = new Dictionary<Guid, GattCharacteristic>();

            KonashiCharacteristics.Add(characteristic.Uuid, characteristic);

            if (characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
            {
                await
                    characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                        GattClientCharacteristicConfigurationDescriptorValue.Notify);

                characteristic.ValueChanged += OnKonashiCharacteristicChanged;
            }
        }

        private void OnKonashiCharacteristicChanged(GattCharacteristic sender, GattValueChangedEventArgs e)
        {
            var uuid = sender.Uuid;

            if (uuid == KonashiUuid.KonashiUartRxNotifyUuid)
            {
                KonashiUartReceived?.Invoke(e.CharacteristicValue.ToArray());
            }
        }

        private async Task WriteCharacteristicData(Guid uuid, byte[] data)
        {
            if (!KonashiCharacteristics.ContainsKey(uuid)) return;

            var characteristic = KonashiCharacteristics[uuid];

            await characteristic.WriteValueAsync(data.AsBuffer(), GattWriteOption.WriteWithoutResponse);
        }

        private async Task<byte[]> ReadCharacteristic(Guid uuid)
        {
            if (!KonashiCharacteristics.ContainsKey(uuid)) return null;

            var characteristic = KonashiCharacteristics[uuid];

            var result = await characteristic.ReadValueAsync();

            return result.Value.ToArray();
        }
    }
}
