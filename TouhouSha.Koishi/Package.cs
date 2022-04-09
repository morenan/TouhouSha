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
    public class Package : IPackage
    {
        public string PackageName => "标准";

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
            yield return new KillCard();
            yield return new MissCard();
            yield return new PeachCard();
            yield return new AttackAllCard();
            yield return new ArrowAllCard();
            yield return new PeachAllCard();
            yield return new HarvestCard();
            yield return new BridgeBoomCard();
            yield return new SheepCard();
            yield return new BirthCard();
            yield return new NegateCard();
            yield return new BorrowKnifeCard();
            yield return new DuelCard();
            yield return new HappyCard();
            yield return new LightCard();
            yield return new AkCard();
            yield return new HisoCard();
            yield return new LouKanCard();
            yield return new HakulouCard();
            yield return new HakkeroCard();
            yield return new GoheCard();
            yield return new GungnirCard();
            yield return new BowCard();
            yield return new YinYangYuCard();
            yield return new DarkBallCard();
            yield return new MarisaHatCard();
            yield return new PlusHorseCard();
            yield return new MinusHorseCard();
        }

        public IEnumerable<Card> GetCards()
        {
            yield return new LightCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 1 };
            yield return new DuelCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 1 };
            yield return new YinYangYuCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 2 };
            yield return new GoheCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 2 };
            yield return new HakkeroCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 2 };
            yield return new BridgeBoomCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 3 };
            yield return new SheepCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 3 };
            yield return new BridgeBoomCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 4 };
            yield return new SheepCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 4 };
            yield return new LouKanCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 5 };
            yield return new PlusHorseCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 5 };
            yield return new HappyCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 6 };
            yield return new HisoCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 6 };
            yield return new AttackAllCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 7 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 7 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 8 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 8 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 9 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 9 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 10 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 10 };
            yield return new NegateCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 11 };
            yield return new SheepCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 11 };
            yield return new HakkeroCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 12 };
            yield return new BridgeBoomCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 12 };
            yield return new MinusHorseCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 13 };
            yield return new AttackAllCard() { CardColor = new CardColor(Enum_CardColor.Spade), CardPoint = 13 };
            yield return new PeachAllCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 1 };
            yield return new ArrowAllCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 1 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 2 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 2 };
            yield return new PeachCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 3 };
            yield return new HarvestCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 3 };
            yield return new PeachCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 4 };
            yield return new HarvestCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 4 };
            yield return new BowCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 5 };
            yield return new MinusHorseCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 5 };
            yield return new PeachCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 6 };
            yield return new HappyCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 6 };
            yield return new PeachCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 7 };
            yield return new BirthCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 7 };
            yield return new PeachCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 8 };
            yield return new BirthCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 8 };
            yield return new PeachCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 9 };
            yield return new BirthCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 9 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 10 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 10 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 11 };
            yield return new BirthCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 11 };
            yield return new PeachCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 12 };
            yield return new BridgeBoomCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 12 };
            yield return new LightCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 12 };
            yield return new PlusHorseCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 13 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Heart), CardPoint = 13 };
            yield return new AkCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 1 };
            yield return new DuelCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 1 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 2 };
            yield return new KoishiHatCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 2 };
            yield return new DarkBallCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 2 };
            yield return new BridgeBoomCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 3 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 3 };
            yield return new BridgeBoomCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 4 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 4 };
            yield return new PlusHorseCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 5 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 5 };
            yield return new HappyCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 6 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 6 };
            yield return new AttackAllCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 7 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 7 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 8 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 8 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 9 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 9 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 10 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 10 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 11 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 11 };
            yield return new BorrowKnifeCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 12 };
            yield return new NegateCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 12 };
            yield return new BorrowKnifeCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 13 };
            yield return new NegateCard() { CardColor = new CardColor(Enum_CardColor.Club), CardPoint = 13 };
            yield return new AkCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 1 };
            yield return new DuelCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 1 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 2 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 2 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 3 };
            yield return new SheepCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 3 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 4 };
            yield return new SheepCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 4 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 5 };
            yield return new GungnirCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 5 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 6 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 6 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 7 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 7 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 8 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 8 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 9 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 9 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 10 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 10 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 11 };
            yield return new MissCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 11 };
            yield return new PeachCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 12 };
            yield return new HakulouCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 12 };
            yield return new MarisaHatCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 12 };
            yield return new KillCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 13 };
            yield return new MinusHorseCard() { CardColor = new CardColor(Enum_CardColor.Diamond), CardPoint = 13 };
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

        public IEnumerable<Trigger> GetGlobalTriggersBeforePlayer()
        {
            yield break;
        }

        public string GetZoneName(string zonekeyname)
        {
            return zonekeyname;
        }
    }
}
