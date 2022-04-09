using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TouhouSha
{
    /// <summary>
    /// StartupScreen.xaml 的交互逻辑
    /// </summary>
    public partial class StartupScreen : UserControl
    {
        public StartupScreen()
        {
            InitializeComponent();
        }

        #region Number

        private DispatcherTimer timer;
        private int tick;

        #endregion

        #region Event Handler

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsVisibleProperty)
            {
                if (IsVisible)
                {
                    Keyboard.Focus(this);
                    this.timer = new DispatcherTimer();
                    this.tick = 0;
                    timer.Interval = TimeSpan.FromMilliseconds(5);
                    timer.Tick += OnTimerTick;
                    timer.Start();
                }
                else
                {
                    timer.Stop();
                    timer.Tick -= OnTimerTick;
                    this.timer = null;
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.Key)
            {
                case Key.Enter:
                case Key.Escape:
                    Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            tick++;
            if (tick <= 300)
            {
                int i0 = tick / 50;
                int i1 = tick % 50;
                if ((i0 & 1) == 0)
                {
                    TL_Page0_Text0.BlurPower = TL_Page0_Text1.BlurPower = 1.0d + 10.0d * i1 / 50.0d; 
                }
                else
                {
                    TL_Page0_Text0.BlurPower = TL_Page0_Text1.BlurPower = 1.0d + 10.0d * (50 - i1) / 50.0d;
                }
                if (tick <= 20)
                {
                    SP_Page0.Opacity = tick / 20.0d;
                    SP_Page0_Transform.ScaleX = SP_Page0_Transform.ScaleY = 0.9d + 0.1d * SP_Page0.Opacity;
                }
                else if (tick <= 300 - 20)
                {
                    SP_Page0.Opacity = 1;
                    SP_Page0_Transform.ScaleX = SP_Page0_Transform.ScaleY = 1.0d + 0.05d * (tick - 20) / (300 - 40);
                }
                else
                {
                    SP_Page0.Opacity = (300 - tick) / 20.0d;
                    SP_Page0_Transform.ScaleX = SP_Page0_Transform.ScaleY = 1.0d + 0.05d + 0.5d * (1 - SP_Page0.Opacity);
                }
            }
            else if (tick <= 600)
            {
                int i0 = tick / 50;
                int i1 = tick % 50;
                if ((i0 & 1) == 0)
                {
                    TL_Page1_Text0.BlurPower = 1.0d + 10.0d * i1 / 50.0d;
                }
                else
                {
                    TL_Page1_Text0.BlurPower = 1.0d + 10.0d * (50 - i1) / 50.0d;
                }
                if (tick <= 320)
                {
                    SP_Page1.Opacity = (tick - 300) / 20.0d;
                    SP_Page1_Transform.ScaleX = SP_Page1_Transform.ScaleY = 0.9d + 0.1d * SP_Page1.Opacity;
                }
                else if (tick <= 600 - 20)
                {
                    SP_Page1.Opacity = 1;
                    SP_Page1_Transform.ScaleX = SP_Page1_Transform.ScaleY = 1.0d + 0.05d * (tick - 320) / (300 - 40);
                }
                else
                {
                    SP_Page1.Opacity = (600 - tick) / 20.0d;
                    SP_Page1_Transform.ScaleX = SP_Page1_Transform.ScaleY = 1.0d + 0.05d + 0.5d * (1 - SP_Page1.Opacity);
                }
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
        }

        #endregion
    }
}
