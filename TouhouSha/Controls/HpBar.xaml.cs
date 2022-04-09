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
    /// HpBar.xaml 的交互逻辑
    /// </summary>
    public partial class HpBar : UserControl
    {
        public HpBar()
        {
            InitializeComponent();
        }

        #region Properties

        #region Hp

        static public readonly DependencyProperty HpProperty = DependencyProperty.Register(
            "Hp", typeof(int), typeof(HpBar),
            new PropertyMetadata(0, OnPropertyChanged_Hp));

        public int Hp
        {
            get { return (int)GetValue(HpProperty); }
            set { SetValue(HpProperty, value); }
        }

        static public void OnPropertyChanged_Hp(object d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HpBar) ((HpBar)d).OnHpChanged(e);
        }

        public virtual void OnHpChanged(DependencyPropertyChangedEventArgs e)
        {
            Update();
        }

        #endregion
        
        #region MaxHp

        static public readonly DependencyProperty MaxHpProperty = DependencyProperty.Register(
            "MaxHp", typeof(int), typeof(HpBar),
            new PropertyMetadata(0, OnPropertyChanged_MaxHp));

        public int MaxHp
        {
            get { return (int)GetValue(MaxHpProperty); }
            set { SetValue(MaxHpProperty, value); }
        }

        static public void OnPropertyChanged_MaxHp(object d, DependencyPropertyChangedEventArgs e)
        {
            if (d is HpBar) ((HpBar)d).OnMaxHpChanged(e);
        }

        public virtual void OnMaxHpChanged(DependencyPropertyChangedEventArgs e)
        {
            Update();
        }

        #endregion

        #endregion

        #region Methods

        public void Update()
        {
            if (MaxHp <= 0 || MaxHp > 8)
            {
                SP_Items.Visibility = Visibility.Collapsed;
                UI_Text.Visibility = Visibility.Visible;
                UI_Text.Text = String.Format("HP:{0}/{1}", Hp, MaxHp);
            }
            else
            {
                SP_Items.Visibility = Visibility.Visible;
                UI_Text.Visibility = Visibility.Collapsed;
                while (SP_Items.Children.Count > MaxHp)
                    SP_Items.Children.RemoveAt(SP_Items.Children.Count - 1);
                while (SP_Items.Children.Count < MaxHp)
                    SP_Items.Children.Add(new HpBarItem() { IsLeft = SP_Items.Children.Count == 0 });
                Enum_HpBarItemStatus status_in = Enum_HpBarItemStatus.Green;
                Enum_HpBarItemStatus status_out = Enum_HpBarItemStatus.Black;
                if (Hp == 1) status_in = Enum_HpBarItemStatus.Red;
                else if (Hp == 2) status_in = Enum_HpBarItemStatus.Yellow;
                for (int i = 0; i < Hp && i < MaxHp; i++)
                    ((HpBarItem)(SP_Items.Children[i])).Status = status_in;
                for (int i = Hp; i < MaxHp; i++)
                    ((HpBarItem)(SP_Items.Children[i])).Status = status_out;
            }
        }

        #endregion

        #region Event Handler

        internal void IvTimer()
        {
            foreach (HpBarItem item in SP_Items.Children)
                item.IvTimer();
        }

        #endregion
        
    }
}
