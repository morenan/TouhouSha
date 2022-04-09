using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TouhouSha.Core.Events;
using TouhouSha.Core.AIs;

namespace TouhouSha.Core
{
    public abstract class Card : ShaObject
    {
        public override string ToString()
        {
            return String.Format("Card:{0}", KeyName);
        }

        public Player Owner
        {
            get { return zone?.Owner; }
        }

        private Zone zone;
        public Zone Zone
        {
            get { return this.zone; }
            set { IvProp0("Zone"); this.zone = value; IvProp1("Zone"); }
        }

        private List<Card> origincards = new List<Card>();
        public List<Card> OriginCards
        {
            get { return this.origincards; }
        }

        private CardType cardtype = new CardType(Enum_CardType.Base, null);
        public CardType CardType
        {
            get { return this.cardtype; }
            set { IvProp0("CardType"); this.cardtype = value; IvProp1("CardType"); }
        }

        private CardColor cardcolor = new CardColor(Enum_CardColor.Spade);
        public CardColor CardColor
        {
            get { return this.cardcolor; }
            set { IvProp0("CardColor"); this.cardcolor = value; IvProp1("CardColor"); }
        }

        private int cardpoint = 1;
        public int CardPoint
        {
            get { return this.cardpoint; }
            set { IvProp0("CardPoint"); this.cardpoint = value; IvProp1("CardPoint"); }
        }
        
        private ConditionFilter usecondition;
        public ConditionFilter UseCondition
        {
            get { return this.usecondition; }
            set { IvProp0("UseCondition"); this.usecondition = value; IvProp1("UseCondition"); }
        }

        private PlayerFilter targetfilter;
        public PlayerFilter TargetFilter
        {
            get { return targetfilter; }
            set { IvProp0("TargetFilter"); this.targetfilter = value; IvProp1("TargetFilter"); }
        }

        private List<Skill> skills = new List<Skill>();
        public List<Skill> Skills
        {
            get { return this.skills; }
        }

        private string comment = String.Empty;
        public string Comment
        {
            get { return this.comment; }
            set { IvProp0("Comment"); this.comment = value; IvProp1("Comment"); }
        }

        public bool IsVirtual
        {
            get { return GetValue("虚拟卡") == 1; }
            set { SetValue("虚拟卡", value ? 1 : 0); }
        }

        public virtual void InitializeCardEvent(CardEvent ev)
        {

        }

        public IEnumerable<Card> GetInitialCards()
        {
            if (origincards.Count() == 0) yield return this;
            foreach (Card card in origincards)
                foreach (Card card2 in card.GetInitialCards())
                    yield return card2;
        }

        public void ActionItsAndOrigins(Action<Card> action)
        {
            action.Invoke(this);
            foreach (Card card in origincards)
                action.Invoke(this);
        }

        public void SetZoneItsAndOrigins(Zone newzone)
        {
            ActionItsAndOrigins((_card) => { _card.Zone = newzone; });
        }

        public abstract CardInfo GetInfo();

        public abstract Card Create();

        public abstract double GetWorthForTarget();

        public virtual CardTargetFilterWorth GetWorthAI()
        {
            return null;
        }

        public virtual CardTargetFilterAuto GetAutoAI()
        {
            return null;
        }

        public Card Clone()
        {
            Card newcard = Create();
            newcard.OriginCards.Clear();
            newcard.OriginCards.Add(this);
            newcard.Zone = Zone;
            newcard.CardColor = CardColor?.Clone();
            newcard.CardPoint = CardPoint;
            return newcard;
        }

        public Card Clone(Card origincard)
        {
            Card newcard = Create();
            newcard.OriginCards.Clear();
            newcard.OriginCards.Add(origincard);
            newcard.Zone = origincard.Zone;
            newcard.CardColor = origincard.CardColor?.Clone();
            newcard.CardPoint = origincard.CardPoint;
            return newcard;
        }

