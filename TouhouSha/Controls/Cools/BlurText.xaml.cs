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

namespace TouhouSha.Controls
{
    /// <summary>
    /// BlurText.xaml 的交互逻辑
    /// </summary>
    public partial class BlurText : UserControl
    {
        public BlurText()
        {
            InitializeComponent();
        }

        #region Properties

        #region Text

        static public readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(BlurText),
            new PropertyMetadata(String.Empty));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion

        #region HighlightForeground

        static public readonly DependencyProperty HighlightForegroundProperty = DependencyProperty.Register(
            "HighlightForeground", typeof(Brush), typeof(BlurText),
            new PropertyMetadata(Brushes.White));

        public Brush HighlightForeground
        {
            get { return (Brush)GetValue(HighlightForegroundProperty); }
            set { SetValue(HighlightForegroundProperty, value); }
        }


        #endregion

        #region HighlightThickness

        static public readonly DependencyProperty HighlightThicknessProperty = DependencyProperty.Register(
            "HighlightThickness", typeof(double), typeof(BlurText),
            new PropertyMetadata(1.0d));

        public double HighlightThickness
        {
            get { return (double)GetValue(HighlightThicknessProperty); }
            set { SetValue(HighlightThicknessProperty, value); }
        }


        #endregion

        #region Orientation

        static public readonly DependencyProperty OrientationProperty = DependencyProperty.Register(
            "Orientation", typeof(Orientation), typeof(BlurText),
            new PropertyMetadata(Orientation.Horizontal));

        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        #endregion

        #region BluePower

        static public readonly DependencyProperty BlurPowerProperty = DependencyProperty.Register(
            "BlurPower", typeof(double), typeof(BlurText),
            new PropertyMetadata(10.0d));

        public double BlurPower
        {
            get { return (double)GetValue(BlurPowerProperty); }
            set { SetValue(BlurPowerProperty, value); }
        }

        #endregion

        #region MaxTextWidth

        static public readonly DependencyProperty MaxTextWidthProperty = DependencyProperty.Register(
            "MaxTextWidth", typeof(double), typeof(BlurText),
            new PropertyMetadata(10000.0d));

        public double MaxTextWidth
        {
            get { return (double)GetValue(MaxTextWidthProperty); }
            set { SetValue(MaxTextWidthProperty, value); }
        }

        #endregion 

        #endregion

    }
}
