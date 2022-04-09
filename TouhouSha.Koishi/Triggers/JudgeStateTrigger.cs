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
    public class JudgeStateTrigger : Trigger, ITriggerInAnyState
    {
        public const string BeHappy = "乐死了";
        public const string BeHungry = "饿死了";

        public JudgeStateTrigger()
        {
            Condition = new JudgeStateCondition();
        }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            switch (state?.KeyName)
            {
                case State.Judge:
                    {
                        Zone judgezone = state.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
                        Card light = null;
                        Card happy = null;
                        Card hunger = null;
                        List<Card> delays = new List<Card>();
                        if (judgezone == null) break;
                        foreach (Card card in judgezone.Cards)
                        {
                            switch (card.KeyName)
                            {
                                case LightCard.Normal: light = card; break;
                                case HappyCard.Normal: happy = card; break;
                                case HungerCard.Normal: hunger = card; break;
                            }
                        }
                        if (light != null) delays.Add(light);
                        if (happy != null) delays.Add(happy);
                        if (hunger != null) delays.Add(hunger);
                        foreach (Card delay in delays)
                        {
                            DelaySpellEvent ev_card = new DelaySpellEvent();
                            ev_card.Target = state.Owner;
                            ev_card.Card = delay;
                            ctx.World.InvokeEvent(ev_card);
                            if (ev_card.Cancel) continue;
                            JudgeEvent ev_judge = new JudgeEvent();
                            ev_judge.Reason = ev_card;
                            ev_judge.JudgeTarget = ev_card.Target;
                            ev_judge.JudgeNumber = 1;
                            ev_judge.JudgeCards.Clear();
                            ctx.World.InvokeEvent(ev_judge);
                        }
                    }
                    break;
                case State.Draw:
                    state.Step = StateChangeEvent.Step_AfterEnd;
                    break;
                case State.UseCard:
                    state.Step = StateChangeEvent.Step_AfterEnd;
                    break;
            }
            base.Action(ctx);
        }
    }
    
    public class JudgeStateCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state?.Ev == null) return false;
            if (state.Owner == null) return false;
            switch (state?.KeyName)
            {
                case State.Judge:
                    if (state.Step != StateChangeEvent.Step_Start) return false;
                    break;
                case State.Draw:
                    if (state.Step != StateChangeEvent.Step_BeforeStart) return false;
                    if (state.Ev.GetValue(JudgeStateTrigger.BeHungry) != 1) return false;
                    break;
                case State.UseCard:
                    if (state.Step != StateChangeEvent.Step_BeforeStart) return false;
                    if (state.Ev.GetValue(JudgeStateTrigger.BeHappy) != 1) return false;
                    break;
                default:
                    return false;
            }
            return true;
        }
    }

    public class DelaySpellCardEffectTrigger : Trigger, ITriggerInState
    {
        public DelaySpellCardEffectTrigger()
        {
            Condition = new DelaySpellCardEffectCondition();
        }

        string ITriggerInState.StateKeyName => State.Handle;

        int ITriggerInState.StateStep => StateChangeEvent.Step_End;

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            JudgeEvent ev_judge = (JudgeEvent)(state.Ev);
            DelaySpellEvent ev_card = (DelaySpellEvent)(ev_judge.Reason);
            switch (ev_card?.Card?.KeyName)
            {
                case LightCard.Normal:
                    {
                        Card light = ev_card.Card;
                        bool damaged = false;
                        if (ev_judge.JudgeCards.Count() > 0
                         && ev_judge.JudgeCards[0].CardColor?.SeemAs(Enum_CardColor.Spade) == true
                         && ev_judge.JudgeCards[0].CardPoint >= 2
                         && ev_judge.JudgeCards[0].CardPoint <= 9)
                        {
                            damaged = true;
                            ctx.World.Damage(null, state.Owner, 3, DamageEvent.Thunder, ev_card);
                        }
                        if (!damaged)
                        {
                            Player next = ctx.World.GetNextAlivePlayer(state.Owner);
                            Zone judgezone = next.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
                            if (judgezone != null)
                                ctx.World.MoveCard(state.Owner, light, judgezone, ev_card);
                            else
                                damaged = true;
                        }
                        if (damaged)
                        {
                            Zone discard = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
                            if (discard != null)
                                ctx.World.MoveCard(state.Owner, light, discard, ev_card);
                        }
                        break;
                    }
                case HappyCard.Normal:
                    {
                        Card happy = ev_card.Card;
                        if (ev_judge.JudgeCards.Count() > 0
                         && ev_judge.JudgeCards[0].CardColor?.SeemAs(Enum_CardColor.Heart) != true)
                            state.Ev.SetValue(JudgeStateTrigger.BeHappy, 1);
                        Zone discard = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
                        if (discard != null
                         && happy.Zone?.Owner == state.Owner
                         && happy.Zone?.KeyName?.Equals(Zone.Judge) == true)
                            ctx.World.MoveCard(state.Owner, happy, discard, ev_card);
                        break;
                    }
                case HungerCard.Normal:
                    {
                        Card hunger = ev_card.Card;
                        if (ev_judge.JudgeCards.Count() > 0
                         && ev_judge.JudgeCards[0].CardColor?.SeemAs(Enum_CardColor.Club) != true)
                            state.Ev.SetValue(JudgeStateTrigger.BeHungry, 1);
                        Zone discard = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
                        if (discard != null
                         && hunger.Zone?.Owner == state.Owner
                         && hunger.Zone?.KeyName?.Equals(Zone.Judge) == true)
                            ctx.World.MoveCard(state.Owner, hunger, discard, ev_card);
                        break;
                    }
            }
        }
    }

    public class DelaySpellCardEffectCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is JudgeEvent)) return false;
            JudgeEvent ev_judge = (JudgeEvent)(state.Ev);
            if (ev_judge.Cancel) return false;
            if (!(ev_judge.Reason is DelaySpellEvent)) return false;
            return true;
        }
    }
}
