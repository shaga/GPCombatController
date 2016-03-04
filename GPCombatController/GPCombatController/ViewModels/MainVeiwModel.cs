using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.Devices.PointOfService;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using GPCombatController.Models.Konashi;

namespace GPCombatController.ViewModels
{
    class ScanLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var state = (bool) value;
            return state ? "Stop SCAN" : "Start Scan";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    class MainVeiwModel : ViewModelBase
    {
        private bool _isScanning;
        private KonashiInfo _selectedInfo;
        private RelayCommand _commandScan;

        public ObservableCollection<KonashiInfo> KonashiInfos { get; set; }

        public bool IsScanning
        {
            get { return _isScanning; }
            set
            {
                if (_isScanning == value) return;
                _isScanning = value;
                OnPropertyChanged();
            }
        }
        
        public KonashiInfo SelectedInfo
        {
            get { return _selectedInfo; }
            set
            {
                if (_selectedInfo == value) return;
                
                if (value != null)
                {
                    AppFrame.Navigate(typeof (ControllerPage), value);
                }
                
                _selectedInfo = null;
                OnPropertyChanged();
            }
        }

        private KonashiScanner Scanner { get; set; }

        private CoreDispatcher Dispatcher => CoreApplication.MainView.CoreWindow.Dispatcher;

        private Frame AppFrame => Window.Current.Content as Frame;

        public ICommand CommandScan => _commandScan ?? (_commandScan = new RelayCommand(Scan));

        public MainVeiwModel()
        {
            KonashiInfos = new ObservableCollection<KonashiInfo>();

            Scanner = new KonashiScanner();
            Scanner.FoundKonashi += OnFoundKonashi;
            Scanner.FinishedScan += async (s, e) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    IsScanning = false;
                });
            };
        }

        private void Scan()
        {
            if (!IsScanning)
            {
                if (KonashiInfos != null)
                {
                    KonashiInfos.Clear();
                }
                else
                {
                    KonashiInfos = new ObservableCollection<KonashiInfo>();
                }
                Scanner.StartScan();
                IsScanning = true;
            }
            else
            {
                Scanner.StopScan();
            }
        }

        private async void OnFoundKonashi(object sender, KonashiInfo konashiInfo)
        {
            if (!(KonashiInfos?.Any(i => i.Address == konashiInfo.Address) ?? true))
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    KonashiInfos.Add(konashiInfo);
                });
            }
        }
    }
}
