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
    public class StateChangeTrigger : Trigger, ITriggerInAnyState
    {
        public const string DefaultKeyName = "阶段更变";

        static public string[] StateArray = new string[]
        {
            State.Begin,
            State.Prepare,
            State.Judge,
            State.Draw,
            State.UseCard,
            State.Discard,
            State.End,
        };

        public StateChangeTrigger()
        {
            KeyName = DefaultKeyName;
            Condition = new StateChangeCondition();
        }
        
        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            Player stateowner = state?.Owner;
            if (state.Ev is GiveExtraStateEvent) return;
            int i = Array.IndexOf(StateArray, state.KeyName);
            if (i >= 0)
            {
                if (i + 1 < StateArray.Length)
                {
                    State nextstate = new State();
                    nextstate.Ev = state.Ev;
                    nextstate.Owner = state.Owner;
                    nextstate.KeyName = StateArray[i + 1];
                    nextstate.Step = 0;
                    switch (nextstate.KeyName)
                    {
                        case State.UseCard:
                            state.SetValue(KillCard.KillMaxUse, 1);
                            state.SetValue(LiqureCard.BullMaxUse, 1);
                            break;
                    }
                    ctx.World.EnterStateAfterState(nextstate, state);
                }
                else if (state.Ev is GiveExtraPhaseEvent)
                {
                    return;
                }
                else
                {
                    PhaseEvent ev_phase = new PhaseEvent();
                    State nextstate = new State();
                    nextstate.Ev = ev_phase;
                    nextstate.Owner = ctx.World.GetNextAlivePlayer(stateowner);
                    nextstate.KeyName = State.Begin;
                    nextstate.Step = 0;
                    ev_phase.StartState = nextstate;
                    ctx.World.EnterStateAfterState(nextstate, state);
                }
                return;
            }
            switch (state.KeyName)
            {
                case State.Dying:
                    {
                        if (stateowner.HP < 0)
                            ctx.World.EnterStateAfterState(state.New(State.Die), state);
                        else if (state.Ev is DamageEvent)
                            ctx.World.EnterStateAfterState(state.New(State.Damaged), state);
                        else if (state.Ev is HealEvent)
                            ctx.World.EnterStateAfterState(state.New(State.Healed), state);
                        else
                            return;
                    }
                    break;
                case State.Die:
                    ctx.World.Die(stateowner, state.Ev);
                    break;
                case State.Damaging:
                    if (state.Ev is DamageEvent)
                    {
                        DamageEvent ev = (DamageEvent)(state.Ev);
                        stateowner.SetValue("HP", Math.Min(stateowner.HP - ev.DamageValue, stateowner.MaxHP), ev);
                        if (stateowner.HP < 0)
                            ctx.World.EnterStateAfterState(state.New(State.Dying), state);
                        else
                            ctx.World.EnterStateAfterState(state.New(State.Damaged), state);
                    }
                    break;
                case State.Healing:
                    if (state.Ev is HealEvent)
                    {
                        HealEvent ev = (HealEvent)(state.Ev);
                        stateowner.SetValue("HP", Math.Min(stateowner.HP + ev.HealValue, stateowner.MaxHP), ev);
                        if (stateowner.HP < 0)
                            ctx.World.EnterStateAfterState(state.New(State.Dying), state);
                        else
                            ctx.World.EnterStateAfterState(state.New(State.Healed), state);
                    }
                    break;
            }
        }
    }

    public class StateChangeCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Step < StateChangeEvent.Step_AfterEnd) return false;
            return true;
        }
    }
}
