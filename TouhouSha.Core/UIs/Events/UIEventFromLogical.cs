using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.UIs.Events
{
    public class UIEventFromLogical : UIEvent
    {
        private Event logicalevent;
        public Event LogicalEvent
        {
            get { return this.logicalevent; }
            set { this.logicalevent = value; }
        }
    }
}
