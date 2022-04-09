using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.UIs.Events
{
    public class UIStateChangeEvent : UIEvent
    {
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

    }
}
