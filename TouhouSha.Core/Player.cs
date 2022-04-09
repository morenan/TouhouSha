using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core.Collections;

namespace TouhouSha.Core
{
    public class Player : ShaObject
    {
        public Player()
        {
            skills.CollectionChanged += Skills_CollectionChanged;
            zones.CollectionChanged += Zones_CollectionChanged;
        }
        public override string ToString()
        {
            return KeyName;
        }

        private List<Charactor> charactors = new List<Charactor>();
        public List<Charactor> Charactors { get { return this.charactors; } }

        private ObservableCollection<Skill> skills = new ObservableCollection<Skill>();
        public ObservableCollection<Skill> Skills { get { return this.skills; } }

        private ObservableCollection<Zone> zones = new ObservableCollection<Zone>();
        public ObservableCollection<Zone> Zones { get { return this.zones; } }
        
        private string name;
        public string Name
        {
            get { return this.name; }
            set { IvProp0("Name"); this.name = value; IvProp1("Name"); }
        }

        private string country;
        public string Country
        {
            get { return this.country; }
            set { IvProp0("Country"); this.country = value; IvProp1("Country"); }
        }

        private PlayerAss ass;
        public PlayerAss Ass
        {
            get { return this.ass; }
            set { IvProp0("Ass"); this.ass = value; IvProp1("Ass"); }
        }

        private IPlayerConsole console;
        public IPlayerConsole Console
        {
            get { return this.console; }
            set { IvProp0("Console"); this.console = value; IvProp1("Console"); }
        }

        private IPlayerConsole trusteeshipconsole;
        public IPlayerConsole TrusteeshipConsole
        {
            get { return this.trusteeshipconsole; }
            set { IvProp0("TrusteeshipConsole"); this.trusteeshipconsole = value; IvProp1("TrusteeshipConsole"); }
        }

        private bool istrusteeship;
        public bool IsTrusteeship
        {
            get { return this.istrusteeship; }
            set { IvProp0("IsTrusteeship"); this.istrusteeship = value; IvProp1("IsTrusteeship"); }
        }

        public IPlayerConsole GetCurrentConsole()
        {
            if (IsTrusteeship) return TrusteeshipConsole;
            if (Console != null) return Console;
            return TrusteeshipConsole;
        }

        public bool IsAlive
        {
            get { return GetValue("IsAlive") == 1; }
            set { SetValue("IsAlive", value ? 1 : 0); }
        }

        public bool IsChained
        {
            get { return GetValue("IsChained") == 1; }
            set { SetValue("IsChained", value ? 1 : 0); }
        }

        public bool IsFacedDown
        {
            get { return GetValue("IsFacedDown") == 1; }
            set { SetValue("IsFacedDown", value ? 1 : 0); }
        }

        public bool IsAssVisibled
        {
            get { return GetValue("IsAssVisibled") == 1; }
            set { SetValue("IsAssVisibled", value ? 1 : 0); }
        }

        public int HP
        {
            get { return GetValue("HP"); }
            set { SetValue("HP", value); }
        }

        public int MaxHP
        {
            get { return GetValue("MaxHP"); }
            set { SetValue("MaxHP", value); }
        }


        public bool IsCountry(string countryname)
        {
            return country?.Equals(countryname) == true;
        }

        #region Calculator Collection

        private CalculatorCollection calculators = new CalculatorCollection();
        public CalculatorCollection Calculators { get { return this.calculators; } }

        public void CalculatorCollectionReset()
        {
            calculators.Clear();
            foreach (Skill skill in skills)
                CalculatorCollectionAdd(skill);
            foreach (Zone zone in zones)
                CalculatorCollectionAdd(zone);
        }

        protected void CalculatorCollectionAdd(Zone zone)
        {
            if (!zone.UseCardSkill) return;
            foreach (Card card in zone.Cards)
                CalculatorCollectionAdd(card);
        }
        
        protected void CalculatorCollectionAdd(Card card)
        {
            foreach (Skill skill in card.Skills)
                CalculatorCollectionAdd(skill);
        }

        protected void CalculatorCollectionAdd(Skill skill)
        {
            foreach (Trigger trigger in skill.Triggers)
                Triggers.Add(trigger);
        }

        protected void CalculatorCollectionRemove(Zone zone)
        {
            if (!zone.UseCardSkill) return;
            foreach (Card card in zone.Cards)
                CalculatorCollectionRemove(card);
        }

        protected void CalculatorCollectionRemove(Card card)
        {
            foreach (Skill skill in card.Skills)
                CalculatorCollectionRemove(skill);
        }

        protected void CalculatorCollectionRemove(Skill skill)
        {
            foreach (Trigger trigger in skill.Triggers)
                Triggers.Remove(trigger);
        }

