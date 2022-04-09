using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public class SkillInitativeEvent : Event
    {
        public const string DefaultKeyName = "技能初发";

        public SkillInitativeEvent()
        {
            KeyName = DefaultKeyName;
        }

        private Skill skill;
        public Skill Skill 
        {
            get { return this.skill; }
            set { this.skill = value; }
        }

        private Player user;
        public Player User 
        { 
            get { return this.user; }
            set { this.user = value; }
        }

        private List<Player> targets = new List<Player>();
        public List<Player> Targets
        {
            get { return this.targets; }
        }

        private List<Card> costs = new List<Card>();
        public List<Card> Costs
        {
            get { return this.costs; }
        }

        public override Player GetHandleStarter()
        {
            return user;
        }

        public override bool IsStopHandle()
        {
            if (User == null || !User.IsAlive) return true;
            return false;
        }
    }
}
