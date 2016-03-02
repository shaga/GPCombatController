using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace GPCombatController.Models.Konashi
{
    class KonashiInfo
    {
        public ulong Address { get; }
        public DeviceInformation Konashi { get; set; }
        public DeviceInformation Battery { get; set; }

        public string DeviceName => Konashi?.Name;

        public KonashiInfo(ulong address, DeviceInformation konashi, DeviceInformation battery)
        {
            Address = address;
            Konashi = konashi;
            Battery = battery;
        }
    }
}
