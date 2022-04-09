using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;

namespace TouhouSha.Koishi.Triggers
{
    public class InitativeSkillTrigger : Trigger, ITriggerInEvent
    {
        public InitativeSkillTrigger()
        {
            Condition = new InitativeSkillCondition();
        }

        string ITriggerInEvent.EventKeyName => SkillInitativeEvent.DefaultKeyName;

        public override void Action(Context ctx)
        {
            SkillInitativeEvent ev = (SkillInitativeEvent)(ctx.Ev);
            ISkillInitative skill = (ISkillInitative)(ev.Skill);
            skill.Action(ctx, ev.User, ev.Targets, ev.Costs);
        }
    }

    public class InitativeSkillCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is SkillInitativeEvent)) return false;
            SkillInitativeEvent ev = (SkillInitativeEvent)(ctx.Ev);
            if (!(ev.Skill is ISkillInitative)) return false;
            return true;
        }
    }
}
