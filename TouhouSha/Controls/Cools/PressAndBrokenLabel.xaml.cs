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
using System.Windows.Media.Effects;

namespace TouhouSha.Controls.Cools
{
    /// <summary>
    /// PressAndBrokenLabel.xaml 的交互逻辑
    /// </summary>
    public partial class PressAndBrokenLabel : UserControl
    {
        public const int Item_Span = 20;
        public const int Item_FadeIn = 20;
        public const int Item_Stay = 100;
        public const int Item_FadeOut = 40;

        public const double Light_BlurMin = 10;
        public const double Light_BlurMax = 15;
        public const double Light_BlurSpan = 50;

        public const int Broken_FragSize = 12;
        
        public class ImageObject
        {
            public ImageSource Source { get; set; }
            public Rect Rect { get; set; }
        }

        public class FragmentBroken
        {
            public Vector Move { get; set; }
            public double RotateX { get; set; }
            public double RotateY { get; set; }
        }
       

        public PressAndBrokenLabel()
        {
            InitializeComponent();
        }

        #region Properties

        #region ImageSource

        static public readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            "ImageSource", typeof(ImageSource), typeof(PressAndBrokenLabel),
            new PropertyMetadata(default(ImageSource), OnPropertyChanged_ImageSource));

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        private static void OnPropertyChanged_ImageSource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PressAndBrokenLabel) ((PressAndBrokenLabel)d).OnImageSourceChanged(e);
        }

        protected virtual void OnImageSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (ImageSource == null) return;
            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
            double dpix = 0.75d;
            double dpiy = 0.75d;
            if (source.CompositionTarget != null)
            {
                dpix *= source.CompositionTarget.TransformFromDevice.M11;
                dpiy *= source.CompositionTarget.TransformFromDevice.M22;
            }
            GD_Label.Width = ImageSource.Width * dpix;
            GD_Label.Height = ImageSource.Height * dpix;
            if (ImageSource is BitmapSource)
            {
                this.items = SplitImage(ImageSource as BitmapSource);
                this.frags = SplitFrags(ImageSource as BitmapSource);
                while (images.Count() < items.Count())
                {
                    Image image = new Image();
                    image.Stretch = Stretch.None;
                    image.Visibility = Visibility.Hidden;
                    image.Effect = new BlurEffect() { Radius = 0 };
                    image.RenderTransform = new MatrixTransform();
                    images.Add(image);
                    UI_Images.Children.Add(image);
                }
                while (imagefrags.Count() < frags.Count())
                {
                    Image image = new Image();
                    image.Stretch = Stretch.None;
                    image.Visibility = Visibility.Hidden;
                    image.RenderTransform = new MatrixTransform();
                    imagefrags.Add(image);
                    UI_Frags.Children.Add(image);
                }
                for (int i = 0; i < items.Count(); i++)
                    images[i].Source = items[i].Source;
                for (int i = items.Count(); i < images.Count(); i++)
                    images[i].Source = null;
                for (int i = 0; i < frags.Count(); i++)
                    imagefrags[i].Source = frags[i].Source;
                for (int i = frags.Count(); i < imagefrags.Count(); i++)
                    imagefrags[i].Source = null;
            }
        }

        #endregion

        #region LightSource

        static public readonly DependencyProperty LightSourceProperty = DependencyProperty.Register(
            "LightSource", typeof(ImageSource), typeof(PressAndBrokenLabel),
            new PropertyMetadata(default(ImageSource), OnPropertyChanged_LightSource));

        public ImageSource LightSource
        {
            get { return (ImageSource)GetValue(LightSourceProperty); }
            set { SetValue(LightSourceProperty, value); }
        }

        private static void OnPropertyChanged_LightSource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PressAndBrokenLabel) ((PressAndBrokenLabel)d).OnLightSourceChanged(e);
        }

        protected virtual void OnLightSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            if (LightSource is BitmapSource)
            {
                this.lights = SplitImage(LightSource as BitmapSource);
                while (imagelights.Count() < lights.Count())
                {
                    Image image = new Image();
                    image.Stretch = Stretch.None;
                    image.Visibility = Visibility.Hidden;
                    image.Effect = new BlurEffect() { Radius = 0 };
                    image.RenderTransform = new MatrixTransform();
                    imagelights.Add(image);
                    UI_Light.Children.Add(image);
                }
                for (int i = 0; i < lights.Count(); i++)
                    imagelights[i].Source = lights[i].Source;
                for (int i = lights.Count(); i < imagelights.Count(); i++)
                    imagelights[i].Source = null;
            }

        }

        #endregion

        #region EnglishText

        static public readonly DependencyProperty EnglishTextProperty = DependencyProperty.Register(
            "EnglishText", typeof(string), typeof(PressAndBrokenLabel),
            new PropertyMetadata("Battle Start!"));

        public string EnglishText
        {
            get { return (string)GetValue(EnglishTextProperty); }
            set { SetValue(EnglishTextProperty, value); }
        }

        #endregion

        #endregion

        #region Number

        private MainWindow mw;
        private DispatcherTimer timer;
        private List<ImageObject> items;
        private List<ImageObject> lights;
        private List<ImageObject> frags;
        private List<Image> images = new List<Image>();
        private List<Image> imagelights = new List<Image>();
        private List<Image> imagefrags = new List<Image>();
        private List<FragmentBroken> fragbks = new List<FragmentBroken>();
        private LinearGradientBrush textbrush;
        private LinearGradientBrush textbrushhl;
        private int tick = 0;
        private bool isactive = false;

        #endregion

        #region Method

        public void AnimationStart()
        {
            Random random = new Random();
            double aw = ImageSource.Width;
            double ah = ImageSource.Height;
            Point center = new Point(aw / 2, ah / 2);
            while (fragbks.Count() < frags.Count())
            {
                FragmentBroken fragbk = new FragmentBroken();
                fragbks.Add(fragbk);
            }
            for (int i = 0; i < frags.Count(); i++)
            {
                ImageObject frag = frags[i];
                FragmentBroken fragbk = fragbks[i];
                Vector power = new Vector(
                    frag.Rect.X + frag.Rect.Width / 2 - center.X,
                    frag.Rect.Y + frag.Rect.Height / 2 - center.Y);
                fragbk.Move = new Vector(
                    (power.X - (random.NextDouble() - 0.5) * 240) / aw * 32,
                    (power.Y - (random.NextDouble() - 0.5) * 240) / ah * 32);
                fragbk.RotateX = (power.X - (random.NextDouble() - 0.5) * 90) / aw * 720;
                fragbk.RotateY = (power.Y - (random.NextDouble() - 0.5) * 90) / ah * 720;
            }
            foreach (Image image in images) image.Visibility = Visibility.Hidden;
            foreach (Image image in imagelights) image.Visibility = Visibility.Hidden;
            foreach (Image image in imagefrags) image.Visibility = Visibility.Hidden;
            UI_Frags_Effect.Radius = 0;
            UI_Frags_Transform.ScaleX = 1;
            UI_Frags_Transform.ScaleY = 1;
            UI_Frags.Opacity = 1;
            textbrush = new LinearGradientBrush
            (
                new GradientStopCollection()
                {
                    new GradientStop(Color.FromArgb(255, 255, 128, 128), 0),
                    new GradientStop(Color.FromArgb(0, 255, 128, 128), 0)
                }
            )
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0)
            };
            textbrushhl = new LinearGradientBrush
            (
                new GradientStopCollection()
                {
                    new GradientStop(Color.FromArgb(255, 255, 255, 255), 0),
                    new GradientStop(Color.FromArgb(0, 255, 255, 255), 0)
                }
            )
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0)
            };
            UI_Text.Foreground = textbrush;
            UI_Text.HighlightForeground = textbrushhl;
            UI_Text.Opacity = 1;
            BD_SeperateLine.BorderBrush = textbrushhl;
            BD_SeperateLine.Opacity = 1;
            tick = 0;
            isactive = true;
            Visibility = Visibility.Visible;
        }

        unsafe
        protected ImageObject CreateObject(BitmapSource bitmap, byte[] data, Int32Rect rect)
        {
            PresentationSource source = PresentationSource.FromVisual(Application.Current.MainWindow);
            double dpix = 0.75d;
            double dpiy = 0.75d;
            if (source.CompositionTarget != null)
            {
                dpix *= source.CompositionTarget.TransformFromDevice.M11;
                dpiy *= source.CompositionTarget.TransformFromDevice.M22;
            }
            int stride = bitmap.PixelWidth * 4;
            ImageObject obj = new ImageObject();
            fixed (byte* p = &data[rect.Y * stride + rect.X * 4])
                obj.Source = BitmapSource.Create(rect.Width, rect.Height, bitmap.DpiX / dpix, bitmap.DpiY / dpiy, bitmap.Format, bitmap.Palette,
                    (IntPtr)p, stride * rect.Height, stride);
            obj.Rect = new Rect(
                dpix * rect.X * bitmap.Width / bitmap.PixelWidth,
                dpiy * rect.Y * bitmap.Height / bitmap.PixelHeight,
                dpix * rect.Width * bitmap.Width / bitmap.PixelWidth,
                dpiy * rect.Height * bitmap.Height / bitmap.PixelHeight);
            return obj;
        }

        protected List<ImageObject> SplitImage(BitmapSource bitmap)
        {
            List<ImageObject> list = new List<ImageObject>();
            byte[] data = new byte[bitmap.PixelWidth * (bitmap.PixelHeight + 1) * 4];
            int x0 = 0;
            bitmap.CopyPixels(data, bitmap.PixelWidth * 4, 0);
            for (int x = 0; x < bitmap.PixelWidth; x++)
            {
                bool columnvisible = false;
                for (int y = 0; y < bitmap.PixelHeight; y++)
                {
                    byte a = data[(y * bitmap.PixelWidth + x) * 4 + 3];
                    if (a != 0) columnvisible = true;
                    if (columnvisible) break;
                }
                if (!columnvisible)
                {
                    if (x0 < x)
                        list.Add(CreateObject(bitmap, data, new Int32Rect(x0, 0, x - x0, bitmap.PixelHeight)));
                    x0 = x + 1;
                }
            }
            if (x0 < bitmap.PixelWidth)
                list.Add(CreateObject(bitmap, data, new Int32Rect(x0, 0, bitmap.PixelWidth - x0, bitmap.PixelHeight)));
            return list;
        }

        protected List<ImageObject> SplitFrags(BitmapSource bitmap)
        {
            List<ImageObject> list = new List<ImageObject>();
            int bw = bitmap.PixelWidth / Broken_FragSize;
            int bh = bitmap.PixelHeight / Broken_FragSize;
            byte[] data = new byte[bitmap.PixelWidth * (bitmap.PixelHeight + 1) * 4];
            int stride = bitmap.PixelWidth * 4;
            bitmap.CopyPixels(data, stride, 0);
            if (bitmap.PixelWidth % Broken_FragSize != 0) bw++;
            if (bitmap.PixelHeight % Broken_FragSize != 0) bh++;
            for (int bx = 0; bx < bw; bx++)
                for (int by = 0; by < bh; by++)
                {
                    int x = bx * Broken_FragSize;
                    int y = by * Broken_FragSize;
                    int w = Math.Min(Broken_FragSize, bitmap.PixelWidth - x);
                    int h = Math.Min(Broken_FragSize, bitmap.PixelHeight - y);
                    bool visible = false;
                    for (int ix = x; ix < x + w; ix++)
                    {
                        for (int iy = y; iy < y + h; iy++)
                        {
                            byte a = data[iy * stride + ix * 4 + 3];
                            if (a != 0) visible = true;
                            if (visible) break;
                        }
                        if (visible) break;
                    }
                    if (visible) list.Add(CreateObject(bitmap, data, new Int32Rect(x, y, w, h)));
                }
            return list;
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
                        timer.Interval = TimeSpan.FromMilliseconds(10);
                        timer.Tick += Timer_Tick;
                        timer.Start();
                    }
                }
                else
                {
                    if (timer == null)
                    {
                        timer.Tick -= Timer_Tick;
                        timer.Stop();
                        this.timer = null;
                    }
                }
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!isactive) return;
            if (mw == null) mw = Application.Current.MainWindow as MainWindow;
            int span0 = Item_Span * (items.Count() - 1) + Item_FadeIn;
            int span1 = Item_Stay;
            int span2 = Item_FadeOut;
            tick++;
            #region 放置图片位置
            for (int i = 0; i < items.Count(); i++)
            {
                ImageObject item = items[i];
                ImageObject lightitem = items[i];
                Image image = images[i];
                Image light = imagelights[i];
                Canvas.SetLeft(image, item.Rect.X);
                Canvas.SetLeft(light, lightitem.Rect.X);
                Canvas.SetTop(image, item.Rect.Y);
                Canvas.SetTop(light, lightitem.Rect.Y);
            }
            #endregion
            #region 放置碎片位置
            for (int i = 0; i < frags.Count(); i++)
            {
                ImageObject fragitem = frags[i];
                Image frag = imagefrags[i];
                Canvas.SetLeft(frag, fragitem.Rect.X);
                Canvas.SetTop(frag, fragitem.Rect.Y);
            }
            #endregion
            #region 过渡显现动画
            if (tick <= span0)
            {
                int tickstart = 0;
                {
                    double r = (double)tick / span0;
                    textbrush.GradientStops[0].Offset = r;
                    textbrush.GradientStops[1].Offset = Math.Min(r + 0.1, 1.0);
                    textbrushhl.GradientStops[0].Offset = r;
                    textbrushhl.GradientStops[1].Offset = Math.Min(r + 0.1, 1.0);
                }
                for (int i = 0; i < items.Count(); i++)
                {
                    ImageObject item = items[i];
                    ImageObject lightitem = items[i];
                    Image image = images[i];
                    Image light = imagelights[i];
                    BlurEffect image_effect = image.Effect as BlurEffect;
                    BlurEffect light_effect = light.Effect as BlurEffect;
                    MatrixTransform image_tran = image.RenderTransform as MatrixTransform;
                    MatrixTransform light_tran = light.RenderTransform as MatrixTransform;
                    Canvas.SetLeft(image, item.Rect.X);
                    Canvas.SetLeft(light, lightitem.Rect.X);
                    Canvas.SetTop(image, item.Rect.Y);
                    Canvas.SetTop(light, lightitem.Rect.Y);
                    if (tick >= tickstart + Item_FadeIn)
                    {
                        Matrix matrix = Matrix.Identity;
                        image.Visibility = Visibility.Visible;
                        light.Visibility = Visibility.Visible;
                        image.Opacity = 1;
                        light.Opacity = 1;
                        image_effect.Radius = 0;
                        light_effect.Radius = Light_BlurMin;
                        image_tran.Matrix = matrix;
                        light_tran.Matrix = matrix;
                    }
                    else if (tick >= tickstart)
                    {
                        double r = Math.Pow(1 - Math.Pow((double)(tickstart + Item_FadeIn - tick) / Item_FadeIn, 5), 0.2);
                        Matrix matrix = Matrix.Identity;
                        matrix.Translate(-30 * (1 - r), 30 * (1 - r));
                        matrix.ScaleAt(2 - r, 2 - r, image.ActualWidth / 2, image.ActualHeight / 2);
                        image.Visibility = Visibility.Visible;
                        light.Visibility = Visibility.Visible;
                        image.Opacity = r;
                        light.Opacity = r;
                        image_effect.Radius = (1 - r) * 10;
                        light_effect.Radius = (2 - r) * Light_BlurMin;
                        image_tran.Matrix = matrix;
                        light_tran.Matrix = matrix;
                        if (tick == tickstart + 1) mw.PlaySE(Enum_SEUsage.Drop);
                    }
                    else
                    {
                        image.Visibility = Visibility.Hidden;
                        light.Visibility = Visibility.Hidden;
                        image.Opacity = 0;
                        light.Opacity = 0;
                    }
                    tickstart += Item_Span;
                }
            }
            #endregion
            #region 显现保持动画
            else if (tick <= span0 + span1)
            {
                int tickstart = span0;
                double r = (double)(Light_BlurSpan / 2 - Math.Abs(Light_BlurSpan / 2 - (tick - tickstart) % Light_BlurSpan)) / (Light_BlurSpan / 2);
                for (int i = 0; i < items.Count(); i++)
                {
                    ImageObject item = items[i];
                    ImageObject lightitem = items[i];
                    Image image = images[i];
                    Image light = imagelights[i];
                    BlurEffect image_effect = image.Effect as BlurEffect;
                    BlurEffect light_effect = light.Effect as BlurEffect;
                    light_effect.Radius = Light_BlurMin + r * (Light_BlurMax - Light_BlurMin);
                }
            }
            #endregion
            #region 破散动画
            else if (tick <= span0 + span1 + span2)
            {
                int tickstart = span0 + span1;
                double r = (double)(tick - tickstart) / span2;
                if (tick == tickstart + 1) mw.PlaySE(Enum_SEUsage.Broken);
                UI_Frags_Transform.ScaleX = 1 + r * 2;
                UI_Frags_Transform.ScaleY = 1 + r * 2;
                UI_Frags_Transform.CenterX = 0;
                UI_Frags_Transform.CenterY = -ActualHeight / 2;
                UI_Frags_Effect.Radius = r * 5;
                UI_Frags.Opacity = 1 - r;
                UI_Text.Opacity = 1 - r;
                BD_SeperateLine.Opacity = 1 - r;
                for (int i = 0; i < items.Count(); i++)
                {
                    ImageObject item = items[i];
                    ImageObject lightitem = items[i];
                    Image image = images[i];
                    Image light = imagelights[i];
                    BlurEffect image_effect = image.Effect as BlurEffect;
                    BlurEffect light_effect = light.Effect as BlurEffect;
                    light_effect.Radius = Light_BlurMax + r * 100;
                    light.Opacity = Math.Max(1 - r * 5, 0);
                    image.Visibility = Visibility.Hidden;
                }
                for (int i = 0; i < frags.Count(); i++)
                {
                    ImageObject fragitem = frags[i];
                    Image frag = imagefrags[i];
                    FragmentBroken fragbk = fragbks[i];
                    MatrixTransform frag_tran = frag.RenderTransform as MatrixTransform;
                    double a0 = fragbk.RotateX * r * Math.PI / 180.0d;
                    double a1 = fragbk.RotateY * r * Math.PI / 180.0d;
                    Matrix m0 = new Matrix(1, 0, 0, 1, fragbk.Move.X * r, fragbk.Move.Y * r);
                    Matrix m1 = new Matrix(1, Math.Sin(a0), 0, Math.Cos(a0), fragbk.Move.X * r, fragbk.Move.Y * r);
                    Matrix m2 = new Matrix(Math.Cos(a0), Math.Sin(a0), 0, 1, fragbk.Move.X * r, fragbk.Move.Y * r);
                    frag_tran.Matrix = m0 * m1 * m2;
                    frag.Visibility = Visibility.Visible;
                }
            }
            #endregion
            #region 动画结束
            else
            {
                isactive = false;
                tick = span0 + span1 + span2 + 1;
                Visibility = Visibility.Collapsed;
            }
            #endregion
        }

        #endregion
    }



}
