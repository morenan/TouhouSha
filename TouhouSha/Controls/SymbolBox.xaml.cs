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
    /// SymbolBox.xaml 的交互逻辑
    /// </summary>
    public partial class SymbolBox : UserControl
    {
        public SymbolBox()
        {
            InitializeComponent();
        }

        #region Properties

        #region Zone
        
        static public readonly DependencyProperty ZoneProperty = DependencyProperty.Register(
            "Zone", typeof(Zone), typeof(SymbolBox),
            new PropertyMetadata(null, OnPropertyChanged_Zone));

        public Zone Zone
        {
            get { return (Zone)GetValue(ZoneProperty); }
            set { SetValue(ZoneProperty, value); }
        }

        static public void OnPropertyChanged_Zone(object d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SymbolBox) ((SymbolBox)d).OnZoneChanged(e);
        }

        public virtual void OnZoneChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is Zone)
            {
                Zone zone = (Zone)(e.OldValue);
                zone.Cards.CollectionChanged -= Cards_CollectionChanged;
            }
            if (e.NewValue is Zone)
            {
                Zone zone = (Zone)(e.NewValue);
                zone.Cards.CollectionChanged += Cards_CollectionChanged;
            }
            Update();
        }

        #endregion

        #endregion

        #region Method

        protected void Update()
        {
            Visibility = Zone != null && Zone.Cards.Count() > 0
                ? Visibility.Visible
                : Visibility.Collapsed;
            TL_Text.Text = String.Format("{0}({1})",
                Zone?.KeyName,
                Zone?.Cards?.Count() ?? 0);
        }

        #endregion

        #region Event Handler
        
        private void Cards_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.Invoke(Update);
        }

        #endregion
    }
}
