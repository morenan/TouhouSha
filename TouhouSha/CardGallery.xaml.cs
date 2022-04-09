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
using System.Windows.Threading;
using TouhouSha.Controls;
using TouhouSha.Core;

namespace TouhouSha
{
    /// <summary>
    /// CardGallery.xaml 的交互逻辑
    /// </summary>
    public partial class CardGallery : UserControl
    {
        public CardGallery()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }
        #region Properties

        #region Cards

        static public readonly DependencyProperty CardsProperty = DependencyProperty.Register(
            "Cards", typeof(IList<Card>), typeof(CardGallery),
            new PropertyMetadata(null, OnPropertyChanged_Cards));

        public IList<Card> Cards
        {
            get { return (IList<Card>)GetValue(CardsProperty); }
            set { SetValue(CardsProperty, value); }
        }

        private static void OnPropertyChanged_Cards(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardGallery) ((CardGallery)d).OnCardsChanged(e);
        }

        protected virtual void OnCardsChanged(DependencyPropertyChangedEventArgs e)
        {
            CardTypeItem all = new CardTypeItem("全卡片");
            Dictionary<string, CardTypeItem> dict = new Dictionary<string, CardTypeItem>();
            if (Cards == null) return;
            foreach (Card card in Cards)
            {
                CardTypeItem item = null;
                all.Cards.Add(card);
                string typename = null;
                string typename2 = null;
                switch (card.CardType?.E)
                {
                    case Enum_CardType.Base: typename = "基本牌"; break;
                    case Enum_CardType.Spell: 
                        typename = "锦囊牌"; 
                        switch (card.CardType?.SubType?.E)
                        {
                            case Enum_CardSubType.Immediate: typename2 = "锦囊牌(非延时)"; break;
                            case Enum_CardSubType.Delay: typename2 = "锦囊牌(延时)"; break;
                        }
                        break;
                    case Enum_CardType.Equip: 
                        typename = "装备牌";
                        switch (card.CardType?.SubType?.E)
                        {
                            case Enum_CardSubType.Weapon: typename2 = "装备牌(武器)"; break;
                            case Enum_CardSubType.Armor: typename2 = "装备牌(防具)"; break;
                            case Enum_CardSubType.HorsePlus: typename2 = "装备牌(攻击UFO)"; break;
                            case Enum_CardSubType.HorseMinus: typename2 = "装备牌(防御UFO)"; break;
                        }
                        break;
                }
                if (!String.IsNullOrEmpty(typename))
                {
                    if (!dict.TryGetValue(typename, out item))
                    {
                        item = new CardTypeItem(typename);
                        dict.Add(typename, item);
                    }
                    item.Cards.Add(card);
                }
                if (!String.IsNullOrEmpty(typename2))
                {
                    if (!dict.TryGetValue(typename2, out item))
                    {
                        item = new CardTypeItem(typename2);
                        dict.Add(typename2, item);
                    }
                    item.Cards.Add(card);
                }
            }
            CardTypes = new CardTypeItem[] { all }.Concat(dict.Values).ToList();
            SelectedCardType = all;
        }

        #endregion

        #region CardTypes

        static public readonly DependencyProperty CardTypesProperty = DependencyProperty.Register(
            "CardTypes", typeof(IList<CardTypeItem>), typeof(CardGallery),
            new PropertyMetadata(null, OnPropertyChanged_CardTypes));

        public IList<CardTypeItem> CardTypes
        {
            get { return (IList<CardTypeItem>)GetValue(CardTypesProperty); }
            set { SetValue(CardTypesProperty, value); }
        }

        private static void OnPropertyChanged_CardTypes(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardGallery) ((CardGallery)d).OnCardTypesChanged(e);
        }

        protected virtual void OnCardTypesChanged(DependencyPropertyChangedEventArgs e)
        {
            SelectedCardType = CardTypes.FirstOrDefault();
        }

        #endregion

        #region CardsOfType

        static public readonly DependencyProperty CardsOfTypeProperty = DependencyProperty.Register(
            "CardsOfType", typeof(IList<Card>), typeof(CardGallery),
            new PropertyMetadata(null, OnPropertyChanged_CardsOfType));

        public IList<Card> CardsOfType
        {
            get { return (IList<Card>)GetValue(CardsOfTypeProperty); }
            set { SetValue(CardsOfTypeProperty, value); }
        }

        private static void OnPropertyChanged_CardsOfType(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardGallery) ((CardGallery)d).OnCardsOfTypeChanged(e);
        }

        protected virtual void OnCardsOfTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            IList<Card> list = CardsOfType ?? new List<Card>();
            foreach (CardGalleryItem item in GD_Chars.Children)
            {
                item.MouseEnter -= CardGalleryItem_MouseEnter;
                item.MouseLeave -= CardGalleryItem_MouseLeave;
                item.MouseDown -= CardGalleryItem_MouseDown;
            }
            GD_Chars.Children.Clear();
            foreach (Card card in list)
            {
                CardGalleryItem item = new CardGalleryItem();
                item.Core = card;
                item.MouseEnter += CardGalleryItem_MouseEnter;
                item.MouseLeave += CardGalleryItem_MouseLeave;
                item.MouseDown += CardGalleryItem_MouseDown;
                GD_Chars.Children.Add(item);
            }
            GD_Chars_UpdateLayout();
        }

        #endregion

        #region SelectedCardType

        static public readonly DependencyProperty SelectedCardTypeProperty = DependencyProperty.Register(
            "SelectedCardType", typeof(CardTypeItem), typeof(CardGallery),
            new PropertyMetadata(null, OnPropertyChanged_SelectedCardType));

        public CardTypeItem SelectedCardType
        {
            get { return (CardTypeItem)GetValue(SelectedCardTypeProperty); }
            set { SetValue(SelectedCardTypeProperty, value); }
        }

        private static void OnPropertyChanged_SelectedCardType(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardGallery) ((CardGallery)d).OnSelectedCardTypeChanged(e);
        }

        protected virtual void OnSelectedCardTypeChanged(DependencyPropertyChangedEventArgs e)
        {
            CardsOfType = SelectedCardType?.Cards ?? new List<Card>();
        }

        #endregion

        #region MouseOverCard

        static public readonly DependencyProperty MouseOverCardProperty = DependencyProperty.Register(
            "MouseOverCard", typeof(Card), typeof(CardGallery),
            new PropertyMetadata(null, OnPropertyChanged_MouseOverCard));

        public Card MouseOverCard
        {
            get { return (Card)GetValue(MouseOverCardProperty); }
            set { SetValue(MouseOverCardProperty, value); }
        }

        private static void OnPropertyChanged_MouseOverCard(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CardGallery) ((CardGallery)d).OnMouseOverCardChanged(e);
        }

        protected virtual void OnMouseOverCardChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #endregion

        #region Method

        protected void GD_Chars_UpdateLayout()
        {
            double uw = 100 + 8;
            double uh = 130 + 8;
            double aw = BD_Chars.ActualWidth;
            double ah = BD_Chars.ActualHeight;
            if (aw < uw) return;
            int columns = (int)(aw / uw);
            int rows = (GD_Chars.Children.Count - 1) / columns + 1;
            while (GD_Chars.RowDefinitions.Count < rows)
                GD_Chars.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(uh, GridUnitType.Pixel) });
            while (GD_Chars.RowDefinitions.Count > rows)
                GD_Chars.RowDefinitions.RemoveAt(GD_Chars.RowDefinitions.Count - 1);
            while (GD_Chars.ColumnDefinitions.Count < columns)
                GD_Chars.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(uw, GridUnitType.Pixel) });
            while (GD_Chars.ColumnDefinitions.Count > columns)
                GD_Chars.ColumnDefinitions.RemoveAt(GD_Chars.ColumnDefinitions.Count - 1);
            for (int i = 0; i < GD_Chars.Children.Count; i++)
            {
                UIElement ue = GD_Chars.Children[i];
                Grid.SetRow(ue, i / columns);
                Grid.SetColumn(ue, i % columns);
            }
        }

        #endregion

        #region Number

        private MainWindow mw;
        private DispatcherTimer timer;
        private int tooltip_tick;
        private int tooltip_tickmax = 20;

        #endregion

        #region Event Handlers

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            IM_Background.Source = App.CreateImage(System.IO.Path.Combine(App.GetApplicationPath(), "Images", "Gallery"));
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsVisibleProperty)
            {
                if (IsVisible)
                {
                    this.timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromMilliseconds(10);
                    timer.Tick += OnTimerTick;
                    timer.Start();
                }
                else
                {
                    timer.Tick -= OnTimerTick;
                    timer.Stop();
                    this.timer = null;
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.mw = App.FindAncestor<MainWindow>(this);
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (UI_CardTooltip.Core != MouseOverCard)
            {
                if (tooltip_tick > 0)
                {
                    UI_CardTooltip.Opacity = (double)(--tooltip_tick) / tooltip_tickmax;
                }
                else
                {
                    UI_CardTooltip.Core = MouseOverCard;
                    if (MouseOverCard == null)
                        UI_CardTooltip.Visibility = Visibility.Collapsed;
                }
            }
            else if (UI_CardTooltip.Core != null)
            {
                UI_CardTooltip.Visibility = Visibility.Visible;
                if (tooltip_tick < tooltip_tickmax)
                {
                    UI_CardTooltip.Opacity = (double)(++tooltip_tick) / tooltip_tickmax;
                }
            }
            if (UI_CardTooltip.IsVisible)
            {
                CardGalleryItem viewitem = GD_Chars.Children.Cast<CardGalleryItem>().FirstOrDefault(_item => _item.Core == UI_CardTooltip.Core);
                if (viewitem != null)
                {
                    Point p = viewitem.TranslatePoint(new Point(0, 0), CV_CharactorTooltip);
                    if (p.X > CV_CharactorTooltip.ActualWidth - p.X - viewitem.ActualWidth)
                        Canvas.SetLeft(UI_CardTooltip, p.X - UI_CardTooltip.ActualWidth);
                    else
                        Canvas.SetLeft(UI_CardTooltip, p.X + viewitem.ActualWidth);
                    Canvas.SetTop(UI_CardTooltip, Math.Min(p.Y, CV_CharactorTooltip.ActualHeight - UI_CardTooltip.ActualHeight));
                }
            }
        }

        private void BD_Chars_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GD_Chars_UpdateLayout();
        }

        private void CardGalleryItem_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseOverCard = (sender as CardGalleryItem).Core;
        }

        private void CardGalleryItem_MouseLeave(object sender, MouseEventArgs e)
        {
            Card card = (sender as CardGalleryItem).Core;
            if (MouseOverCard == card)
                MouseOverCard = null;
        }

        private void CardGalleryItem_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void BN_Return_Click(object sender, RoutedEventArgs e)
        {
            mw?.HomeStartMenu();
        }

        #endregion

    }
    public class CardTypeItem
    {
        public CardTypeItem(string _name)
        {
            this.name = _name;
        }

        public override string ToString()
        {
            return Name;
        }

        private string name;
        public string Name { get { return this.name; } }

        private List<Card> cards = new List<Card>();
        public List<Card> Cards { get { return this.cards; } }
    }
}
