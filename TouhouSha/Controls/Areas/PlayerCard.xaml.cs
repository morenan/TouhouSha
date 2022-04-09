using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.UIs;

namespace TouhouSha.Controls
{
    /// <summary>
    /// PlayerCard.xaml 的交互逻辑
    /// </summary>
    public partial class PlayerCard : UserControl, IGameBoardArea
    {
        public PlayerCard()
        {
            InitializeComponent();
        }

        #region Properties

        #region Player

        static public readonly DependencyProperty PlayerProperty = DependencyProperty.Register(
            "Player", typeof(Player), typeof(PlayerCard),
            new PropertyMetadata(null, OnPropertyChanged_Player));

        public Player Player
        {
            get { return (Player)GetValue(PlayerProperty); }
            set { SetValue(PlayerProperty, value); }
        }

        static public void OnPropertyChanged_Player(object d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PlayerCard) ((PlayerCard)d).OnPlayerChanged(e);
        }

        public virtual void OnPlayerChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is Player)
            {
                Player player = (Player)(e.OldValue);
                player.ShaPropertyChanged -= OnPlayerShaPropertyChanged;
                player.Zones.CollectionChanged -= OnPlayerZonesCollectionChanged;
            }   
            if (e.NewValue is Player)
            {
                Player player = (Player)(e.NewValue);
                player.ShaPropertyChanged += OnPlayerShaPropertyChanged;
                player.Zones.CollectionChanged += OnPlayerZonesCollectionChanged;
            }
            UpdateInfo();
            UpdateHp();
            UpdateFacedDown();
            UpdateSymbols();
            FindEquipZone();
        }
        
        #endregion

        #region EquipZone

        static public readonly DependencyProperty EquipZoneProperty = DependencyProperty.Register(
            "EquipZone", typeof(EquipZone), typeof(PlayerCard),
            new PropertyMetadata(null, OnPropertyChanged_EquipZone));

        public EquipZone EquipZone
        {
            get { return (EquipZone)GetValue(EquipZoneProperty); }
            set { SetValue(EquipZoneProperty, value); }
        }

        static public void OnPropertyChanged_EquipZone(object d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PlayerCard) ((PlayerCard)d).OnEquipZoneChanged(e);
        }

        public virtual void OnEquipZoneChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is EquipZone)
            {
                EquipZone zone = (EquipZone)(e.OldValue);
                zone.EquipsChanged -= EquipZone_EquipsChanged;
            }
            if (e.NewValue is EquipZone)
            {
                EquipZone zone = (EquipZone)(e.NewValue);
                zone.EquipsChanged += EquipZone_EquipsChanged;
            }
        }
        
        #endregion
        
        #region CardWidth

        static public readonly DependencyProperty CardWidthProperty = DependencyProperty.Register(
            "CardWidth", typeof(double), typeof(PlayerCard),
            new PropertyMetadata(120.0d));

        public double CardWidth
        {
            get { return (double)GetValue(CardWidthProperty); }
            set { SetValue(CardWidthProperty, value); }
        }

        #endregion

        #region CardHeight

        static public readonly DependencyProperty CardHeightProperty = DependencyProperty.Register(
            "CardHeight", typeof(double), typeof(PlayerCard),
            new PropertyMetadata(180.0d));

        public double CardHeight
        {
            get { return (double)GetValue(CardHeightProperty); }
            set { SetValue(CardHeightProperty, value); }
        }

        #endregion

        #region IsMyPhase

        static public readonly DependencyProperty IsMyPhaseProperty = DependencyProperty.Register(
            "IsMyPhase", typeof(bool), typeof(PlayerCard),
            new PropertyMetadata(false, OnPropertyChanged_IsMyPhase));

        public bool IsMyPhase
        {
            get { return (bool)GetValue(IsMyPhaseProperty); }
            set { SetValue(IsMyPhaseProperty, value); }
        }

        private static void OnPropertyChanged_IsMyPhase(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PlayerCard) ((PlayerCard)d).OnIsMyPhaseChanged(e);
        }

        protected virtual void OnIsMyPhaseChanged(DependencyPropertyChangedEventArgs e)
        {
            UI_Cool.Visibility = IsMyPhase ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region IsHandling

        static public readonly DependencyProperty IsHandlingProperty = DependencyProperty.Register(
            "IsHandling", typeof(bool), typeof(PlayerCard),
            new PropertyMetadata(false, OnPropertyChanged_IsHandling));

        public bool IsHandling
        {
            get { return (bool)GetValue(IsHandlingProperty); }
            set { SetValue(IsHandlingProperty, value); }
        }

        private static void OnPropertyChanged_IsHandling(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PlayerCard) ((PlayerCard)d).OnIsHandlingChanged(e);
        }

        protected virtual void OnIsHandlingChanged(DependencyPropertyChangedEventArgs e)
        {
            UI_Cool2.Visibility = IsMyPhase ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region About Selection System

        #region IsEnterSelecting

        static public readonly DependencyProperty IsEnterSelectingProperty = DependencyProperty.Register(
            "IsEnterSelecting", typeof(bool), typeof(PlayerCard),
            new PropertyMetadata(false, OnPropertyChanged_IsEnterSelecting));

        public bool IsEnterSelecting
        {
            get { return (bool)GetValue(IsEnterSelectingProperty); }
            set { SetValue(IsEnterSelectingProperty, value); }
        }

        private static void OnPropertyChanged_IsEnterSelecting(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PlayerCard) ((PlayerCard)d).OnIsEnterSelectingChanged(e);
        }

        protected virtual void OnIsEnterSelectingChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region CanSelect

        static public readonly DependencyProperty CanSelectProperty = DependencyProperty.Register(
            "CanSelect", typeof(bool), typeof(PlayerCard),
            new PropertyMetadata(false, OnPropertyChanged_CanSelect));

        public bool CanSelect
        {
            get { return (bool)GetValue(CanSelectProperty); }
            set { SetValue(CanSelectProperty, value); }
        }

        private static void OnPropertyChanged_CanSelect(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PlayerCard) ((PlayerCard)d).OnCanSelectChanged(e);
        }

        protected virtual void OnCanSelectChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region IsWillSelecting

        static public readonly DependencyProperty IsWillSelectingProperty = DependencyProperty.Register(
            "IsWillSelecting", typeof(bool), typeof(PlayerCard),
            new PropertyMetadata(false, OnPropertyChanged_IsWillSelecting));

        public bool IsWillSelecting
        {
            get { return (bool)GetValue(IsWillSelectingProperty); }
            set { SetValue(IsWillSelectingProperty, value); }
        }

        private static void OnPropertyChanged_IsWillSelecting(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PlayerCard) ((PlayerCard)d).OnIsWillSelectingChanged(e);
        }

        protected virtual void OnIsWillSelectingChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #region IsSelected

        static public readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(
            "IsSelected", typeof(bool), typeof(PlayerCard),
            new PropertyMetadata(false, OnPropertyChanged_IsSelected));

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        private static void OnPropertyChanged_IsSelected(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PlayerCard) ((PlayerCard)d).OnIsSelectedChanged(e);
        }

        protected virtual void OnIsSelectedChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #endregion

        #endregion

        #region Numbers

        
        #endregion

        #region Methods

        protected void UpdateInfo()
        {
            if (Player == null) return;
            Charactor char0 = Player.Charactors.FirstOrDefault();
            CharactorInfoCore charinfo = App.GetCharactorInfo(char0);
            IM_Card.Source = charinfo.Image.Source;
            HT_Name.Text = Player.Name;
            HT_Country.Text = App.GetCountryName(Player.Country);
            
        }

        protected void UpdateHp()
        {
            Dispatcher.Invoke(() =>
            {
                Player player = Player;
                if (player == null) return;
                UI_Hp.MaxHp = player.MaxHP;
                UI_Hp.Hp = player.HP;
            }); 
        }

        protected void UpdateCardLayout()
        {
            Canvas.SetTop(UI_Equips, CardHeight - UI_Equips.ActualHeight - 8);
            UI_Equips.Width = CardWidth - 16;
        }

        protected void UpdateEquipItems()
        {
            if (EquipZone == null) return;
            for (int i = 0; i < EquipZone.Cells.Count(); i++)
            {
                while (i >= UI_Equips.Items.Count())
                    UI_Equips.Items.Add(null);
                if (EquipZone.Cells[i].CardIndex < 0)
                    UI_Equips.Items[i] = null;
                else if (EquipZone.Cells[i].CardIndex >= EquipZone.Cards.Count())
                    UI_Equips.Items[i] = null;
                else 
                    UI_Equips.Items[i] = EquipZone.Cards[EquipZone.Cells[i].CardIndex];
            }
        }

        protected void FindEquipZone()
        {
            EquipZone = Player?.Zones?.FirstOrDefault(_zone => _zone is EquipZone) as EquipZone;
        }

        protected void UpdateAlive()
        {

        }

        protected void UpdateChained()
        {

        }

        protected void UpdateFacedDown()
        {
            BD_FacedDown.Visibility = Player?.IsFacedDown == true ? Visibility.Visible : Visibility.Collapsed;
        }

        protected void UpdateSymbols()
        {
            if (Player == null) return;
            int index = 0;
            foreach (Zone zone in Player.Zones)
            {
                if ((zone.Flag & Enum_ZoneFlag.LabelOnPlayer) == Enum_ZoneFlag.None) continue;
                while (index >= SP_Symbols.Children.Count) SP_Symbols.Children.Add(new SymbolBox());
                ((SymbolBox)(SP_Symbols.Children[index++])).Zone = zone;
            }
            while (index < SP_Symbols.Children.Count) SP_Symbols.Children.RemoveAt(SP_Symbols.Children.Count - 1);
        }
        

        #endregion

        #region IGameBoardArea

        bool IGameBoardArea.KeptCards => false;

        IList<Card> IGameBoardArea.Cards => new List<Card>();

        Point? IGameBoardArea.GetExpectedPosition(Canvas cardcanvas, Card card)
        {
            Point p0 = TranslatePoint(new Point(0, 0), cardcanvas);
            double aw = ActualWidth;
            double ah = ActualHeight;
            double uw = CardView.DefaultWidth;
            double uh = CardView.DefaultHeight;
            double x0 = (aw - uw) / 2;
            double y0 = (ah - uh) / 2;
            return new Point(
                p0.X + x0,
                p0.Y + y0);
        }

        #endregion

        #region Event Handler

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsVisibleProperty)
            {
            }
            if (e.Property == CardWidthProperty
             || e.Property == CardHeightProperty)
                UpdateCardLayout();
        }

        private void OnPlayerShaPropertyChanged(object sender, ShaPropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "HP":
                case "MaxHP":
                    Dispatcher.Invoke(UpdateHp);
                    break;
                case "IsAlive":
                    Dispatcher.Invoke(UpdateAlive);
                    break;
                case "IsFacedDown":
                    Dispatcher.Invoke(UpdateFacedDown);
                    break;
                case "IsChained":
                    Dispatcher.Invoke(UpdateChained);
                    break;
            }
        }

        private void OnPlayerZonesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                FindEquipZone();
                UpdateSymbols();
            });
        }

        private void EquipZone_EquipsChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(UpdateEquipItems);
        }

        private void UI_Equips_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCardLayout();
        }

        #endregion

    }

}
