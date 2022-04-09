using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Koishi.Cards;

namespace TouhouSha.Koishi.Triggers
{
    public class HandleLiqureTrigger : Trigger, ITriggerInState
    {
        public const string DefaultKeyName = "响应酒";

        public HandleLiqureTrigger()
        {
            KeyName = DefaultKeyName;
            Condition = new HandleLiqureCondition();
        }

        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            int bullup = state.Owner.GetValue(LiqureCard.BullUp);
            state.Owner.SetValue(LiqureCard.BullUp, bullup + 1);
        }
    }

    public class HandleLiqureCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (ev.Cancel) return false;
            if (ev.Card.KeyName?.Equals(LiqureCard.Normal) != true) return false;
            return true;
        }
    }
}
