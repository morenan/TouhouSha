using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core.UIs;

namespace TouhouSha.Core.AIs
{
    public abstract class DesktopCardFilterWorth
    {
        private AITools aitools;
        public AITools AITools
        {
            get { return this.aitools; }
            set { this.aitools = value; }
        }

        public abstract double GetWorth(Context ctx, DesktopCardBoardCore core, IEnumerable<Card> selecteds, Card want);
        public abstract double GetWorthNo(Context ctx, DesktopCardBoardCore core);

    }
}
