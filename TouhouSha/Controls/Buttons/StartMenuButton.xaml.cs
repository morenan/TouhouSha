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
    /// StartMenuButton.xaml 的交互逻辑
    /// </summary>
    public partial class StartMenuButton : Button
    {
        public StartMenuButton()
        {
            InitializeComponent();
        }

        #region Properties

        #region ImageSource

        static public readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            "ImageSource", typeof(ImageSource), typeof(StartMenuButton),
            new PropertyMetadata(null, OnPropertyChanged_ImageSource));

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        private static void OnPropertyChanged_ImageSource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StartMenuButton) ((StartMenuButton)d).OnImageSourceChanged(e);
        }
       
        protected virtual void OnImageSourceChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #endregion
    }
}
