using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
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
using TouhouSha.Controls;
using TouhouSha.Controls.Cools;

namespace TouhouSha
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("winmm.dll", EntryPoint = "PlaySound")]
        static public extern bool PlaySound(string pszSound, IntPtr hmod, int fdwSound);

        public class TestCardFilter : CardFilter
        {
            public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
            {
                if (selecteds.Count() >= 1) return false;
                return true;
            }

            public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
            {
                return selecteds.Count() >= 1;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Config.SoundConfig.PropertyChanged += SoundConfig_PropertyChanged;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                Dispatcher.Invoke(() =>
                {
                    /*
                    App.LoadPackages((_callback) => { }, (_message) => { }, false, true);
                    App.FaceDownCardImage = ImageHelper.CreateImage(System.IO.Path.Combine(App.GetApplicationPath(), "Images", "FaceDown.png"));
                    List<Card> cards = new List<Card>();
                    foreach (IPackage package in App.PackageList)
                        cards.AddRange(package.GetCards());

                    UI_GameBoard.World = new World();
                    UI_GameBoard.World.Shuffle(cards);

                    DesktopCardBoardCore core = new DesktopCardBoardCore();
                    core.KeyName = "黑盒测试";
                    core.Message = "请选择一张卡";
                    core.Flag = Enum_DesktopCardBoardFlag.SelectCardAndYes;
                    core.Flag |= Enum_DesktopCardBoardFlag.CannotNo;
                    core.CardFilter = new TestCardFilter();
                    
                    DesktopCardBoardZone zone = new DesktopCardBoardZone(core);
                    zone.Message = "手牌区";
                    zone.Flag = Enum_DesktopZoneFlag.FaceDown;
                    zone.Flag |= Enum_DesktopZoneFlag.CanZip;
                    for (int i = 0; i < 5; i++)
                        zone.Cards.Add(cards[i]);
                    core.Zones.Add(zone);

                    zone = new DesktopCardBoardZone(core);
                    zone.Message = "装备区";
                    zone.Flag = Enum_DesktopZoneFlag.CanZip;
                    for (int i = 5; i < 9; i++)
                        zone.Cards.Add(cards[i]);
                    core.Zones.Add(zone);

                    zone = new DesktopCardBoardZone(core);
                    zone.Message = "判定区";
                    zone.Flag = Enum_DesktopZoneFlag.CanZip;
                    for (int i = 9; i < 12; i++)
                        zone.Cards.Add(cards[i]);
                    core.Zones.Add(zone);

                    UI_GameBoard.ShowDesktopBoard(core);
                    */

                    FrameworkElement screen = new StartupScreen();
                    screen.IsVisibleChanged += OnStartupScreenIsVisibleChanged;
                    GD_Main.Children.Add(screen);
                    PlaySound(Enum_SoundUsage.Menu);
                });
            });
        }

        #region Properties

        #region Sound

        static public readonly DependencyProperty SoundProperty = DependencyProperty.Register(
            "Sound", typeof(SoundObject), typeof(MainWindow),
            new PropertyMetadata(null, OnPropertyChanged_Sound));

        public SoundObject Sound
        {
            get { return (SoundObject)GetValue(SoundProperty); }
            set { SetValue(SoundProperty, value); }
        }

        private static void OnPropertyChanged_Sound(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MainWindow) ((MainWindow)d).OnSoundChanged(e);
        }

        protected virtual void OnSoundChanged(DependencyPropertyChangedEventArgs e)
        {
            if (soundfadetimer == null)
            {
                this.soundfadetimer = new DispatcherTimer();
                soundfadetimer.Interval = TimeSpan.FromMilliseconds(10);
                soundfadetimer.Tick += OnSoundFading;
                soundfadetimer.Start();
            }
            if (issoundfadingout) return;
            this.issoundfadingin = false;
            this.issoundfadingout = true;
        }

        private void OnSoundFading(object sender, EventArgs e)
        {
            if (issoundfadingout)
            {
                if (soundfadingtick <= 0)
                {
                    issoundfadingout = false;
                    UI_SoundPlayer.Volume = 0;
                    UI_SoundPlayer.Stop();
                    if (Sound == null) return;
                    UI_SoundPlayer.Source = new Uri(Sound.Filename);
                    UI_SoundPlayer.Play();
                    issoundfadingin = true;
                }
                UI_SoundPlayer.Volume = Config.SoundConfig.Volume_Bgm * --soundfadingtick / 50;
            }
            if (issoundfadingin)
            {
                if (soundfadingtick >= 50)
                {
                    UI_SoundPlayer.Volume = Config.SoundConfig.Volume_Bgm;
                    issoundfadingin = false;
                }
                UI_SoundPlayer.Volume = Config.SoundConfig.Volume_Bgm * ++soundfadingtick / 50;
            }
        }

        private DispatcherTimer soundfadetimer;
        private bool issoundfadingout;
        private bool issoundfadingin;
        private int soundfadingtick;

        #endregion

        #endregion

        #region SE

        public void PlaySE(Enum_SEUsage seusage)
        {
            SEObject seobj = SoundResources.Default[seusage];
            if (seobj == null) return;
            PlaySound(seobj.Filename, IntPtr.Zero, 0x00020001);
        }

        #endregion


        protected void LoadPrimaryResources()
        {
            string dir_app = App.GetApplicationPath();
            string dir_logo = System.IO.Path.Combine(dir_app, "Images", "Logo");
            string dir_button = System.IO.Path.Combine(dir_app, "Images", "Buttons");
            string dir_anim = System.IO.Path.Combine(dir_app, "Images", "Animations");
            List<string> imgfiles = new List<string>();
            for (int i = 0; i < 2; i++)
            {
                string file_logo = System.IO.Path.Combine(dir_logo, "Logo" + i);
                imgfiles.Add(file_logo);
            }
            for (int i = 0; i < 10; i++)
            {
                string file_button = System.IO.Path.Combine(dir_button, "Button" + i);
                imgfiles.Add(file_button);
            }
            imgfiles.Add(System.IO.Path.Combine(dir_anim, "Fire", "Fire"));
            imgfiles.Add(System.IO.Path.Combine(dir_anim, "Heal", "Heal"));
            for (int i = 0; i < 9; i++)
            {
                string file_thunder = System.IO.Path.Combine(dir_anim, "Thunder", i.ToString());
                imgfiles.Add(file_thunder);
            }
            imgfiles.Add(System.IO.Path.Combine(dir_button, "Standard5"));
            imgfiles.Add(System.IO.Path.Combine(dir_button, "Standard8"));
            imgfiles.Add(System.IO.Path.Combine(dir_button, "KOF"));
            imgfiles.Add(System.IO.Path.Combine(dir_button, "FightLandlord"));
            UI_LoadingPage.Visibility = Visibility.Visible;
            UI_LoadingPage.LoadPrimaryResources(imgfiles);
        }

        public void PlaySound(Enum_SoundUsage soundusage)
        {
            Random random = new Random();
            SoundPackage package = SoundResources.Default[soundusage];
            SoundObject sound = null;
            if (package.PreferredIndex >= 0 && package.PreferredIndex < package.Sounds.Count())
                sound = package.Sounds[package.PreferredIndex];
            else if (package.Sounds.Count() > 0)
                sound = package.Sounds[random.Next() % package.Sounds.Count()];
            Sound = sound;
        }

        public void HomeStartMenu()
        {
            //UI_StartMenu.Visibility = Visibility.Visible;
            UI_LoadingPage.Visibility = Visibility.Collapsed;
            UI_CharactorGallery.Visibility = Visibility.Collapsed;
            UI_CardGallery.Visibility = Visibility.Collapsed;
            UI_StartMenu.Floor = Enum_StartMenuFloor.Root;
            PlaySound(Enum_SoundUsage.Menu);
        }

        public void StartSingleGame(Enum_GameMode gamemode)
        {
            World world = new World();
            world.GameMode = gamemode;
            UI_GameBoard.World = world;
            UI_LoadingPage.Visibility = Visibility.Visible;
            UI_LoadingPage.LoadGame(UI_GameBoard);
        }

        public void ShowCharactorGallery()
        {
            UI_StartMenu.Visibility = Visibility.Collapsed;
            UI_LoadingPage.Visibility = Visibility.Visible;
            UI_LoadingPage.LoadCharactorGallery(UI_CharactorGallery);
        }

        public void ShowCardGallery()
        {
            UI_StartMenu.Visibility = Visibility.Collapsed;
            UI_LoadingPage.Visibility = Visibility.Visible;
            UI_LoadingPage.LoadCardGallery(UI_CardGallery);
        }

        public void ShowConfigPage()
        {
            App.LoadPackages((_prog) => { }, (_hint) => { }, false, false);
            UI_ConfigPage.Visibility = Visibility.Visible;
        }

        public void CloseConfigPage()
        {
            UI_ConfigPage.Visibility = Visibility.Collapsed;
        }
        
        private void OnStartupScreenIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)(e.NewValue))
            {
                GD_Main.Children.Remove(sender as UIElement);
                LoadPrimaryResources();
            }
        }

        private void UI_SoundPlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            UI_SoundPlayer.Position = TimeSpan.Zero;
            UI_SoundPlayer.Play();
        }

        private void SoundConfig_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Volume_Bgm":
                    UI_SoundPlayer.Volume = Config.SoundConfig.Volume_Bgm;
                    break;
            }
        }

    }
}
