using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        static public string GetApplicationPath()
        {
            return System.IO.Directory.GetParent(System.Windows.Forms.Application.ExecutablePath).FullName;
        }

        static public T FindAncestor<T>(DependencyObject ue)
        {
            try
            {
                while (ue != null && !(ue is T))
                    ue = VisualTreeHelper.GetParent(ue)
                        ?? (ue as FrameworkElement)?.Parent
                        ?? (ue as FrameworkElement)?.TemplatedParent;
            }
            catch (Exception)
            {
                return default(T);
            }
            return ue is T ? (T)((object)(ue)) : default(T);
        }

        static public ImageSource CreateImage(string filename)
        {
            return ImageHelper.CreateImage(filename);
        }

        static public readonly List<IPackage> PackageList = new List<IPackage>();

        static public bool IsPackagesLoaded { get; private set; } = false;
        static public bool IsCharactorsAllLoaded { get; private set; } = false;
        static public bool IsCardsAllLoaded { get; private set; } = false;

        static public void LoadPackages(Action<double> callback, Action<string> messagehint, bool loadacharactors, bool loadcards)
        {
            callback?.Invoke(0.0d);
            if (!IsPackagesLoaded)
            {
                messagehint?.Invoke("正在加载数据库...");
                PackageList.Clear();
                PackageList.Add(new Koishi.Package());
                PackageList.Add(new Koishi.Package2());
                PackageList.Add(new Reimu.Package());
                Config.GameConfig.UsedPackages.Clear();
                Config.GameConfig.UsedPackages.AddRange(PackageList);
                IsPackagesLoaded = true;
            }
            callback?.Invoke(0.1d);
            if (!IsCharactorsAllLoaded && loadacharactors)
            {
                messagehint?.Invoke("正在加载角色图片...");
                List<Charactor> charlist = new List<Charactor>();
                foreach (IPackage package in PackageList)
                    charlist.AddRange(package.GetCharactors());
                for (int i = 0; i < charlist.Count(); i++)
                {
                    GetCharactorInfo(charlist[i]);
                    callback?.Invoke(0.1d + 0.7d * i / charlist.Count());
                }
                IsCharactorsAllLoaded = true;
            }
            callback?.Invoke(0.8d);
            if (!IsCardsAllLoaded && loadcards)
            {
                messagehint?.Invoke("正在加载卡片图片...");
                List<Card> cardlist = new List<Card>();
                foreach (IPackage package in PackageList)
                    cardlist.AddRange(package.GetCardInstances());
                for (int i = 0; i < cardlist.Count(); i++)
                {
                    GetCardInfo(cardlist[i]);
                    callback?.Invoke(0.8d + 0.2d * i / cardlist.Count());
                }
                IsCardsAllLoaded = true;
            }
            callback?.Invoke(1.0d);
        }

        static public readonly Dictionary<string, CardInfo> CardInfoDictionary = new Dictionary<string, CardInfo>();

        static public readonly Dictionary<Charactor, CharactorInfoCore> CharactorInfoDirectory = new Dictionary<Charactor, CharactorInfoCore>();

        static public readonly Dictionary<string, string> CountryNameDictonary = new Dictionary<string, string>();

        static public ImageSource FaceDownCardImage = null;
        
        static public CardInfo GetCardInfo(string cardkeyname)
        {
            CardInfo info = null;
            CardInfoDictionary.TryGetValue(cardkeyname, out info);
            return info;
        }

        static public CardInfo GetCardInfo(Card card)
        {
            CardInfo info = null;
            if (String.IsNullOrEmpty(card.KeyName)) return null;
            if (CardInfoDictionary.TryGetValue(card.KeyName, out info)) return info;
            info = card.GetInfo();
            CardInfoDictionary.Add(card.KeyName, info);
            return info;
        }

        static public CharactorInfoCore GetCharactorInfo(Charactor charactor)
        {
            CharactorInfoCore info = null;
            if (CharactorInfoDirectory.TryGetValue(charactor, out info)) return info;
            info = charactor.GetInfo();
            CharactorInfoDirectory.Add(charactor, info);
            return info;
        }

        static public string GetCountryName(string country)
        {
            string name = null;
            if (String.IsNullOrEmpty(country)) return String.Empty;
            if (CountryNameDictonary.TryGetValue(country, out name)) return name;
            foreach (IPackage package in PackageList)
            {
                name = package.GetCountryName(country);
                if (!String.IsNullOrEmpty(name))
                {
                    CountryNameDictonary.Add(country, name);
                    return name;
                }
            }
            return String.Empty;
        }
        
        static public string GetCardColorEmojiText(CardColor color)
        {
            switch (color?.E)
            {
                case Enum_CardColor.Club: return "♣";
                case Enum_CardColor.Spade: return "♠";
                case Enum_CardColor.Diamond: return "♦";
                case Enum_CardColor.Heart: return "♥";
            }
            return String.Empty;
        }

        static public Brush GetCardWpfColor(CardColor color)
        {
            switch (color?.E)
            {
                case Enum_CardColor.Black:
                case Enum_CardColor.Club:
                case Enum_CardColor.Spade: return Brushes.Black;
                case Enum_CardColor.Red:
                case Enum_CardColor.Diamond: 
                case Enum_CardColor.Heart: return Brushes.Red;
            }
            return Brushes.Black;
        }

        static public string GetCardPokerPoint(int point)
        {
            switch (point)
            {
                case 1: return "A";
                case 11: return "J";
                case 12: return "Q";
                case 13: return "K";
                default: return point.ToString();
            }
        }

        

    }
}
