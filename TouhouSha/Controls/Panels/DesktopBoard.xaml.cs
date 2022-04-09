using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
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
using System.Threading;
using System.Windows.Media.Animation;

namespace TouhouSha.Controls
{
    /// <summary>
    /// 将一些特定的牌放置到这个桌面面板，以供选择和整理。
    /// 过河拆桥，顺手牵羊等卡片要经过这个面板完成最后一步操作。
    /// </summary>
    public partial class DesktopBoard : BoxedControl
    {
        public DesktopBoard()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        #region Properties

        #region Core

        static public readonly DependencyProperty CoreProperty = DependencyProperty.Register(
            "Core", typeof(DesktopCardBoardCore), typeof(DesktopBoard),
            new PropertyMetadata(null, OnPropertyChanged_Core));

        public DesktopCardBoardCore Core
        {
            get { return (DesktopCardBoardCore)GetValue(CoreProperty); }
            set { SetValue(CoreProperty, value); }
        }

        private static void OnPropertyChanged_Core(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DesktopBoard) ((DesktopBoard)d).OnCoreChanged(e);
        }

        protected virtual void OnCoreChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Core == null) return;
            Message = Core.Message;
            BN_No.Visibility = ((Core.Flag & Enum_DesktopCardBoardFlag.CannotNo) != Enum_DesktopCardBoardFlag.None)
                ? Visibility.Collapsed : Visibility.Visible;
            BN_Yes.Visibility = ((Core.Flag & Enum_DesktopCardBoardFlag.SelectCardAndYes) != Enum_DesktopCardBoardFlag.None)
                ? Visibility.Collapsed : Visibility.Visible;
            SP_Buttons.Visibility = BN_Yes.Visibility == Visibility.Collapsed && BN_No.Visibility == Visibility.Collapsed
                ? Visibility.Collapsed : Visibility.Visible;
            BN_Yes.IsEnabled = false;
            Reset();
            Resize();
            UpdateCardsSelectable();
        }

        #endregion

        #region Message

        static public readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof(string), typeof(DesktopBoard),
            new PropertyMetadata(String.Empty));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        #endregion

        #region TimeValue

        static public readonly DependencyProperty TimeValueProperty = DependencyProperty.Register(
            "TimeValue", typeof(double), typeof(DesktopBoard),
            new PropertyMetadata(0.0d));

        public double TimeValue
        {
            get { return (double)GetValue(TimeValueProperty); }
            set { SetValue(TimeValueProperty, value); }
        }

        #endregion

        #region Timeout

        static public readonly DependencyProperty TimeoutProperty = DependencyProperty.Register(
            "Timeout", typeof(double), typeof(DesktopBoard),
            new PropertyMetadata(45.0d));

        public double Timeout
        {
            get { return (double)GetValue(TimeoutProperty); }
            set { SetValue(TimeoutProperty, value); }
        }

        #endregion

        #region TimeAnim

        static public DependencyProperty TimeAnimProperty = DependencyProperty.Register(
            "TimeAnim", typeof(DoubleAnimation), typeof(DesktopBoard),
            new PropertyMetadata(null, OnPropertyChanged_TimeAnim));

        public DoubleAnimation TimeAnim
        {
            get { return (DoubleAnimation)GetValue(TimeAnimProperty); }
            set { SetValue(TimeAnimProperty, value); }
        }

        private static void OnPropertyChanged_TimeAnim(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DesktopBoard) ((DesktopBoard)d).OnTimeAnimChanged(e);
        }

        protected virtual void OnTimeAnimChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is DoubleAnimation)
            {
                DoubleAnimation oldvalue = (DoubleAnimation)(e.OldValue);
                oldvalue.Completed -= OnTimeout;
            }
            if (e.NewValue is DoubleAnimation)
            {
                DoubleAnimation newvalue = (DoubleAnimation)(e.NewValue);
                newvalue.Completed += OnTimeout;
                BeginAnimation(TimeValueProperty, newvalue, HandoffBehavior.SnapshotAndReplace);
            }
        }

        #endregion

        #endregion

        #region Number

        /// <summary>
        /// 游戏显示和控制界面。
        /// </summary>
        private GameBoard gameboard;
        /// <summary>
        /// 卡显示的垃圾堆。
        /// </summary>
        private List<CardView> freeviews = new List<CardView>();
        /// <summary>
        /// 卡逻辑与卡显示关联。
        /// </summary>
        private Dictionary<Card, CardView> card2views = new Dictionary<Card, CardView>();
        /// <summary>
        /// 正在进行动画的卡，逻辑与显示关联。
        /// </summary>
        private Dictionary<Card, CardView> card2anims = new Dictionary<Card, CardView>();

        /// <summary>
        /// 正在鼠标挪动的卡片的原本的卡区域。
        /// </summary>
        private DesktopCardBoardZone dragfrom;
        /// <summary>
        /// 挪动前的区内所在索引。
        /// </summary>
        private int dragfromindex;
        /// <summary>
        /// 正在鼠标挪动的卡片。
        /// </summary>
        private Card dragcard;
        /// <summary>
        /// 鼠标开始拖动的点相对于卡显示左上角的位置。
        /// </summary>
        private Vector dragvec;
        /// <summary>
        /// 要准备插入的卡区域。
        /// </summary>
        private DesktopCardBoardZone insezone;
        /// <summary>
        /// 要尊准备插入的位置。
        /// </summary>
        private int inseindex;

        #endregion 

        #region Method

        /// <summary>
        /// 重置整个UI。
        /// </summary>
        protected void Reset()
        {
            DesktopCardBoardCore core = Core;
            if (core == null) return;
            // 清空UI元素，将卡UI放到垃圾堆。
            #region UI清理
            SP_Zones.Children.Clear();
            freeviews.AddRange(card2views.Values);
            card2views.Clear();
            foreach (CardView cardview in freeviews)
            {
                cardview.Card = null;
                cardview.Visibility = Visibility.Collapsed;
            }
            #endregion
            // 生成和逻辑对应的UI卡区和卡显示
            #region UI生成
            foreach (DesktopCardBoardZone zone in core.Zones)
            {
                // 因为卡显示要跨区进行动画，所以卡区内部是一个空的Border。
                // 将所有的卡放置到上层Canvas，使用动画来进行布局。
                GroupBox gb = new GroupBox();
                Border bd = new Border();
                gb.DataContext = zone;
                gb.Header = zone.Message;
                gb.Content = bd;
                gb.HorizontalAlignment = HorizontalAlignment.Stretch;
                bd.Height = CardView.DefaultHeight;
                bd.HorizontalAlignment = HorizontalAlignment.Left;
                SP_Zones.Children.Add(gb);
                // 显示区内所有的卡。
                foreach (Card card in zone.Cards)
                {
                    CardView cardview = null;
                    // 捡垃圾。
                    if (freeviews.Count() > 0)
                    {
                        cardview = freeviews.LastOrDefault();
                        freeviews.RemoveAt(freeviews.Count() - 1);
                    }
                    // 创建新元素。
                    else
                    {
                        cardview = new CardView();
                        cardview.Visibility = Visibility.Collapsed;
                        cardview.MouseDown += CardView_MouseDown;
                        cardview.MouseUp += CardView_MouseUp;
                        cardview.MouseMove += CardView_MouseMove;
                        CV_Cards.Children.Add(cardview);
                    }
                    // 和逻辑绑定。
                    cardview.Card = card;
                    card2views.Add(card, cardview);
                    // 所在区背面显示。
                    cardview.IsFaceDown = ((zone.Flag & Enum_DesktopZoneFlag.FaceDown) != Enum_DesktopZoneFlag.None);
                }
            }
            #endregion
        }

        /// <summary>
        /// 给每个卡区UI设置尺寸。
        /// </summary>
        protected void Resize()
        {
            if (gameboard == null) return;
            // 设置最大尺寸，要保留主界面一定边缘。
            MaxWidth = Math.Max(400, gameboard.ActualWidth - 64);
            MaxHeight = Math.Max(320, gameboard.ActualHeight - 64);
            // 设置每个卡区UI的尺寸。
            foreach (GroupBox gb in SP_Zones.Children)
            {
                DesktopCardBoardZone zone = gb.DataContext as DesktopCardBoardZone;
                Border bd = gb.Content as Border;
                // 这个卡区的卡可以重叠。
                if ((zone.Flag & Enum_DesktopZoneFlag.CanZip) != Enum_DesktopZoneFlag.None)
                {
                    bd.Height = CardView.DefaultHeight;
                    bd.Width = Math.Min(CardView.DefaultWidth * zone.Cards.Count(), MaxWidth - 32);
                }
                // 这个卡区的卡不能重叠，以标准方阵来布局。
                else
                {
                    int maxcolumns = (int)((MaxWidth - 32) / CardView.DefaultWidth);
                    int rows = (zone.Cards.Count() - 1) / maxcolumns + 1;
                    int columns = Math.Min(zone.Cards.Count(), maxcolumns);
                    bd.Height = CardView.DefaultHeight * rows;
                    bd.Width = CardView.DefaultWidth * columns;
                }
            }
        }

        /// <summary>
        /// 更新每张卡的可选择性。
        /// </summary>
        protected void UpdateCardsSelectable()
        {
            if (gameboard == null) return;
            Context ctx = gameboard.World.GetContext();
            DesktopCardBoardCore core = Core;
            if (core == null) return;
            // 卡片是否支持拖动。
            bool candrag = ((core.Flag & Enum_DesktopCardBoardFlag.CanDrag) != Enum_DesktopCardBoardFlag.None);
            // 选择卡的安放区，如果有这个区，所有要选择的卡通过拖动的方式放置到这里。
            DesktopCardBoardZone selectzone = core.Zones.FirstOrDefault(_zone => (_zone.Flag & Enum_DesktopZoneFlag.AsSelected) != Enum_DesktopZoneFlag.None);
            if (selectzone != null) candrag = true;
            // 更新每个区的每张卡的可选择性。
            foreach (DesktopCardBoardZone zone in core.Zones)
                foreach (Card card in zone.Cards)
                {
                    CardView cardview = null;
                    bool canselect = false;
                    if (!card2views.TryGetValue(card, out cardview))
                        continue;
                    // 拖动模式，所在区的Loster检查是否能拖动。
                    if (candrag && zone.CardLoster?.CanLost(zone, card) == true)
                        canselect = true;
                    // 选择模式，由选择器判定是否能选择。
                    if (!candrag && core.CardFilter?.CanSelect(ctx, core.SelectedCards, card) == true)
                        canselect = true;
                    // 进入选择模式
                    cardview.IsEnterSelecting = true;
                    // 更新是否能选择的属性。
                    cardview.CanSelect = canselect;
                    // 更新已经被选择的属性，如果有选择区可以设置为false，以是否在选择区进行判定。
                    cardview.IsSelected = selectzone == null && core.SelectedCards.Contains(card);
                }
            // 是否可以结束选择。
            BN_Yes.IsEnabled = core.CardFilter.Fulfill(ctx, core.SelectedCards);
        }

        /// <summary>
        /// 测量得出鼠标放置点所代表的这个区的第几张卡。
        /// </summary>
        /// <param name="zone">逻辑卡区</param>
        /// <param name="want">想要插入的卡</param>
        /// <param name="border">卡区内UI</param>
        /// <param name="p">鼠标点（相对于内UI）</param>
        /// <returns></returns>
        protected int GetInsertIndex(DesktopCardBoardZone zone, Card want, Border border, Point p)
        {
            int index = -1;
            int count = zone.Cards.Count() + 1;
            // 过窄的内UI是不合法的。
            if (border.ActualWidth < CardView.DefaultWidth) return -1;
            // 折叠模式。
            if ((zone.Flag & Enum_DesktopZoneFlag.CanZip) != Enum_DesktopZoneFlag.None)
            {
                // 非折叠态，以单位卡牌宽度计算。
                if (CardView.DefaultWidth * count <= border.ActualWidth)
                    index = (int)(p.X / CardView.DefaultWidth);
                // 折叠态，以折叠间隔计算。
                else
                    index = (int)((p.X * (count - 1)) / (border.ActualWidth - CardView.DefaultWidth));
            }
            // 方阵模式：计算所在行和列，并求得索引。
            else
            {
                int maxcolumns = (int)(border.ActualWidth / CardView.DefaultWidth);
                int rows = (count - 1) / maxcolumns + 1;
                int columns = Math.Min(zone.Cards.Count(), maxcolumns);
                int row = (int)(p.Y / CardView.DefaultHeight);
                int column = (int)(p.X / CardView.DefaultWidth);
                index = row * columns + column;
            }
            // 保证索引合法。
            index = Math.Min(index, count - 1);
            // 索引不合法时。
            if (index < 0) return index;
            // 当前区的Getter不允许插入时。
            if (zone.CardGetter?.CanGet(zone, want, index) != true) return -1;
            return index;
        }

        /// <summary>
        /// 使用动画来对这个逻辑区进行重布局。
        /// </summary>
        /// <param name="zone">要进行重布局的区</param>
        protected void AnimatableArrange(DesktopCardBoardZone zone)
        {
            // 找到逻辑区对应的视觉区
            foreach (GroupBox gb in SP_Zones.Children)
            {
                if (gb.DataContext != zone) continue;
                Border border = gb.Content as Border;
                int index = 0;
                int count = zone.Cards.Count();
                Point p0 = border.TranslatePoint(new Point(0, 0), CV_Cards);
                //if (dragfrom == zone) count--;
                if (insezone == zone) count++;
                if (count == 0) break;
                #region 折叠模式
                if ((zone.Flag & Enum_DesktopZoneFlag.CanZip) != Enum_DesktopZoneFlag.None)
                {
                    // 计算卡的间隔。
                    double span = CardView.DefaultWidth;
                    // 空间不足时，计算折叠后的间隔。
                    if (CardView.DefaultWidth * count > border.ActualWidth)
                        span = (border.ActualWidth - CardView.DefaultWidth) / (count - 1);
                    // 对所有的卡安排动画。
                    foreach (Card card in zone.Cards)
                    {
                        //if (card == dragcard) continue;
                        if (zone == insezone && index == inseindex) index++;
                        CardView cardview = null;
                        CardMove moveanim = null;
                        // 正在进行动画的，重定向为新位置。
                        if (card2anims.TryGetValue(card, out cardview))
                        {
                            moveanim = cardview.Anims.FirstOrDefault(_anim => _anim is CardMove) as CardMove;
                            if (moveanim == null)
                            {
                                moveanim = new CardMove();
                                cardview.Anims.Add(moveanim);
                            }
                        }
                        // 没有进行动画的，安排动画。
                        else if (card2views.TryGetValue(card, out cardview))
                        {
                            moveanim = new CardMove();
                            cardview.Anims.Add(moveanim);
                            card2anims.Add(card, cardview);
                        }
                        // 动画终点重定位，并重新开始。
                        moveanim.From = cardview.Position;
                        moveanim.To = new Point(p0.X + index * span, p0.Y);
                        moveanim.Time = 0;
                    }
                }
                #endregion
                #region 方阵模式
                else
                {
                    int maxcolumns = (int)(border.ActualWidth / CardView.DefaultWidth);
                    int rows = (count - 1) / maxcolumns + 1;
                    int columns = Math.Min(zone.Cards.Count(), maxcolumns);
                    // 对所有的卡安排动画。
                    foreach (Card card in zone.Cards)
                    {
                        //if (card == dragcard) continue;
                        if (zone == insezone && index == inseindex) index++;
                        CardView cardview = null;
                        CardMove moveanim = null;
                        // 正在进行动画的，重定向为新位置。
                        if (card2anims.TryGetValue(card, out cardview))
                        {
                            moveanim = cardview.Anims.FirstOrDefault(_anim => _anim is CardMove) as CardMove;
                            if (moveanim == null)
                            {
                                moveanim = new CardMove();
                                cardview.Anims.Add(moveanim);
                            }
                        }
                        // 没有进行动画的，安排动画。
                        else if (card2views.TryGetValue(card, out cardview))
                        {
                            moveanim = new CardMove();
                            cardview.Anims.Add(moveanim);
                            card2anims.Add(card, cardview);
                        }
                        // 动画终点重定位，并重新开始。
                        int row = index / columns;
                        int column = index % columns;
                        moveanim.From = cardview.Position;
                        moveanim.To = new Point(p0.X + CardView.DefaultWidth * row, p0.Y + CardView.DefaultHeight * column);
                        moveanim.Time = 0;
                    }
                }
                #endregion 
                break;
            }
        }

        /// <summary>
        /// 使用内部定时器来处理动画。
        /// </summary>
        protected override void InternalTimerTick()
        {
            foreach (GroupBox gb in SP_Zones.Children)
            {
                DesktopCardBoardZone zone = gb.DataContext as DesktopCardBoardZone;
                Border border = gb.Content as Border;
                Point p0 = border.TranslatePoint(new Point(0, 0), CV_Cards);
                int count = zone.Cards.Count();
                int index = 0;
                //if (dragfrom == zone) count--;
                if (insezone == zone) count++;
                if (count == 0) break;
                #region 折叠模式
                if ((zone.Flag & Enum_DesktopZoneFlag.CanZip) != Enum_DesktopZoneFlag.None)
                {
                    double span = CardView.DefaultWidth;
                    if (CardView.DefaultWidth * count > border.ActualWidth)
                        span = (border.ActualWidth - CardView.DefaultWidth) / (count - 1);
                    foreach (Card card in zone.Cards)
                    {
                        //if (card == dragcard) continue;
                        if (zone == insezone && index == inseindex) index++;
                        CardView cardview = null;
                        if (card2anims.TryGetValue(card, out cardview))
                        {
                            cardview.Animation();
                            if (cardview.Anims.Count() == 0)
                                card2anims.Remove(card);
                            index++;
                            continue;
                        }
                        if (!card2views.TryGetValue(card, out cardview))
                        {
                            index++; 
                            continue;
                        }
                        cardview.Position = new Point(p0.X + index * span, p0.Y);
                        cardview.Visibility = Visibility.Visible;
                        index++;
                    }
                }
                #endregion
                #region 方阵模式
                else
                {
                    int maxcolumns = (int)(border.ActualWidth / CardView.DefaultWidth);
                    int rows = (count - 1) / maxcolumns + 1;
                    int columns = Math.Min(zone.Cards.Count(), maxcolumns);
                    foreach (Card card in zone.Cards)
                    {
                        //if (card == dragcard) continue;
                        if (zone == insezone && index == inseindex) index++;
                        CardView cardview = null;
                        if (card2anims.TryGetValue(card, out cardview))
                        {
                            cardview.Animation();
                            if (cardview.Anims.Count() == 0)
                                card2anims.Remove(card);
                            index++;
                            continue;
                        }
                        if (!card2views.TryGetValue(card, out cardview))
                        {
                            index++;
                            continue;
                        }
                        int row = index / columns;
                        int column = index % columns;
                        cardview.Position = new Point(p0.X + CardView.DefaultWidth * row, p0.Y + CardView.DefaultHeight * column);
                        cardview.Visibility = Visibility.Visible;
                        index++;
                    }
                }
                #endregion 
            }
        }

        #endregion

        #region Event Handler

        public event RoutedEventHandler Yes;

        public event RoutedEventHandler No;

        private void BN_Yes_Click(object sender, RoutedEventArgs e)
        {
            Yes?.Invoke(this, e);
        }

        private void BN_No_Click(object sender, RoutedEventArgs e)
        {
            No?.Invoke(this, e);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.gameboard = App.FindAncestor<GameBoard>(this);
            gameboard.SizeChanged += OnGameBoardSizeChanged;
            Resize();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {

        }

        private void OnGameBoardSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Resize();
        }

        private void OnTimeout(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 鼠标按下一个卡片时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CardView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (gameboard == null) return;
            Context ctx = gameboard.World.GetContext();
            if (!(sender is CardView)) return;
            CardView cardview = (CardView)sender;
            DesktopCardBoardCore core = Core;
            if (core == null) return;
            bool candrag = ((core.Flag & Enum_DesktopCardBoardFlag.CanDrag) != Enum_DesktopCardBoardFlag.None);
            DesktopCardBoardZone selectzone = core.Zones.FirstOrDefault(_zone => (_zone.Flag & Enum_DesktopZoneFlag.AsSelected) != Enum_DesktopZoneFlag.None);
            if (selectzone != null) candrag = true;
            #region 选择模式
            if (!candrag)
            {
                // 取消选择。
                if (cardview.IsSelected)
                {
                    core.SelectedCards.Remove(cardview.Card);
                    UpdateCardsSelectable();
                }
                // 进行选择。
                else if (cardview.CanSelect)
                {
                    core.SelectedCards.Add(cardview.Card);
                    UpdateCardsSelectable();
                }
                // 仅选择一个可以直接返回。
                if (core.CardFilter?.Fulfill(ctx, core.SelectedCards) == true
                 && core.SelectedCards.Count() == 1)
                    Yes?.Invoke(this, e);
            }
            #endregion
            #region 拖动模式
            else
            {
                dragfrom = null;
                dragfromindex = -1;
                dragcard = cardview.Card;
                dragvec = (Vector)e.GetPosition(cardview);
                foreach (DesktopCardBoardZone zone in core.Zones)
                {
                    int index = zone.Cards.IndexOf(cardview.Card); 
                    if (index < 0) continue;
                    dragfrom = zone;
                    dragfromindex = index;
                    break;
                }
                if (dragfrom != null)
                {
                    dragfrom.Cards.RemoveAt(dragfromindex);
                    cardview.CaptureMouse();
                }
            }
            #endregion
        }

        private void CardView_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is CardView)) return;
            CardView cardview = (CardView)sender;
            if (dragfrom == null) return;
            if (cardview.Card != dragcard) return;
            cardview.ReleaseMouseCapture();
            if (insezone != null && inseindex >= 0 && inseindex <= insezone.Cards.Count())
            {
                dragfrom.Cards.Remove(dragcard);
                insezone.Cards.Insert(inseindex, dragcard);
            }
            else
            {
                dragfrom.Cards.Insert(dragfromindex, dragcard);
                AnimatableArrange(dragfrom);
            }
            dragfrom = null;
            dragfromindex = -1;
            dragcard = null;
            dragvec = default(Vector);
            insezone = null;
            inseindex = -1;
            cardview.ReleaseMouseCapture();
        }

        private void CardView_MouseMove(object sender, MouseEventArgs e)
        {
            if (!(sender is CardView)) return;
            CardView cardview = (CardView)sender;
            if (cardview.Card != dragcard) return;
            Point p = e.GetPosition(CV_Cards);
            cardview.Position = p - dragvec;
            DesktopCardBoardZone _insezone = null;
            int _inseindex = -1;
            foreach (GroupBox gb in SP_Zones.Children)
            {
                DesktopCardBoardZone zone = gb.DataContext as DesktopCardBoardZone;
                Border bd = gb.Content as Border;
                p = e.GetPosition(bd);
                if (p.X < 0) continue;
                if (p.Y < 0) continue;
                if (p.X > bd.ActualWidth) continue;
                if (p.Y > bd.ActualHeight) continue;
                int index = GetInsertIndex(zone, cardview.Card, bd, p);
                if (index < 0) continue;
                _insezone = zone;
                _inseindex = index;
                break;
            }
            if (_insezone != insezone || inseindex != _inseindex)
            {
                DesktopCardBoardZone _insezone_old = insezone;
                insezone = _insezone;
                inseindex = _inseindex;
                if (_insezone_old != null) AnimatableArrange(_insezone_old);
                if (insezone != null && insezone != _insezone_old) AnimatableArrange(insezone);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        }

        #endregion
    }
}
