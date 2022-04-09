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
    /// GameModeButton.xaml 的交互逻辑
    /// </summary>
    public partial class GameModeButton : Button
    {
        public GameModeButton()
        {
            InitializeComponent();
        }

        #region Properties

        #region ImageSource

        static public readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
            "ImageSource", typeof(ImageSource), typeof(GameBoard),
            new PropertyMetadata(null));

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        #endregion

        #region Text

        static public readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(GameBoard),
            new PropertyMetadata(String.Empty));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion 

        #endregion
    }
}
