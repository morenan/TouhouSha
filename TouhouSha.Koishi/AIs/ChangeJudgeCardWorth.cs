using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.AIs;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Cards;

namespace TouhouSha.Koishi.AIs
{
    public class ChangeJudgeCardWorth : CardFilterWorth
    {
        public override double GetWorthNo(Context ctx, SelectCardBoardCore core)
        {
            State state = ctx.World.GetCurrentState();
            if (state?.Ev is JudgeEvent)
            {
                JudgeEvent ev_judge = (JudgeEvent)(state.Ev);
                if (ev_judge.JudgeCards.Count() == 0) return 0;
                return GetWorthJudge(ctx, core.Controller, ev_judge, ev_judge.JudgeCards[0]);
            }
            return 0.0d;
        }

        public override double GetWorth(Context ctx, SelectCardBoardCore core, IEnumerable<Card> selecteds, Card want)
        {
            State state = ctx.World.GetCurrentState();
            double lostworth = -AITools.WorthAcc.GetWorth(ctx, core.Controller, want);
            if (state?.Ev is JudgeEvent)
            {
                JudgeEvent ev_judge = (JudgeEvent)(state.Ev);
                return GetWorthJudge(ctx, core.Controller, ev_judge, want) + lostworth;
            }
            return lostworth;
        }

        protected virtual double GetWorthJudge(Context ctx, Player controller, JudgeEvent ev_judge, Card judgecard)
        {
            if (ev_judge.Reason is DelaySpellEvent)
            {
                DelaySpellEvent ev_delay = (DelaySpellEvent)(ev_judge.Reason);
                switch (ev_delay.Card?.KeyName)
                {
                    case HappyCard.Normal:
                        if (judgecard.CardColor?.SeemAs(Enum_CardColor.Heart) == true)
                            return 0;
                        else
                            return -AITools.WorthAcc.GetWorthInPhase(ctx, controller, ev_delay.Target);
                    case HungerCard.Normal:
                        if (judgecard.CardColor?.SeemAs(Enum_CardColor.Club) == true)
                            return 0;
                        else
                            return -AITools.WorthAcc.GetWorthExpected(ctx, controller, ev_delay.Target.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true)) * 2;
                    case LightCard.Normal:
                        if (judgecard.CardColor?.SeemAs(Enum_CardColor.Spade) == true
                         && judgecard.CardPoint >= 2
                         && judgecard.CardPoint <= 9)
                            return -AITools.WorthAcc.GetWorthPerHp(ctx, controller, ev_delay.Target) * 3;
                        else
                            return 0;
                }
            }
            return 0;
        }
    }
}
