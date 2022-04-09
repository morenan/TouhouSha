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

namespace TouhouSha.Controls.Cools
{
    /// <summary>
    /// CardWaveCool2.xaml 的交互逻辑
    /// </summary>
    public partial class CardWaveCool2 : UserControl
    {
        public CardWaveCool2()
        {
            InitializeComponent();
            InitializeBorders();
        }

        #region Properties

        #region Color0

        static public readonly DependencyProperty Color0Property = DependencyProperty.Register(
            "Color0", typeof(Color), typeof(CardWaveCool2),
            new PropertyMetadata(Colors.AntiqueWhite, OnPropertyChanged_Color0));

        public Color Color0
        {
            get { return (Color)GetValue(Color0Property); }
            set { SetValue(Color0Property, value); }
        }

        static public void OnPropertyChanged_Color0(object d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardWaveCool2) ((CardWaveCool2)d).OnColor0Changed(e);
        }

        public virtual void OnColor0Changed(DependencyPropertyChangedEventArgs e)
        {
            UpdateBorderBrush();
        }

        #endregion

        #region Color1

        static public readonly DependencyProperty Color1Property = DependencyProperty.Register(
            "Color1", typeof(Color), typeof(CardWaveCool2),
            new PropertyMetadata(Colors.White, OnPropertyChanged_Color1));

        public Color Color1
        {
            get { return (Color)GetValue(Color1Property); }
            set { SetValue(Color1Property, value); }
        }

        static public void OnPropertyChanged_Color1(object d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardWaveCool2) ((CardWaveCool2)d).OnColor1Changed(e);
        }

        public virtual void OnColor1Changed(DependencyPropertyChangedEventArgs e)
        {
            UpdateBorderBrush();
        }

        #endregion 

        #endregion

        #region Number

        private DispatcherTimer timer;
        private List<Border> borders = new List<Border>();
        private int tick;
        private int tickmax = 120;
        private double opacity0 = 0;
        private double opacity1 = 0.2;
        private double opacity2 = 0;

        #endregion

        #region Method

        protected void InitializeBorders()
        {
            borders.Clear();
            GD_Main.Children.Clear();
            for (int i = 0; i < 4; i++)
            {
                Border border = new Border();
                border.BorderBrush = new LinearGradientBrush(new GradientStopCollection()
                {
                    new GradientStop(Color1, 0.0d),
                    new GradientStop(Color0, 0.0d),
                    new GradientStop(Color1, 0.5d),
                    new GradientStop(Color0, 1.0d),
                    new GradientStop(Color1, 1.0d),
                });
                border.Background = new SolidColorBrush(
                    Color.FromArgb(
                        (byte)(Color1.A * 0.1),
                        Color1.R,
                        Color1.G,
                        Color1.B));
                border.BorderThickness = new Thickness(1.0d);
                border.VerticalAlignment = VerticalAlignment.Stretch;
                border.HorizontalAlignment = HorizontalAlignment.Stretch;
                border.SnapsToDevicePixels = true;
                border.Opacity = 0;
                borders.Add(border);
                GD_Main.Children.Add(border);
            }
        }

        protected void UpdateBorderBrush()
        {
            foreach (Border border in borders)
            {
                LinearGradientBrush lgb = border.BorderBrush as LinearGradientBrush;
                lgb.GradientStops[0].Color = Color1;
                lgb.GradientStops[1].Color = Color0;
                lgb.GradientStops[2].Color = Color1;
                lgb.GradientStops[1].Color = Color0;
                lgb.GradientStops[2].Color = Color1;
                border.Background = new SolidColorBrush(
                    Color.FromArgb(
                        (byte)(Color1.A * 0.1),
                        Color1.R,
                        Color1.G,
                        Color1.B));
            }
        }

        protected Color GetGrad(double r)
        {
            Color c0 = Color0;
            Color c1 = Color1;
            return Color.FromArgb(
                (byte)(c0.A + (c1.A - c0.A) * r),
                (byte)(c0.R + (c1.R - c0.R) * r),
                (byte)(c0.G + (c1.G - c0.G) * r),
                (byte)(c0.B + (c1.B - c0.B) * r));
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
                        this.timer = new DispatcherTimer();
                        timer.Interval = TimeSpan.FromMilliseconds(20);
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
                        this.timer = null;
                    }
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            double aw = ActualWidth;
            double ah = ActualHeight;
            int tick = this.tick;
            int opaspan0 = tickmax * 8 / 10;
            int opaspan1 = tickmax - opaspan0;
            this.tick = (this.tick + 1) % tickmax;
            for (int i = 0; i < borders.Count(); i++)
            {
                Border border = borders[i];
                LinearGradientBrush lgb = border.BorderBrush as LinearGradientBrush;
                double x1 = (0.5 + 2.0 * tick / tickmax) % 1.0;
                double x0 = x1 - 0.5;
                double x2 = x1 + 0.5;
                double rb = 1 - (double)tick / tickmax;
                border.Opacity = (tick < opaspan0)
                    ? (opacity0 + (opacity1 - opacity0) * tick / opaspan0)
                    : (opacity1 + (opacity2 - opacity1) * (tick - opaspan0) / opaspan1);
                border.Margin = new Thickness(aw * rb * 0.49, ah * rb * 0.5, aw * rb * 0.49, ah * rb * 0.5);
                lgb.GradientStops[2].Offset = x2;
                if (x0 < 0)
                {
                    Color c = GetGrad(-x0 * 2);
                    lgb.GradientStops[0].Offset = 0;
                    lgb.GradientStops[0].Color = c;
                    lgb.GradientStops[1].Offset = 0;
                    lgb.GradientStops[1].Color = c;
                }
                else
                {
                    Color c = GetGrad(x0 * 2);
                    lgb.GradientStops[0].Offset = 0;
                    lgb.GradientStops[0].Color = c;
                    lgb.GradientStops[1].Offset = x0;
                    lgb.GradientStops[1].Color = Color0;
                }
                if (x2 > 1)
                {
                    Color c = GetGrad((x2 - 1) * 2);
                    lgb.GradientStops[3].Offset = 1;
                    lgb.GradientStops[3].Color = c;
                    lgb.GradientStops[4].Offset = 1;
                    lgb.GradientStops[4].Color = c;
                }
                else
                {
                    Color c = GetGrad((1 - x2) * 2);
                    lgb.GradientStops[3].Offset = x2;
                    lgb.GradientStops[3].Color = Color0;
                    lgb.GradientStops[4].Offset = 1;
                    lgb.GradientStops[4].Color = c;
                }
                tick = (tick + tickmax / borders.Count()) % tickmax;
            }
        }

        #endregion
    }
}
