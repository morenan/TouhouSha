using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;

namespace TouhouSha.Koishi.Triggers
{
    public class SkillOnceResetTrigger : SkillTrigger, ITriggerInEvent
    {
        public SkillOnceResetTrigger(Skill _skill, string _key_used) : base(_skill)
        {
            this.key_used = _key_used;
            Condition = new SkillOnceResetCondition(skill, key_used);
        }

        string ITriggerInEvent.EventKeyName { get => SkillOnceResetEvent.DefaultKeyName; }

        private string key_used;
        public string Key_Used { get { return this.key_used; } }

        public override void Action(Context ctx)
        {
            skill.SetValue(key_used, 0);
        }
    }

    public class SkillOnceResetCondition : ConditionFilterFromSkill
    {
        public SkillOnceResetCondition(Skill _skill, string _key_used) : base(_skill)
        {
            this.key_used = _key_used;
        }

        private string key_used;
        public string Key_Used { get { return this.key_used; } }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is SkillOnceResetEvent)) return false;
            SkillOnceResetEvent ev = (SkillOnceResetEvent)(ctx.Ev);
            if (ev.Skill != skill) return false;
            if (skill.GetValue(key_used) != 1) return false;
            return true;
        }
    }
    
}
