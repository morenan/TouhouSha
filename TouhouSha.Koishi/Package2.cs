using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TouhouSha.Core;
using TouhouSha.Koishi.Cards;
using TouhouSha.Koishi.Cards.Weapons;
using TouhouSha.Koishi.Cards.Armors;
using TouhouSha.Koishi.Cards.Horses;

namespace TouhouSha.Koishi
{
    public class Package2 : IPackage
    {
        public string PackageName => "军争";

        public bool IsNecessary => true;

        public ImageSource GetCardImage(string cardkeyname)
        {
            return null;
        }

        public string GetCardName(string cardkeyname)
        {
            return cardkeyname;
        }

        public IEnumerable<Card> GetCardInstances()
        {
            yield return new KillCard() { KeyName = KillCard.Thunder };
            yield return new KillCard() { KeyName = KillCard.Fire };
            yield return new LiqureCard();
            yield return new ChainCard();
            yield return new FireCard();
            yield return new HungerCard();
            yield return new ZuiDaiCard();
            yield return new WishMaskCard();
            yield return new LaevatainCard();
            yield return new HammerCard();
            yield return new KoishiHatCard();
            
        }

        public IEnumerable<Card> GetCards()
        {
            yield return new LightCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 1 };
            yield return new ZuiDaiCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 2 };
            yield return new LiqureCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 3 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 4, KeyName = KillCard.Thunder };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 5, KeyName = KillCard.Thunder };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 6, KeyName = KillCard.Thunder };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 7, KeyName = KillCard.Thunder };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 8, KeyName = KillCard.Thunder };
            yield return new LiqureCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 9 };
            yield return new HungerCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 10 };
            yield return new ChainCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 11 };
            yield return new ChainCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 12 };
            yield return new NegateCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 13 };
            yield return new NegateCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 1 };
            yield return new FireCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 2 };
            yield return new FireCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 3 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 4, KeyName = KillCard.Fire };
            yield return new PeachCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 5 };
            yield return new PeachCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 6 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 7, KeyName = KillCard.Fire };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 8 };
            yield return new LiqureCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 9 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 10, KeyName = KillCard.Fire };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 11 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 12 };
            yield return new NegateCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 13 };
            yield return new WishMaskCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 1 };
            yield return new HammerCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 2 };
            yield return new LiqureCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 3 };
            yield return new HungerCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 4 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 5, KeyName = KillCard.Thunder };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 6, KeyName = KillCard.Thunder };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 7, KeyName = KillCard.Thunder };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 8, KeyName = KillCard.Thunder };
            yield return new LiqureCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 9 };
            yield return new ChainCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 10 };
            yield return new ChainCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 11 };
            yield return new ChainCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 12 };
            yield return new ChainCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 13 };
            yield return new LaevatainCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 1 };
            yield return new PeachCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 2 };
            yield return new PeachCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 3 };
            yield return new FireCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 4, KeyName = KillCard.Fire };
            yield return new FireCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 5, KeyName = KillCard.Fire };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 6 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 7 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 8 };
            yield return new LiqureCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 9 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 10 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 11 };
            yield return new FireCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 12 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 13 };
            yield break;
        }

        public IEnumerable<Charactor> GetCharactors()
        {
            yield break;
        }

        public Color? GetCountryColor(string country)
        {
            return default(Color?);
        }

        public string GetCountryName(string country)
        {
            return country;
        }

        public IEnumerable<Trigger> GetGlobalTriggersAfterPlayer()
        {
            yield break;
        }

        public IEnumerable<Trigger> GetGlobalTriggersBeforePlayer()
        {
            Assembly ass = Assembly.GetExecutingAssembly();
            foreach (Type type in ass.GetTypes())
            {
                if (!type.Namespace.StartsWith("TouhouSha.Koishi.Triggers")) continue;
                if (!typeof(Trigger).IsAssignableFrom(type)) continue;
                foreach (ConstructorInfo ci in type.GetConstructors())
                {
                    if (ci.GetParameters().Length > 0) continue;
                    Trigger trigger = ci.Invoke(new object[] { }) as Trigger;
                    if (trigger == null) break;
                    yield return trigger;
                    break;
                }
            }
        }

        public string GetZoneName(string zonekeyname)
        {
            return zonekeyname;
        }
    }
}
