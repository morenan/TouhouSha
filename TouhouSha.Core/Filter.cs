using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core.AIs;

namespace TouhouSha.Core
{
    public class Filter : ShaPriorityObject
    {
        public override string ToString()
        {
            return String.Format("Filter:{0}", KeyName);
        }
    }

    public class ConditionFilter : Filter
    {
        public override string ToString()
        {
            return String.Format("ConditionFilter:{0}", KeyName);
        }
        public virtual bool Accept(Context ctx)
        {
            return false;
        }
    }

    public class PlayerFilter : Filter
    {
        public override string ToString()
        {
            return String.Format("PlayerFilter:{0}", KeyName);
        }
        public virtual Enum_PlayerFilterFlag GetFlag(Context ctx)
        {
            return Enum_PlayerFilterFlag.None;
        }

        public virtual bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            return false;
        }
        
        public virtual bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return true;
        }

        public virtual PlayerFilterWorth GetWorthAI()
        {
            return null;
        }
       
        public virtual PlayerFilterAuto GetAutoAI()
        {
            return null;
        }
       
    }

    public class CardFilter : Filter
    {
        public override string ToString()
        {
            return String.Format("CardFilter:{0}", KeyName);
        }
        public virtual Enum_CardFilterFlag GetFlag(Context ctx)
        {
            return Enum_CardFilterFlag.None;
        }    
        
        public virtual bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            return false;
        }

        public virtual bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return true;
        }

        public virtual CardFilterWorth GetWorthAI()
        {
            return null;
        }

        public virtual DesktopCardFilterWorth GetDesktopWorthAI()
        {
            return null;
        }

        public virtual CardFilterAuto GetAutoAI()
        {
            return null;
        }

        public virtual DesktopCardFilterAuto GetDesktopAutoAI()
        {
            return null;
        }
    }
    
    public class ConditionFilterFromSkill : ConditionFilter, IFilterFromSkill
    {
        public ConditionFilterFromSkill(Skill _skill) { this.skill = _skill; }

        protected Skill skill;
        public Skill Skill { get { return this.skill; } }
    }

    public class ConditionFilterFromCard : ConditionFilter, IFilterFromCard
    {
        public ConditionFilterFromCard(Card _card) { this.card = _card; }

        protected Card card;
        public Card Card { get { return this.card; } }

    }

    public class PlayerFilterFromSkill : PlayerFilter, IFilterFromSkill
    {
        public PlayerFilterFromSkill(Skill _skill) { this.skill = _skill; }

        protected Skill skill;
        public Skill Skill { get { return this.skill; } }

    }

    public class PlayerFilterFromCard : PlayerFilter, IFilterFromCard
    {
        public PlayerFilterFromCard(Card _card) { this.card = _card; }

        protected Card card;
        public Card Card { get { return this.card; } }
    }

    public class CardFilterFromSkill : CardFilter, IFilterFromSkill
    {
        public CardFilterFromSkill(Skill _skill) { this.skill = _skill; }

        protected Skill skill;
        public Skill Skill { get { return this.skill; } }
    }

    public class CardFilterFromCard : CardFilter, IFilterFromCard
    {
        public CardFilterFromCard(Card _card) { this.card = _card; }

        protected Card card;
        public Card Card { get { return this.card; } }

    }

    public class OverrideConditionFilter : ConditionFilter
    {
        private ConditionFilter oldfilter;
        public ConditionFilter OldFilter
        {
            get { return this.oldfilter; }
            set { this.oldfilter = value; }
        }
    }

    public class OverridePlayerFilter : PlayerFilter
    {
        private PlayerFilter oldfilter;
        public PlayerFilter OldFilter
        {
            get { return this.oldfilter; }
            set { this.oldfilter = value; }
        }
    }

    public class OverrideCardFilter : CardFilter
    {
        private CardFilter oldfilter;
        public CardFilter OldFilter
        {
            get { return this.oldfilter; }
            set { this.oldfilter = value; }
        }
    }
    
    public enum Enum_PlayerFilterFlag
    {
        None = 0,
        ForceAll = 0x00000001,
    }

    public enum Enum_CardFilterFlag
    {
        None = 0,
        ForceAll = 0x00000001,
        ToUse = 0x00000002,
        ToHandle = 0x00000004,
    }

    public interface IFilterFromSkill
    {
        Skill Skill { get; }
    }

    public interface IFilterFromCard
    {
        Card Card { get; }
    }
   
    public interface ICardFilterRequiredCardTypes
    {
        IEnumerable<string> RequiredCardTypes { get; }
    }
}
