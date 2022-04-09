using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.AIs
{
    public abstract class CardTargetFilterWorth
    {
        private AITools aitools;
        public AITools AITools
        {
            get { return this.aitools; }
            set { this.aitools = value; }
        }
        public virtual double GetUseWorth(Context ctx, Card card, Player user, PlayerFilter targetfilter)
        {
            List<Player> targets = new List<Player>();
            double worthcard = AITools.WorthAcc.GetWorth(ctx, user, card);
            return AITools.StepOptimizeAlgorithm(ctx, targetfilter, targets, target => GetWorth(ctx, card, user, targets, target)) - worthcard;
        }

        public abstract double GetWorth(Context ctx, Card card, Player user, IEnumerable<Player> selecteds, Player want);
    }
}
