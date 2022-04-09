using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.AIs
{
    /// <summary>
    /// 计算主动发动一个技能的收益，并自动选择对象和卡。
    /// </summary>
    public abstract class InitativeSkillAuto
    {
        private AITools aitools;
        public AITools AITools
        {
            get { return this.aitools; }
            set { this.aitools = value; }
        }
        public abstract double GetWorth(Context ctx, Skill skill, Player user);
        public abstract bool GetSelection(Context ctx, Skill skill, Player user, List<Player> targets, List<Card> costs);
    }
}
