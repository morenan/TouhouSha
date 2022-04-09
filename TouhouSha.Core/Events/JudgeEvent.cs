using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public class JudgeEvent : Event
    {
        private Player judgetarget;
        public Player JudgeTarget
        {
            get { return this.judgetarget; }
            set { this.judgetarget = value; }
        }

        private List<Card> judgecards = new List<Card>();
        public List<Card> JudgeCards
        {
            get { return this.judgecards; }
        }

        private int judgenumber = 1;
        public int JudgeNumber
        {
            get { return this.judgenumber; }
            set { this.judgenumber = value; }
        }

        public override Player GetHandleStarter()
        {
            return JudgeTarget;
        }

        public override bool IsStopHandle()
        {
            if (JudgeTarget == null) return true;
            if (!JudgeTarget.IsAlive) return true;
            return false;
        }

    }
}
