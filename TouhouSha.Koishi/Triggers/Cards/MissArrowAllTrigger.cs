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
    public class MissArrowAllTrigger : Trigger, ITriggerInState
    {
        public const string DefaultKeyName = "出闪响应万箭齐发";
        public const string HasMissed = "闪了";

        public MissArrowAllTrigger()
        {
            KeyName = DefaultKeyName;
            Condition = new MissArrowAllCondition();
        }

        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(state.Ev);
            ctx.World.RequireCard(MissCard.Normal, String.Format("{0}打出了万箭齐发，请打出一张闪响应。", ev.Source.Name),
                state.Owner,
                new MissArrowAllCardFilter(1, 1),
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
                    state.SetValue(HasMissed, 1);
                },
                () => 
                {
                    state.SetValue(HasMissed, 0);
                });
        }
    }

    public class MissArrowAllCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (ev.Card.KeyName?.Equals(ArrowAllCard.Normal) != true) return false;
            return true;
        }
    }

    /// <summary>
    /// 触发器【没有出闪响应杀的后果】
    /// </summary>
    public class NotMissArrowTrigger : Trigger, ITriggerInState
    {
        public const string DefaultKeyName = "没有出闪响应万箭齐发的后果";

        public NotMissArrowTrigger()
        {
            KeyName = DefaultKeyName;
            Condition = new NotMissArrowCondition();
        }
        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(state.Ev);
            DamageEvent damageevent = ctx.World.GetDamageEvent(ev.Source, state.Owner, 1, DamageEvent.Normal, ev);
            ctx.World.InvokeEventAfterState(damageevent, state);
        }
    }

    public class NotMissArrowCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return false;
            if (state.GetValue(MissArrowAllTrigger.HasMissed) == 1) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (ev.Card.KeyName?.Equals(ArrowAllCard.Normal) != true) return false;
            return true;
        }
    }

    public class MissArrowAllCardFilter : CardFilter, ICardFilterRequiredCardTypes
    {
        public MissArrowAllCardFilter(int _mincount, int _maxcount)
        {
            this.mincount = _maxcount;
            this.maxcount = _maxcount;
        }

        private int mincount = 1;
        private int maxcount = 1;

        IEnumerable<string> ICardFilterRequiredCardTypes.RequiredCardTypes
        {
            get { return new string[] { MissCard.Normal }; }
        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() + 1 > maxcount) return false;
            if (want.Zone?.KeyName?.Equals(Zone.Hand) != true) return false;
            if (want.KeyName?.Equals(MissCard.Normal) != true) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return (selecteds.Count() >= mincount);
        }

        public override CardFilterWorth GetWorthAI()
        {
            return new MissArrowAllCardWorthAI();
        }
    }

    public class MissArrowAllCardWorthAI : CardFilterWorth
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
