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
using System.Windows.Media.Effects;
using TouhouSha.Core;

namespace TouhouSha
{
    /// <summary>
    /// StartMenu.xaml 的交互逻辑
    /// </summary>
    public partial class StartMenu : UserControl
    {
        public const double Logo_Right = 40;
        public const double Logo_Top = 32;
        public const double Button_StartLeft = 32;
        public const double Button_StartBottom = 32;
        public const double Button_Zap = 32;
        public const double Button_AnimMove = 32;
        public const int Fade_TickMax = 20;
        public const int Button_TickOneMax = 20;
        public const int Button_TickSpan = 10;

        public const int Logo_ShadowSpan = 40;

        static public int GetTickMax(Enum_StartMenuFloor floor)
        {
            switch (floor)
            {
                case Enum_StartMenuFloor.None: return 0;
                case Enum_StartMenuFloor.Root: return 5 * Button_TickSpan + Button_TickOneMax;
                case Enum_StartMenuFloor.Libraries: return 4 * Button_TickSpan + Button_TickOneMax;
                case Enum_StartMenuFloor.GameModeSelect: return 5 * Button_TickSpan + Button_TickOneMax;
            }
            return 0;
        }

        public StartMenu()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        #region Properites

        #region Floor

        static public readonly DependencyProperty FloorProperty = DependencyProperty.Register(
            "Floor", typeof(Enum_StartMenuFloor), typeof(StartMenu),
            new PropertyMetadata(Enum_StartMenuFloor.None, OnPropertyChanged_Floor));

        public Enum_StartMenuFloor Floor
        {
            get { return (Enum_StartMenuFloor)GetValue(FloorProperty); }
            set { SetValue(FloorProperty, value); }
        }

        private static void OnPropertyChanged_Floor(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StartMenu) ((StartMenu)d).OnFloorChanged(e);
        }

