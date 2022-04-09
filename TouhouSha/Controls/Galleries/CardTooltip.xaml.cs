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
    /// CardTooltip.xaml 的交互逻辑
    /// </summary>
    public partial class CardTooltip : UserControl
    {
        public CardTooltip()
        {
            InitializeComponent();
        }

        #region Properties

        #region Core

        static public readonly DependencyProperty CoreProperty = DependencyProperty.Register(
            "Core", typeof(Card), typeof(CardTooltip),
            new PropertyMetadata(null, OnPropertyChanged_Core));

        public Card Core
        {
            get { return (Card)GetValue(CoreProperty); }
            set { SetValue(CoreProperty, value); }
        }

        private static void OnPropertyChanged_Core(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardTooltip) ((CardTooltip)d).OnCoreChanged(e);
        }

        protected virtual void OnCoreChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Core != null)
            {
                Info = App.GetCardInfo(Core);
                switch (Core.CardType?.E)
                {
                    case Enum_CardType.Base: 
                        TL_Type.Text = "基本牌";
                        TL_SubType.Text = String.Empty;
                        break;
                    case Enum_CardType.Spell:
                        TL_Type.Text = "锦囊牌";
                        switch (Core.CardType?.SubType?.E)
                        {
                            case Enum_CardSubType.Immediate: TL_SubType.Text = "非延时"; break;
                            case Enum_CardSubType.Delay: TL_SubType.Text = "延时"; break;
                        }
                        break;
                    case Enum_CardType.Equip:
                        TL_Type.Text = "装备牌";
                        switch (Core.CardType?.SubType?.E)
                        {
                            case Enum_CardSubType.Weapon: TL_SubType.Text = "武器"; break;
                            case Enum_CardSubType.Armor: TL_SubType.Text = "防具"; break;
                            case Enum_CardSubType.HorsePlus: TL_SubType.Text = "攻击UFO"; break;
                            case Enum_CardSubType.HorseMinus: TL_SubType.Text = "防御UFO"; break;
                        }
                        break;
                }
            }
        }

        #endregion

        #region Info

        static public readonly DependencyProperty InfoProperty = DependencyProperty.Register(
            "Info", typeof(CardInfo), typeof(CardTooltip),
            new PropertyMetadata(null, OnPropertyChanged_Info));

        public CardInfo Info
        {
            get { return (CardInfo)GetValue(InfoProperty); }
            set { SetValue(InfoProperty, value); }
        }

        private static void OnPropertyChanged_Info(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardTooltip) ((CardTooltip)d).OnInfoChanged(e);
        }

        protected virtual void OnInfoChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Info != null)
            {
                TL_Name.Text = Info.Name;
                TL_Description.Text = Info.Description;
            }
        }

        #endregion

        #endregion
    }
}
