using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using GPCombatController.Models.Konashi;

namespace GPCombatController.Models
{
    class CombatController
    {
        private const string MoveForeStr = "#MA7000$";
        private const string MoveBackStr = "#MA9000$";
        private const string MoveRightStr = "#MA0070$";
        private const string MoveLeftStr = "#MA0090$";

        private KonashiManager Konashi { get; set; }

        public async void Init(DeviceInformation konashi, DeviceInformation battery)
        {
            Konashi = new KonashiManager();

            await Konashi.InitKonashi(konashi, battery);

            await Konashi.ConfigUart(true, EKonashiUartBaudrate.UartRate38K4);
        }

        public async void MoveFore()
        {
            await Konashi.SendUartData(MoveForeStr);
        }

        public async void MoveBack()
        {
            await Konashi.SendUartData(MoveBackStr);
        }

        public async void MoveRight()
        {
            await Konashi.SendUartData(MoveRightStr);
        }

        public async void MoveLeft()
        {
            await Konashi.SendUartData(MoveLeftStr);
        }
    }
}
