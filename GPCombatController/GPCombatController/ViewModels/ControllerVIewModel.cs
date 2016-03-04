using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using GPCombatController.Models;
using GPCombatController.Models.Konashi;

namespace GPCombatController.ViewModels
{
    class ControllerViewModel : ViewModelBase, IDisposable
    {
        private ECrossKeyState _state;
        private KonashiInfo KonashiInfo { get; set; }
        private CombatController Controller { get; set; }

        public void Dispose()
        {
            Controller?.Dispose();
        }

        public void Init(KonashiInfo info)
        {
            KonashiInfo = info;
            Controller = new CombatController();
            Controller.Init(info.Konashi, info.Battery);
        }

        public string StateStr
        {
            get
            {
                switch(State)
                {
                    case ECrossKeyState.Left:
                        return "左";
                    case ECrossKeyState.Up:
                        return "前";
                    case ECrossKeyState.Right:
                        return "右";
                    case ECrossKeyState.Down:
                        return "後";
                }

                return "止";
            }
        }

        public ECrossKeyState State
        {
            get { return _state; }
            set
            {
                if (value == _state) return;
                _state = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(StateStr));
                switch (value)
                {
                    case ECrossKeyState.None:
                        Controller?.MoveStop();
                        break;
                    case ECrossKeyState.Left:
                        Controller?.MoveLeft();
                        break;
                    case ECrossKeyState.Up:
                        Controller?.MoveFore();
                        break;
                    case ECrossKeyState.Right:
                        Controller?.MoveRight();
                        break;
                    case ECrossKeyState.Down:
                        Controller?.MoveBack();
                        break;
                }
            }
        }
    }
}
