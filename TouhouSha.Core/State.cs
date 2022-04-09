using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TouhouSha.Core
{
    public class State : ShaObject
    {
        public const string GameStart = "游戏开始";
        public const string GameEnd = "游戏结束";
        public const string Begin = "开始阶段";
        public const string Prepare = "准备阶段";
        public const string Judge = "判定阶段";
        public const string Draw = "摸牌阶段";
        public const string UseCard = "出牌阶段";
        public const string Discard = "弃牌阶段";
        public const string End = "结束阶段";
        public const string Damaging = "伤害结算前";
        public const string Damaged = "伤害阶段";
        public const string Healing = "回复结算前";
        public const string Healed = "回复阶段";
        public const string Dying = "濒死阶段";
        public const string Die = "死亡阶段";
        public const string Handle = "响应阶段";

        public override string ToString()
        {
            return String.Format("State:{0}", KeyName);
        }

        private Event ev;
        public Event Ev
        {
            get { return this.ev; }
            set { this.ev = value; }
        }

        private Player owner;
        public Player Owner
        {
            get { return this.owner; }
            set { this.owner = value; }
        }

        private int step;
        public int Step
        {
            get { return this.step; }
            set { this.step = value; }
        }
        
        public State Clone()
        {
            State newstate = new State();
            newstate.ev = ev;
            newstate.owner = owner;
            newstate.step = step;
            return newstate;
        }
    }
    

}
