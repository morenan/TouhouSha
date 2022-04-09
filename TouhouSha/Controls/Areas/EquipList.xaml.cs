using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using TouhouSha.Core;

namespace TouhouSha.Controls
{
    /// <summary>
    /// EquipList.xaml 的交互逻辑
    /// </summary>
    public partial class EquipList : UserControl
    {
        public EquipList()
        {
            InitializeComponent();
            Items = new ObservableCollection<Card>() { null, null, null, null };
        }

        #region Properties

        #region Items
        
        static public readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items", typeof(ObservableCollection<Card>), typeof(GameBoard),
            new PropertyMetadata(null));

        public ObservableCollection<Card> Items
        {
            get { return (ObservableCollection<Card>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        #endregion

        #endregion
    }

    [ValueConversion(typeof(CardColor), typeof(Brush))]
    public class CardColorToForeground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is CardColor)) return Brushes.White;
            CardColor color = (CardColor)value;
            switch (color.E)
            {
                case Enum_CardColor.Red:
                case Enum_CardColor.Heart:
                case Enum_CardColor.Diamond: return Brushes.IndianRed;
                default: return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(CardColor), typeof(string))]
    public class CardColorToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is CardColor)) return Brushes.White;
            CardColor color = (CardColor)value;
            return App.GetCardColorEmojiText(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(int), typeof(string))]
    public class CardPointToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int)) return String.Empty;
            int i = (int)value;
            return App.GetCardPokerPoint(i);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    [ValueConversion(typeof(string), typeof(string))]
    public class CardKeyNameToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string)) return String.Empty;
            string keyname = (string)value;
            return App.GetCardInfo(keyname)?.Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    

}
