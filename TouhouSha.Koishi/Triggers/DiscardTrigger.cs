using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;

namespace TouhouSha.Koishi.Triggers
{
    /// <summary>
    /// 触发器【弃牌阶段弃牌】
    /// </summary>
    public class DiscardTrigger : Trigger, ITriggerInState
    {
        public const string ExtraHandCapacity = "额外的手牌上限";

        public DiscardTrigger()
        {
            Condition = new DiscardTriggerCondition();
        }

        string ITriggerInState.StateKeyName { get => State.Discard; }
        
        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return;
            if (state.Owner == null) return;
            // 手牌上限=体力值+额外结算。
            int cardcapacity = state.Owner.HP;
            cardcapacity += ctx.World.CalculateValue(ctx, state.Owner, ExtraHandCapacity);
            // 有手牌区才能弃牌。
            Zone handzone = state.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return;
            // 不超过上限不用弃牌。
            if (cardcapacity >= handzone.Cards.Count()) return;
            // 弃牌阶段弃牌事件，能够知道是弃牌阶段丢弃的牌。
            DiscardStateEvent ev = new DiscardStateEvent();
            ev.Source = state.Owner; 
            // 由当前玩家选择手牌来丢弃。
            ctx.World.DiscardCard(state.Owner, handzone.Cards.Count() - cardcapacity, ev, false);
        }
    }

    public class DiscardTriggerCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            return true;
        }
    }
}
