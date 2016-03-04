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
using GPCombatController.Models;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace GPCombatController.Views
{

    public class KeyStateFillConverter : IValueConverter
    {
        private static readonly SolidColorBrush NotActiveBrush = new SolidColorBrush(Colors.White);
        private static readonly SolidColorBrush ActiveBrush = new SolidColorBrush(Colors.Red);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var param = string.Empty;
            var key = ECrossKeyState.None;
            try
            {
                param = (string)parameter;
                key = (ECrossKeyState)Enum.Parse(typeof(ECrossKeyState), param);
            }
            catch (Exception)
            {
                return NotActiveBrush;
            }

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
        private const long RelasedKeyId = -1;

        private double _verticalSpaceSize;
        private double _horizontalSpaceSize;
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

        public static readonly DependencyProperty KeyStateProperty =
            DependencyProperty.Register(nameof(KeyState), typeof(ECrossKeyState), typeof(CrossKeyInput),
                PropertyMetadata.Create(ECrossKeyState.None, OnKeyStateChanged));

        private static void OnKeyStateChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var input = obj as CrossKeyInput;

            input?.OnPropertyChanged(nameof(KeyState));
        }

        public ECrossKeyState KeyState
        {
            get { return (ECrossKeyState) GetValue(KeyStateProperty); }
            set { SetValue(KeyStateProperty, value); }
        }

        private double PointLeftMaxX => GridPad.ActualWidth/4;
        private double PointRightMinX => GridPad.ActualWidth*3/4;
        private double PointUpMaxY => GridPad.ActualHeight/4;
        private double PointDownMinY => GridPad.ActualHeight*3/4;
        private double PointHoriMinX => GridPad.ActualWidth*3/8;
        private double PointHoriMaxX => GridPad.ActualWidth*5/8;
        private double PointVerMinY => GridPad.ActualHeight*3/8;
        private double PointVerMaxY => GridPad.ActualHeight*5/8;

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

            UpdateKeyState(e, false);
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

            var id = e.Pointer.PointerId;

            if (id == PressedId || PressedId == RelasedKeyId)
            {
                UpdateKeyState(e, true);
            }
        }

        private void UpdateKeyState(PointerRoutedEventArgs args, bool isMoved)
        {
            var pointerId = args.Pointer.PointerId;
            var pos = args.GetCurrentPoint(GridPad).RawPosition;

            if (pos.X < PointLeftMaxX && PointVerMinY < pos.Y && pos.Y < PointVerMaxY)
            {
                KeyState = ECrossKeyState.Left;
                PressedId = pointerId;
            }
            else if (pos.X > PointRightMinX && PointVerMinY < pos.Y && pos.Y < PointVerMaxY)
            {
                KeyState = ECrossKeyState.Right;
                PressedId = pointerId;
            }
            else if (pos.Y < PointUpMaxY && PointHoriMinX < pos.X && pos.X < PointHoriMaxX)
            {
                KeyState = ECrossKeyState.Up;
                PressedId = pointerId;
            }
            else if (pos.Y > PointDownMinY && PointHoriMinX < pos.X && pos.X < PointHoriMaxX)
            {
                KeyState = ECrossKeyState.Down;
                PressedId = pointerId;
            }
            else if (isMoved && PressedId == pointerId)
            {
                KeyState = ECrossKeyState.None;
                PressedId =  pointerId;
            }
            else if (PressedId == RelasedKeyId)
            {
                KeyState = ECrossKeyState.None;
            }
        }
    }
}
