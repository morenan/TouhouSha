using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Filters;
using TouhouSha.Reimu.Charactors.Moriya;

namespace TouhouSha.Reimu.Filters
{
    public class PointBattleTargetSelector2 : OverridePlayerFilter
    {
        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            if (selecteds.Count() >= 1) return false;
            if (OldFilter is PlayerFilterFromSkill)
            {
                PlayerFilterFromSkill fromskill = (PlayerFilterFromSkill)OldFilter;
                if (want == fromskill.Skill.Owner) return false;
            }
            if (want.Skills.FirstOrDefault(_skill => _skill is Suwako_Skill_1) != null)
                return true;
            else if (want.Skills.FirstOrDefault(_skill => _skill is Tensoku_Skill_0) != null)
                return Tensoku.GetPowerZone(want).Cards.Count() > 0;
            else
                return OldFilter?.CanSelect(ctx, selecteds, want) ?? false;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return OldFilter?.Fulfill(ctx, selecteds) ?? false;
        }
    }
}
