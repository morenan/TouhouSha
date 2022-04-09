using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public class SkillEvent : Event, IMultiTargetsEvent
    {
        public const string DefaultKeyName = "发动技能";

        public SkillEvent()
        {
            KeyName = DefaultKeyName;
        }

        private Player source;
        public Player Source
        {
            get { return this.source; }
            set { this.source = value; }
        }
        
        private Skill skill;
        public Skill Skill
        {
            get { return this.skill; }
            set { this.skill = value; }
        }

        private List<Player> targets = new List<Player>();
        public List<Player> Targets
        {
            get { return this.targets; }
        }

        public override Player GetHandleStarter()
        {
            return Source;
        }

        public override bool IsStopHandle()
        {
            if (Source == null || !Source.IsAlive) return true;
            return false;
        }
    }

    public class CardSkillEvent : SkillEvent
    {
        public new const string DefaultKeyName = "发动卡牌技能";
        
        public CardSkillEvent()
        {
            KeyName = DefaultKeyName;
        }
        
        private Card card;
        public Card Card
        {
            get { return this.card; }
            set { this.card = value; }
        }
    }

    public class SkillDoneEvent : SkillEvent
    {
        public new const string DefaultKeyName = "发动技能完毕";
       
        public SkillDoneEvent(SkillEvent ev)
        {
            KeyName = DefaultKeyName;
            Reason = ev.Reason;
            Skill = ev.Skill;
            Source = ev.Source;
            Targets.Clear();
            Targets.AddRange(ev.Targets); 
        }
    }

    public class CardSkillDoneEvent : CardSkillEvent
    {
        public new const string DefaultKeyName = "卡片发动技能完毕";
        
        public CardSkillDoneEvent(CardSkillEvent ev)
        {
            KeyName = DefaultKeyName;
            Reason = ev.Reason;
            Card = ev.Card;
            Skill = ev.Skill;
            Source = ev.Source;
            Targets.Clear();
            Targets.AddRange(ev.Targets);
        }
    }
}
