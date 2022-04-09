using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core.Events;

namespace TouhouSha.Core
{
    public class Zone : ShaObject
    {
        public const string Draw = "摸牌堆";
        public const string Desktop = "桌面";
        public const string Discard = "弃牌堆";
        public const string Hand = "手牌";
        public const string Equips = "装备栏";
        public const string Judge = "判定区";

        public override string ToString()
        {
            return String.Format("Zone:{0} for {1}", KeyName, Owner?.Name ?? "null");
        }

        private Player owner;
        public Player Owner
        {
            get { return this.owner; }
            set { this.owner = value; }
        }

        private Enum_ZoneFlag flag = Enum_ZoneFlag.None;
        public Enum_ZoneFlag Flag
        {
            get { return this.flag; }
            set { this.flag = value; }
        }

        private ObservableCollection<Card> cards = new ObservableCollection<Card>();
        public ObservableCollection<Card> Cards
        {
            get { return this.cards; }
        }

        private bool allowconverted;
        public bool AllowConverted
        {
            get { return this.allowconverted; }
            set { IvProp0("AllowConverted"); this.allowconverted = value; IvProp1("AllowConverted"); }
        }

        private bool usecardskill;
        public bool UseCardSkill
        {
            get { return this.usecardskill; }
            set { IvProp0("UseCardSkill"); this.usecardskill = value; IvProp1("UseCardSkill"); }
        }

        public void Shuffle()
        {
            Random r = new Random();
            for (int i = 0; i < cards.Count(); i++)
            {
                int j = r.Next() % cards.Count();
                Card temp = cards[i]; cards[i] = cards[j]; cards[j] = temp;
            }
        }

        public void Remove(Card card)
        {
            if (card == null) return;
            if (card.Zone == null) return;
            if (card.Zone != this)
            {
                card.Zone.Remove(card);
                return;
            }
            card.SetZoneItsAndOrigins(null);
            cards.Remove(card);
            if (!AllowConverted)
            {
                List<Card> initcards = card.GetInitialCards().ToList();
                foreach (Card _card in initcards)
                    cards.Remove(_card);
            }
            else
            {
                card.ActionItsAndOrigins((_card) => { cards.Remove(_card); });
                foreach (Card _card in cards.ToArray())
                {
                    bool removed = false;
                    _card.ActionItsAndOrigins((__card) =>
                    {
                        if (__card != card) return;
                        removed |= cards.Remove(_card);
                    });
                    if (removed)
                        _card.SetZoneItsAndOrigins(null);
                }
            }
        }

        public void Add(Card card, Enum_MoveCardFlag flag = Enum_MoveCardFlag.None)
        {
            if (card == null) return;
            if (card.Zone == this) return;
            if (card.Zone != null) card.Zone.Remove(card);
            if (card.Zone != null) return;
            if (AllowConverted)
            {
                card.SetZoneItsAndOrigins(this);
                if ((flag & Enum_MoveCardFlag.MoveToFirst) != Enum_MoveCardFlag.None)
                    cards.Insert(0, card);
                else 
                    cards.Add(card);
                return;
            }
            List<Card> initcards = card.GetInitialCards().ToList();
            card.SetZoneItsAndOrigins(this);
            if ((flag & Enum_MoveCardFlag.MoveToFirst) != Enum_MoveCardFlag.None)
            {
                initcards.Reverse();
                foreach (Card _card in initcards)
                    cards.Insert(0, _card);
            }
            else
            {
                foreach (Card _card in initcards)
                    cards.Add(_card);
            }
        }

        public void AddRange(IEnumerable<Card> cards, Enum_MoveCardFlag flag = Enum_MoveCardFlag.None)
        {
            if ((flag & Enum_MoveCardFlag.MoveToFirst) != Enum_MoveCardFlag.None)
            {
                foreach (Card card in cards.Reverse())
                    Add(card, flag);
            }
            else
            {
                foreach (Card card in cards)
                    Add(card, flag);
            }
        }
        
    }

    public class EquipZone : Zone
    {
        public EquipZone()
        {
            Cards.CollectionChanged += Cards_CollectionChanged;
            cells.Add(new EquipCell(Enum_CardSubType.Weapon));
            cells.Add(new EquipCell(Enum_CardSubType.Armor));
            cells.Add(new EquipCell(Enum_CardSubType.HorsePlus));
            cells.Add(new EquipCell(Enum_CardSubType.HorseMinus));
        }
        
        private List<EquipCell> cells = new List<EquipCell>();
        public List<EquipCell> Cells
        {
            get { return this.cells; }
        }
        
        public void CardCellReset()
        {
            for (int i = 0; i < cells.Count(); i++)
            {
                EquipCell cell = cells[i];
                cell.CardIndex = -1;
            }
            for (int i = 0; i < Cards.Count(); i++)
            {
                Card card = Cards[i];
                Enum_CardSubType celltype = card.CardType?.SubType?.E ?? Enum_CardSubType.Weapon;
                for (int j = 0; j < cells.Count(); j++)
                {
                    EquipCell cell = cells[j];
                    if (!cell.IsEnabled) continue;
                    if (cell.E != celltype) continue;
                    if (cell.CardIndex >= 0) continue;
                    cell.CardIndex = i;
                    break;
                }
            }
            EquipsChanging?.Invoke(this, new EventArgs());
            EquipsChanged?.Invoke(this, new EventArgs());
        }
        
        private void Cards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CardCellReset();
        }

        public event System.EventHandler EquipsChanging;
        public event System.EventHandler EquipsChanged;
    }

    public class EquipCell : ShaObject
    {
        public EquipCell(Enum_CardSubType _e)
        {
            this.e = _e;
            this.isenabled = true;
            this.cardindex = -1;
        }

        private Enum_CardSubType e;
        public Enum_CardSubType E
        {
            get { return this.e; }
        }

        private bool isenabled;
        public bool IsEnabled
        {
            get { return this.isenabled; }
            set { IvProp0("IsEnabled"); this.isenabled = value; IvProp1("IsEnabled"); }
        }

        private int cardindex;
        public int CardIndex
        {
            get { return this.cardindex; }
            set { IvProp0("CardIndex"); this.cardindex = value; IvProp1("CardIndex"); }
        }
    }
    
    public enum Enum_ZoneFlag
    {
        None = 0,
        LabelOnPlayer = 1,
        CardUnvisibled = 2,
    }
}
