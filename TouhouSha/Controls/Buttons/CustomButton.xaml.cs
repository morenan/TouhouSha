using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace TouhouSha.Controls
{
    /// <summary>
    /// CustomButton.xaml 的交互逻辑
    /// </summary>
    public partial class CustomButton : Button
    {
        public CustomButton()
        {
            InitializeComponent();
            CustomBorder = FindResource("StartMenuBorder") as CustomButtonBorder;
            Foreground = Brushes.White;
            HorizontalContentAlignment = HorizontalAlignment.Left;
            VerticalContentAlignment = VerticalAlignment.Center;
            Padding = new Thickness(8, 0, 0, 0);
        }

        #region Properties

        #region CustomBorder

        static public readonly DependencyProperty CustomBorderProperty = DependencyProperty.Register(
            "CustomBorder", typeof(CustomButtonBorder), typeof(CustomButton),
            new PropertyMetadata(null));

        public CustomButtonBorder CustomBorder
        {
            get { return (CustomButtonBorder)GetValue(CustomBorderProperty); }
            set { SetValue(CustomBorderProperty, value); }
        }

        #endregion

        #endregion

        #region Event Handler

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsEnabledProperty)
            {
                if (IsEnabled) Visibility = Visibility.Visible;
                else Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(200);
                    Dispatcher.Invoke(() =>
                    {
                        if (!IsEnabled) Visibility = Visibility.Collapsed;
                    });
                });
            }
        }
        
        #endregion

    }

    public abstract class CustomButtonBorder : DependencyObject
    {
        static public readonly CustomButtonBorder Default = new CustomButtonSampleBorder();

        #region Background

        static public readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(
            "Background", typeof(Brush), typeof(CustomButtonBorder),
            new PropertyMetadata(Brushes.White));

        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        #endregion

        #region Foreground

        static public readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
            "Foreground", typeof(Brush), typeof(CustomButtonBorder),
            new PropertyMetadata(Brushes.Black));

        public Brush Foreground
        {
            get { return (Brush)GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        #endregion

        #region MouseOverBackground

        static public readonly DependencyProperty MouseOverBackgroundProperty = DependencyProperty.Register(
            "MouseOverBackground", typeof(Brush), typeof(CustomButtonBorder),
            new PropertyMetadata(Brushes.Gray));

        public Brush MouseOverBackground
        {
            get { return (Brush)GetValue(MouseOverBackgroundProperty); }
            set { SetValue(MouseOverBackgroundProperty, value); }
        }

        #endregion
        
        #region MouseDownBackground

        static public readonly DependencyProperty MouseDownBackgroundProperty = DependencyProperty.Register(
            "MouseDownBackground", typeof(Brush), typeof(CustomButtonBorder),
            new PropertyMetadata(Brushes.DarkBlue));

        public Brush MouseDownBackground
        {
            get { return (Brush)GetValue(MouseDownBackgroundProperty); }
            set { SetValue(MouseDownBackgroundProperty, value); }
        }

        #endregion
    }

    public class CustomButtonSampleBorder : CustomButtonBorder
    {
        #region Properties
        
        #region BorderMargin

        static public readonly DependencyProperty BorderMarginProperty = DependencyProperty.Register(
            "BorderMargin", typeof(Thickness), typeof(CustomButtonSampleBorder),
            new PropertyMetadata(default(Thickness)));

        public Thickness BorderMargin
        {
            get { return (Thickness)GetValue(BorderMarginProperty); }
            set { SetValue(BorderMarginProperty, value); }
        }

        #endregion

        #region BorderThickness

        static public readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register(
            "BorderThickness", typeof(double), typeof(CustomButtonSampleBorder),
            new PropertyMetadata(1.0d));

        public double BorderThickness
        {
            get { return (double)GetValue(BorderThicknessProperty); }
            set { SetValue(BorderThicknessProperty, value); }
        }

        #endregion

        #region BorderBrush

        static public readonly DependencyProperty BorderBrushProperty = DependencyProperty.Register(
            "BorderBrush", typeof(Brush), typeof(CustomButtonSampleBorder),
            new PropertyMetadata(Brushes.White));

        public Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }
        
        #endregion

        #endregion
    }

    public class CustomButtonTileBorder : CustomButtonBorder
    {
        #region Properties

        #region ImageLeftTop

        static public readonly DependencyProperty ImageLeftTopProperty = DependencyProperty.Register(
            "ImageLeftTop", typeof(ImageSource), typeof(CustomButtonSampleBorder),
            new PropertyMetadata(null));

        public ImageSource ImageLeftTop
        {
            get { return (ImageSource)GetValue(ImageLeftTopProperty); }
            set { SetValue(ImageLeftTopProperty, value); }
        }

        #endregion

        #region ImageRightTop

        static public readonly DependencyProperty ImageRightTopProperty = DependencyProperty.Register(
            "ImageRightTop", typeof(ImageSource), typeof(CustomButtonSampleBorder),
            new PropertyMetadata(null));

        public ImageSource ImageRightTop
        {
            get { return (ImageSource)GetValue(ImageRightTopProperty); }
            set { SetValue(ImageRightTopProperty, value); }
        }

        #endregion

        #region ImageLeftBottom

        static public readonly DependencyProperty ImageLeftBottomProperty = DependencyProperty.Register(
            "ImageLeftBottom", typeof(ImageSource), typeof(CustomButtonSampleBorder),
            new PropertyMetadata(null));

        public ImageSource ImageLeftBottom
        {
            get { return (ImageSource)GetValue(ImageLeftBottomProperty); }
            set { SetValue(ImageLeftBottomProperty, value); }
        }

        #endregion

        #region ImageRightBottom

        static public readonly DependencyProperty ImageRightBottomProperty = DependencyProperty.Register(
            "ImageRightBottom", typeof(ImageSource), typeof(CustomButtonSampleBorder),
            new PropertyMetadata(null));

        public ImageSource ImageRightBottom
        {
            get { return (ImageSource)GetValue(ImageRightBottomProperty); }
            set { SetValue(ImageRightBottomProperty, value); }
        }

        #endregion

        #region ImageLeft

        static public readonly DependencyProperty ImageLeftProperty = DependencyProperty.Register(
            "ImageLeft", typeof(ImageSource), typeof(CustomButtonSampleBorder),
            new PropertyMetadata(null));

        public ImageSource ImageLeft
        {
            get { return (ImageSource)GetValue(ImageLeftProperty); }
            set { SetValue(ImageLeftProperty, value); }
        }

        #endregion

        #region ImageRight

        static public readonly DependencyProperty ImageRightProperty = DependencyProperty.Register(
            "ImageRight", typeof(ImageSource), typeof(CustomButtonSampleBorder),
            new PropertyMetadata(null));

        public ImageSource ImageRight
        {
            get { return (ImageSource)GetValue(ImageRightProperty); }
            set { SetValue(ImageRightProperty, value); }
        }

        #endregion
        #region ImageTop

        static public readonly DependencyProperty ImageTopProperty = DependencyProperty.Register(
            "ImageTop", typeof(ImageSource), typeof(CustomButtonSampleBorder),
            new PropertyMetadata(null));

        public ImageSource ImageTop
        {
            get { return (ImageSource)GetValue(ImageTopProperty); }
            set { SetValue(ImageTopProperty, value); }
        }

        #endregion

        #region ImageBottom

        static public readonly DependencyProperty ImageBottomProperty = DependencyProperty.Register(
            "ImageBottom", typeof(ImageSource), typeof(CustomButtonSampleBorder),
            new PropertyMetadata(null));

        public ImageSource ImageBottom
        {
            get { return (ImageSource)GetValue(ImageBottomProperty); }
            set { SetValue(ImageBottomProperty, value); }
        }

        #endregion
        
        #endregion

    }

    public class CustomButtonBorderSelector : DataTemplateSelector
    {
        public DataTemplate Sample { get; set; }

        public DataTemplate Tile { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            object border = container.GetValue(CustomButton.CustomBorderProperty);
            if (border is CustomButtonSampleBorder) return Sample;
            if (border is CustomButtonTileBorder) return Tile;
            return Sample;
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class TrueHidden : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool && (bool)value) ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is Visibility && ((Visibility)value) == Visibility.Hidden);
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class FalseHidden : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is Visibility && ((Visibility)value) == Visibility.Visible);
        }
    }
   
}
