using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Koishi.Cards;

namespace TouhouSha.Koishi.Triggers
{
    public class SheepTrigger : Trigger, ITriggerInState
    {
        public const string DefaultKeyName = "顺手牵羊响应";

        public SheepTrigger()
        {
            KeyName = DefaultKeyName;
            Condition = new SheepCondition();
        }

        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            CardEvent ev = (CardEvent)(state.Ev);
            ctx.World.StealTargetCard(ev.Source, state.Owner, 1, ev, true, true);
        }
    }

    public class SheepCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return false;
            if (state.GetValue(NegateCard.HasBeenNegated) == 1) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (ev.Card?.KeyName?.Equals(SheepCard.Normal) != true) return false;
            return true;
        }
    }

}
