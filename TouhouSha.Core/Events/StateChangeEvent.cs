using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public class StateChangeEvent : Event
    {
        public const string DefaultKeyName = "阶段更变时";
        public const int Step_BeforeStart = 0;
        public const int Step_Start = 5;
        public const int Step_End = 10;
        public const int Step_AfterEnd = 15;

        public StateChangeEvent()
        {
            KeyName = DefaultKeyName;
        }

        private State oldstate;
        public State OldState
        {
            get { return this.oldstate; }
            set { this.oldstate = value; }
        }

        private State newstate;
        public State NewState
        {
            get { return this.newstate; }
            set { this.newstate = value; }
        }

        public override Player GetHandleStarter()
        {
            return NewState?.Owner;
        }

        public override bool IsStopHandle()
        {
            if (newstate?.Owner != null && !newstate.Owner.IsAlive) return true;
            return false;
        }
    }
}