        #endregion

        #region Trigger Collection

        private TriggerCollection triggers = new TriggerCollection();
        public TriggerCollection Triggers { get { return this.triggers; } }
        
        public void TriggerCollectionReset()
        {
            triggers.Clear();
            foreach (Skill skill in skills)
                TriggerCollectionAdd(skill);
            foreach (Zone zone in zones)
                TriggerCollectionAdd(zone);
        }
        
        protected void TriggerCollectionAdd(Zone zone)
        {
            if (!zone.UseCardSkill) return;
            foreach (Card card in zone.Cards)
                TriggerCollectionAdd(card);
        }

        protected void TriggerCollectionAdd(Card card)
        {
            foreach (Skill skill in card.Skills)
                TriggerCollectionAdd(skill);
        }

        protected void TriggerCollectionAdd(Skill skill)
        {
            foreach (Calculator calculator in skill.Calculators)
                calculators.Add(calculator);
        }

        protected void TriggerCollectionRemove(Zone zone)
        {
            if (!zone.UseCardSkill) return;
            foreach (Card card in zone.Cards)
                TriggerCollectionRemove(card);
        }

        protected void TriggerCollectionRemove(Card card)
        {
            foreach (Skill skill in card.Skills)
                TriggerCollectionRemove(skill);
        }

        protected void TriggerCollectionRemove(Skill skill)
        {
            foreach (Calculator calculator in skill.Calculators)
                calculators.Remove(calculator);
        }

        #endregion

        #region Event Handler

        private void Skills_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                CalculatorCollectionReset();
                TriggerCollectionReset();
                return;
            }
            if (e.OldItems != null)
                foreach (Skill skill in e.OldItems)
                {
                    skill.Owner = null;
                    CalculatorCollectionRemove(skill);
                    TriggerCollectionRemove(skill);
                }
            if (e.NewItems != null)
                foreach (Skill skill in e.NewItems)
                {
                    skill.Owner = this;
                    CalculatorCollectionAdd(skill);
                    TriggerCollectionAdd(skill);
                }
        }

        private void Zones_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                CalculatorCollectionReset();
                TriggerCollectionReset();
                return;
            }
            if (e.OldItems != null)
                foreach (Zone zone in e.OldItems)
                {
                    zone.PropertyChanging -= Zone_PropertyChanging;
                    zone.PropertyChanged -= Zone_PropertyChanged;
                    zone.Cards.CollectionChanged -= Zone_Cards_CollectionChanged;
                    CalculatorCollectionRemove(zone);
                    TriggerCollectionRemove(zone);
                }
            if (e.NewItems != null)
                foreach (Zone zone in e.NewItems)
                {
                    zone.PropertyChanging += Zone_PropertyChanging;
                    zone.PropertyChanged += Zone_PropertyChanged;
                    zone.Cards.CollectionChanged += Zone_Cards_CollectionChanged;
                    CalculatorCollectionAdd(zone);
                    TriggerCollectionAdd(zone);
                }
        }

        private void Zone_Cards_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                CalculatorCollectionReset();
                TriggerCollectionReset();
                return;
            }
            if (e.OldItems != null)
                foreach (Card card in e.OldItems)
                {
                    CalculatorCollectionRemove(card);
                    TriggerCollectionRemove(card);
                }
            if (e.NewItems != null)
                foreach (Card card in e.NewItems)
                {
                    CalculatorCollectionAdd(card);
                    TriggerCollectionAdd(card);
                }
        }

        private void Zone_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (!(sender is Zone)) return;
            Zone zone = (Zone)sender;
            if (!e.PropertyName.Equals("UseCardSkill")) return;
            CalculatorCollectionRemove(zone);
        }

        private void Zone_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is Zone)) return;
            Zone zone = (Zone)sender;
            if (!e.PropertyName.Equals("UseCardSkill")) return;
            CalculatorCollectionAdd(zone);

        }

        #endregion
        
    }

    public class PlayerAss
    {
        public PlayerAss(Enum_PlayerAss _e) { this.e = _e; }

        private Enum_PlayerAss e;
        public Enum_PlayerAss E { get { return this.e;  } }

        public override string ToString()
        {
            switch (e)
            {
                case Enum_PlayerAss.Leader: return "主公";
                case Enum_PlayerAss.Slave: return "忠臣";
                case Enum_PlayerAss.Avenger: return "反贼";
                case Enum_PlayerAss.Spy: return "内奸";
            }
            return base.ToString();
        }
    }

    public enum Enum_PlayerAss
    {
        Leader,
        Slave,
        Avenger,
        Spy,
    }
}
