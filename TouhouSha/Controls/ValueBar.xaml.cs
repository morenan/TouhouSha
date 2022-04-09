using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TouhouSha.Controls
{
    /// <summary>
    /// ValueBar.xaml 的交互逻辑
    /// </summary>
    public partial class ValueBar : RangeBase
    {
        public ValueBar()
        {
            InitializeComponent();
        }

        #region Properties

        #region OnlyInt

        static public readonly DependencyProperty OnlyIntProperty = DependencyProperty.Register(
            "OnlyInt", typeof(bool), typeof(ValueBar),
            new PropertyMetadata(false));

        public bool OnlyInt
        {
            get { return (bool)GetValue(OnlyIntProperty); }
            set { SetValue(OnlyIntProperty, value); }
        }

        #endregion

        #region ShowValue

        static public readonly DependencyProperty ShowValueProperty = DependencyProperty.Register(
            "ShowValue", typeof(string), typeof(ValueBar),
            new PropertyMetadata(String.Empty));

        public string ShowValue
        {
            get { return (string)GetValue(ShowValueProperty); }
            set { SetValue(ShowValueProperty, value); }
        }

        #endregion

        #endregion

        #region Number

        private FrameworkElement ui_thumb_space;
        private Thumb ui_thumb;
        private double thumbvalue;
        private bool showvalue_changing;

        #endregion

        #region Method

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ui_thumb_space = GetTemplateChild("PART_Thumb_Space") as FrameworkElement;
            this.ui_thumb = GetTemplateChild("PART_Thumb") as Thumb;
            if (ui_thumb != null)
            {
                ui_thumb.DragStarted += UI_Thumb_DragStarted;
                ui_thumb.DragDelta += UI_Thumb_DragDelta;
                ui_thumb.SizeChanged += UI_Thumb_SizeChanged;
            }
            if (ui_thumb_space != null)
            {
                ui_thumb_space.SizeChanged += UI_Thumb_Space_SizeChanged;
            }
        }


        #endregion

        #region Method

        protected void UpdateThumbLocation()
        {
            if (ui_thumb_space == null) return;
            if (ui_thumb == null) return;
            double w0 = ui_thumb_space.ActualWidth;
            double w1 = ui_thumb.ActualWidth;
            double value = ui_thumb.IsDragging ? thumbvalue : Value; 
            value = Math.Min(Math.Max(value, Minimum), Maximum);
            ui_thumb.Margin = new Thickness((value - Minimum) / (Maximum - Minimum) * (w0 - w1), 0, 0, 0);
        }

        #endregion 

        #region Event Handler

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == ValueProperty
             || e.Property == MinimumProperty
             || e.Property == MaximumProperty)
            {
                if (e.Property == ValueProperty && OnlyInt)
                    Value = Math.Min(Math.Max(Math.Floor(Value), Minimum), Maximum);
                showvalue_changing = true;
                if (OnlyInt)
                    ShowValue = ((int)Math.Floor(Value)).ToString();
                else
                    ShowValue = String.Format("{0:f2}", Value);
                showvalue_changing = false;
                if (ui_thumb != null && !ui_thumb.IsDragging)
                    UpdateThumbLocation();
            }
            if (e.Property == ShowValueProperty
             && !showvalue_changing)
            {
                double d = 0;
                if (double.TryParse(ShowValue, out d))
                    Value = d;
                else
                {
                    showvalue_changing = true;
                    if (OnlyInt)
                        ShowValue = ((int)Math.Floor(Value)).ToString();
                    else
                        ShowValue = String.Format("{0:f2}", Value);
                    showvalue_changing = false;
                }
            }
            if (e.Property == OnlyIntProperty
             && OnlyInt)
            {
                Value = Math.Min(Math.Max(Math.Floor(Value), Minimum), Maximum);
            }
        }

        private void UI_Thumb_Space_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateThumbLocation();
        }

        private void UI_Thumb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateThumbLocation();
        }

        private void UI_Thumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            thumbvalue = Value;
        }

        private void UI_Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            if (ui_thumb_space == null) return;
            if (ui_thumb == null) return;
            double w0 = ui_thumb_space.ActualWidth;
            double w1 = ui_thumb.ActualWidth;
            thumbvalue += e.HorizontalChange * (Maximum - Minimum) / (w0 - w1);
            UpdateThumbLocation();
            if (OnlyInt)
                Value = Math.Min(Math.Max(Math.Floor(thumbvalue), Minimum), Maximum);
            else 
                Value = Math.Min(Math.Max(thumbvalue, Minimum), Maximum);
        }

        #endregion
    }

}
