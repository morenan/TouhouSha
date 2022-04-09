using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using TouhouSha.Controls;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;
using TouhouSha.Core.UIs.Events;
using TouhouSha.Controls.Cools;

namespace TouhouSha
{
    /// <summary>
    /// GameBoard.xaml 的交互逻辑
    /// </summary>
    /// <remarks>
    /// 整个游戏的显示和控制台。
    /// 处理World运行时的各种问答交互和事件动画。
    /// </remarks>
    public partial class GameBoard : UserControl
    {
        public GameBoard()
        {
            InitializeComponent();
        }

        #region Properties

        #region World

        static public readonly DependencyProperty WorldProperty = DependencyProperty.Register(
            "World", typeof(World), typeof(GameBoard),
            new PropertyMetadata(null, OnPropertyChanged_World));
        
        /// <summary>
        /// 游戏世界
        /// </summary>
        public World World
        {
            get { return (World)GetValue(WorldProperty); }
            set { SetValue(WorldProperty, value); }
        }

        private static void OnPropertyChanged_World(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GameBoard) ((GameBoard)d).OnWorldChanged(e);
        }

        protected void OnWorldChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is World)
            {
                World world = (World)(e.OldValue);
                world.UIEvent -= World_UIEvent;
            }
            if (e.NewValue is World)
            {
                World world = (World)(e.NewValue);
                world.UIEvent += World_UIEvent;
            }
            ResetPlayers();
        }

        #endregion

        #region CurrentPlayer

        static public readonly DependencyProperty CurrentPlayerProperty = DependencyProperty.Register(
            "CurrentPlayer", typeof(Player), typeof(GameBoard),
            new PropertyMetadata(null, OnPropertyChanged_CurrentPlayer));

        /// <summary>
        /// 当前第一人称玩家
        /// </summary>
        public Player CurrentPlayer
        {
            get { return (Player)GetValue(CurrentPlayerProperty); }
            set { SetValue(CurrentPlayerProperty, value); }
        }

        private static void OnPropertyChanged_CurrentPlayer(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GameBoard) ((GameBoard)d).OnCurrentPlayerChanged(e);
        }

        protected void OnCurrentPlayerChanged(DependencyPropertyChangedEventArgs e)
        {
            this.console = CurrentPlayer != null ? new GameBoardConsole(this, CurrentPlayer) : null;
            ResetPlayers();
        }

        #endregion

        #region Status

        static public readonly DependencyProperty StatusProperty = DependencyProperty.Register(
            "Status", typeof(Enum_GameBoardAction), typeof(GameBoard),
            new PropertyMetadata(Enum_GameBoardAction.None, OnPropertyChanged_Status));

        /// <summary>
        /// 控制状态。
        /// </summary>
        public Enum_GameBoardAction Status
        {
            get { return (Enum_GameBoardAction)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        private static void OnPropertyChanged_Status(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GameBoard) ((GameBoard)d).OnStatusChanged(e);
        }

        protected void OnStatusChanged(DependencyPropertyChangedEventArgs e)
        {
            UI_Ask.IsAsking = Status == Enum_GameBoardAction.Asking;
            UI_CharactorSelectPanel.Visibility = Status == Enum_GameBoardAction.CharactorSelecting ? Visibility.Visible : Visibility.Collapsed;
            UI_CharactorSelectPanel.IsEnabled = Status == Enum_GameBoardAction.CharactorSelecting;
            UI_ListPanel.Visibility = Status == Enum_GameBoardAction.ListSelecting ? Visibility.Visible : Visibility.Collapsed;
            UI_ListPanel.IsEnabled = Status == Enum_GameBoardAction.ListSelecting;
            ShowOrHideCardSelectExtraArea();
            UpdateHandCardAboutSelection();
            UpdatePlayerAboutSelection();
            UpdateAskButtons();
        }

        #endregion

        #region PhaseStateOwner

        static public readonly DependencyProperty PhaseStateOwnerProperty = DependencyProperty.Register(
            "PhaseStateOwner", typeof(Player), typeof(GameBoard),
            new PropertyMetadata(null, OnPropertyChanged_PhaseStateOwner));

        /// <summary>
        /// 拥有回合流程阶段的玩家
        /// </summary>
        public Player PhaseStateOwner
        {
            get { return (Player)GetValue(PhaseStateOwnerProperty); }
            set { SetValue(PhaseStateOwnerProperty, value); }
        }

        private static void OnPropertyChanged_PhaseStateOwner(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GameBoard) ((GameBoard)d).OnPhaseStateOwnerChanged(e);
        }

        protected void OnPhaseStateOwnerChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is Player)
            {
                Player player = (Player)(e.OldValue);
                PlayerCard playercard = null;
                if (player2views.TryGetValue(player, out playercard))
                    playercard.IsMyPhase = false;
            }
            if (e.NewValue is Player)
            {
                Player player = (Player)(e.NewValue);
                PlayerCard playercard = null;
                if (player2views.TryGetValue(player, out playercard))
                    playercard.IsMyPhase = true;
            }
        }

        #endregion

        #region HandleStateOwner

        static public readonly DependencyProperty HandleStateOwnerProperty = DependencyProperty.Register(
            "HandleStateOwner", typeof(Player), typeof(GameBoard),
            new PropertyMetadata(null, OnPropertyChanged_HandleStateOwner));

        /// <summary>
        /// 拥有(响应,伤害,回复,濒死)阶段的玩家
        /// </summary>
        public Player HandleStateOwner
        {
            get { return (Player)GetValue(HandleStateOwnerProperty); }
            set { SetValue(HandleStateOwnerProperty, value); }
        }

        private static void OnPropertyChanged_HandleStateOwner(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GameBoard) ((GameBoard)d).OnHandleStateOwnerChanged(e);
        }

        protected void OnHandleStateOwnerChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is Player)
            {
                Player player = (Player)(e.OldValue);
                PlayerCard playercard = null;
                if (player2views.TryGetValue(player, out playercard))
                    playercard.IsHandling = false;
            }
            if (e.NewValue is Player)
            {
                Player player = (Player)(e.NewValue);
                PlayerCard playercard = null;
                if (player2views.TryGetValue(player, out playercard))
                    playercard.IsHandling = true;
            }
        }

        #endregion

        #endregion

        #region Number

        /// <summary>
        /// 当前玩家的控制端。
        /// </summary>
        private GameBoardConsole console;

        private SelectCharactorBoardCore sc_chars;
        private SelectCardBoardCore sc_cards;
        private SelectPlayerBoardCore sc_players;
        private SelectPlayerAndCardBoardCore sc_pncs;
        private ListBoardCore sc_list;
        private CardFilter usecardfilter;
        private Card cardwillused;
        private ISkillCardConverter skillconvert;
        private ISkillInitative skillinitative;
        
        private bool asked_yes;
        private bool selcard_yes;
        private bool selplayer_yes;
        private bool selpnc_yes;
        private bool sellist_yes;
        private Charactor selectedcharactor;
        private List<Card> selectedcards = new List<Card>();
        private List<Player> selectedplayers = new List<Player>();
        private List<object> selecteditems = new List<object>();
        private Event selectedevent;

        #endregion

        #region Methods

        #region Players

        private Dictionary<Player, PlayerCard> player2views = new Dictionary<Player, PlayerCard>();

        protected void ResetPlayers()
        {
            foreach (PlayerCard playercard in player2views.Values)
                playercard.Player = null;
            player2views.Clear();
            CV_Players.Children.Clear();
            if (World == null) return;
            if (CurrentPlayer == null) return;
            UI_CurrentPlayer.Player = CurrentPlayer;
            player2views.Add(CurrentPlayer, UI_CurrentPlayer);
            List<Player> others = World.GetAlivePlayersStartHere(CurrentPlayer).ToList();
            foreach (Player other in others)
            {
                PlayerCard playercard = new PlayerCard();
                playercard.Player = other;
                playercard.Width = playercard.CardWidth + 20;
                playercard.Height = playercard.CardHeight + 20;
                CV_Players.Children.Add(playercard);
                player2views.Add(other, playercard);
            }
        }

        protected void LayoutPlayers()
        {
            double aw = ActualWidth;
            double ah = ActualHeight;
            double cw = UI_CurrentPlayer.ActualWidth;
            double ch = UI_CurrentPlayer.ActualHeight;
            int n0 = (int)((ah - DP_CurrentPlayer.ActualHeight) / ch);
            int n1 = (int)((aw - cw * 2) / cw);
            while (CV_Players.Children.Count - n0 * 2 <= 0) n0--;
            n1 = CV_Players.Children.Count - n0 * 2;
            for (int i = 0; i < CV_Players.Children.Count; i++)
            {
                PlayerCard playercard = (PlayerCard)(CV_Players.Children[i]);
                if (i < n0)
                {
                    Canvas.SetLeft(playercard, aw - cw);
                    Canvas.SetTop(playercard, ah - DP_CurrentPlayer.ActualHeight - i * ch);
                }
                else if (i >= CV_Players.Children.Count - n0)
                {
                    Canvas.SetLeft(playercard, 0);
                    Canvas.SetTop(playercard, ah - DP_CurrentPlayer.ActualHeight - (CV_Players.Children.Count - i - 1) * ch);
                }
                else
                {
                    Canvas.SetLeft(playercard, (aw - n1 * cw) / 2 + (i - n0) * cw);
                    Canvas.SetTop(playercard, 0);
                }
            }
        }

        #endregion

        #region Cards

        private List<CardView> cardviews = new List<CardView>();
        private List<CardView> cardviewhiddens = new List<CardView>();
        private Dictionary<Card, CardView> card2views = new Dictionary<Card, CardView>();
        private Dictionary<Card, CardView> card2anims = new Dictionary<Card, CardView>();
        private List<UIMoveCardEvent> moveevents = new List<UIMoveCardEvent>();
        private List<UICardEvent> cardevents = new List<UICardEvent>();
        private List<UISkillActive> skillactiveevents = new List<UISkillActive>();
        private List<UIStateChangeEvent> stateevents = new List<UIStateChangeEvent>();
        private List<UIDamageEvent> damageevents = new List<UIDamageEvent>();
        private List<UIHealEvent> healevents = new List<UIHealEvent>();
        private AutoResetEvent cardanimwait = new AutoResetEvent(false);
        private DispatcherTimer cardanimtimer;
        private int cardanimtick;
        
        public CardView GetView(Card card)
        {
            CardView cardview = null;
            if (card2views.TryGetValue(card, out cardview))
                return cardview;
            return null;
        }

        public CardView Show(Card card)
        {
            CardView cardview = null;
            if (card2views.TryGetValue(card, out cardview))
                return cardview;
            if (cardviewhiddens.Count() > 0)
            {
                cardview = cardviewhiddens.LastOrDefault();
                cardviewhiddens.RemoveAt(cardviewhiddens.Count() - 1);
                cardview.Visibility = Visibility.Visible;
            }
            if (cardview == null)
            {
                cardview = new CardView();
                cardview.MouseDown += CardView_MouseDown;
                CV_Cards.Children.Add(cardview);   
            }
            if (cardview != null)
            {
                cardview.Card = card;
                card2views.Add(card, cardview);
            }
            return cardview;
        }

        public void Hide(CardView cardview)
        {
            cardview.Card = null;
            cardview.Visibility = Visibility.Collapsed;
            cardviewhiddens.Add(cardview);
        }

        public IGameBoardArea GetAreaNotKept(Zone zone)
        {
            if (zone.Owner != null)
            {
                PlayerCard playerview = null;
                player2views.TryGetValue(zone.Owner, out playerview);
                return playerview;
            }
            switch (zone.KeyName)
            {
                case Zone.Desktop:
                case Zone.Discard:
                    return UI_DiscardHeapPlacer;
                case Zone.Draw:
                    return UI_DrawHeapPlacer;
            }
            return UI_DiscardHeapPlacer;
        }

        public IGameBoardArea GetArea(Zone zone)
        {
            if (zone.Owner == CurrentPlayer && zone.KeyName?.Equals(Zone.Hand) == true) return UI_Hands;
            if (zone.Owner != null)
            {
                PlayerCard playerview = null;
                player2views.TryGetValue(zone.Owner, out playerview);
                return playerview;
            }
            switch (zone.KeyName)
            {
                case Zone.Desktop:
                case Zone.Discard:
                    return UI_DesktopPlacer;
                case Zone.Draw:
                    return UI_DrawHeapPlacer;
            }
            return null;
        }
      
        public IEnumerable<IGameBoardArea> GetKeptCardAreas()
        {
            yield return UI_Hands;
            yield return UI_DesktopPlacer;
        }

        protected void AnimMoveCrossArea(Card card, Zone from, Zone to)
        {
            CardView cardview = GetView(card);
            IGameBoardArea areafrom = null;
            IGameBoardArea areato = GetArea(to);
            bool isshowedbefore = cardview != null;
            bool isshowedafter = areato != null && areato.KeptCards;
            if (isshowedbefore)
                areafrom = GetArea(from);
            else
                areafrom = GetAreaNotKept(from);
            if (!isshowedbefore)
            {
                cardview = Show(card);
                cardview.Visibility = Visibility.Visible;
                cardview.Opacity = 0;
            }
            if (areato == null)
                areato = GetAreaNotKept(to);
            Point startpoint = cardview?.Position
                ?? areafrom?.GetExpectedPosition(CV_Cards, card)
                ?? default(Point);
            Point endpoint = areato?.GetExpectedPosition(CV_Cards, card)
                ?? default(Point);
            CardMove anim0 = new CardMove();
            CardAnim anim1 = null;
            anim0.From = startpoint;
            anim0.To = endpoint;
            if (!isshowedbefore && !isshowedafter)
                anim1 = new CardShowWaitHide();
            if (isshowedbefore && !isshowedafter)
                anim1 = new CardWaitHide();
            if (!isshowedbefore && isshowedafter)
                anim1 = new CardShowWait();
            cardview.Anims.Clear();
            cardview.Anims.Add(anim0);
            if (anim1 != null) cardview.Anims.Add(anim1);
        }
       
        protected void AnimHideDesktop(Card card)
        {
            CardView cardview = GetView(card);
            if (cardview == null) return;
            cardview.Anims.Clear();
            cardview.Anims.Add(new CardWaitHide() { WaitTime = 0, TimeMax = 10 });
            if (!card2anims.ContainsKey(card))
                card2anims.Add(card, cardview);
        }

        protected void AnimArrange(Card card, IGameBoardArea area)
        {
            CardView cardview = GetView(card);
            Point startpoint = cardview?.Position
                ?? default(Point);
            Point endpoint = area?.GetExpectedPosition(CV_Cards, card)
                ?? default(Point);
            CardMove anim0 = new CardMove();
            anim0.From = startpoint;
            anim0.To = endpoint;
            cardview.Anims.Clear();
            cardview.Anims.Add(anim0);
        }

        #endregion

        #region Targets

        private List<CrossLineWave> targetlinetrash = new List<CrossLineWave>();

        public void AnimTarget(Player source, Player target)
        {
            PlayerCard view0 = null;
            PlayerCard view1 = null;

            if (source == null) return;
            if (target == null) return;
            if (!player2views.TryGetValue(source, out view0)) return;
            if (!player2views.TryGetValue(target, out view1)) return;
            Point p0 = view0.TranslatePoint(new Point(view0.ActualWidth / 2, view0.ActualHeight / 2), CV_Lines);
            Point p1 = view1.TranslatePoint(new Point(view0.ActualWidth / 2, view0.ActualHeight / 2), CV_Lines);

            CrossLineWave line = targetlinetrash.LastOrDefault();
            if (line == null)
            {
                line = new CrossLineWave();
                line.IsVisibleChanged += TargetLine_IsVisibleChanged;
                CV_Lines.Children.Add(line);
            }   
            else
            {
                targetlinetrash.RemoveAt(targetlinetrash.Count() - 1);
            }
            line.StartPoint = p0;
            line.EndPoint = p1;
            line.AnimationStart();
        }

        private void TargetLine_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is CrossLineWave)) return;
            CrossLineWave line = (CrossLineWave)sender;
            if (line.IsVisible) return;
            targetlinetrash.Add(line);
        }

        #endregion

        #region Skill Actives

        public class SkillTextAnim
        {
            public int Tick { get; set; }
            public Point StartPosition { get; set; }
        }

        private List<BlurText> skilltexttrash = new List<BlurText>();
        private Dictionary<BlurText, SkillTextAnim> skilltextanims = new Dictionary<BlurText, SkillTextAnim>();
        private DispatcherTimer skilltexttimer;

        public void AnimSkillActive(Player player, Skill skill)
        {
            BlurText ui_text = skilltexttrash.LastOrDefault();
            PlayerCard playercard = null;
            if (player == null) return;
            if (!player2views.TryGetValue(player, out playercard)) return;
            Point p0 = playercard.TranslatePoint(new Point(playercard.ActualWidth / 2, playercard.ActualHeight - 40), CV_Skills);
            if (ui_text == null)
            {
                ui_text = new BlurText();
                ui_text.Foreground = Brushes.White;
                ui_text.HighlightForeground = Brushes.DeepSkyBlue;
                ui_text.HighlightThickness = 0;
                ui_text.BlurPower = 5;
                ui_text.FontSize = 20;
                ui_text.FontFamily = new FontFamily("幼圆");
                CV_Skills.Children.Add(ui_text);
            }
            else
            {
                //ui_text.Visibility = Visibility.Visible;
                skilltexttrash.RemoveAt(skilltexttrash.Count() - 1);
            }
            SkillTextAnim anim = new SkillTextAnim();
            anim.StartPosition = p0;
            anim.Tick = 0;
            Canvas.SetLeft(ui_text, p0.X);
            Canvas.SetTop(ui_text, p0.Y);
            ui_text.Text = skill.GetInfo().Name;
            ui_text.Opacity = 0;
            ui_text.Visibility = Visibility.Visible;
            skilltextanims.Add(ui_text, anim);
        }

        #endregion

        #region Damages

        private Dictionary<string, List<ICoolAnim>> damageanimstrash = new Dictionary<string, List<ICoolAnim>>();
        private Dictionary<ICoolAnim, string> damageanim2names = new Dictionary<ICoolAnim, string>();
        private Dictionary<ICoolAnim, Player> damageanim2players = new Dictionary<ICoolAnim, Player>();

        protected ICoolAnim CreateDamageAnim(string damagetype)
        {
            switch (damagetype)
            {
                case DamageEvent.Normal:
                    break;
                case DamageEvent.Fire:
                    return new FireAnim();
                case DamageEvent.Thunder:
                    return new ThunderAnim();
            }
            return null;
        }

        public void AnimDamage(Player player, UIDamageEvent ev)
        {
            List<ICoolAnim> trashlist = null;
            ICoolAnim anim = null;
            if (!String.IsNullOrEmpty(ev.DamageType)
             && damageanimstrash.TryGetValue(ev.DamageType, out trashlist))
                anim = trashlist.LastOrDefault();
            if (anim != null)
                trashlist.RemoveAt(trashlist.Count() - 1);
            else
            {
                anim = CreateDamageAnim(ev.DamageType);
                anim.IsVisibleChanged += DamageAnim_IsVisibleChanged;
                if (anim is FrameworkElement)
                {
                    FrameworkElement fe = (FrameworkElement)anim;
                    fe.SizeChanged += DamageAnim_SizeChanged;
                    CV_Anims.Children.Add(fe);
                }
            }
            damageanim2names.Add(anim, ev.DamageType);
            damageanim2players.Add(anim, player);
            AnimDamageLocate(anim);
            anim.AnimationStart();
        }

        protected void AnimDamageLocate(ICoolAnim anim)
        {
            FrameworkElement fe = (FrameworkElement)anim;
            Player player = null;
            if (!damageanim2players.TryGetValue(anim, out player)) return;
            PlayerCard playercard = null;
            if (!player2views.TryGetValue(player, out playercard)) return;
            Point p = playercard.TranslatePoint(new Point(0, 0), CV_Anims);
            Canvas.SetLeft(fe, p.X + playercard.ActualWidth / 2 - fe.ActualWidth / 2);
            Canvas.SetTop(fe, p.X + playercard.ActualHeight / 2 - fe.ActualHeight / 2);
        }

        private void DamageAnim_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            AnimDamageLocate(sender as ICoolAnim);
        }

        private void DamageAnim_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is ICoolAnim)) return;
            ICoolAnim anim = (ICoolAnim)sender;
            string damagetype = null;
            if (damageanim2names.TryGetValue(anim, out damagetype))
            {
                List<ICoolAnim> trashlist = null;
                if (!damageanimstrash.TryGetValue(damagetype, out trashlist))
                {
                    trashlist = new List<ICoolAnim>();
                    damageanimstrash.Add(damagetype, trashlist);  
                }
                trashlist.Add(anim);
                damageanim2names.Remove(anim);
            }
            damageanim2players.Remove(anim);
        }

        #endregion

        #region UI <==> World

        #region Ask

        public void BeginAsk(string message)
        {
            UI_Ask.Message = message;
            UI_Ask.Timeout = Config.GameConfig.Timeout_Ask;
            Status = Enum_GameBoardAction.Asking;
        }

        public bool GetAskedResult()
        {
            return asked_yes;
        }

        protected void UpdateAskButtons()
        {
            switch (Status)
            {
                case Enum_GameBoardAction.Asking:
                    UI_Ask.BN_Yes.Visibility = Visibility.Visible;
                    UI_Ask.BN_No.Visibility = Visibility.Visible;
                    UI_Ask.BN_Yes.IsEnabled = true;
                    UI_Ask.BN_No.IsEnabled = true;
                    break;
                case Enum_GameBoardAction.CardSelecting:
                    UI_Ask.BN_Yes.Visibility = Visibility.Visible;
                    UI_Ask.BN_No.Visibility = Visibility.Visible;
                    UI_Ask.BN_Yes.IsEnabled = CanEndCardSelectWithYes();
                    UI_Ask.BN_No.IsEnabled = sc_cards?.CanCancel == true;
                    break;
                case Enum_GameBoardAction.PlayerSelecting:
                    UI_Ask.BN_Yes.Visibility = Visibility.Visible;
                    UI_Ask.BN_No.Visibility = Visibility.Visible;
                    UI_Ask.BN_Yes.IsEnabled = sc_players?.PlayerFilter?.Fulfill(World.GetContext(), selectedplayers) == true;
                    UI_Ask.BN_No.IsEnabled = sc_players?.CanCancel == true;
                    break;
                case Enum_GameBoardAction.PlayerAndCardSelecting:
                    UI_Ask.BN_Yes.Visibility = Visibility.Visible;
                    UI_Ask.BN_No.Visibility = Visibility.Visible;
                    UI_Ask.BN_Yes.IsEnabled = CanEndPlayerAndCardSelectWithYes();
                    UI_Ask.BN_No.IsEnabled = sc_pncs?.CanCancel == true;
                    break;
                case Enum_GameBoardAction.CardUsing:
                    UI_Ask.BN_Yes.Visibility = Visibility.Visible;
                    UI_Ask.BN_No.Visibility = Visibility.Visible;
                    UI_Ask.BN_Yes.IsEnabled = CanPostEventInUseCardState();
                    UI_Ask.BN_No.IsEnabled = true;
                    break;
                default:
                    UI_Ask.BN_Yes.Visibility = Visibility.Collapsed;
                    UI_Ask.BN_No.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        #endregion

        #region Charactor Select

        public void BeginCharactorSelect(SelectCharactorBoardCore core)
        {
            this.sc_chars = core;
            UI_CharactorSelectPanel.ItemsSource = core.Charactors;
            UI_CharactorSelectPanel.SelectedItem = null;
            Status = Enum_GameBoardAction.CharactorSelecting;
        }

        public Charactor GetSelectedCharactor()
        {
            return selectedcharactor;
        }

        #endregion

        #region Card Select

        public void BeginCardSelect(SelectCardBoardCore core)
        {
            this.sc_cards = core;
            selectedcards.Clear();
            Status = Enum_GameBoardAction.CardSelecting;
        }

        public List<Card> GetSelectedCards(out bool isyes)
        {
            isyes = selcard_yes;
            if (skillconvert != null)
            {
                bool convert_isvalid = false;
                return GetConvertedCards(out convert_isvalid);
            }
            return selectedcards;
        }

        protected List<Card> GetConvertedCards(out bool isvalid)
        {
            if (skillconvert == null)
            {
                isvalid = false;
                return null;
            }
            Context ctx = World.GetContext();
            CardFilter cardfilter = GetSelectCardFilter();
            List<Card> convertedcards = new List<Card>();
            List<Card> remainscards = new List<Card>();
            foreach (Card card in selectedcards)
            {
                remainscards.Add(card);
                if (skillconvert.CardFilter.Fulfill(ctx, remainscards))
                {
                    if (remainscards.Count() == 1)
                        convertedcards.Add(skillconvert.CardConverter.GetValue(ctx, remainscards[0]));
                    else
                        convertedcards.Add(skillconvert.CardConverter.GetCombine(ctx, remainscards));
                    remainscards.Clear();
                }
            }
            if (remainscards.Count() > 0
             || !cardfilter.Fulfill(ctx, convertedcards))
            {
                isvalid = false;
                return null;
            }
            isvalid = true;
            return convertedcards;
        }

        public CardFilter GetSelectCardFilterWithoutReplaced()
        {
            switch (Status)
            {
                case Enum_GameBoardAction.CardSelecting:
                    return sc_cards?.CardFilter;
                case Enum_GameBoardAction.PlayerAndCardSelecting:
                    return sc_pncs?.CardFilter;
                case Enum_GameBoardAction.CardUsing:
                    return skillinitative?.CostFilter ?? usecardfilter;
            }
            return null;
        }

        public CardFilter GetSelectCardFilter()
        {
            CardFilter result = GetSelectCardFilterWithoutReplaced();
            if (result == null) return result;
            return World.TryReplaceNewCardFilter(result, null);
        }

        protected bool CanEndCardSelectWithYes()
        {
            if (skillconvert != null)
            {
                bool isvalid = false;
                GetConvertedCards(out isvalid);
                return isvalid;
            }
            return sc_cards?.CardFilter?.Fulfill(World.GetContext(), selectedcards) == true;
        }

        protected void ShowOrHideCardSelectExtraArea()
        {

        }

        protected void UpdateHandCardAboutSelection()
        {
            World world = World;
            Context ctx = world.GetContext();
            IGameBoardArea handarea = UI_Hands;
            CardFilter cardfilter = GetSelectCardFilter();
            if (cardfilter == null)
            {
                foreach (Card card in handarea.Cards)
                {
                    CardView cardview = GetView(card);
                    if (cardview == null) continue;
                    cardview.IsEnterSelecting = false;
                    cardview.IsSelected = false;
                    cardview.CanSelect = false;
                }
            }
            else if (skillconvert?.CardFilter != null && skillconvert?.CardConverter != null)
            {
                List<Card> convertedcards = new List<Card>();
                List<Card> remainscards = new List<Card>();
                foreach (Card card in selectedcards)
                {
                    remainscards.Add(card);
                    if (skillconvert.CardFilter.Fulfill(ctx, remainscards))
                    {
                        if (remainscards.Count() == 1)
                            convertedcards.Add(skillconvert.CardConverter.GetValue(ctx, remainscards[0]));
                        else
                            convertedcards.Add(skillconvert.CardConverter.GetCombine(ctx, remainscards));
                        remainscards.Clear();
                    }
                }
                foreach (Card card in handarea.Cards)
                {
                    CardView cardview = GetView(card);
                    if (cardview == null) continue;
                    Card newcard = world.CalculateCard(ctx, card);
                    cardview.IsEnterSelecting = true;
                    if (selectedcards.Contains(card))
                    {
                        cardview.IsSelected = true;
                        cardview.CanSelect = false;
                    }
                    else
                    {
                        cardview.IsSelected = false;
                        cardview.CanSelect =
                            !cardfilter.Fulfill(ctx, convertedcards)
                         && skillconvert.CardFilter.CanSelect(ctx, selectedcards, newcard) == true;
                    }
                }
            }
            else
            {
                foreach (Card card in handarea.Cards)
                {
                    CardView cardview = GetView(card);
                    if (cardview == null) continue;
                    Card newcard = world.CalculateCard(ctx, card);
                    cardview.IsEnterSelecting = true;
                    if (selectedcards.Contains(card))
                    {
                        cardview.IsSelected = true;
                        cardview.CanSelect = false;
                    }
                    else
                    {
                        cardview.IsSelected = false;
                        cardview.CanSelect = cardfilter.CanSelect(ctx, selectedcards, newcard);
                    }
                }
            }
        }

        #endregion

        #region Player Select

        public void BeginPlayerSelect(SelectPlayerBoardCore core)
        {
            this.sc_players = core;
            selectedplayers.Clear();
            Status = Enum_GameBoardAction.PlayerSelecting;
        }

        public List<Player> GetSelectedPlayers(out bool isyes)
        {
            isyes = selplayer_yes;
            return selectedplayers;
        }

        public PlayerFilter GetSelectPlayerFilterWithoutReplaced()
        {
            switch (Status)
            {
                case Enum_GameBoardAction.PlayerSelecting:
                    return sc_players?.PlayerFilter;
                case Enum_GameBoardAction.PlayerAndCardSelecting:
                    return sc_pncs?.PlayerFilter;
                case Enum_GameBoardAction.CardUsing:
                    return skillinitative?.TargetFilter ?? cardwillused?.TargetFilter;
            }
            return null;
        }

        public PlayerFilter GetSelectPlayerFilter()
        {
            PlayerFilter result = GetSelectPlayerFilterWithoutReplaced();
            if (result == null) return result;
            return World.TryReplaceNewPlayerFilter(result, null);
        }

        protected void UpdatePlayerAboutSelection()
        {
            World world = World;
            Context ctx = world.GetContext();
            PlayerFilter targetfilter = GetSelectPlayerFilter();
            if (targetfilter == null)
            {
                foreach (Player player in world.GetAlivePlayers().ToArray())
                {
                    PlayerCard playerview = null;
                    if (!player2views.TryGetValue(player, out playerview)) continue;
                    playerview.IsEnterSelecting = false;
                    playerview.CanSelect = false;
                    playerview.IsSelected = false;
                }
            }
            else
            {
                foreach (Player player in world.GetAlivePlayers().ToArray())
                {
                    PlayerCard playerview = null;
                    if (!player2views.TryGetValue(player, out playerview)) continue;
                    playerview.IsEnterSelecting = true;
                    if (selectedplayers.Contains(player))
                    {
                        playerview.IsSelected = true;
                        playerview.CanSelect = false;
                    }
                    else
                    {
                        playerview.IsSelected = false;
                        playerview.CanSelect = targetfilter.CanSelect(ctx, selectedplayers, player);
                    }
                }
            }
        }

        #endregion

        #region Player & Card Select

        public void BeginPlayerAndCardSelect(SelectPlayerAndCardBoardCore core)
        {
            this.sc_pncs = core;
            selectedcards.Clear();
            selectedplayers.Clear();
            Status = Enum_GameBoardAction.PlayerAndCardSelecting;
        }

        public void GetSelectedPlayersAndCards(out bool isyes, out List<Player> players, out List<Card> cards)
        {
            isyes = selpnc_yes;
            players = selectedplayers;
            cards = selectedcards;
        }

        protected bool CanEndPlayerAndCardSelectWithYes()
        {
            if (sc_pncs?.PlayerFilter?.Fulfill(World.GetContext(), selectedplayers) != true) return false;
            if (skillconvert != null)
            {
                bool isvalid = false;
                GetConvertedCards(out isvalid);
                return isvalid;
            }
            return sc_pncs?.CardFilter?.Fulfill(World.GetContext(), selectedcards) == true;
        }

        #endregion

        #region List Select

        public void BeginListSelect(ListBoardCore core)
        {
            this.sc_list = core;
            selecteditems.Clear();
            Status = Enum_GameBoardAction.ListSelecting;
        }

        public List<object> GetSelectedItems(out bool isyes)
        {
            isyes = sellist_yes;
            return selecteditems;
        }

        #endregion 

        #region Desktop Board

        public void ShowDesktopBoard(DesktopCardBoardCore core)
        {
            UI_DesktopBoard.Core = core;
            UI_DesktopBoard.Show();
        }

        public void ControlDesktopBoard(DesktopCardBoardCore core)
        {
            
        }

        public void CloseDesktopBoard(DesktopCardBoardCore core)
        {
            
        }

        #endregion

        #region Use Card State

        public void BeginUseCardState(Context ctx)
        {
            this.usecardfilter = new UseCardStateCardFilter(ctx);
            this.selectedevent = new LeaveUseCardStateEvent(ctx.World.GetPlayerState());
            Status = Enum_GameBoardAction.CardUsing;
        }
       
        public Event GetEventInUseCardState()
        {
            return selectedevent;
        }

        protected void UpdateCardWillUsed()
        {
            if (Status != Enum_GameBoardAction.CardUsing
             || skillinitative != null)
            {
                this.cardwillused = null;
                return;
            }
            if (skillconvert != null)
            {
                bool isvalid = false;
                List<Card> converts = GetConvertedCards(out isvalid);
                if (!isvalid)
                    this.cardwillused = null;
                else
                    this.cardwillused = converts.FirstOrDefault();
                return;
            }
            this.cardwillused = selectedcards.FirstOrDefault();
        }

        protected bool CanPostEventInUseCardState()
        {
            Context ctx = World.GetContext();
            CardFilter cardfilter = GetSelectCardFilter();
            PlayerFilter playerfilter = GetSelectPlayerFilter();
            if (cardfilter == null) return false;
            if (playerfilter == null) return false;
            if (!playerfilter.Fulfill(ctx, selectedplayers)) return false;
            if (skillconvert != null)
            {
                bool isvalid = false;
                GetConvertedCards(out isvalid);
                if (!isvalid) return false;
            }
            else
            {
                if (!cardfilter.Fulfill(ctx, selectedcards)) return false;
            }
            return false;
        }

        protected void PostEventInUseCardState()
        {
            if (!CanPostEventInUseCardState())
            {
                State state = World.GetPlayerState();
                this.selectedevent = new LeaveUseCardStateEvent(state);
                return;
            }
            if (skillinitative != null)
            {
                bool isyes_notused = false;
                List<Card> costs = GetSelectedCards(out isyes_notused);
                List<Player> targets = GetSelectedPlayers(out isyes_notused);
                SkillInitativeEvent ev = new SkillInitativeEvent();
                ev.Skill = skillinitative as Skill;
                ev.User = CurrentPlayer;
                ev.Costs.Clear();
                ev.Costs.AddRange(costs);
                ev.Targets.Clear();
                ev.Targets.AddRange(targets);
                this.selectedevent = ev;
                return;
            }
            if (cardwillused != null)
            {
                bool isyes_notused = false;
                List<Player> targets = GetSelectedPlayers(out isyes_notused);
                CardEvent ev = new CardEvent();
                ev.Card = cardwillused;
                ev.Source = CurrentPlayer;
                ev.Targets.Clear();
                ev.Targets.AddRange(targets);
                this.selectedevent = ev;
                return;
            }
            {
                State state = World.GetPlayerState();
                this.selectedevent = new LeaveUseCardStateEvent(state);
                return;
            }
        }

        protected void PostLeaveEventInUseCardState()
        {
            State state = World.GetPlayerState();
            this.selectedevent = new LeaveUseCardStateEvent(state);
        }

        #endregion

        #endregion

        #endregion

        #region Event Handler

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            IM_Background.Source = App.CreateImage(System.IO.Path.Combine(App.GetApplicationPath(), "Images", "Game"));
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsVisibleProperty)
            {
                if (IsVisible)
                {
                            
                }
                else
                {
                    
                }
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            LayoutPlayers();
        }

        private void World_UIEvent(object sender, UIEvent e)
        {
            Dispatcher.BeginInvoke((ThreadStart)delegate ()
            {
                #region 搜集动画事件
                moveevents.Clear();
                cardevents.Clear();
                skillactiveevents.Clear();
                damageevents.Clear();
                healevents.Clear();
                UIEventFind(e);
                #endregion
                #region 卡片移动动画
                // 更改过的卡片区，需要有动画来重整理布局。
                HashSet<IGameBoardArea> changedareas = new HashSet<IGameBoardArea>();
                // 桌面区的UI容器。
                IGameBoardArea desktop_area = UI_DesktopPlacer;
                // 桌面区的理想容量，建议尽量非折叠显示所有卡片。
                int desktop_capacity = (int)(ActualWidth / CardView.DefaultWidth);
                // 事先检查桌面牌数是否超过了理想容量，先进先出去掉卡牌。
                while (desktop_area.Cards.Count() > desktop_capacity)
                {
                    List<Card> removeds = ((List<Card>)(desktop_area.Cards)).GetRange(0, desktop_area.Cards.Count() - desktop_capacity);
                    ((List<Card>)(desktop_area.Cards)).RemoveRange(0, desktop_area.Cards.Count() - desktop_capacity);
                    foreach (Card card in removeds)
                        AnimHideDesktop(card);
                }
                // UI卡片容器区的添加和删除。
                // 卡片逻辑区映射一个卡片容器区，逻辑区的移入和移除同样对应于容器区。
                // 注意，弃牌区和桌面区同样映射于UI桌面区。
                foreach (UIMoveCardEvent movecard in moveevents)
                    foreach (Card card0 in movecard.MovedCards)
                        foreach (Card card in card0.GetInitialCards())
                        {
                            IGameBoardArea area0 = GetArea(movecard.OldZone);
                            IGameBoardArea area1 = GetArea(movecard.NewZone);
                            if (area0 != null && area0 != area1 && area0.KeptCards)
                            {
                                area0.Cards.Remove(card);
                                if (!changedareas.Contains(area0))
                                    changedareas.Add(area0);
                            }
                            if (area1 != null && area1 != area0 && area1.KeptCards)
                            {
                                area1.Cards.Add(card);
                                if (!changedareas.Contains(area1))
                                    changedareas.Add(area1);
                            }
                        }
                // 给每个移动卡片注册卡片动画。
                foreach (UIMoveCardEvent movecard in moveevents)
                    foreach (Card card0 in movecard.MovedCards)
                        foreach (Card card in card0.GetInitialCards())
                        {
                            if (card2anims.ContainsKey(card)) continue;
                            AnimMoveCrossArea(card, movecard.OldZone, movecard.NewZone);
                            CardView cardview = GetView(card);
                            if (cardview != null) card2anims.Add(card, cardview);
                        }
                // 给每个更改过的卡片区注册整理动画。
                foreach (IGameBoardArea area in changedareas)
                    foreach (Card card in area.Cards)
                    {
                        if (card2anims.ContainsKey(card)) continue;
                        AnimArrange(card, area);
                        CardView cardview = GetView(card);
                        if (cardview != null) card2anims.Add(card, cardview);
                    }
                // 创建卡片动画帧渲染定时器。
                if (cardanimtimer == null)
                {
                    cardanimtimer = new DispatcherTimer();
                    cardanimtimer.Interval = TimeSpan.FromMilliseconds(10);
                    cardanimtimer.Tick += CardAnimTimer_Tick;
                    cardanimtimer.Start();
                }
                // 没有卡片动画，继续后台运行。
                if (card2anims.Count() == 0)
                    cardanimwait.Set();
                #endregion
                #region 卡片使用动画
                // 绘制指向目标的直线型动画。
                foreach (UICardEvent ev in cardevents)
                {
                    if (!ev.ShowTargets) continue;
                    foreach (Player target in ev.CardTargets)
                        AnimTarget(ev.CardUser, target);
                }
                #endregion
                #region 技能使用动画
                foreach (UISkillActive ev in skillactiveevents)
                {
                    // 发动者卡牌上方，浮动文字宣称技能。
                    AnimSkillActive(ev.SkillActiver, ev.Skill);
                    // 绘制指向目标的直线型动画。
                    if (ev.ShowTargets) 
                        foreach (Player target in ev.SkillTargets)
                            AnimTarget(ev.SkillActiver, target);
                }
                // 创建技能动画帧渲染定时器。
                if (skilltexttimer == null)
                {
                    skilltexttimer = new DispatcherTimer();
                    skilltexttimer.Interval = TimeSpan.FromMilliseconds(10);
                    skilltexttimer.Tick += SkillTextTimer_Tick;
                    skilltexttimer.Start();
                }
                #endregion
                #region 伤害动画技能

                #endregion
                #region 回复动画技能

                #endregion
            });
            // 卡片移动完毕才可后续后台运行
            cardanimwait.WaitOne();
        }

        private void CardAnimTimer_Tick(object sender, EventArgs e)
        {
            if (card2anims.Count() == 0) return;
            foreach (KeyValuePair<Card, CardView> kvp in card2anims.ToArray())
            {
                kvp.Value.Animation();
                if (kvp.Value.Anims.Count() == 0) 
                    card2anims.Remove(kvp.Key);
            }
            if (card2anims.Count() == 0) cardanimwait.Reset();
        }

        private void SkillTextTimer_Tick(object sender, EventArgs e)
        {
            int tickmax = 100;
            foreach (KeyValuePair<BlurText, SkillTextAnim> kvp in skilltextanims.ToArray())
            {
                int tick = ++kvp.Value.Tick;
                kvp.Key.Opacity = tick < 10 ? tick / 10.0d
                    : tick >= tickmax - 10 ? (tick - tickmax + 10) / 10.0d
                    : 1.0d;
                Canvas.SetLeft(kvp.Key, kvp.Value.StartPosition.X);
                Canvas.SetTop(kvp.Key, kvp.Value.StartPosition.Y + 60.0d * tick / tickmax);
                if (tick >= tickmax)
                {
                    kvp.Key.Visibility = Visibility.Collapsed;
                    skilltextanims.Remove(kvp.Key);
                    skilltexttrash.Add(kvp.Key);
                }
            }
        }

        protected void UIEventFind(UIEvent e)
        {
            if (e is UIEventGroup)
            {
                UIEventGroup group = (UIEventGroup)e;
                foreach (UIEvent sub in group.Items)
                    UIEventFind(sub);
            }
            else if (e is UIEventFromLogical)
            {
                UIEventFromLogical fromlogic = (UIEventFromLogical)e;
                if (fromlogic.LogicalEvent is MoveCardDoneEvent)
                {
                    MoveCardDoneEvent mc = (MoveCardDoneEvent)(fromlogic.LogicalEvent);
                    List<Card> initcards = new List<Card>();
                    foreach (Card card in mc.MovedCards)
                        initcards.AddRange(card.GetInitialCards());
                    UIMoveCardEvent uimc = new UIMoveCardEvent(initcards, mc.OldZone, mc.NewZone);
                    moveevents.Add(uimc);
                }
                else if (fromlogic.LogicalEvent is CardPreviewEvent)
                {
                    CardPreviewEvent cp = (CardPreviewEvent)(fromlogic.LogicalEvent);
                    UICardEvent uice = new UICardEvent();
                    uice.Card = cp.Card;
                    uice.CardUser = cp.Source;
                    uice.CardTargets.Clear();
                    uice.CardTargets.AddRange(cp.Targets);
                    uice.ShowTargets = !(cp.Reason is CardEventBase);
                    cardevents.Add(uice);
                }
                else if (fromlogic.LogicalEvent is SkillEvent)
                {
                    SkillEvent se = (SkillEvent)(fromlogic.LogicalEvent);
                    UISkillActive uisa = new UISkillActive();
                    uisa.Skill = se.Skill;
                    uisa.SkillActiver = se.Source;
                    uisa.SkillTargets.Clear();
                    uisa.SkillTargets.AddRange(se.Targets);
                    skillactiveevents.Add(uisa);
                }
                else if (fromlogic.LogicalEvent is StateChangeEvent)
                {
                    StateChangeEvent sc = (StateChangeEvent)(fromlogic.LogicalEvent);
                    UIStateChangeEvent uisc = new UIStateChangeEvent();
                    uisc.OldState = sc.OldState;
                    uisc.NewState = sc.NewState;
                    stateevents.Add(uisc);
                }
                else if (fromlogic.LogicalEvent is DamageEvent)
                {
                    DamageEvent da = (DamageEvent)(fromlogic.LogicalEvent);
                    UIDamageEvent uida = new UIDamageEvent();
                    uida.Source = da.Source;
                    uida.Target = da.Target;
                    uida.DamageValue = da.DamageValue;
                    uida.DamageType = da.DamageType;
                    damageevents.Add(uida);
                }
                else if (fromlogic.LogicalEvent is HealEvent)
                {
                    HealEvent he = (HealEvent)(fromlogic.LogicalEvent);
                    UIHealEvent uihe = new UIHealEvent();
                    uihe.Source = he.Source;
                    uihe.Target = he.Target;
                    uihe.HealValue = he.HealValue;
                    uihe.HealType = he.HealType;
                    healevents.Add(uihe);
                }
            }
        }

        #region UI_Ask

        private void UI_Ask_Yes(object sender, RoutedEventArgs e)
        {
            switch (Status)
            {
                case Enum_GameBoardAction.Asking:
                    asked_yes = true;
                    Status = Enum_GameBoardAction.None;
                    console.WorldContiune();
                    break;
                case Enum_GameBoardAction.CardSelecting:
                    selcard_yes = true;
                    Status = Enum_GameBoardAction.None;
                    console.WorldContiune();
                    break;
                case Enum_GameBoardAction.PlayerSelecting:
                    selplayer_yes = true;
                    Status = Enum_GameBoardAction.None;
                    console.WorldContiune();
                    break;
                case Enum_GameBoardAction.PlayerAndCardSelecting:
                    selpnc_yes = true;
                    Status = Enum_GameBoardAction.None;
                    console.WorldContiune();
                    break;
                case Enum_GameBoardAction.CardUsing:
                    PostEventInUseCardState();
                    Status = Enum_GameBoardAction.None;
                    console.WorldContiune();
                    break;
            }
        }

        private void UI_Ask_No(object sender, RoutedEventArgs e)
        {
            switch (Status)
            {
                case Enum_GameBoardAction.Asking:
                    asked_yes = false;
                    Status = Enum_GameBoardAction.None;
                    console.WorldContiune();
                    break;
                case Enum_GameBoardAction.CardSelecting:
                    selcard_yes = false;
                    Status = Enum_GameBoardAction.None;
                    console.WorldContiune();
                    break;
                case Enum_GameBoardAction.PlayerSelecting:
                    selplayer_yes = false;
                    Status = Enum_GameBoardAction.None;
                    console.WorldContiune();
                    break;
                case Enum_GameBoardAction.PlayerAndCardSelecting:
                    selpnc_yes = false;
                    Status = Enum_GameBoardAction.None;
                    console.WorldContiune();
                    break;
                case Enum_GameBoardAction.CardUsing:
                    PostLeaveEventInUseCardState();
                    Status = Enum_GameBoardAction.None;
                    console.WorldContiune();
                    break;
            }
        }

        #endregion

        #region Card

        private void CardView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is CardView)) return;
            CardView cardview = (CardView)sender;
            if (cardview.IsSelected)
            {
                //cardview.IsSelected = false;
                selectedcards.Remove(cardview.Card);
                UpdateHandCardAboutSelection();
                UpdateAskButtons();
            }
            else if (cardview.CanSelect)
            {
                //cardview.IsSelected = true;
                selectedcards.Add(cardview.Card);
                UpdateHandCardAboutSelection();
                UpdateAskButtons();
            }
        }

        #endregion

        #region Player

        private void PlayerCard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is PlayerCard)) return;
            PlayerCard playerview = (PlayerCard)sender;
            if (playerview.IsSelected)
            {
                //playerview.IsSelected = false;
                selectedplayers.Remove(playerview.Player);
                UpdateCardWillUsed();
                UpdatePlayerAboutSelection();
                UpdateAskButtons();
            }
            else if (playerview.CanSelect)
            {
                //playerview.IsSelected = true;
                selectedplayers.Add(playerview.Player);
                UpdateCardWillUsed();
                UpdatePlayerAboutSelection();
                UpdateAskButtons();
            }
        }

        #endregion 

        #endregion

    }

    public class GameBoardConsole : IPlayerConsole
    {
        public GameBoardConsole(GameBoard _parent, Player _controller)
        {
            this.parent = _parent;
            this.controller = _controller;
            this.worldstop = new AutoResetEvent(false);
        }

        #region Number

        private GameBoard parent;
        public GameBoard Parent
        {
            get { return this.parent; }
        }

        private Player controller;
        public Player Controller
        {
            get { return this.controller; }
        }

        Player IPlayerConsole.Owner
        {
            get { return Controller; }
        }

        private AutoResetEvent worldstop;

        #endregion

        #region Method

        protected void WaitAndDispatch(Action action)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                parent.Dispatcher.Invoke(action);
            });
            worldstop.WaitOne();
        }

        internal void WorldContiune()
        {
            worldstop.Set();
        }

        #endregion

        #region IPlayerConsole

        void IPlayerConsole.MarkCards(IList<Card> cards)
        {

        }

        void IPlayerConsole.MarkCharactors(IList<Charactor> charactors)
        {

        }

        void IPlayerConsole.MarkFilters(IList<Filter> filters)
        {

        }

        void IPlayerConsole.MarkPlayers(IList<Player> players)
        {

        }

        void IPlayerConsole.MarkSkills(IList<Skill> skills)
        {

        }

        void IPlayerConsole.MarkZones(IList<Zone> zones)
        {

        }

        bool IPlayerConsole.Ask(Context ctx, string keyname, string message, int timeout)
        {
            WaitAndDispatch(() => { parent.BeginAsk(message); });
            return parent.GetAskedResult();
        }

        void IPlayerConsole.SelectCharactor(SelectCharactorBoardCore core)
        {
            WaitAndDispatch(() => { parent.BeginCharactorSelect(core); });
            core.SelectedCharactor = parent.GetSelectedCharactor();
        }

        void IPlayerConsole.SelectCards(SelectCardBoardCore core)
        {
            WaitAndDispatch(() => { parent.BeginCardSelect(core); });
            bool isyes = false;
            core.SelectedCards.Clear();
            core.SelectedCards.AddRange(parent.GetSelectedCards(out isyes));
            core.IsYes = isyes;
        }

        void IPlayerConsole.SelectPlayers(SelectPlayerBoardCore core)
        {
            WaitAndDispatch(() => { parent.BeginPlayerSelect(core); });
            bool isyes = false;
            core.SelectedPlayers.Clear();
            core.SelectedPlayers.AddRange(parent.GetSelectedPlayers(out isyes));
            core.IsYes = isyes;
        }

        void IPlayerConsole.SelectDesktop(DesktopCardBoardCore core)
        {
            WaitAndDispatch(() => { parent.ShowDesktopBoard(core); });
            if (!core.IsAsync)
            {
                bool isyes = false;
                core.SelectedCards.Clear();
                core.SelectedCards.AddRange(parent.GetSelectedCards(out isyes));
                core.IsYes = isyes;
            }
        }

        void IPlayerConsole.ControlDesktop(DesktopCardBoardCore core)
        {
            WaitAndDispatch(() => { parent.ControlDesktopBoard(core); });
        }

        void IPlayerConsole.CloseDesktop(DesktopCardBoardCore core)
        {
            WaitAndDispatch(() => { parent.CloseDesktopBoard(core); });
        }

        void IPlayerConsole.SelectList(ListBoardCore core)
        {
            WaitAndDispatch(() => { parent.BeginListSelect(core); });
            bool isyes = false;
            core.SelectedItems.Clear();
            core.SelectedItems.AddRange(parent.GetSelectedItems(out isyes));
            core.IsYes = isyes;
        }

        Event IPlayerConsole.QuestEventInUseCardState(Context ctx)
        {
            WaitAndDispatch(() => { parent.BeginUseCardState(ctx); });
            return parent.GetEventInUseCardState();
        }

        #endregion

    }


    public enum Enum_GameBoardAction
    {
        None,
        CharactorSelecting,
        CardUsing,
        CardSelecting,
        PlayerSelecting,
        PlayerAndCardSelecting,
        ListSelecting,
        Asking,
    }

    public enum Enum_GameBoardActiveFlag
    {
        None = 0,
        Hand = 1,
        Equip = 2,
        Judge = 4,
        OtherZones = 8,
        OtherPlayers = 16,
    }

}
