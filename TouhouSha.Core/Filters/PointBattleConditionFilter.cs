using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Filters
{
    public class PointBattleConditionFilter : ConditionFilterFromSkill
    {
        public PointBattleConditionFilter(Skill _skill) : base(_skill)
        {
        }
        
        public override bool Accept(Context ctx)
        {
            Zone handzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return false;
            if (handzone.Cards.Count() == 0) return false;
            return true;
        }
    }

    public class PointBattleTargetSelector : PlayerFilterFromSkill
    {
        public PointBattleTargetSelector(Skill _skill) : base(_skill)
        {
        }
        
        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            if (selecteds.Count() >= 1) return false;
            if (want == skill.Owner) return false;
            Zone handzone = want.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return false;
            if (handzone.Cards.Count() == 0) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() >= 1;
        }
    }
}
