using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.UIs.Events
{
    public class UIEventGroup : UIEvent
    {
        private List<UIEvent> items = new List<UIEvent>();
        public List<UIEvent> Items { get { return this.items; } }
    }
}
