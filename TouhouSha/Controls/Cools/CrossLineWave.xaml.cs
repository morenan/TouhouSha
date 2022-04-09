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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TouhouSha.Controls.Cools
{
    /// <summary>
    /// CrossLineWave.xaml 的交互逻辑
    /// </summary>
    public partial class CrossLineWave : Canvas
    {
        public CrossLineWave()
        {
            InitializeComponent();
            Initialize();
            StartPoint = new Point(0, 0);
            EndPoint = new Point(300, 300);
            AnimationStart();
        }

        #region Properties

        #region StartPoint

        static public readonly DependencyProperty StartPointProperty = DependencyProperty.Register(
            "StartPoint", typeof(Point), typeof(CrossLineWave),
            new PropertyMetadata(default(Point)));
       
        public Point StartPoint
        {
            get { return (Point)GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        #endregion

        #region EndPoint

        static public readonly DependencyProperty EndPointProperty = DependencyProperty.Register(
            "EndPoint", typeof(Point), typeof(CrossLineWave),
            new PropertyMetadata(default(Point)));

        public Point EndPoint
        {
            get { return (Point)GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }

        #endregion

        #region BaseColor

        static public readonly DependencyProperty BaseColorProperty = DependencyProperty.Register(
            "BaseColor", typeof(Color), typeof(CrossLineWave),
            new PropertyMetadata(Colors.White));

        public Color BaseColor
        {
            get { return (Color)GetValue(BaseColorProperty); }
            set { SetValue(BaseColorProperty, value); }
        }

        #endregion 

        #endregion

        #region Number

        private DispatcherTimer timer;
        private int tick = 40;
        private int tickmax = 40;
        private int tickfullline = 10;
        private int tickfadein = 5;
        private int tickfadeout = 5;
        private Rectangle primaryline;
        private List<Path> wavepaths = new List<Path>();
        private List<List<double>> wavedatas = new List<List<double>>();
        private double zipzapspan = 120;
        private double phasespan = 120;
        private double wavethickness = 1;
        private double shakepower = 8;
        private double shakepowerplus = 0;
        private double pathdelayfactor = 0.02d;
        private int phasecount = 2;
        
        #endregion

        #region Method

        protected void Initialize()
        {
            this.primaryline = new Rectangle();
            primaryline.Height = 4;
            primaryline.Fill = GetFill(0);
            Children.Add(primaryline);
            for (int i = 0; i < 4; i++) 
                for (int j = 0; j < 3; j++)
                {
                    Path path = new Path();
                    path.Fill = GetFill(j);
                    path.Effect = GetEffect(j);
                    wavepaths.Add(path);
                    Children.Add(path);
                }
        }

        protected Brush GetFill(int fadelevel)
        {
            return new SolidColorBrush(GetColor(fadelevel));
        }

        protected Color GetColor(int fadelevel)
        {
            switch (fadelevel)
            {
                case 0: return Color.FromArgb((byte)(BaseColor.A * 0.4), BaseColor.R, BaseColor.G, BaseColor.B);
                case 1: return Color.FromArgb((byte)(BaseColor.A * 0.2), BaseColor.R, BaseColor.G, BaseColor.B);
                case 2: return Color.FromArgb((byte)(BaseColor.A * 0.1), BaseColor.R, BaseColor.G, BaseColor.B);
                case 3: return Color.FromArgb(0, BaseColor.R, BaseColor.G, BaseColor.B);
            }
            return BaseColor;

        }

        protected Effect GetEffect(int fadelevel)
        {
            return new BlurEffect()
            {
                Radius = fadelevel == 1 ? 8 : fadelevel == 2 ? 15 : 2
            };
        }
 
        protected Point GetBezierPoint(List<double> datas, double x, double f, int fadelevel)
        {
            Vector v = EndPoint - StartPoint;
            v.Normalize();
            Vector lawer = new Vector(v.Y, -v.X);
            int zi = (int)(x / zipzapspan);
            double zo = x % zipzapspan;
            Point p0 = StartPoint + v * (zi * zipzapspan);
            Point p1 = StartPoint + v * ((zi + 1) * zipzapspan);
            if (zi + 1 >= datas.Count()) p1 = EndPoint;
            if (zi >= 0 && zi < datas.Count()) p0 += lawer * datas[zi];
            if (zi + 1 >= 0 && zi + 1 < datas.Count()) p1 += lawer * datas[zi + 1];
            Point p2 = p0 + (p1 - p0) * zo / zipzapspan;
            p2 += lawer * f * wavethickness * (fadelevel == 1 ? 1.5d : fadelevel == 2 ? 2.0d : 1.0d);
            return p2;
        }

        protected Point GetBezierPoint2To3(Point c0, Point p0, Point c1, bool isc1)
        {
            Vector pv0 = p0 - c0;
            Vector pv1 = c1 - p0;
            Vector cv = c1 - c0;
            if (isc1)
                return p0 + cv * pv1.Length / (pv0.Length + pv1.Length);
            else
                return p0 - cv * pv0.Length / (pv0.Length + pv1.Length);
        }

        public void AnimationStart()
        {
            Random random = new Random();
            Vector v = EndPoint - StartPoint;
            primaryline.Width = v.Length;
            primaryline.RenderTransform = new RotateTransform(Vector.AngleBetween(new Vector(1, 0), v));
            Canvas.SetLeft(primaryline, StartPoint.X);
            Canvas.SetTop(primaryline, StartPoint.Y);
            wavedatas.Clear();
            for (int i = 0; i < wavepaths.Count(); i += 3)
            {
                List<double> data = new List<double>();
                double phasefactor = (((i & 1) != 0) ? 1 : -1);
                for (double x = zipzapspan; x < v.Length; x += zipzapspan)
                {
                    data.Add(phasefactor * (shakepower + (i / 3) * shakepowerplus) * random.NextDouble());
                    phasefactor *= -1;
                }
                wavedatas.Add(data);
            }
            tick = 0;
            Visibility = Visibility.Visible;
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

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (tick >= tickmax)
            {
                Visibility = Visibility.Collapsed;
                return;
            }
            tick++;
            if (tick < tickfadein)
                Opacity = (double)tick / tickfadein;
            else if (tick >= tickmax - tickfadeout)
                Opacity = (double)(tickmax - tick) / tickfadeout;
            else
                Opacity = 1;
            
            Vector v = EndPoint - StartPoint;
            //double x1 = v.Length * tick / tickmax;
            LinearGradientBrush linegrad = new LinearGradientBrush();
            double offset_end = Math.Min(1, (double)tick / tickfullline);
            double offset_endbuf = Math.Min(1, offset_end + 0.1);
            double offset_start = (double)tick / tickmax;
            double offset_startbuf = Math.Max(0, offset_start - 0.1);
            Color color = GetColor(0);
            Color color_fadeout = GetColor(3);
            linegrad.StartPoint = new Point(0, 0);
            linegrad.EndPoint = new Point(1, 0);
            linegrad.GradientStops.Add(new GradientStop(color_fadeout, offset_startbuf));
            linegrad.GradientStops.Add(new GradientStop(color, offset_start));
            linegrad.GradientStops.Add(new GradientStop(color, offset_end));
            linegrad.GradientStops.Add(new GradientStop(color_fadeout, offset_endbuf));
            primaryline.Fill = linegrad;
            
            for (int i = 0; i < wavepaths.Count(); i += 3)
            {
                StreamGeometry[] geos = new StreamGeometry[] { new StreamGeometry(), new StreamGeometry(), new StreamGeometry() };
                StreamGeometryContext[] stxs = geos.Select(_geo => _geo.Open()).ToArray();
                List<double> datas = wavedatas[i / 3]; 
                for (int j = 0; j < 3; j++)
                {
                    int pi = 0;
                    double x = v.Length * ((double)tick / tickmax - pathdelayfactor * i);
                    double tailweight = 1;
                    List<Point> bezier2s = new List<Point>();
                    List<Point> bezier3s = new List<Point>();
                    if (x < 0)
                    {
                        stxs[j].Close();
                        wavepaths[i + j].Data = geos[j];
                        continue;
                    }
                    for (; pi < phasecount && x >= 0; pi++, x -= phasespan)
                    {
                        double _x2 = x;
                        double _x0 = x - phasespan;
                        if (x < phasespan)
                        {
                            tailweight = x / phasespan;
                            _x0 = 0;
                        }
                        double _x1 = (_x0 + _x2) / 2;
                        Point _p0 = GetBezierPoint(datas, _x0, (phasecount - pi - 0.0d) / phasecount, j);
                        Point _p1 = GetBezierPoint(datas, _x1, (phasecount - pi - 0.5d) / phasecount, j);
                        Point _p2 = GetBezierPoint(datas, _x2, (phasecount - pi - 1.0d) / phasecount, j);
                        bezier2s.Add(_p0);
                        bezier2s.Add(_p1);
                        bezier2s.Add(_p2);
                        //if (pi == 0) stxs[j].BeginFigure(_p0, true, true);
                        //stxs[j].BezierTo(_p0, _p1, _p2, false, false);
                    }
                    int pi_max = pi - 1;
                    for (pi = 0; pi < bezier2s.Count(); pi += 3)
                    {
                        Point p0 = bezier2s[pi];
                        Point p1 = bezier2s[pi + 1];
                        Point p2 = bezier2s[pi + 2];
                        bezier3s.Add(p0);
                        if (pi - 2 >= 0)
                            bezier3s.Add(GetBezierPoint2To3(bezier2s[pi - 2], p0, p1, true));
                        else
                            bezier3s.Add(p1);
                        if (pi + 4 < bezier2s.Count() && pi + 6 >= bezier2s.Count())
                            bezier3s.Add(p1 + (GetBezierPoint2To3(p1, p2, bezier2s[pi + 4], false) - p1) * tailweight);
                        else if (pi + 4 < bezier2s.Count())
                            bezier3s.Add(GetBezierPoint2To3(p1, p2, bezier2s[pi + 4], false));
                        else
                            bezier3s.Add(p1);
                        bezier3s.Add(p2);
                    }
                    for (pi = 0; pi < bezier3s.Count(); pi += 4)
                    {
                        if (pi == 0) stxs[j].BeginFigure(bezier3s[0], true, true);
                        stxs[j].BezierTo(bezier3s[pi + 1], bezier3s[pi + 2], bezier3s[pi + 3], false, false);
                    }
                    bezier2s.Clear();
                    bezier3s.Clear();
                    for (pi = pi_max, x += phasespan; pi >= 0; pi--, x += phasespan)
                    {
                        double _x2 = x;
                        double _x0 = x - phasespan;
                        if (x < phasespan) _x0 = 0;
                        double _x1 = (_x0 + _x2) / 2;
                        Point _p0 = GetBezierPoint(datas, _x0, -(phasecount - pi - 0.0d) / phasecount, j);
                        Point _p1 = GetBezierPoint(datas, _x1, -(phasecount - pi - 0.5d) / phasecount, j);
                        Point _p2 = GetBezierPoint(datas, _x2, -(phasecount - pi - 1.0d) / phasecount, j);
                        bezier2s.Add(_p2);
                        bezier2s.Add(_p1);
                        bezier2s.Add(_p0);
                    }
                    for (pi = 0; pi < bezier2s.Count(); pi += 3)
                    {
                        Point p0 = bezier2s[pi];
                        Point p1 = bezier2s[pi + 1];
                        Point p2 = bezier2s[pi + 2];
                        bezier3s.Add(p0);
                        if (pi - 2 >= 0 && pi - 6 < 0)
                            bezier3s.Add(p1 + (GetBezierPoint2To3(bezier2s[pi - 2], p0, p1, true) - p1) * tailweight);
                        else if (pi - 2 >= 0)
                            bezier3s.Add(GetBezierPoint2To3(bezier2s[pi - 2], p0, p1, true));
                        else
                            bezier3s.Add(p1);
                        if (pi + 4 < bezier2s.Count())
                            bezier3s.Add(GetBezierPoint2To3(p1, p2, bezier2s[pi + 4], false));
                        else
                            bezier3s.Add(p1);
                        bezier3s.Add(p2);
                    }
                    for (pi = 0; pi < bezier3s.Count(); pi += 4)
                        stxs[j].BezierTo(bezier3s[pi + 1], bezier3s[pi + 2], bezier3s[pi + 3], false, false);
                    stxs[j].Close();
                    wavepaths[i + j].Data = geos[j];
                }
            }
        }

        #endregion




    }
}
