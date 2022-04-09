using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core
{
    public class Config : INotifyPropertyChanged
    {
        static public readonly GameConfig GameConfig = new GameConfig();
        static public readonly SoundConfig SoundConfig = new SoundConfig();
        static public readonly ScreenConfig ScreenConfig = new ScreenConfig();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void IvProp(string name) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
    }

    public class GameConfig : Config
    {
        private List<IPackage> usedpackages = new List<IPackage>();
        public List<IPackage> UsedPackages
        {
            get { return this.usedpackages; }
        }
        
        private bool doublespy = false;
        public bool DoubleSpy
        {
            get { return this.doublespy; }
            set { this.doublespy = value; IvProp("DoubleSpy"); }
        }

        private int timeout_usecard = 20;
        public int Timeout_UseCard
        {
            get { return this.timeout_usecard; }
            set { this.timeout_usecard = value; IvProp("Timeout_UseCard"); }
        }

        private int timeout_handle = 15;
        public int Timeout_Handle
        {
            get { return this.timeout_handle; }
            set { this.timeout_handle = value; IvProp("Timeout_Handle"); }
        }

        private int timeout_ask = 15;
        public int Timeout_Ask
        {
            get { return this.timeout_ask; }
            set { this.timeout_ask = value; IvProp("Timeout_Ask"); }
        }

        private int leadercharactorselectnumber = 2;
        public int LeaderCharactorsSelectNumber
        {
            get { return this.leadercharactorselectnumber; }
            set { this.leadercharactorselectnumber = value; IvProp("LeaderCharactorsSelectNumber"); }

        }
        
        private int charactorsselectnumber = 3;
        public int CharactorsSelectNumber
        {
            get { return this.charactorsselectnumber; }
            set { this.charactorsselectnumber = value; IvProp("CharactorsSelectNumber"); }
        }

    }

    public class ScreenConfig : Config
    {
        private Enum_ImageQuality imagequality = Enum_ImageQuality.Normal;
        public Enum_ImageQuality ImageQuality
        {
            get { return this.imagequality; }
            set { this.imagequality = value; IvProp("ImageQuality"); }
        }

        private Enum_CardVelocity cardvelocity = Enum_CardVelocity.Normal;
        public Enum_CardVelocity CardVelocity
        {
            get { return this.cardvelocity; }
            set { this.cardvelocity = value; IvProp("CardVelocity"); }
        }

        private Enum_DamageShakePower damageshakepower = Enum_DamageShakePower.Normal;
        public Enum_DamageShakePower DamageShakePower
        {
            get { return this.damageshakepower; }
            set { this.damageshakepower = value; IvProp("DamageShakePower"); }
        }

        private int desktoplinecount = 1;
        public int DesktopLineCount
        {
            get { return this.desktoplinecount; }
            set { this.desktoplinecount = value; IvProp("DesktopLineCount"); }
        }

    }

    public class SoundConfig : Config
    {
        private double volume_bgm = 0.5;
        public double Volume_Bgm
        {
            get { return this.volume_bgm; }
            set { this.volume_bgm = value; IvProp("Volume_Bgm"); }
        }

        private double volume_se = 0.5;
        public double Volume_Se
        {
            get { return this.volume_se; }
            set { this.volume_se = value; IvProp("Volume_Se"); }
        }

        private double volume_voice = 0.7;
        public double Volume_Voice
        {
            get { return this.volume_voice; }
            set { this.volume_voice = value; IvProp("Volume_Voice"); }
        }
    }
   
    public enum Enum_ImageQuality
    {
        Low = 0,
        Normal = 1,
        High = 2,
    }

    public enum Enum_CardVelocity
    {
        Slow = 0,
        Normal = 1,
        Fast = 2,
        Immediate = 3,
    }

    public enum Enum_DamageShakePower
    {
        None = 0,
        Quiet = 1,
        Normal = 2,
        Rapid = 3,
    }
}
