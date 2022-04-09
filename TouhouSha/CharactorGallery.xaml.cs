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
using TouhouSha.Core;
using TouhouSha.Controls;
using TouhouSha.Core.UIs;

namespace TouhouSha
{
    /// <summary>
    /// CharactorGallery.xaml 的交互逻辑
    /// </summary>
    public partial class CharactorGallery : UserControl
    {
        public CharactorGallery()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        #region Properties

        #region Charactors
        
        static public readonly DependencyProperty CharactorsProperty = DependencyProperty.Register(
            "Charactors", typeof(IList<Charactor>), typeof(CharactorGallery),
            new PropertyMetadata(null, OnPropertyChanged_Charactors));
        
        public IList<Charactor> Charactors
        {
            get { return (IList<Charactor>)GetValue(CharactorsProperty); }
            set { SetValue(CharactorsProperty, value); }
        }

        private static void OnPropertyChanged_Charactors(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CharactorGallery) ((CharactorGallery)d).OnCharactorsChanged(e);
        }

        protected virtual void OnCharactorsChanged(DependencyPropertyChangedEventArgs e)
        {
            CountryItem all = new CountryItem("All", "全阵营");
            Dictionary<string, CountryItem> dist = new Dictionary<string, CountryItem>();
            if (Charactors == null) return;
            foreach (Charactor charactor in Charactors)
            {
                CountryItem item = null;
                all.Charactors.Add(charactor);
                if (!String.IsNullOrEmpty(charactor.Country))
                {
                    if (!dist.TryGetValue(charactor.Country, out item))
                    {
                        string name = App.GetCountryName(charactor.Country);
                        item = new CountryItem(charactor.Country, name);
                        dist.Add(charactor.Country, item);
                    }
                    item.Charactors.Add(charactor);
                }
                foreach (string country in charactor.OtherCountries)
                {
                    if (!dist.TryGetValue(country, out item))
                    {
                        string name = App.GetCountryName(country);
                        item = new CountryItem(country, name);
                        dist.Add(country, item);
                    }
                    item.Charactors.Add(charactor);
                }
            }
            Countries = new CountryItem[] { all }.Concat(dist.Values).ToList();
            SelectedCountry = all;
        }

        #endregion

        #region Countries

        static public readonly DependencyProperty CountriesProperty = DependencyProperty.Register(
            "Countries", typeof(IList<CountryItem>), typeof(CharactorGallery),
            new PropertyMetadata(null, OnPropertyChanged_Countries));

        public IList<CountryItem> Countries
        {
            get { return (IList<CountryItem>)GetValue(CountriesProperty); }
            set { SetValue(CountriesProperty, value); }
        }

