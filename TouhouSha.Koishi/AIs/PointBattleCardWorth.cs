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
    public class PointBattleCardWorth : CardFilterWorth
    {
        public PointBattleCardWorth(PointBattleEventBase _battleevent)
        {
            this.battleevent = _battleevent;
        }
        
        private PointBattleEventBase battleevent;
        public PointBattleEventBase BattleEvent { get { return this.battleevent; } }
         
        public override double GetWorth(Context ctx, SelectCardBoardCore core, IEnumerable<Card> selecteds, Card want)
        {
            if (core.Controller == BattleEvent.Source)
            {
                double prob = AITools.CardGausser.GetProbablyOfCardPointNotLess(ctx, BattleEvent.Target, want.CardPoint);      
                return (1 - prob) * GetWorthWin(ctx, core.Controller)
                    + prob * GetWorthLose(ctx, core.Controller);
            }
            else if (core.Controller == BattleEvent.Target)
            {
                double prob = 1-AITools.CardGausser.GetProbablyOfCardPointNotLess(ctx, BattleEvent.Source, want.CardPoint);
                return (1 - prob) * GetWorthWin(ctx, core.Controller)
                    + prob * GetWorthLose(ctx, core.Controller);
            }
            return 0.0d;
        }

        public override double GetWorthNo(Context ctx, SelectCardBoardCore core)
        {
            return 0.0d;
        }

        public virtual double GetWorthWin(Context ctx, Player controller)
        {
            double worth = 0;
            worth += AITools.WorthAcc.GetWorthPointBattleWin(ctx, controller, BattleEvent.Source, BattleEvent);
            worth += AITools.WorthAcc.GetWorthPointBattleLose(ctx, controller, BattleEvent.Target, BattleEvent);
            return worth;
        }

        public virtual double GetWorthLose(Context ctx, Player controller)
        {
            double worth = 0;
            worth += AITools.WorthAcc.GetWorthPointBattleLose(ctx, controller, BattleEvent.Source, BattleEvent);
            worth += AITools.WorthAcc.GetWorthPointBattleWin(ctx, controller, BattleEvent.Target, BattleEvent);
            return worth;
        }
    }
}
