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
    public class HandlePeachTrigger : Trigger, ITriggerInState
    {
        public const string DefaultKeyName = "响应桃";

        public HandlePeachTrigger()
        {
            KeyName = DefaultKeyName;
            Condition = new HandlePeachCondition();
        }

        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            CardEvent ev = (CardEvent)(state.Ev);
            ctx.World.Heal(ev.Source, state.Owner, 1, HealEvent.Normal, ev);
        }
    }

    public class HandlePeachCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (ev.Cancel) return false;
            if (ev.Card.KeyName?.Equals(PeachCard.Normal) != true) return false;
            return true;
        }
    }

}
