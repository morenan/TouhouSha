using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Cards;
using TouhouSha.Reimu.Charactors.SelfCrafts;

namespace TouhouSha.Reimu.AIs
{
    public class PointBattleCardWorth : TouhouSha.Koishi.AIs.PointBattleCardWorth
    {
        public PointBattleCardWorth(PointBattleEventBase _battleevent) : base(_battleevent)
        {
            
        }

        public override double GetWorthWin(Context ctx, Player controller)
        {
            double baseworth = base.GetWorthWin(ctx, controller);
            if (BattleEvent?.Reason is SkillEvent)
            {
                SkillEvent ev_skill = (SkillEvent)(BattleEvent.Reason);
                switch (ev_skill.Skill?.KeyName)
                {
                    case Sanae.Skill_1.DefaultKeyName: return baseworth + 4d;
                }
            }
            return baseworth;
        }

        public override double GetWorthLose(Context ctx, Player controller)
        {
            double baseworth = base.GetWorthLose(ctx, controller);
            return baseworth;
        }
    }
}
