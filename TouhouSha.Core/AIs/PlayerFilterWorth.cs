using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core.UIs;

namespace TouhouSha.Core.AIs
{
    public abstract class PlayerFilterWorth
    {
        private AITools aitools;
        public AITools AITools
        {
            get { return this.aitools; }
            set { this.aitools = value; }
        }

        public abstract double GetWorth(Context ctx, SelectPlayerBoardCore core, IEnumerable<Player> selecteds, Player want);
        public abstract double GetWorthNo(Context ctx, SelectPlayerBoardCore core);

    }
}
