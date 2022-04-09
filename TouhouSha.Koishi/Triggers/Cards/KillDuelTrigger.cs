using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.AIs;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Cards;


namespace TouhouSha.Koishi.Triggers
{
    public class KillDuelTrigger : Trigger, ITriggerInState
    {
        public const string DefaultKeyName = "出杀响应决斗";
        public const string HasKilled = "杀了";

        public KillDuelTrigger()
        {
            KeyName = DefaultKeyName;
            Condition = new KillDuelCondition();
        }

        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(state.Ev);
            ctx.World.RequireCard(MissCard.Normal, "请打出一张杀响应决斗。",
                state.Owner,
                new KillDuelCardFilter(1, 1),
                true, 10,
                (cards) =>
                {
                    CardEvent ev_handle = new CardEvent();
                    ev_handle.Reason = ev;
                    ev_handle.Card = cards.FirstOrDefault();
                    ev_handle.Source = state.Owner;
                    ev_handle.Targets.Clear();
                    ctx.World.InvokeEvent(ev_handle);
                    if (ev_handle.Cancel) return;
                    state.SetValue(HasKilled, 1);
                },
                () => 
                {
                    state.SetValue(HasKilled, 0);
                });
        }
    }

    public class KillDuelCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (ev.Card.KeyName?.Equals(DuelCard.Normal) != true) return false;
            return true;
        }
    }

    /// <summary>
    /// 触发器【出杀响应决斗二次判定】
    /// </summary>
    public class KillDuelTrigger2 : Trigger, ITriggerInState
    {
        public const string DefaultKeyName = "出杀响应决斗二次判定";

        public KillDuelTrigger2()
        {
            KeyName = DefaultKeyName;
            Condition = new KillDuelCondition2();
        }
        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(state.Ev);
            int targetindex = ev.GetValue("TargetIndex");
            Player another = ev.Source != state.Owner ? ev.Source : ev.Targets[targetindex];
            // 出过了【杀】，切换对方响应。
            if (state.GetValue(KillDuelTrigger.HasKilled) == 1)
            {
                State nextstate = new State();
                nextstate.Ev = state.Ev;
                nextstate.KeyName = State.Handle;
                nextstate.Owner = another;
                nextstate.Step = 0;
                ctx.World.EnterStateAfterState(nextstate, state);
            }
            // 没有则受到对方的一点伤害。
            else
            {
                DamageEvent damageevent = ctx.World.GetDamageEvent(another, state.Owner, 1, DamageEvent.Normal, ev);
                ctx.World.InvokeEventAfterState(damageevent, state);
            }
        }
    }

    public class KillDuelCondition2 : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (ev.Card.KeyName?.Equals(DuelCard.Normal) != true) return false;
            return true;
        }
    }

    public class KillDuelCardFilter : CardFilter, ICardFilterRequiredCardTypes
    {
        public KillDuelCardFilter(int _mincount, int _maxcount)
        {
            this.mincount = _maxcount;
            this.maxcount = _maxcount;
        }

        private int mincount = 1;
        private int maxcount = 1;

        IEnumerable<string> ICardFilterRequiredCardTypes.RequiredCardTypes
        {
            get { return new string[] { KillCard.Normal, KillCard.Fire, KillCard.Thunder }; }
        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() + 1 > maxcount) return false;
            if (want.Zone?.KeyName?.Equals(Zone.Hand) != true) return false;
            if (!KillCard.IsKill(want)) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return (selecteds.Count() >= mincount);
        }

        public override CardFilterWorth GetWorthAI()
        {
            return new KillDuelCardWorthAI();
        }
    }

    public class KillDuelCardWorthAI : CardFilterWorth
    {
        public override double GetWorthNo(Context ctx, SelectCardBoardCore core)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return 0.0d;
            CardEvent ev = (CardEvent)(state.Ev);
            int damagevalue = ctx.World.CalculateValue(ctx, ev, CardEvent.DamageValue);
            return -AITools.WorthAcc.GetWorthPerHp(ctx, state.Owner, state.Owner) * damagevalue;
        }

        public override double GetWorth(Context ctx, SelectCardBoardCore core, IEnumerable<Card> selecteds, Card want)
        {
            return -AITools.WorthAcc.GetWorth(ctx, want.Owner, want);
        }
    }
}
