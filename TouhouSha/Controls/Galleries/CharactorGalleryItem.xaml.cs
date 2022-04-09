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
using TouhouSha.Core.UIs;

namespace TouhouSha.Controls
{
    /// <summary>
    /// CharactorGalleryItem.xaml 的交互逻辑
    /// </summary>
    public partial class CharactorGalleryItem : UserControl
    {
        public CharactorGalleryItem()
        {
            InitializeComponent();
        }

        #region Properties

        #region Core
        
        static public readonly DependencyProperty CoreProperty = DependencyProperty.Register(
            "Core", typeof(Charactor), typeof(CharactorGalleryItem),
            new PropertyMetadata(null, OnPropertyChanged_Core));

        public Charactor Core
        {
            get { return (Charactor)GetValue(CoreProperty); }
            set { SetValue(CoreProperty, value); }
        }

        private static void OnPropertyChanged_Core(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CharactorGalleryItem) ((CharactorGalleryItem)d).OnCoreChanged(e);
        }

        protected virtual void OnCoreChanged(DependencyPropertyChangedEventArgs e)
        {
            Info = Core != null ? App.GetCharactorInfo(Core) : null;
        }

        #endregion

        #region Info

        static public readonly DependencyProperty InfoProperty = DependencyProperty.Register(
            "Info", typeof(CharactorInfoCore), typeof(CharactorGalleryItem),
            new PropertyMetadata(null, OnPropertyChanged_Info));

        public CharactorInfoCore Info
        {
            get { return (CharactorInfoCore)GetValue(InfoProperty); }
            set { SetValue(InfoProperty, value); }
        }

        private static void OnPropertyChanged_Info(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CharactorGalleryItem) ((CharactorGalleryItem)d).OnInfoChanged(e);
        }

        protected virtual void OnInfoChanged(DependencyPropertyChangedEventArgs e)
        {

        }


        #endregion
        
        #endregion
    }
}
