using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.AllJoyn;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GPCombatController.Models.Konashi;

namespace GPCombatController.Models
{
    public enum ECrossKeyState
    {
        None,
        Left,
        Up,
        Right,
        Down,
    }

    class CombatController : IDisposable
    {
        private static readonly TimeSpan RepeatSpan = new TimeSpan(0, 0, 0, 500);
        private const string MoveStopStr = "#MA0000$";
        private const string MoveForeStr = "#MA7000$";
        private const string MoveBackStr = "#MA9000$";
        private const string MoveRightStr = "#MA0070$";
        private const string MoveLeftStr = "#MA0090$";

        private ECrossKeyState State { get; set; }
        private KonashiManager Konashi { get; set; }

        private DispatcherTimer RepeatTimer { get; set; }
        private CoreDispatcher Dispatcher => CoreApplication.MainView.CoreWindow.Dispatcher;

        private string StateStr
        {
            get
            {
                switch (State)
                {
                    case ECrossKeyState.Up:
                        return MoveForeStr;
                    case ECrossKeyState.Down:
                        return MoveBackStr;
                    case ECrossKeyState.Left:
                        return MoveLeftStr;
                    case ECrossKeyState.Right:
                        return MoveRightStr;
                }
                return MoveStopStr;
            }
        }

        public CombatController()
        {
            RepeatTimer = new DispatcherTimer() {Interval = RepeatSpan};
            RepeatTimer.Tick += OnRepeatSendState;
        }

        ~CombatController()
        {
            Dispose();
        }

        public void Dispose()
        {
            Konashi?.Dispose();
        }

        public async void Init(DeviceInformation konashi, DeviceInformation battery)
        {
            Konashi = new KonashiManager();

            await Konashi.InitKonashi(konashi, battery);

            await Konashi.ConfigUart(true, EKonashiUartBaudrate.UartRate38K4);
        }

        public void MoveStop()
        {
            SetMoveState(ECrossKeyState.None);
        }

        public void MoveFore()
        {
            SetMoveState(ECrossKeyState.Up);
        }

        public void MoveBack()
        {
            SetMoveState(ECrossKeyState.Down);
        }

        public void MoveRight()
        {
            SetMoveState(ECrossKeyState.Right);
        }

        public void MoveLeft()
        {
            SetMoveState(ECrossKeyState.Left);
        }

        private async void SetMoveState(ECrossKeyState state)
        {
            var isForce = state == State;
            State = state;
            await Konashi.SendUartData(StateStr, isForce);
        }

        private async void OnRepeatSendState(object sender, object parameter)
        {
            if (State == ECrossKeyState.None) return;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await Konashi.SendUartData(StateStr);
            });
        }
    }
}
