using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Cards;

namespace TouhouSha.Koishi.Triggers
{
    public class EventEnterStateTrigger : Trigger, ITriggerInAnyEvent
    {
        public const string DefaultKeyName = "事件进阶段";
        
        public EventEnterStateTrigger()
        {
            KeyName = DefaultKeyName;
        }

        public override void Action(Context ctx)
        {
            if (ctx.Ev?.Cancel == true) return;
            if (ctx.Ev is DamageEvent)
            {
                DamageEvent ev = (DamageEvent)(ctx.Ev);
                State state = new State();
                state.Ev = ev;
                state.KeyName = State.Damaging;
                state.Owner = ev.Target;
                state.Step = 0;
                ctx.World.EnterStateAfterEvent(state, ev);
            }
            else if (ctx.Ev is HealEvent)
            {
                HealEvent ev = (HealEvent)(ctx.Ev);
                State state = new State();
                state.Ev = ev;
                state.KeyName = State.Healing;
                state.Owner = ev.Target;
                state.Step = 0;
                ctx.World.EnterStateAfterEvent(state, ev);
            }
            else if (ctx.Ev is JudgeEvent)
            {
                JudgeEvent ev = (JudgeEvent)(ctx.Ev);
                State state = new State();
                state.Ev = ev;
                state.KeyName = State.Handle;
                state.Owner = ev.JudgeTarget;
                state.Step = 0;
                ctx.World.EnterStateAfterEvent(state, ev);
            }
        }
    }


    public class EventEnterStateCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            return true;
        }
    }


}
