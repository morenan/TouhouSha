using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public class DieEvent : Event
    {
        public const string DefaultKeyName = "死亡事件";

        public DieEvent()
        {
            KeyName = DefaultKeyName;
        }

        private Player source;
        public Player Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        public override Player GetHandleStarter()
        {
            return source;
        }

        public override bool IsStopHandle()
        {
            return false;
        }

    }
}
