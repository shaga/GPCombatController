using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml;

namespace GPCombatController.Models.Konashi
{
    class KonashiScanner
    {
        private BluetoothLEAdvertisementWatcher Wathter { get; set; }

        private DeviceInformationCollection KonashiServices { get; set; }
        private DeviceInformationCollection BatteryServices { get; set; }
        private DispatcherTimer ScanTimer { get; set; }

        public event EventHandler<KonashiInfo> FoundKonashi;

        public event EventHandler FinishedScan;

        public bool IsScanning
            =>
                (Wathter?.Status ?? BluetoothLEAdvertisementWatcherStatus.Stopped) ==
                BluetoothLEAdvertisementWatcherStatus.Started;
 
        public KonashiScanner()
        {
            Wathter = new BluetoothLEAdvertisementWatcher();
            //Wathter.AdvertisementFilter.Advertisement.ServiceUuids.Add(KonashiUuid.KonashiServiceUuid);
            Wathter.Received += OnReceivedAdvertisement;
            Wathter.Stopped += (s, e) => FinishedScan?.Invoke(this, EventArgs.Empty);

            ScanTimer = new DispatcherTimer();
            ScanTimer.Interval = new TimeSpan(0, 0, 10);
            ScanTimer.Tick += (s, e) => StopScan();
        }

        public async void StartScan()
        {
            if (IsScanning) return;

            KonashiServices = await DeviceInformation.FindAllAsync(KonashiUuid.KonashiServiceSelector);
            BatteryServices = await DeviceInformation.FindAllAsync(KonashiUuid.BatteryServiceSelector);

            Wathter.Start();
            ScanTimer.Start();
        }

        public void StopScan()
        {
            ScanTimer.Stop();
            if (IsScanning)
            {
                Wathter.Stop();
            }
        }

        private void OnReceivedAdvertisement(BluetoothLEAdvertisementWatcher wathAdvertisementWatcher,
            BluetoothLEAdvertisementReceivedEventArgs e)
        {
            if ((KonashiServices?.Count ?? 0) == 0 || (BatteryServices?.Count ?? 0) == 0) return;
            var address = $"_{e.BluetoothAddress.ToString("x12")}";

            var konashi = KonashiServices.FirstOrDefault(i => i.Id.Contains(address));
            var battery = BatteryServices.FirstOrDefault(i => i.Id.Contains(address));

            if (konashi != null && battery != null)
            {
                var info = new KonashiInfo(e.BluetoothAddress, konashi, battery);
                FoundKonashi?.Invoke(this, info);
            }
        }
    }
}
