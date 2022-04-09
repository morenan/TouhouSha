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
    /// <summary>
    /// 触发器【出闪响应杀】
    /// </summary>
    public class MissKillTrigger : Trigger, ITriggerInState
    {
        public const string DefaultKeyName = "出闪响应杀";
        public const string MissMinimum = "最小出闪数量";
        public const string MissMaximum = "最大出闪数量";
        public const string HasMissed = "闪了";

        public MissKillTrigger()
        {
            KeyName = DefaultKeyName;
            Condition = new MissKillCondition();
        }

        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }
        
        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(state.Ev);
            // 出闪的最小数量和最大数量，默认都是1，如果被吕布杀就变成了2。
            ev.SetValue(MissMinimum, 1);
            ev.SetValue(MissMaximum, 1);
            int minvalue = ctx.World.CalculateValue(ctx, ev, MissMinimum);
            int maxvalue = ctx.World.CalculateValue(ctx, ev, MissMaximum);
            // 由被杀方选择闪打出。
            ctx.World.RequireCard(MissCard.Normal, String.Format("{0}打出了杀，请打出{1}张闪响应。", ev.Source.Name, minvalue), 
                state.Owner, 
                new MissKillCardFilter(minvalue, maxvalue), 
                true, 10,
                // 选择打出的【闪】后。
                (cards) => 
                {
                    // 触发出闪事件。
                    CardEvent ev_handle = new CardEvent();
                    ev_handle.Reason = ev;
                    ev_handle.Card = cards.FirstOrDefault();
                    ev_handle.Source = state.Owner;
                    ev_handle.Targets.Clear();
                    ctx.World.InvokeEvent(ev_handle);
                    if (ev_handle.Cancel) return;
                    // 设置已经闪避的标记。
                    state.SetValue(HasMissed, 1);
                },
                // 不选择出【闪】。
                () => 
                {
                    // 不设置已经闪避的标记。
                    state.SetValue(HasMissed, 0);
                });
        }
    }

    public class MissKillCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return false;
            if (state.GetValue(MissKillTrigger.HasMissed) == 1) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (!KillCard.IsKill(ev.Card)) return false;
            return true;
        }
    }

    /// <summary>
    /// 触发器【没有出闪响应杀的后果】
    /// </summary>
    public class NotMissKillTrigger : Trigger, ITriggerInState
    {
        public const string DefaultKeyName = "没有出闪响应杀的后果";
        
        public NotMissKillTrigger()
        {
            KeyName = DefaultKeyName;
            Condition = new NotMissKillCondition();
        }
        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(state.Ev);
            int damagevalue = ctx.World.CalculateValue(ctx, ev, CardEvent.DamageValue);
            string damagetype = DamageEvent.Normal;
            switch (ev.Card.KeyName)
            {
                case KillCard.Fire: damagetype = DamageEvent.Fire; break;
                case KillCard.Thunder: damagetype = DamageEvent.Thunder; break;
            }
            DamageEvent damageevent = ctx.World.GetDamageEvent(ev.Source, state.Owner, damagevalue, damagetype, ev);
            ctx.World.InvokeEventAfterState(damageevent, state);
        }
    }

    public class NotMissKillCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return false;
            if (state.GetValue(MissKillTrigger.HasMissed) == 1) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (!KillCard.IsKill(ev.Card)) return false;
            return true;
        }
    }

    public class MissKillCardFilter : CardFilter, ICardFilterRequiredCardTypes
    {
        public MissKillCardFilter(int _mincount, int _maxcount)
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
            return new MissKillCardWorthAI();
        }
    }

    public class MissKillCardWorthAI : CardFilterWorth
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
