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
    /// 触发器【摸牌阶段摸牌】
    /// </summary>
    public class DrawTrigger : Trigger, ITriggerInState
    {
        public const string ExtraDrawNumber = "额外的摸牌数量";
        
        public DrawTrigger()
        {
            Condition = new DrawTriggerCondition();
        }

        string ITriggerInState.StateKeyName { get => State.Draw; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return;
            if (state.Owner == null) return;
            // 摸牌阶段摸牌事件，能够知道是摸牌阶段摸的牌。
            DrawStateEvent ev = new DrawStateEvent();
            ev.Source = state.Owner;
            // 摸牌数量=2+额外摸牌数量
            int drawnumber = 2;
            drawnumber += ctx.World.CalculateValue(ctx, state.Owner, ExtraDrawNumber);
            // 进行摸牌
            ctx.World.DrawCard(state.Owner, drawnumber, ev);
        }
    }

    public class DrawTriggerCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            return true;
        }
    }
}
