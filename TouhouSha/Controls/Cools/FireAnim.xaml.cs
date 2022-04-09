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
    /// FireAnim.xaml 的交互逻辑
    /// </summary>
    public partial class FireAnim : UserControl, ICoolAnim
    {
        static public ImageSource ImageMartix;
        static public List<ImageSource> Frames = new List<ImageSource>();

        static public void SetupImageResources(Dictionary<string, ImageSource> imgres)
        {
            string dir_app = App.GetApplicationPath();
            string dir_anim = System.IO.Path.Combine(dir_app, "Images", "Animations");
            string file_fire = System.IO.Path.Combine(dir_anim, "Fire", "Fire");
            ImageMartix = imgres[file_fire];
            BuildFrames();
        }

        unsafe static public void BuildFrames()
        {
            if (!(ImageMartix is BitmapSource)) return;
            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
            double dpix = 0.75d;
            double dpiy = 0.75d;
            if (source.CompositionTarget != null)
            {
                dpix *= source.CompositionTarget.TransformFromDevice.M11;
                dpiy *= source.CompositionTarget.TransformFromDevice.M22;
            }
            BitmapFrame bitmap = (BitmapFrame)(ImageMartix);
            byte[] data = new byte[bitmap.PixelWidth * (bitmap.PixelHeight + 1) * 4];
            int stride = bitmap.PixelWidth * 4;
            bitmap.CopyPixels(data, stride, 0);
            int bw0 = bitmap.PixelWidth / 4;
            int bh0 = bitmap.PixelHeight / 4;
            for (int biy = 0; biy < 4; biy++) 
                for (int bix = 0; bix < 4; bix++)
                {
                    int bx = bix * bw0;
                    int by = biy * bh0;
                    int bw = Math.Min(bitmap.PixelWidth - bx, bw0);
                    int bh = Math.Min(bitmap.PixelWidth - bx, bh0);
                    fixed (byte* p = &data[by * stride + bx * 4])
                        Frames.Add(BitmapSource.Create(bw, bh, bitmap.DpiX / dpix, bitmap.DpiY / dpiy, bitmap.Format, bitmap.Palette,
                            (IntPtr)p, stride * bh, stride));
                }
        }

        public FireAnim()
        {
            InitializeComponent();
        }

        private DispatcherTimer timer;
        private int tick = 0;

        public void AnimationStart()
        {
            tick = 0;
            Visibility = Visibility.Visible;
        }

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
                        timer.Interval = TimeSpan.FromMilliseconds(50);
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
            if (tick >= Frames.Count())
                Visibility = Visibility.Collapsed;
            else 
                UI_Image.Source = Frames[tick++];
        }
    }
}
