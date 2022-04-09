using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.UIs
{
    public class QuestEventInUseCardStateEventArgs
    {
        public QuestEventInUseCardStateEventArgs(Player _owner)
        {
            this.owner = _owner;
            this.result = null;
        }

        private Player owner;
        public Player Owner
        {
            get { return this.owner; }
        }

        private Event result;
        public Event Result
        {
            get { return this.result; }
            set { this.result = value; }
        }
    }

    public delegate void QuestEventInUseCardStateEventHandler(object sender, QuestEventInUseCardStateEventArgs e);
}
