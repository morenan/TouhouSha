using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public class DelaySpellEvent : Event
    {
        public const string DefaultKeyName = "延迟锦囊结算";

        public DelaySpellEvent()
        {
            KeyName = DefaultKeyName;
        }

        private Player target;
        public Player Target
        {
            get { return this.target; }
            set { this.target = value; }
        }

        private Card card;
        public Card Card
        {
            get { return this.card; }
            set { this.card = value; }
        }
        public override Player GetHandleStarter()
        {
            return Target;
        }

        public override bool IsStopHandle()
        {
            if (target == null || !target.IsAlive) return true;
            return false;
        }

    }
}