        private static void OnPropertyChanged_Countries(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CharactorGallery) ((CharactorGallery)d).OnCountriesChanged(e);
        }

        protected virtual void OnCountriesChanged(DependencyPropertyChangedEventArgs e)
        {
            SelectedCountry = Countries.FirstOrDefault();
        }

        #endregion

        #region CharactorsOfCountry

        static public readonly DependencyProperty CharactorsOfCountryProperty = DependencyProperty.Register(
            "CharactorsOfCountry", typeof(IList<Charactor>), typeof(CharactorGallery),
            new PropertyMetadata(null, OnPropertyChanged_CharactorsOfCountry));

        public IList<Charactor> CharactorsOfCountry
        {
            get { return (IList<Charactor>)GetValue(CharactorsOfCountryProperty); }
            set { SetValue(CharactorsOfCountryProperty, value); }
        }

        private static void OnPropertyChanged_CharactorsOfCountry(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CharactorGallery) ((CharactorGallery)d).OnCharactorsOfCountryChanged(e);
        }

        protected virtual void OnCharactorsOfCountryChanged(DependencyPropertyChangedEventArgs e)
        {
            IList<Charactor> list = CharactorsOfCountry ?? new List<Charactor>();
            foreach (CharactorGalleryItem item in GD_Chars.Children)
            {
                item.MouseEnter -= CharactorGalleryItem_MouseEnter;
                item.MouseLeave -= CharactorGalleryItem_MouseLeave;
                item.MouseDown -= CharactorGalleryItem_MouseDown;
            }
            GD_Chars.Children.Clear();
            foreach (Charactor charactor in list)
            {
                CharactorGalleryItem item = new CharactorGalleryItem();
                item.Core = charactor;
                item.MouseEnter += CharactorGalleryItem_MouseEnter;
                item.MouseLeave += CharactorGalleryItem_MouseLeave;
                item.MouseDown += CharactorGalleryItem_MouseDown;
                GD_Chars.Children.Add(item);
            }
            GD_Chars_UpdateLayout();
        }
        
        #endregion

        #region SelectedCountry

        static public readonly DependencyProperty SelectedCountryProperty = DependencyProperty.Register(
            "SelectedCountry", typeof(CountryItem), typeof(CharactorGallery),
            new PropertyMetadata(null, OnPropertyChanged_SelectedCountry));

        public CountryItem SelectedCountry
        {
            get { return (CountryItem)GetValue(SelectedCountryProperty); }
            set { SetValue(SelectedCountryProperty, value); }
        }

        private static void OnPropertyChanged_SelectedCountry(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CharactorGallery) ((CharactorGallery)d).OnSelectedCountryChanged(e);
        }

        protected virtual void OnSelectedCountryChanged(DependencyPropertyChangedEventArgs e)
        {
            CharactorsOfCountry = SelectedCountry?.Charactors ?? new List<Charactor>();
        }

        #endregion

        #region MouseOverCharactor

        static public readonly DependencyProperty MouseOverCharactorProperty = DependencyProperty.Register(
            "MouseOverCharactor", typeof(Charactor), typeof(CharactorGallery),
            new PropertyMetadata(null, OnPropertyChanged_MouseOverCharactor));

        public Charactor MouseOverCharactor
        {
            get { return (Charactor)GetValue(MouseOverCharactorProperty); }
            set { SetValue(MouseOverCharactorProperty, value); }
        }

        private static void OnPropertyChanged_MouseOverCharactor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CharactorGallery) ((CharactorGallery)d).OnMouseOverCharactorChanged(e);
        }

        protected virtual void OnMouseOverCharactorChanged(DependencyPropertyChangedEventArgs e)
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
            if (UI_CharactorTooltip.Core != MouseOverCharactor)
            {
                if (tooltip_tick > 0)
                {
                    UI_CharactorTooltip.Opacity = (double)(--tooltip_tick) / tooltip_tickmax;
                    UI_AbilityRadarTooltip.Opacity = UI_CharactorTooltip.Opacity;
                }
                else
                {
                    UI_CharactorTooltip.Core = MouseOverCharactor;
                    if (MouseOverCharactor == null)
                    {
                        UI_CharactorTooltip.Visibility = Visibility.Collapsed;
                        UI_AbilityRadarTooltip.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        CharactorInfoCore charinfo = App.GetCharactorInfo(MouseOverCharactor);
                        UI_AbilityRadarTooltip.Core = charinfo?.AbilityRadar;
                    }
                }
            }
            else if (UI_CharactorTooltip.Core != null)
            {
                UI_CharactorTooltip.Visibility = Visibility.Visible;
                UI_AbilityRadarTooltip.Visibility = Visibility.Visible;
                if (tooltip_tick < tooltip_tickmax)
                {
                    UI_CharactorTooltip.Opacity = (double)(++tooltip_tick) / tooltip_tickmax;
                    UI_AbilityRadarTooltip.Opacity = UI_CharactorTooltip.Opacity;
                }
            }
            if (UI_CharactorTooltip.IsVisible)
            {
                CharactorGalleryItem viewitem = GD_Chars.Children.Cast<CharactorGalleryItem>().FirstOrDefault(_item => _item.Core == UI_CharactorTooltip.Core);
                if (viewitem != null)
                {
                    Point p = viewitem.TranslatePoint(new Point(0, 0), CV_CharactorTooltip);
                    if (p.X > CV_CharactorTooltip.ActualWidth - p.X - viewitem.ActualWidth)
                    {
                        Canvas.SetLeft(UI_CharactorTooltip, p.X - UI_CharactorTooltip.ActualWidth);
                        if (p.X - UI_CharactorTooltip.ActualWidth > UI_AbilityRadarTooltip.ActualWidth)
                            Canvas.SetLeft(UI_AbilityRadarTooltip, p.X - UI_CharactorTooltip.ActualWidth - UI_AbilityRadarTooltip.ActualWidth);
                        else 
                            Canvas.SetLeft(UI_AbilityRadarTooltip, p.X);
                    }
                    else
                    {
                        Canvas.SetLeft(UI_CharactorTooltip, p.X + viewitem.ActualWidth);
                        if (CV_CharactorTooltip.ActualWidth - p.X - viewitem.ActualWidth - UI_CharactorTooltip.ActualWidth > UI_AbilityRadarTooltip.ActualWidth)
                            Canvas.SetLeft(UI_AbilityRadarTooltip, p.X + viewitem.ActualWidth + UI_CharactorTooltip.ActualWidth);
                        else
                            Canvas.SetLeft(UI_AbilityRadarTooltip, p.X - UI_AbilityRadarTooltip.ActualWidth);
                    }
                    if (p.Y > CV_CharactorTooltip.ActualHeight - p.Y - viewitem.ActualHeight)
                        Canvas.SetTop(UI_AbilityRadarTooltip, p.Y - UI_AbilityRadarTooltip.ActualHeight);
                    else
                        Canvas.SetTop(UI_AbilityRadarTooltip, p.Y + viewitem.ActualHeight);
                    Canvas.SetTop(UI_CharactorTooltip, Math.Min(p.Y, CV_CharactorTooltip.ActualHeight - UI_CharactorTooltip.ActualHeight));
                }
            }
        }

        private void BD_Chars_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GD_Chars_UpdateLayout();
        }

        private void CharactorGalleryItem_MouseEnter(object sender, MouseEventArgs e)
        {
            MouseOverCharactor = (sender as CharactorGalleryItem).Core;
        }

        private void CharactorGalleryItem_MouseLeave(object sender, MouseEventArgs e)
        {
            Charactor charactor = (sender as CharactorGalleryItem).Core;
            if (MouseOverCharactor == charactor)
                MouseOverCharactor = null;
        }

        private void CharactorGalleryItem_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void BN_Return_Click(object sender, RoutedEventArgs e)
        {
            mw?.HomeStartMenu();
        }

        #endregion

    }

    public class CountryItem
    {
        public CountryItem(string _keyname, string _name)
        {
            this.keyname = _keyname;
            this.name = _name;
        }

        public override string ToString()
        {
            return Name;
        }

        private string keyname;
        public string KeyName { get { return this.keyname; } }  
       
        private string name;
        public string Name { get { return this.name; } }

        private List<Charactor> charactors = new List<Charactor>();
        public List<Charactor> Charactors { get { return this.charactors; } }
    }
}