        protected virtual void OnFloorChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Floor != Enum_StartMenuFloor.None) Visibility = Visibility.Visible;
            if (!IsVisible) return;
            this.anim_floorfrom = (Enum_StartMenuFloor)e.OldValue;
            this.anim_floorto = (Enum_StartMenuFloor)e.NewValue;
            this.anim_tick = 0;
        }

        #endregion

        #region ToDo

        static public readonly DependencyProperty ToDoProperty = DependencyProperty.Register(
            "ToDo", typeof(Enum_StartMenuToDo), typeof(StartMenu),
            new PropertyMetadata(Enum_StartMenuToDo.None, OnPropertyChanged_ToDo));

        public Enum_StartMenuToDo ToDo
        {
            get { return (Enum_StartMenuToDo)GetValue(ToDoProperty); }
            set { SetValue(ToDoProperty, value); }
        }

        private static void OnPropertyChanged_ToDo(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StartMenu) ((StartMenu)d).OnToDoChanged(e);
        }

        protected virtual void OnToDoChanged(DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion

        #endregion

        #region Number

        private MainWindow mw;
        private DispatcherTimer timer;
        private Enum_StartMenuFloor anim_floorfrom = Enum_StartMenuFloor.None;
        private Enum_StartMenuFloor anim_floorto = Enum_StartMenuFloor.None;
        private int fade_tick = 0;
        private int anim_tick = 0;
        private int logo_tick = 0;
        private List<Image> logo_images = new List<Image>();
        private BlurEffect logo_light;
        private Enum_GameMode gamemode = Enum_GameMode.StandardPlayers8;

        #endregion

        #region Method

        protected List<Button> GetButtonList(Enum_StartMenuFloor floor)
        {
            switch (floor)
            {
                case Enum_StartMenuFloor.Root:
                    return new List<Button>() { BN_SingleGame, BN_MultiGame, BN_Setting, BN_Libraries, BN_Thanks, BN_Exit };
                case Enum_StartMenuFloor.Libraries:
                    return new List<Button>() { BN_Charactors, BN_Cards, BN_Rules, BN_Libraries_Return };
                case Enum_StartMenuFloor.GameModeSelect:
                    return new List<Button>() { BN_Standard8, BN_Standard5, BN_KOF, BN_FightLandlord, BN_GameMode_Return };
            }
            return new List<Button>();
        }

        public void SetupImageResources(Dictionary<string, ImageSource> imgres)
        {
            for (int i = 0; i < 2; i++)
            {
                Image image = new Image();
                image.Source = imgres["Logo" + i];
                image.Stretch = Stretch.None;
                image.Visibility = Visibility.Collapsed;
                Panel.SetZIndex(image, -20);
                if (i == 0) Panel.SetZIndex(image, -10);
                if (i > 0)
                {
                    this.logo_light = new BlurEffect();
                    logo_light.Radius = 10;
                    image.Effect = logo_light;
                }
                logo_images.Add(image);
                CV_All.Children.Add(image);
            }
            BN_SingleGame.ImageSource = imgres["Button0"];
            BN_MultiGame.ImageSource = imgres["Button1"];
            BN_Setting.ImageSource = imgres["Button2"];
            BN_Libraries.ImageSource = imgres["Button3"];
            BN_Thanks.ImageSource = imgres["Button4"];
            BN_Exit.ImageSource = imgres["Button5"];
            BN_Cards.ImageSource = imgres["Button6"];
            BN_Charactors.ImageSource = imgres["Button7"];
            BN_Rules.ImageSource = imgres["Button8"];
            BN_Libraries_Return.ImageSource = imgres["Button9"];
            BN_Standard5.ImageSource = imgres["Standard5"];
            BN_Standard8.ImageSource = imgres["Standard8"];
            BN_KOF.ImageSource = imgres["KOF"];
            BN_FightLandlord.ImageSource = imgres["FightLandlord"];
        }

        #endregion

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            IM_Background.Source = App.CreateImage(System.IO.Path.Combine(App.GetApplicationPath(), "Images", "Start"));
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.mw = App.FindAncestor<MainWindow>(this);
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsVisibleProperty)
            {
                if (IsVisible)
                {
                    for (int i = 1; i < logo_images.Count(); i++)
                        logo_images[i].Opacity = i == 1 ? 1.0d : 0.0d;

                    this.anim_floorfrom = Enum_StartMenuFloor.None;
                    this.anim_floorto = Floor;
                    this.anim_tick = 0;
                    this.logo_tick = 0;
                    this.fade_tick = 0;
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

        private void OnTimerTick(object sender, EventArgs e)
        {
            // 淡入
            if (anim_floorto != Enum_StartMenuFloor.None && fade_tick < Fade_TickMax)
            {
                Opacity = (double)++fade_tick / Fade_TickMax;
                return;
            }

            // Logo动画
            if (++logo_tick >= 2 * Logo_ShadowSpan) logo_tick = 0;
            for (int i = 0; i < logo_images.Count(); i++)
            {
                Canvas.SetLeft(logo_images[i], ActualWidth - Logo_Right - logo_images[i].ActualWidth);
                Canvas.SetTop(logo_images[i], Logo_Top);
                logo_images[i].Visibility = Visibility.Visible;
            }
            logo_light.Radius = (double)Math.Abs(logo_tick - Logo_ShadowSpan) / Logo_ShadowSpan * 20 + 10; 
            // 按钮动画
            int tickmax0 = GetTickMax(anim_floorfrom);
            int tickmax1 = GetTickMax(anim_floorto);
            List<Button> buttons0 = GetButtonList(anim_floorfrom);
            List<Button> buttons1 = GetButtonList(anim_floorto);
            // 屏蔽不用的按钮
            if (anim_tick == tickmax0)
            {
                foreach (UIElement ue in CV_All.Children)
                {
                    if (!(ue is Button)) continue;
                    Button bn = (Button)ue;
                    bn.Visibility = buttons1.Contains(bn) ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            // 按钮动画
            if (anim_tick < tickmax0 + tickmax1)
            {
                anim_tick++;
                if (anim_tick < tickmax0)
                {
                    if (anim_floorfrom == Enum_StartMenuFloor.GameModeSelect)
                    {
                        int tickline = 0;
                        for (int i = 0; i < buttons0.Count(); i++)
                        {
                            Button bn = buttons0[i];
                            double r = 1;
                            if (anim_tick >= tickline)
                            {
                                r = 1 - (double)(anim_tick - tickline) / Button_TickOneMax;
                                r = Math.Max(r, 0);
                            }
                            bn.Opacity = r;
                            Canvas.SetLeft(bn, 32 + (i & 1) * (buttons0[0].ActualWidth + 16));
                            Canvas.SetTop(bn, 32 + (i / 2) * (buttons0[0].ActualHeight + 16) + (1 - r) * ((i & 1) == 0 ? 1 : -1) * Button_AnimMove);
                            tickline += Button_TickSpan;
                        }
                    }
                    else
                    {
                        double x = Button_StartLeft;
                        double y = ActualHeight - Button_StartBottom;
                        int tickline = 0;
                        for (int i = 0; i < buttons0.Count(); i++)
                        {
                            Button bn = buttons0[i];
                            double r = 1;
                            if (anim_tick >= tickline)
                            {
                                r = 1 - (double)(anim_tick - tickline) / Button_TickOneMax;
                                r = Math.Max(r, 0);
                            }
                            bn.Opacity = r;
                            Canvas.SetLeft(bn, x);
                            Canvas.SetTop(bn, y - bn.ActualHeight + (1 - r) * ((i & 1) == 0 ? 1 : -1) * Button_AnimMove);
                            x += bn.ActualWidth;
                            y += ((i & 1) == 0 ? -1 : 1) * Button_Zap;
                            tickline += Button_TickSpan;
                        }
                    }
                }
                else if (anim_tick < tickmax0 + tickmax1)
                {
                    if (anim_floorto == Enum_StartMenuFloor.GameModeSelect)
                    {
                        int tickline = tickmax0;
                        for (int i = 0; i < buttons1.Count(); i++)
                        {
                            Button bn = buttons1[i];
                            double r = 0;
                            if (anim_tick >= tickline)
                            {
                                r = (double)(anim_tick - tickline) / Button_TickOneMax;
                                r = Math.Min(r, 1);
                            }
                            bn.Opacity = r;
                            Canvas.SetLeft(bn, 32 + (i & 1) * (buttons1[0].ActualWidth + 16));
                            Canvas.SetTop(bn, 32 + (i / 2) * (buttons1[0].ActualHeight + 16) + (1 - r) * ((i & 1) == 0 ? 1 : -1) * Button_AnimMove);
                            tickline += Button_TickSpan;
                        }
                    }
                    else
                    {
                        double x = Button_StartLeft;
                        double y = ActualHeight - Button_StartBottom;
                        int tickline = tickmax0;
                        for (int i = 0; i < buttons1.Count(); i++)
                        {
                            Button bn = buttons1[i];
                            double r = 0;
                            if (anim_tick >= tickline)
                            {
                                r = (double)(anim_tick - tickline) / Button_TickOneMax;
                                r = Math.Min(r, 1);
                            }
                            bn.Opacity = r;
                            Canvas.SetLeft(bn, x);
                            Canvas.SetTop(bn, y - bn.ActualHeight + (1 - r) * ((i & 1) == 0 ? 1 : -1) * Button_AnimMove);
                            x += bn.ActualWidth;
                            y += ((i & 1) == 0 ? -1 : 1) * Button_Zap;
                            tickline += Button_TickSpan;
                        }
                    }
                }
            }
            else
            {
                // 实时调整位置
                if (Floor == Enum_StartMenuFloor.GameModeSelect)
                {
                    for (int i = 0; i < buttons1.Count(); i++)
                    {
                        Button bn = buttons1[i];
                        bn.Opacity = 1;
                        Canvas.SetLeft(bn, 32 + (i & 1) * (buttons1[0].ActualWidth + 16));
                        Canvas.SetTop(bn, 32 + (i / 2) * (buttons1[0].ActualHeight + 16));
                    }

                }
                else
                {
                    double x = Button_StartLeft;
                    double y = ActualHeight - Button_StartBottom;
                    for (int i = 0; i < buttons1.Count(); i++)
                    {
                        Button bn = buttons1[i];
                        bn.Opacity = 1;
                        Canvas.SetLeft(bn, x);
                        Canvas.SetTop(bn, y - bn.ActualHeight);
                        x += bn.ActualWidth;
                        y += ((i & 1) == 0 ? -1 : 1) * Button_Zap;
                    }
                }
                // 淡出
                if (anim_floorto == Enum_StartMenuFloor.None && fade_tick > 0)
                {
                    Opacity = (double)--fade_tick / Fade_TickMax;
                    if (fade_tick == 0)
                    {
                        switch (ToDo)
                        {
                            case Enum_StartMenuToDo.StartSingleGame:
                                mw.StartSingleGame(gamemode);
                                break;
                            case Enum_StartMenuToDo.ShowCharactorGallery:
                                mw.ShowCharactorGallery();
                                break;
                            case Enum_StartMenuToDo.ShowCardGallery:
                                mw.ShowCardGallery();
                                break;
                            case Enum_StartMenuToDo.Quit:
                                Application.Current.Shutdown();
                                break;
                        }
                        ToDo = Enum_StartMenuToDo.None;
                        Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void StartMenuButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender == BN_SingleGame)
            {
                Floor = Enum_StartMenuFloor.GameModeSelect;
            }
            if (sender == BN_MultiGame)
            {
               
            }
            if (sender == BN_Setting)
            {
                mw?.ShowConfigPage();
            }
            if (sender == BN_Libraries)
            {
                Floor = Enum_StartMenuFloor.Libraries;
            }
            if (sender == BN_Thanks)
            {
            }
            if (sender == BN_Exit)
            {
                ToDo = Enum_StartMenuToDo.Quit;
                Floor = Enum_StartMenuFloor.None;
            }
            if (sender == BN_Charactors)
            {
                ToDo = Enum_StartMenuToDo.ShowCharactorGallery;
                Floor = Enum_StartMenuFloor.None;
            }
            if (sender == BN_Cards)
            {
                ToDo = Enum_StartMenuToDo.ShowCardGallery;
                Floor = Enum_StartMenuFloor.None;
            }
            if (sender == BN_Libraries_Return
             || sender == BN_GameMode_Return)
            {
                Floor = Enum_StartMenuFloor.Root;
            }
            if (sender == BN_Standard8)
            {
                gamemode = Enum_GameMode.StandardPlayers8;
                ToDo = Enum_StartMenuToDo.StartSingleGame;
                Floor = Enum_StartMenuFloor.None;
            }
            if (sender == BN_Standard5)
            {
                gamemode = Enum_GameMode.StandardPlayers5;
                ToDo = Enum_StartMenuToDo.StartSingleGame;
                Floor = Enum_StartMenuFloor.None;
            }
            if (sender == BN_KOF)
            {
                gamemode = Enum_GameMode.KOF;
                ToDo = Enum_StartMenuToDo.StartSingleGame;
                Floor = Enum_StartMenuFloor.None;
            }
            if (sender == BN_FightLandlord)
            {
                gamemode = Enum_GameMode.FightLandlord;
                ToDo = Enum_StartMenuToDo.StartSingleGame;
                Floor = Enum_StartMenuFloor.None;
            }
        }
    }

    public enum Enum_StartMenuFloor
    {
        None,
        Root,
        Libraries,
        GameModeSelect,
    }

    public enum Enum_StartMenuToDo
    {
        None,
        StartSingleGame,
        ShowCharactorGallery,
        ShowCardGallery,
        Quit,
    }
}
