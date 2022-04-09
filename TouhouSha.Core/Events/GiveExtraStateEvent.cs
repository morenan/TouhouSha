using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public class GiveExtraStateEvent : Event
    {
        public const string DefaultKeyName = "进入额外阶段";
        
        public GiveExtraStateEvent()
        {
            KeyName = DefaultKeyName;
        }

        private State startstate;
        public State StartState
        {
            get { return this.startstate; }
            set { this.startstate = value; }
        }

        public override Player GetHandleStarter()
        {
            return startstate?.Owner;
        }
        public override bool IsStopHandle()
        {
            if (startstate?.Owner == null) return true;
            if (!startstate.Owner.IsAlive) return true;
            return false;
        }
    }
}
