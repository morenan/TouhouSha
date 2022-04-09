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
    /// <summary>
    /// 出牌阶段，响应出牌方各种事件的触发器。
    /// </summary>
    public class UseCardStateTrigger : Trigger, ITriggerInState
    {
        public const string DefaultKeyName = "出牌阶段动作";

        public UseCardStateTrigger()
        {
            KeyName = DefaultKeyName;
            Condition = new UseCardStateCondition();
        }

        string ITriggerInState.StateKeyName => State.UseCard;
        int ITriggerInState.StateStep => StateChangeEvent.Step_Start;

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            Player owner = state.Owner;
            Event ev = null;
            while (!(ev is LeaveUseCardStateEvent))
            {
                ev = ctx.World.QuestEventInUseCardState(owner);
                ctx.World.InvokeEvent(ev);
            }
        }
    }

    public class UseCardStateCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Owner == null) return false;
            return true;
        }
    }
}
