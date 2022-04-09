using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    /// <summary>
    /// 出牌阶段发起这个事件，结束此阶段。
    /// </summary>
    public class LeaveUseCardStateEvent : Event
    {
        public const string DefaultKeyName = "结束出牌阶段";

        public LeaveUseCardStateEvent(State _state)
        {
            KeyName = DefaultKeyName;
            this.state = _state;
        }

        private State state;
        public State State
        {
            get { return this.state; }
        }

        public override Player GetHandleStarter()
        {
            return state.Owner;
        }

        public override bool IsStopHandle()
        {
            if (state?.Owner == null) return true;
            if (!state.Owner.IsAlive) return true;
            return false;
        }
    }
}
