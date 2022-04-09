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
using System.Windows.Media.Animation;
using EventHandler = System.EventHandler;
using TouhouSha.Core.UIs;

namespace TouhouSha.Controls
{
    /// <summary>
    /// CharactorSelectPanel.xaml 的交互逻辑
    /// </summary>
    public partial class CharactorSelectPanel : BoxedControl
    {
        public CharactorSelectPanel()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }


        #region Properties

        #region ItemsSource

        static public DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource", typeof(IList<Charactor>), typeof(CharactorSelectPanel),
            new PropertyMetadata(null, OnPropertyChanged_ItemsSource));

        public IList<Charactor> ItemsSource
        {
            get { return (IList<Charactor>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        private static void OnPropertyChanged_ItemsSource(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CharactorSelectPanel) ((CharactorSelectPanel)d).OnItemsSourceChanged(e);
        }

        protected virtual void OnItemsSourceChanged(DependencyPropertyChangedEventArgs e)
        {
            IList<Charactor> charlist = ItemsSource;
            if (charlist == null) return;
            double uw = 100 + 8;
            double uh = 130 + 8;
            int columns = Math.Min(8, ItemsSource.Count);
            int rows = (charlist.Count - 1) / columns + 1;
            while (GD_Chars.RowDefinitions.Count < rows)
                GD_Chars.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(uh, GridUnitType.Pixel) });
            while (GD_Chars.RowDefinitions.Count > rows)
                GD_Chars.RowDefinitions.RemoveAt(GD_Chars.RowDefinitions.Count - 1);
            while (GD_Chars.ColumnDefinitions.Count < columns)
                GD_Chars.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(uw, GridUnitType.Pixel) });
            while (GD_Chars.ColumnDefinitions.Count > columns)
                GD_Chars.ColumnDefinitions.RemoveAt(GD_Chars.ColumnDefinitions.Count - 1);
            foreach (CharactorGalleryItem item in GD_Chars.Children)
            {
                item.MouseEnter -= OnCharactorImageMouseEnter;
                item.MouseLeave -= OnCharactorImageMouseLeave;
                item.MouseDown -= OnCharactorImageMouseDown;
                item.Core = null;
            }
            GD_Chars.Children.Clear();
            tick = 0;
            for (int i = 0; i < charlist.Count; i++)
            {
                CharactorGalleryItem item = new CharactorGalleryItem();
                item.Core = charlist[i];
                item.Opacity = 0;
                item.Margin = new Thickness(0, -60, 0, 60);
                item.MouseEnter += OnCharactorImageMouseEnter;
                item.MouseLeave += OnCharactorImageMouseLeave;
                item.MouseDown += OnCharactorImageMouseDown;
                GD_Chars.Children.Add(item);
                Grid.SetRow(item, i / columns);
                Grid.SetColumn(item, i % columns);
            }
        }

        #endregion

        #region MouseOverItem

        static public DependencyProperty MouseOverItemProperty = DependencyProperty.Register(
            "MouseOverItem", typeof(Charactor), typeof(CharactorSelectPanel),
            new PropertyMetadata(null));

        public Charactor MouseOverItem
        {
            get { return (Charactor)GetValue(MouseOverItemProperty); }
            set { SetValue(MouseOverItemProperty, value); }
        }

        #endregion

        #region SelectedItem

        static public DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem", typeof(Charactor), typeof(CharactorSelectPanel),
            new PropertyMetadata(null));

        public Charactor SelectedItem
        {
            get { return (Charactor)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        #endregion

        #region Message

        static public readonly DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof(string), typeof(CharactorSelectPanel),
            new PropertyMetadata(String.Empty));

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        #endregion

        #region TimeValue

        static public readonly DependencyProperty TimeValueProperty = DependencyProperty.Register(
            "TimeValue", typeof(double), typeof(CharactorSelectPanel),
            new PropertyMetadata(0.0d));

        public double TimeValue
        {
            get { return (double)GetValue(TimeValueProperty); }
            set { SetValue(TimeValueProperty, value); }
        }

        #endregion

        #region Timeout

        static public readonly DependencyProperty TimeoutProperty = DependencyProperty.Register(
            "Timeout", typeof(double), typeof(CharactorSelectPanel),
            new PropertyMetadata(45.0d));

        public double Timeout
        {
            get { return (double)GetValue(TimeoutProperty); }
            set { SetValue(TimeoutProperty, value); }
        }

        #endregion

        #region TimeAnim

        static public DependencyProperty TimeAnimProperty = DependencyProperty.Register(
            "TimeAnim", typeof(DoubleAnimation), typeof(CharactorSelectPanel),
            new PropertyMetadata(null, OnPropertyChanged_TimeAnim));

        public DoubleAnimation TimeAnim
        {
            get { return (DoubleAnimation)GetValue(TimeAnimProperty); }
            set { SetValue(TimeAnimProperty, value); }
        }

        private static void OnPropertyChanged_TimeAnim(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CharactorSelectPanel) ((CharactorSelectPanel)d).OnTimeAnimChanged(e);
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

        private int tick;
        private ICharactorTooltipPlacer tooltipplacer;
        private Canvas ui_tooltipcanvas;
        private CharactorTooltip ui_tooltip;
        private AbilityRadarTooltip ui_radar;
        private int tooltip_tick;
        private int tooltip_tickmax = 20;

        #endregion

        #region Method

        protected override void InternalTimerTick()
        {
            base.InternalTimerTick();
            #region 渐变动画
            if (tick < GD_Chars.Children.Count * 10 + 10)
            {
                tick++;
                for (int i = 0; i < GD_Chars.Children.Count; i++)
                {
                    CharactorGalleryItem item = (CharactorGalleryItem)(GD_Chars.Children[i]);
                    if (tick - 10 * i <= 0)
                    {
                        item.Opacity = 0;
                        item.Margin = new Thickness(0, -60, 0, 60);
                    }
                    else if (tick - 10 * i <= 20)
                    {
                        item.Opacity = (tick - 10 * i) / 20.0d;
                        item.Margin = new Thickness(0, -60 * (1 - item.Opacity), 0, 60 * (1 - item.Opacity));
                    }
                    else
                    {
                        item.Opacity = 1;
                        item.Margin = new Thickness(0);
                    }
                }
                if (tick >= GD_Chars.Children.Count * 10 + 10)
                {
                    DoubleAnimation timeanim = new DoubleAnimation();
                    timeanim.From = Timeout;
                    timeanim.To = 0;
                    timeanim.Duration = new Duration(TimeSpan.FromSeconds(Timeout));
                    TimeAnim = timeanim;
                }
            }
            #endregion
            #region 角色提示
            if (ui_tooltip.Core != MouseOverItem)
            {
                if (tooltip_tick > 0)
                {
                    ui_tooltip.Opacity = (double)(--tooltip_tick) / tooltip_tickmax;
                    ui_radar.Opacity = ui_tooltip.Opacity;
                }
                else
                {
                    ui_tooltip.Core = MouseOverItem;
                    if (MouseOverItem == null)
                    {
                        ui_tooltip.Visibility = Visibility.Collapsed;
                        ui_radar.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        CharactorInfoCore charinfo = App.GetCharactorInfo(MouseOverItem);
                        ui_radar.Core = charinfo?.AbilityRadar;
                    }
                }
            }
            else if (ui_tooltip.Core != null)
            {
                ui_tooltip.Visibility = Visibility.Visible;
                ui_radar.Visibility = Visibility.Visible;
                if (tooltip_tick < tooltip_tickmax)
                {
                    ui_tooltip.Opacity = (double)(++tooltip_tick) / tooltip_tickmax;
                    ui_radar.Opacity = ui_tooltip.Opacity;
                }
            }
            if (ui_tooltip.IsVisible)
            {
                CharactorGalleryItem viewitem = GD_Chars.Children.Cast<CharactorGalleryItem>().FirstOrDefault(_item => _item.Core == ui_tooltip.Core);
                if (viewitem != null)
                {
                    Point p = viewitem.TranslatePoint(new Point(0, 0), ui_tooltipcanvas);
                    if (p.X > ui_tooltipcanvas.ActualWidth - p.X - viewitem.ActualWidth)
                    {
                        Canvas.SetLeft(ui_tooltip, p.X - ui_tooltip.ActualWidth);
                        if (p.X - ui_tooltip.ActualWidth > ui_radar.ActualWidth)
                            Canvas.SetLeft(ui_radar, p.X - ui_tooltip.ActualWidth - ui_radar.ActualWidth);
                        else
                            Canvas.SetLeft(ui_radar, p.X);
                    }
                    else
                    {
                        Canvas.SetLeft(ui_tooltip, p.X + viewitem.ActualWidth);
                        if (ui_tooltipcanvas.ActualWidth - p.X - viewitem.ActualWidth - ui_tooltip.ActualWidth > ui_radar.ActualWidth)
                            Canvas.SetLeft(ui_radar, p.X + viewitem.ActualWidth + ui_tooltip.ActualWidth);
                        else
                            Canvas.SetLeft(ui_radar, p.X - ui_radar.ActualWidth);
                    }
                    if (p.Y > ui_tooltipcanvas.ActualHeight - p.Y - viewitem.ActualHeight)
                        Canvas.SetTop(ui_radar, p.Y - ui_radar.ActualHeight);
                    else
                        Canvas.SetTop(ui_radar, p.Y + viewitem.ActualHeight);
                    Canvas.SetTop(ui_tooltip, Math.Min(p.Y, ui_tooltipcanvas.ActualHeight - ui_tooltip.ActualHeight));
                }
            }
            #endregion 
        }


        #endregion

        #region Event Handler

        public event EventHandler Selected;

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsVisibleProperty 
             && !IsVisible)
            {
                ui_tooltip.Visibility = Visibility.Collapsed;
                ui_radar.Visibility = Visibility.Collapsed;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.tooltipplacer = App.FindAncestor<ICharactorTooltipPlacer>(this);
            this.ui_tooltipcanvas = tooltipplacer?.GetCanvas();
            this.ui_tooltip = new CharactorTooltip() { Visibility = Visibility.Collapsed };
            this.ui_radar = new AbilityRadarTooltip() { Visibility = Visibility.Collapsed };
            if (ui_tooltipcanvas != null)
            {
                ui_tooltipcanvas.Children.Add(ui_tooltip);
                ui_tooltipcanvas.Children.Add(ui_radar);
            }
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.tooltipplacer = null;
        }

        private void OnCharactorImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (tick < GD_Chars.Children.Count * 10 + 10) return;
            if (!(sender is CharactorGalleryItem)) return;
            CharactorGalleryItem item = (CharactorGalleryItem)sender;
            TimeAnim = null;
            SelectedItem = item.Core;
            MouseOverItem = null;
            Selected?.Invoke(this, e);
        }

        private void OnCharactorImageMouseEnter(object sender, MouseEventArgs e)
        {
            if (SelectedItem != null) return;
            if (tick < GD_Chars.Children.Count * 10 + 10) return;
            if (!(sender is CharactorGalleryItem)) return;
            CharactorGalleryItem item = (CharactorGalleryItem)sender;
            if (MouseOverItem != item.Core) MouseOverItem = item.Core;    
        }

        private void OnCharactorImageMouseLeave(object sender, MouseEventArgs e)
        {
            if (SelectedItem != null) return;
            if (tick < GD_Chars.Children.Count * 10 + 10) return;
            if (!(sender is CharactorGalleryItem)) return;
            CharactorGalleryItem item = (CharactorGalleryItem)sender;
            if (MouseOverItem == item.Core) MouseOverItem = null;
        }

        private void OnTimeout(object sender, EventArgs e)
        {
            Random random = new Random();
            TimeAnim = null;
            SelectedItem = ItemsSource[random.Next() % ItemsSource.Count()];
            MouseOverItem = null;
            Selected?.Invoke(this, e);
        }

        #endregion

    }

    public interface ICharactorTooltipPlacer
    {
        Canvas GetCanvas();
    }
}
