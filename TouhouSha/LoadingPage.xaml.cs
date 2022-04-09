using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
using TouhouSha.Core;
using TouhouSha.Controls.Cools;

namespace TouhouSha
{
    /// <summary>
    /// LoadingPage.xaml 的交互逻辑
    /// </summary>
    public partial class LoadingPage : UserControl
    {
        public LoadingPage()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        #region Number

        private MainWindow mw;
        private DispatcherTimer timer;
        private double progvalue = 0;
        private string message = "少女祈祷中...";

        #endregion

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.mw = App.FindAncestor<MainWindow>(this);
        }

        public void LoadPrimaryResources(List<string> imgfiles)
        {
            progvalue = 0;
            message = "少女祈祷中...";
            Task.Factory.StartNew(() =>
            {
                Dictionary<string, ImageSource> imgres = new Dictionary<string, ImageSource>();
                int imgdone = 0;
                foreach (string imgfile in imgfiles)
                {
                    string imgkey = System.IO.Path.GetFileNameWithoutExtension(imgfile);
                    ImageSource imgsrc = ImageHelper.CreateImage(imgfile);
                    if (imgsrc is BitmapSource)
                    {
                        BitmapSource bitmap = (BitmapSource)imgsrc;
                        int x = 0;
                        int y = 0;
                        int w = bitmap.PixelWidth;
                        int h = bitmap.PixelHeight;
                        byte[] data = new byte[w * h * 4];
                        bitmap.CopyPixels(new Int32Rect(x, y, w, h), data, w * 4, 0);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            BitmapSource newbitmap = BitmapSource.Create(w, h, bitmap.DpiX * 2, bitmap.DpiY * 2, bitmap.Format, bitmap.Palette, data, w * 4);
                            imgsrc = newbitmap;
                        });
                    }
                    imgres.Add(imgkey, imgsrc);
                    progvalue = (double)++imgdone / imgfiles.Count();
                }
                progvalue = 1.0d;
                do { Thread.Sleep(200); } while (mw == null);
                Dispatcher.Invoke(() =>
                {
                    FireAnim.SetupImageResources(imgres);
                    ThunderAnim.SetupImageResources(imgres);
                    HealAnim.SetupImageResources(imgres);
                    mw.UI_StartMenu.SetupImageResources(imgres);
                    mw.UI_StartMenu.Floor = Enum_StartMenuFloor.Root;
                    Visibility = Visibility.Collapsed;
                });
            });


        }

        public void LoadCharactorGallery(CharactorGallery ui_gallery)
        {
            progvalue = 0;
            Task.Factory.StartNew(() =>
            {
                if (!App.IsCharactorsAllLoaded)
                    App.LoadPackages(
                        (_callback_value) => { progvalue = 1.0 * _callback_value; },
                        (_message) => { message = _message; },
                        true, false);
                List<Charactor> list = new List<Charactor>();
                int pi = 0;
                foreach (IPackage package in App.PackageList)
                    list.AddRange(package.GetCharactors());
                progvalue = 1;
                Thread.Sleep(200);
                Dispatcher.Invoke(() =>
                {
                    ui_gallery.Charactors = list;
                    ui_gallery.Visibility = Visibility.Visible;
                    Visibility = Visibility.Collapsed;
                    mw.PlaySound(Enum_SoundUsage.Gallery);
                });
            });
        }

        public void LoadCardGallery(CardGallery ui_card)
        {
            progvalue = 0;
            Task.Factory.StartNew(() =>
            {
                if (!App.IsCardsAllLoaded)
                    App.LoadPackages(
                        (_callback_value) => { progvalue = 1.0 * _callback_value; },
                        (_message) => { message = _message; },
                        false, true);
                List<Card> list = new List<Card>();
                foreach (IPackage package in App.PackageList)
                    list.AddRange(package.GetCardInstances());
                progvalue = 1;
                Thread.Sleep(200);
                Dispatcher.Invoke(() =>
                {
                    ui_card.Cards = list;
                    ui_card.Visibility = Visibility.Visible;
                    Visibility = Visibility.Collapsed;
                    mw.PlaySound(Enum_SoundUsage.Gallery);
                });
            });
        }

        public void LoadGame(GameBoard ui_gameboard)
        {
            
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            IM_Background.Source = App.CreateImage(System.IO.Path.Combine(App.GetApplicationPath(), "Images", "Loading"));
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsVisibleProperty)
            {
                if (IsVisible)
                {
                    this.timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromMilliseconds(20);
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
            UI_ProgressBar.Value = progvalue;
            BT_Message.Text = message;
        }
    }

    [ValueConversion(typeof(double), typeof(string))]
    public class GetProgBarShowText : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is double)) return "0 %";
            return (int)((double)value * 100) + " %";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 0.0d;
        }
    }
    
    public class GetProgBarPartClip : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3) return new RectangleGeometry(Rect.Empty);
            for (int i = 0; i < 3; i++) if (!(values[i] is double)) return new RectangleGeometry(Rect.Empty);
            double v = (double)(values[0]);
            double aw = (double)(values[1]);
            double ah = (double)(values[2]);
            return new RectangleGeometry(new Rect(0, 0, v * aw, ah));
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { 0.0d, 0.0d, 0.0d };
        }
    }
}
