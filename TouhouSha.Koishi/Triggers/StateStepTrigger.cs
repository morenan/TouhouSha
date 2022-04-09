using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;

namespace TouhouSha.Koishi.Triggers
{
    public class StateStepTrigger : Trigger, ITriggerInAnyState
    {
        public StateStepTrigger()
        {
            KeyName = "阶段步进";
            Condition = new StateStepCondition();
        }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            State newstate = state.Clone();
            newstate.Step++;
            ctx.World.EnterStateAfterState(newstate, state);
        }

    }
    public class StateStepCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is StateChangeEvent)) return false;
            StateChangeEvent ev = (StateChangeEvent)(ctx.Ev);
            if (ev.NewState == null) return false;
            if (ev.NewState.Step < StateChangeEvent.Step_AfterEnd) return false;
            return true;
        }
    }
}
