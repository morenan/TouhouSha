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
    /// HealAnim.xaml 的交互逻辑
    /// </summary>
    public partial class HealAnim : UserControl
    {
        static public ImageSource Image;

        static public void SetupImageResources(Dictionary<string, ImageSource> imgres)
        {
            string dir_app = App.GetApplicationPath();
            string dir_anim = System.IO.Path.Combine(dir_app, "Images", "Animations");
            string file_heal = System.IO.Path.Combine(dir_anim, "Heal", "Heal");
            Image = imgres[file_heal];
        }

        public HealAnim()
        {
            InitializeComponent();
            InitializeImage();
        }

        private DispatcherTimer timer;
        private int tick;

        protected void InitializeImage()
        {
            if (Image == null) return;
            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
            double dpix = 0.75d;
            double dpiy = 0.75d;
            if (source.CompositionTarget != null)
            {
                dpix *= source.CompositionTarget.TransformFromDevice.M11;
                dpiy *= source.CompositionTarget.TransformFromDevice.M22;
            }
            UI_Image.Source = Image;
            UI_Image.Width = UI_Image.Source.Width * dpix;
            UI_Image.Height = UI_Image.Source.Height * dpiy;
        }

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
            int tickmax = 40;
            double x = ((tick % 20) - 10) * (((tick / 20) & 1) != 0 ? -1.0d : 1.0d);
            double y = -40.0d * tick / tickmax;
            Opacity = tick < 5 ? tick / 5.0d
                : tick >= tickmax - 5 ? (tick - tickmax + 5) / 5.0d
                : 1.0d;
            UI_Image.Margin = new Thickness(x, y, -x, -y);
            if (++tick >= tickmax)
            {
                Visibility = Visibility.Collapsed;
                return;
            }

        }



    }
}
