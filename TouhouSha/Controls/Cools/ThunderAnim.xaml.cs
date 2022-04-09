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
    /// ThunderAnim.xaml 的交互逻辑
    /// </summary>
    public partial class ThunderAnim : UserControl, ICoolAnim
    {
        static public List<ImageSource> Frames = new List<ImageSource>();

        static public void SetupImageResources(Dictionary<string, ImageSource> imgres)
        {
            string dir_app = App.GetApplicationPath();
            string dir_anim = System.IO.Path.Combine(dir_app, "Images", "Animations");
            Frames.Clear();
            for (int i = 0; i < 9; i++)
            {
                string file_thunder = System.IO.Path.Combine(dir_anim, "Thunder", i.ToString());
                Frames.Add(imgres[file_thunder]);
            }
        }

        public ThunderAnim()
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
            {
                Visibility = Visibility.Collapsed;
                return;
            }
            
            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
            double dpix = 0.75d;
            double dpiy = 0.75d;
            if (source.CompositionTarget != null)
            {
                dpix *= source.CompositionTarget.TransformFromDevice.M11;
                dpiy *= source.CompositionTarget.TransformFromDevice.M22;
            }
            UI_Image.Source = Frames[tick++];
            UI_Image.Width = UI_Image.Source.Width * dpix;
            UI_Image.Height = UI_Image.Source.Height * dpiy;
            if (tick >= Frames.Count()) tick = 0;
        }
    }
}
