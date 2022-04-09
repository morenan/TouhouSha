using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.AIs;
using TouhouSha.Core.UIs;
using TouhouSha.Core.Events;


namespace TouhouSha.Koishi.AIs
{
    public class PointMultiBattleCardWorth : CardFilterWorth
    {
        public PointMultiBattleCardWorth(PointMultiBattleEventBase _battleevent)
        {
            this.battleevent = _battleevent;
        }

        private PointMultiBattleEventBase battleevent;
        public PointMultiBattleEventBase BattleEvent { get { return this.battleevent; } }

        public override double GetWorth(Context ctx, SelectCardBoardCore core, IEnumerable<Card> selecteds, Card want)
        {
            if (core.Controller == BattleEvent.Source)
            {
                double worth = 0.0d;
                foreach (Player target in BattleEvent.Targets)
                {
                    double prob = AITools.CardGausser.GetProbablyOfCardPointNotLess(ctx, target, want.CardPoint);
                    worth += (1 - prob) * GetWorthWin(ctx, core.Controller, target)
                            + prob * GetWorthLose(ctx, core.Controller, target);
                }
                return worth;
            }
            else if (BattleEvent.Targets.Contains(core.Controller))
            {
                double prob = 1 - AITools.CardGausser.GetProbablyOfCardPointNotLess(ctx, BattleEvent.Source, want.CardPoint);
                return (1 - prob) * GetWorthWin(ctx, core.Controller, core.Controller)
                    + prob * GetWorthLose(ctx, core.Controller, core.Controller);
            }
            return 0.0d;
        }

        public override double GetWorthNo(Context ctx, SelectCardBoardCore core)
        {
            return 0.0d;
        }

        public virtual double GetWorthWin(Context ctx, Player controller, Player target)
        {
            double worth = 0;
            worth += AITools.WorthAcc.GetWorthPointBattleWin(ctx, controller, BattleEvent.Source, BattleEvent);
            worth += AITools.WorthAcc.GetWorthPointBattleLose(ctx, controller, target, BattleEvent);
            return worth;
        }

        public virtual double GetWorthLose(Context ctx, Player controller, Player target)
        {
            double worth = 0;
            worth += AITools.WorthAcc.GetWorthPointBattleLose(ctx, controller, BattleEvent.Source, BattleEvent);
            worth += AITools.WorthAcc.GetWorthPointBattleWin(ctx, controller, target, BattleEvent);
            return worth;
        }
    }
}
