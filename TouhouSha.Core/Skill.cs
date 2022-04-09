using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core.UIs;
using TouhouSha.Core.AIs;
using TouhouSha.Core.Events;

namespace TouhouSha.Core
{
    public abstract class Skill : ShaPriorityObject
    {
        public override string ToString()
        {
            return String.Format("Skill:{0}", KeyName);
        }

        private Player owner;
        public Player Owner
        {
            get { return this.owner; }
            set { this.owner = value; }
        }

        private List<Calculator> calculators = new List<Calculator>();
        public List<Calculator> Calculators
        {
            get { return this.calculators; }
        }
        
        private List<Trigger> triggers = new List<Trigger>();
        public List<Trigger> Triggers
        {
            get { return this.triggers; }
        }

        public bool IsHidden
        {
            get { return GetValue("隐藏") == 1; }
            set { SetValue("隐藏", value ? 1 : 0); }
        }

        public bool IsLocked
        {
            get { return GetValue("锁定技") == 1; }
            set { SetValue("锁定技", value ? 1 : 0); }
        }

        public bool IsOnce
        {
            get { return GetValue("限定技") == 1; }
            set { SetValue("限定技", value ? 1 : 0); }
        }

        public bool IsLeader
        {
            get { return GetValue("主公技") == 1; }
            set { SetValue("主公技", value ? 1 : 0); }
        }

        public bool IsLeaderForLeader
        {
            get { return GetValue("主公技给主公") == 1; }
            set { SetValue("主公技给主公", value ? 1 : 0); }
        }

        public bool IsLeaderForSlave
        {
            get { return GetValue("主公技给忠臣") == 1; }
            set { SetValue("主公技给忠臣", value ? 1 : 0); }
        }
        
        public abstract Skill Clone();

        public abstract Skill Clone(Card newcard);
        
        public abstract SkillInfo GetInfo();

        public virtual InitativeSkillWorth GetWorthAI() { return null; }

        public virtual InitativeSkillAuto GetAutoAI() { return null; }

    }
    
    public interface ISkillInitative
    {
        ConditionFilter UseCondition { get; }

        PlayerFilter TargetFilter { get; }

        CardFilter CostFilter { get; }

        void Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs);
    }
    
    public interface ISkillCardConverter
    {
        ConditionFilter UseCondition { get; }

        CardFilter CardFilter { get; }

        CardCalculator CardConverter { get; }
    }

    public interface ISkillCardMultiConverter : ISkillCardConverter
    {
        IEnumerable<string> GetCardTypes(Context ctx);

        void SetSelectedCardType(Context ctx, string cardtype);
        
        string SelectedCardType { get; }
    }

    public interface ISkillCardMultiConverter2 : ISkillCardConverter
    {
        void SetEnabledCardTypes(Context ctx, IEnumerable<string> cardtypes);

        void CancelEnabledCardTypes(Context ctx);

        IEnumerable<string> EnabledCardTypes { get; }
    }

    public interface ISkillCardConverterBefore
    {
        void Before(Context ctx, SkillEvent ev_skill, CardEvent ev_card);
    }

    public interface ISkillOnceKnowned
    {
        string UsedProperty { get; }
    }
}
