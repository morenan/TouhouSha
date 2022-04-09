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

namespace TouhouSha.Controls.Cools
{
    /// <summary>
    /// CardWaveCool.xaml 的交互逻辑
    /// </summary>
    public partial class CardWaveCool : UserControl
    {
        public CardWaveCool()
        {
            InitializeComponent();
        }

        #region Number

        private DispatcherTimer timer;
        private Path path0;
        private Path path1;
        private Path path2;
        private List<BezierWave> leftwaves = new List<BezierWave>();
        private List<BezierWave> rightwaves = new List<BezierWave>();
        private List<BezierWave> topwaves = new List<BezierWave>();
        private List<BezierWave> bottomwaves = new List<BezierWave>();

        private double m0 = 6;
        private double m1 = 12;
        private double m2 = 14;
        private double power0 = 1.0;
        private double power1 = 0.8;
        private double power2 = 0.6;
        
        #endregion

        #region Method

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.path0 = GetTemplateChild("PART_CoolPath0") as Path;
            this.path1 = GetTemplateChild("PART_CoolPath1") as Path;
            this.path2 = GetTemplateChild("PART_CoolPath2") as Path;
        }

        protected void DrawBorder(List<BezierWave> wavelist, StreamGeometryContext[] stxs, int direction)
        {
            double aw = ActualWidth;
            double ah = ActualHeight;
            foreach (BezierWave wave in wavelist.ToArray())
            {
                double livepower = 1 - Math.Abs(++wave.Liveline - 50) / 50.0d;
                Point[,] p = new Point[3,7];
                p[0,0] = new Point(m0, m2 + wave.StartOffset - wave.StartBuffer);
                p[1,0] = new Point(m1, m2 + wave.StartOffset - wave.StartBuffer);
                p[2,0] = new Point(m2, m2 + wave.StartOffset - wave.StartBuffer);
                p[0,2] = new Point(m0 - wave.StartPower * livepower * power0, m2 + wave.StartOffset);
                p[1,2] = new Point(m1 - wave.StartPower * livepower * power1, m2 + wave.StartOffset);
                p[2,2] = new Point(m2 - wave.StartPower * livepower * power2, m2 + wave.StartOffset);
                p[0,3] = new Point(m0 - wave.CenterPower * livepower * power0, m2 + wave.CenterOffset);
                p[1,3] = new Point(m1 - wave.CenterPower * livepower * power1, m2 + wave.CenterOffset);
                p[2,3] = new Point(m2 - wave.CenterPower * livepower * power2, m2 + wave.CenterOffset);
                p[0,4] = new Point(m0 - wave.EndPower * livepower * power0, m2 + wave.EndOffset);
                p[1,4] = new Point(m1 - wave.EndPower * livepower * power1, m2 + wave.EndOffset);
                p[2,4] = new Point(m2 - wave.EndPower * livepower * power2, m2 + wave.EndOffset);
                p[0,6] = new Point(m0, m2 + wave.EndOffset + wave.EndBuffer);
                p[1,6] = new Point(m1, m2 + wave.EndOffset + wave.EndBuffer);
                p[2,6] = new Point(m2, m2 + wave.EndOffset + wave.EndBuffer);
                p[0,1] = p[0,2] - (p[0,3] - p[0,2]) * 0.2;
                p[1,1] = p[1,2] - (p[1,3] - p[1,2]) * 0.15;
                p[2,1] = p[2,2] - (p[2,3] - p[2,2]) * 0.1;
                p[0,5] = p[0,4] - (p[0,3] - p[0,4]) * 0.2;
                p[1,5] = p[1,4] - (p[1,3] - p[2,4]) * 0.15;
                p[2,5] = p[1,4] - (p[2,3] - p[2,4]) * 0.1;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 7; j++)
                        switch (direction)
                        {
                            case 1:
                                p[i, j] = new Point(p[i, j].Y, ah - p[i, j].X);
                                break;
                            case 2:
                                p[i, j] = new Point(aw - p[i, j].X, ah - p[i, j].Y);
                                break;
                            case 3:
                                p[i, j] = new Point(aw - p[i, j].Y, p[i, j].X);
                                break;
                        }
                    stxs[i].LineTo(p[i, 0], false, false);
                    stxs[i].BezierTo(p[i, 0], p[i, 1], p[i, 2], false, false);
                    stxs[i].BezierTo(p[i, 2], p[i, 3], p[i, 4], false, false);
                    stxs[i].BezierTo(p[i, 4], p[i, 5], p[i, 6], false, false);
                }
                if (wave.Liveline >= 100) wavelist.Remove(wave);
            }
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
                    this.timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromMilliseconds(10);
                    timer.Tick += Timer_Tick;
                    timer.Start();
                }
                else
                {
                    timer.Stop();
                    timer.Tick -= Timer_Tick;
                    this.timer = null;
                }
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            Clip = new RectangleGeometry(new Rect(sizeInfo.NewSize));
        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            Random random = new Random();
            double aw = ActualWidth;
            double ah = ActualHeight;
            StreamGeometry[] geos = new StreamGeometry[3]
            {
                new StreamGeometry(),
                new StreamGeometry(),
                new StreamGeometry()
            };
            StreamGeometryContext[] stxs = geos.Select(_geo => _geo.Open()).ToArray();
            stxs[0].BeginFigure(new Point(m0, m0), true, true);
            stxs[1].BeginFigure(new Point(m1, m1), true, true);
            stxs[2].BeginFigure(new Point(m2, m2), true, true);
            DrawBorder(leftwaves, stxs, 0);
            stxs[0].LineTo(new Point(m0, ah - m0), false, false);
            stxs[1].LineTo(new Point(m1, ah - m1), false, false);
            stxs[2].LineTo(new Point(m2, ah - m2), false, false);
            DrawBorder(bottomwaves, stxs, 1);
            stxs[0].LineTo(new Point(aw - m0, ah - m0), false, false);
            stxs[1].LineTo(new Point(aw - m1, ah - m1), false, false);
            stxs[2].LineTo(new Point(aw - m2, ah - m2), false, false);
            DrawBorder(rightwaves, stxs, 2);
            stxs[0].LineTo(new Point(aw - m0, m0), false, false);
            stxs[1].LineTo(new Point(aw - m1, m1), false, false);
            stxs[2].LineTo(new Point(aw - m2, m2), false, false);
            DrawBorder(topwaves, stxs, 3);
            stxs[0].Close();
            stxs[1].Close();
            stxs[2].Close();
            path0.Data = geos[0];
            path1.Data = geos[1];
            path2.Data = geos[2];
            foreach (List<BezierWave> wavelist in new List<BezierWave>[] {leftwaves, rightwaves, topwaves, bottomwaves })
            {
                int i = random.Next() % (wavelist.Count() + 1);
                double total = (wavelist == leftwaves || wavelist == rightwaves) ? ah : aw;
                double x0 = i - 1 >= 0 ? (wavelist[i - 1].EndOffset + wavelist[i - 1].EndBuffer) : 0;
                double x6 = i < wavelist.Count() ? (wavelist[i].EndOffset + wavelist[i].EndBuffer) : (total - m2 * 2);
                double x1 = x0 + (x6 - x0) * random.NextDouble();
                if (x6 - x1 >= 24)
                {
                    double len = Math.Max(24, Math.Min(x6 - x1, 60 * random.NextDouble()));
                    double x2 = x1 + len * 0.2;
                    double x3 = x2 + len * 0.4;
                    double x4 = x3 + len * 0.4;
                    double x5 = x4 + len * 0.2;
                    double p0 = len * (0.05 + random.NextDouble() * 0.05);
                    double p1 = p0 + len * (0.05 + random.NextDouble() * 0.05);
                    double p2 = len * (0.05 + random.NextDouble() * 0.05);
                    BezierWave wave = new BezierWave();
                    wave.StartOffset = x2;
                    wave.CenterOffset = x3;
                    wave.EndOffset = x4;
                    wave.StartBuffer = x2 - x1;
                    wave.EndBuffer = x5 - x4;
                    wave.StartPower = p0;
                    wave.CenterPower = p1;
                    wave.EndPower = p2;
                    wavelist.Insert(i, wave);
                }
            }
        }

        #endregion
    }

    public class BezierWave
    {
        public int Liveline;

        public double StartBuffer;

        public double EndBuffer;

        public double StartOffset;

        public double CenterOffset;

        public double EndOffset;

        public double StartPower;

        public double CenterPower;

        public double EndPower;
    }
}
