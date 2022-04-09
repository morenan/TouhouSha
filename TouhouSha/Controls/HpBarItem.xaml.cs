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
    /// HpBarLeftItem.xaml 的交互逻辑
    /// </summary>
    public partial class HpBarItem : UserControl
    {
        public HpBarItem()
        {
            InitializeComponent();
            pathofstatus.Add(UI_Black);
            pathofstatus.Add(UI_Red);
            pathofstatus.Add(UI_Yellow);
            pathofstatus.Add(UI_Green);
        }

        #region Properties

        #region IsLeft

        static public readonly DependencyProperty IsLeftProperty = DependencyProperty.Register(
            "IsLeft", typeof(bool), typeof(HpBarItem),
            new PropertyMetadata(true, OnPropertyChanged_IsLeft));

        public bool IsLeft
        {
            get { return (bool)GetValue(IsLeftProperty); }
            set { SetValue(IsLeftProperty, value); }
        }

        static public void OnPropertyChanged_IsLeft(object d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HpBarItem) ((HpBarItem)d).OnIsLeftChanged(e);
        }

        public virtual void OnIsLeftChanged(DependencyPropertyChangedEventArgs e)
        {
            int tag = IsLeft ? 0 : 1;
            foreach (Path path in pathofstatus)
                path.Tag = tag;
        }

        #endregion

        #region Status
        
        static public readonly DependencyProperty StatusProperty = DependencyProperty.Register(
            "Status", typeof(Enum_HpBarItemStatus), typeof(HpBarItem),
            new PropertyMetadata(Enum_HpBarItemStatus.Black));

        public Enum_HpBarItemStatus Status
        {
            get { return (Enum_HpBarItemStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        #endregion

        #endregion

        #region Number

        private List<Path> pathofstatus = new List<Path>();

        #endregion

        #region Event Handler

        internal void IvTimer()
        {
            Enum_HpBarItemStatus status = Status;
            for (int i = 0; i < pathofstatus.Count(); i++)
            {
                Enum_HpBarItemStatus e = (Enum_HpBarItemStatus)i;
                Path path = pathofstatus[i];
                if (e == status)
                {
                    if (path.Opacity >= 1) continue;
                    path.Opacity = Math.Min(path.Opacity + 0.04, 1);
                }
                else
                {
                    if (path.Opacity <= 0) continue;
                    path.Opacity = Math.Min(path.Opacity - 0.04, 0);
                }
            }
        }

        #endregion
    }

    public enum Enum_HpBarItemStatus
    {
        Black = 0,
        Red = 1,
        Green = 2,
        Yellow = 3,
    }
}
