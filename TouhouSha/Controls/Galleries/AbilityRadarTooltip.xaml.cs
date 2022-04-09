using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TouhouSha.Core.UIs;

namespace TouhouSha.Controls
{
    /// <summary>
    /// AbilityRadarTooltip.xaml 的交互逻辑
    /// </summary>
    public partial class AbilityRadarTooltip : UserControl
    {
        public AbilityRadarTooltip()
        {
            InitializeComponent();
            InitializeRadarModel();
        }

        #region Properties
        
        #region Core

        static public readonly DependencyProperty CoreProperty = DependencyProperty.Register(
            "Core", typeof(AbilityRadar), typeof(AbilityRadarTooltip),
            new PropertyMetadata(null, OnPropertyChanged_Core));

        public AbilityRadar Core
        {
            get { return (AbilityRadar)GetValue(CoreProperty); }
            set { SetValue(CoreProperty, value); }
        }

        private static void OnPropertyChanged_Core(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AbilityRadarTooltip) ((AbilityRadarTooltip)d).OnCoreChanged(e);
        }

        protected virtual void OnCoreChanged(DependencyPropertyChangedEventArgs e)
        {
            tick = 0;
        }

        #endregion

        #endregion 

        private Polygon radar;
        private DispatcherTimer timer;
        private int tick;
        private int tickmax = 50;

        protected void InitializeRadarModel()
        {
            for (int i = 1; i <= 5; i++)
            {
                double r = (double)i / 10;
                Polygon polygon = new Polygon();
                polygon.Fill = i == 5 ? new SolidColorBrush(Color.FromArgb(0x40, 0x40, 0x40, 0x40)) : null;
                polygon.Stroke = new SolidColorBrush(Color.FromArgb(0x40, 0xFF, 0xFF, 0xFF));
                polygon.StrokeThickness = 1;
                polygon.SnapsToDevicePixels = true;
                polygon.Stretch = Stretch.None;
                for (int j = 0; j < 6; j++)
                {
                    double a = Math.PI / 3 * j;
                    double x = 0.5 + Math.Sin(a) * r;
                    double y = 0.5 - Math.Cos(a) * r;
                    x *= Width - 64;
                    y *= Height - 64;
                    polygon.Points.Add(new Point(x, y));
                }
                CV_Radar.Children.Add(polygon);
            }
            this.radar = new Polygon();
            radar.Fill = Brushes.LightGreen;
            radar.Stroke = Brushes.White;
            radar.StrokeThickness = 1;
            radar.SnapsToDevicePixels = true;
            radar.Stretch = Stretch.None;
            for (int j = 0; j < 6; j++)
                radar.Points.Add(new Point(0.5, 0.5));
            CV_Radar.Children.Add(radar);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsVisibleProperty)
            { 
                if (IsVisible)
                {
                    this.timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromMilliseconds(10);
                    timer.Tick += OnTimerTick;
                    timer.Start();
                }
                else
                {
                    timer.Tick -= OnTimerTick;
                    timer.Stop();
                    this.timer = null;
                }
            }
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (Core == null) return;
            if (tick >= tickmax) return;
            tick++;
            double r0 = 1 - Math.Pow(tickmax - tick, 2) / Math.Pow(tickmax, 2);
            for (int i = 0; i < 6; i++)
            {
                double r = i == 0 ? Core.Attack : i == 1 ? Core.Defence : i == 2 ? Core.Control : i == 3 ? Core.Auxiliary : i == 4 ? Core.LastStages : i == 5 ? Core.Difficulty : 0;
                r *= r0 / 10;
                double a = Math.PI / 3 * i;
                double x = 0.5 + Math.Sin(a) * r;
                double y = 0.5 - Math.Cos(a) * r;
                x *= Width - 64;
                y *= Height - 64;
                radar.Points[i] = new Point(x, y);
            }
        }
    }
}
