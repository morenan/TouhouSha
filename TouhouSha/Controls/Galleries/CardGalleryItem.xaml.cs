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
using TouhouSha.Core;

namespace TouhouSha.Controls
{
    /// <summary>
    /// CardGalleryItem.xaml 的交互逻辑
    /// </summary>
    public partial class CardGalleryItem : UserControl
    {
        public CardGalleryItem()
        {
            InitializeComponent();
        }
        #region Properties

        #region Core

        static public readonly DependencyProperty CoreProperty = DependencyProperty.Register(
            "Core", typeof(Card), typeof(CardGalleryItem),
            new PropertyMetadata(null, OnPropertyChanged_Core));

        public Card Core
        {
            get { return (Card)GetValue(CoreProperty); }
            set { SetValue(CoreProperty, value); }
        }

        private static void OnPropertyChanged_Core(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardGalleryItem) ((CardGalleryItem)d).OnCoreChanged(e);
        }

        protected virtual void OnCoreChanged(DependencyPropertyChangedEventArgs e)
        {
            Info = Core != null ? App.GetCardInfo(Core) : null;
        }

        #endregion

        #region Info

        static public readonly DependencyProperty InfoProperty = DependencyProperty.Register(
            "Info", typeof(CardInfo), typeof(CardGalleryItem),
            new PropertyMetadata(null, OnPropertyChanged_Info));

        public CardInfo Info
        {
            get { return (CardInfo)GetValue(InfoProperty); }
            set { SetValue(InfoProperty, value); }
        }

        private static void OnPropertyChanged_Info(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardGalleryItem) ((CardGalleryItem)d).OnInfoChanged(e);
        }

        protected virtual void OnInfoChanged(DependencyPropertyChangedEventArgs e)
        {

        }


        #endregion

        #endregion
    }
}
