using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GPCombatController.Annotations;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace GPCombatController.Views
{
    public enum ECrossKeyState
    {
        None,
        Left,
        Up,
        Right,
        Down,
    }

    public class KeyStateFillConverter : IValueConverter
    {
        private static readonly SolidColorBrush NotActiveBrush = new SolidColorBrush(Colors.White);
        private static readonly SolidColorBrush ActiveBrush = new SolidColorBrush(Colors.Red);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var param = (string)parameter;
            var key = (ECrossKeyState) Enum.Parse(typeof (ECrossKeyState), param);
            var state = (ECrossKeyState)value;

            return key == state ? ActiveBrush : NotActiveBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    public sealed partial class CrossKeyInput : UserControl, INotifyPropertyChanged
    {
        private double _verticalSpaceSize;
        private double _horizontalSpaceSize;
        private ECrossKeyState _keyState;
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public double VerticalSpaceSize
        {
            get { return _verticalSpaceSize; }
            set
            {
                if (value.Equals(_verticalSpaceSize)) return;
                _verticalSpaceSize = value;
                OnPropertyChanged();
            }
        }

        public double HorizontalSpaceSize
        {
            get { return _horizontalSpaceSize; }
            set
            {
                if (value.Equals(_horizontalSpaceSize)) return;
                _horizontalSpaceSize = value;
                OnPropertyChanged();
            }
        }

        public ECrossKeyState KeyState
        {
            get { return _keyState; }
            set
            {
                if (value == _keyState) return;
                _keyState = value;
                OnPropertyChanged();
            }
        }

        private long PressedId { get; set; }

        public CrossKeyInput()
        {
            this.InitializeComponent();

            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            double rectSize = 0;

            if (ActualHeight < ActualWidth)
            {
                rectSize = ActualHeight - 20;
            }
            else
            {
                rectSize = ActualWidth - 20;
            }

            if (rectSize <= 0) return;

            VerticalSpaceSize = (ActualHeight - rectSize)/2;
            HorizontalSpaceSize = (ActualWidth - rectSize)/2;
            Debug.WriteLine($"W:{HorizontalSpaceSize} H:{VerticalSpaceSize}");
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);

            var pos = e.GetCurrentPoint(GridPad);

            if (pos.RawPosition.Y < GridPad.ActualHeight/4 &&
                GridPad.ActualWidth *3/8 < pos.RawPosition.X &&
                pos.RawPosition.X < GridPad.ActualWidth *5/8)
            {
                PressedId = e.Pointer.PointerId;
                KeyState = ECrossKeyState.Up;
            }
            else if (pos.RawPosition.Y > GridPad.ActualHeight *3/4 &&
                     GridPad.ActualWidth *3/8 < pos.RawPosition.X &&
                     pos.RawPosition.X < GridPad.ActualWidth *5/8)
            {
                PressedId = e.Pointer.PointerId;
                KeyState = ECrossKeyState.Down;
            }
            else if (pos.RawPosition.X < GridPad.ActualWidth /4 &&
                     GridPad.ActualHeight *3/8 < pos.RawPosition.Y &&
                     pos.RawPosition.Y < GridPad.ActualHeight *5/8)
            {
                PressedId = e.Pointer.PointerId;
                KeyState = ECrossKeyState.Left;
            }
            else if (pos.RawPosition.X > GridPad.ActualWidth *3/4 &&
                     GridPad.ActualHeight *3/8 < pos.RawPosition.Y &&
                     pos.RawPosition.Y < GridPad.ActualHeight *5/8)
            {
                PressedId = e.Pointer.PointerId;
                KeyState = ECrossKeyState.Right;
            }
            else
            {
                PressedId = e.Pointer.PointerId;
                KeyState = ECrossKeyState.None;
            }

        }

        protected override void OnPointerCanceled(PointerRoutedEventArgs e)
        {
            base.OnPointerCanceled(e);

            if (PressedId != e.Pointer.PointerId) return;

            PressedId = -1;
            KeyState = ECrossKeyState.None;
        }

        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);

            if (PressedId != e.Pointer.PointerId) return;

            PressedId = -1;
            KeyState = ECrossKeyState.None;
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);

            if (PressedId != e.Pointer.PointerId) return;

            PressedId = -1;
            KeyState = ECrossKeyState.None;
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);
        }
    }
}