        public Card Clone(IEnumerable<Card> origincards)
        {
            if (origincards.Count() == 0) return Clone();
            Card newcard = Clone(origincards.FirstOrDefault());
            if (origincards.Count() == 1) return newcard;
            HashSet<Enum_CardColor> colors = new HashSet<Enum_CardColor>();
            newcard.OriginCards.Clear();
            newcard.OriginCards.AddRange(origincards);
            newcard.Zone = origincards.FirstOrDefault()?.Zone;
            newcard.CardPoint = -1;
            foreach (Card card in origincards)
            {
                if (card.CardColor?.E == null) continue;
                if (colors.Contains(card.CardColor.E)) continue;
                colors.Add(card.CardColor.E);
            }
            if (colors.Count() == 1) return newcard;
            if (colors.Count() == 2 
             && colors.Contains(Enum_CardColor.Heart)
             && colors.Contains(Enum_CardColor.Diamond))
            {
                newcard.CardColor = new CardColor(Enum_CardColor.Red);
                return newcard;
            }
            if (colors.Count() == 2
            && colors.Contains(Enum_CardColor.Spade)
            && colors.Contains(Enum_CardColor.Club))
            {
                newcard.CardColor = new CardColor(Enum_CardColor.Black);
                return newcard;
            }
            newcard.CardColor = new CardColor(Enum_CardColor.None);
            return newcard;
        }
    }

    public class CardType
    {
        public CardType(Enum_CardType _E, CardSubType _subtype)
        {
            this.e = _E;
            this.subtype = _subtype;
        }

        private Enum_CardType e;
        public Enum_CardType E { get { return this.e; } }

        private CardSubType subtype;
        public CardSubType SubType { get { return this.subtype; } }

        public CardType Clone()
        {
            CardType newtype = new CardType(e, subtype?.Clone());
            return newtype;
        }
        
    }

    public class CardSubType
    {
        public CardSubType(Enum_CardSubType _e) { this.e = _e; }

        private Enum_CardSubType e;
        public Enum_CardSubType E { get { return this.e; } }

        public CardSubType Clone()
        {
            return new CardSubType(e);
        }
    }

    public class CardColor
    {

        public CardColor(Enum_CardColor _e)
        {
            this.e = _e;
        }

        private Enum_CardColor e;
        public Enum_CardColor E { get { return this.e; } }

        public CardColor Clone()
        {
            return new CardColor(e);
        }

        public bool SeemAs(Enum_CardColor color)
        {
            switch (color)
            {
                case Enum_CardColor.Red:
                    return e == Enum_CardColor.Red || e == Enum_CardColor.Heart || e == Enum_CardColor.Diamond;
                case Enum_CardColor.Black:
                    return e == Enum_CardColor.Black || e == Enum_CardColor.Club || e == Enum_CardColor.Spade;
                case Enum_CardColor.None:
                    return e == Enum_CardColor.None;
                default:
                    return e == color;
            }
            
        }
    }

    public class CardInfo
    {
        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        private string description;
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        private CardImage image;
        public CardImage Image
        {
            get { return this.image; }
            set { this.image = value; }
        }
    }

    public class CardImage
    {
        private ImageSource source;
        public ImageSource Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        private Rect rect = new Rect(0, 0, 1, 1);
        public Rect Rect
        {
            get { return this.rect; }
            set { this.rect = value; }
        }

        private string author;
        public string Author
        {
            get { return this.author; }
            set { this.author = value; }
        }

        private string pixivid;
        public string PixivID
        {
            get { return this.pixivid; }
            set { this.pixivid = value; }
        }
    }

    public enum Enum_CardType
    {
        Base,
        Equip,
        Spell,
    }

    public enum Enum_CardSubType
    {
        Weapon,
        Armor,
        HorsePlus,
        HorseMinus,
        Immediate,
        Delay,
    }

    public enum Enum_CardColor
    {
        None,
        Heart,
        Diamond,
        Spade,
        Club,
        Red,
        Black,
    }
}

