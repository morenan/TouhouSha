using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;

namespace TouhouSha.Controls
{
    public class BoxedControl : UserControl
    {
        #region Number

        private Border ui_border_0;
        private Border ui_border_1;
        private Grid ui_fade;
        private DispatcherTimer timer;
        private int tick;
        private bool tohide;

        #endregion

        #region Method


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ui_border_0 = GetTemplateChild("PART_Border_0") as Border;
            ui_border_0.Clip = new RectangleGeometry(Rect.Empty);
            ui_border_1 = GetTemplateChild("PART_Border_1") as Border;
            ui_border_1.Clip = new RectangleGeometry(Rect.Empty);
            ui_fade = GetTemplateChild("PART_Fade") as Grid;
            ui_fade.Opacity = 0;
        }

        public void Show()
        {
            tohide = false;
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            tohide = true;
        }

        protected virtual void InternalTimerTick()
        {
            
        }

        #endregion 

        #region Event Handler

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);   
            if (e.Property == IsVisibleProperty)
            {
                if (IsVisible)
                {
                    if (timer == null)
                    {
                        timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromMilliseconds(10);
                        timer.Tick += Timer_Tick;
                        timer.Start();
                    }
                }
                else
                {
                    if (timer != null)
                    {
                        timer.Stop();
                        timer.Tick -= Timer_Tick;
                        timer = null;
                    }
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double r = 1.0d;
            InternalTimerTick();
            if (tohide)
            {
                if (tick == 0)
                {
                    Visibility = Visibility.Collapsed;
                    return;
                }
                r = (--tick) / 20.0d;
            }
            else
            {
                if (tick >= 20) return;
                r = (++tick) / 20.0d;
            }
            if (ui_fade != null) ui_fade.Opacity = r;
            if (ui_border_0 != null) ui_border_0.Clip = new RectangleGeometry(new Rect(
                0, 0, ui_border_0.ActualWidth * r, ui_border_0.ActualHeight));
            if (ui_border_1 != null) ui_border_1.Clip = new RectangleGeometry(new Rect(
                ui_border_1.ActualWidth * (1 - r), 0, ui_border_1.ActualWidth * r, ui_border_1.ActualHeight));
        }

        #endregion
    }
}
