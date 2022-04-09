using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public class DiscardStateEvent : Event
    {
        private Player source;
        public Player Source
        {
            get { return this.source; }
            set { this.source = value; }
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
}
