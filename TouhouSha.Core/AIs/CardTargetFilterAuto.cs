using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.AIs
{
    public abstract class CardTargetFilterAuto
    {
        private AITools aitools;
        public AITools AITools
        {
            get { return this.aitools; }
            set { this.aitools = value; }
        }

        public abstract double GetUseWorth(Context ctx, Card card, Player user);

        public abstract List<Player> GetSelection(Context ctx, Card card, Player user);
    }
}
