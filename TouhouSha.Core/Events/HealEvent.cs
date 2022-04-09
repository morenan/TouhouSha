using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public class HealEvent : Event
    {
        public const string DefaultKeyName = "回复体力时";
        public const string Normal = "回复";
        
        public HealEvent()
        {
            KeyName = DefaultKeyName;
        }

        private Player source;
        public Player Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        private Player target;
        public Player Target
        {
            get { return this.target; }
            set { this.target = value; }
        }

        private string healtype;
        public string HealType
        {
            get { return this.healtype; }
            set { this.healtype = value; }
        }

        private int healvalue;
        public int HealValue
        {
            get { return this.healvalue; }
            set { this.healvalue = value; }
        }

        public override Player GetHandleStarter()
        {
            return Source;
        }
        public override bool IsStopHandle()
        {
            if (Source != null && !Source.IsAlive) return true;
            if (Target == null || !Target.IsAlive) return true;
            return false;
        }
    }
}
