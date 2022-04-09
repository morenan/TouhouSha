using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.AIs
{
    /// <summary>
    /// 计算主动发动一个技能的收益，目标和选择卡独立计算收益。
    /// </summary>
    public abstract class InitativeSkillWorth
    {
        /// <summary>
        /// 默认的估价和选择目标和消耗卡的算法。假设目标和消耗卡不相关（比如不是刘备那种给牌）。
        /// </summary>
        /// <param name="worthgetter"></param>
        /// <param name="ctx"></param>
        /// <param name="skill"></param>
        /// <param name="user"></param>
        /// <param name="targets"></param>
        /// <param name="costs"></param>
        /// <returns></returns>
        public virtual double DefaultAlgorithm(Context ctx, Skill skill, Player user
            ,out List<Player> _targets_out, out List<Card> _costs_out)
        {
            PlayerFilter targetfilter = ctx.World.TryReplaceNewPlayerFilter(((ISkillInitative)skill).TargetFilter, null);
            CardFilter costfilter = ctx.World.TryReplaceNewCardFilter(((ISkillInitative)skill).CostFilter, null);
            List<Player> targets = new List<Player>();
            List<Card> costs = new List<Card>();
            double worth = 0;
            worth += AITools.StepOptimizeAlgorithm(ctx, targetfilter, targets, target => GetWorthSelect(ctx, skill, user, targets, target));
            worth += AITools.StepOptimizeAlgorithm(ctx, user, costfilter, costs, cost => GetWorthSelect(ctx, skill, user, costs, cost));
            _targets_out = targets;
            _costs_out = costs;
            return worth;
        }

        private AITools aitools;
        public AITools AITools
        {
            get { return this.aitools; }
            set { this.aitools = value; }
        }

        public virtual double GetWorth(Context ctx, Skill skill, Player user)
        {
            List<Player> targets = null;
            List<Card> costs = null;
            return DefaultAlgorithm(ctx, skill, user, out targets, out costs);
        }
        
        public abstract double GetWorthSelect(Context ctx, Skill skill, Player user, IEnumerable<Player> selecteds, Player want);
        public abstract double GetWorthSelect(Context ctx, Skill skill, Player user, IEnumerable<Card> selecteds, Card want);
    }
}
