using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Cards;

namespace TouhouSha.Reimu.Charactors.Homos
{
    /// <summary>
    /// 角色【露米娅】
    /// </summary>
    /// <remarks>
    /// HP:4 红魔
    /// 【宵暗】当你或者你距离为1以内的角色成为你以外的角色使用的黑色卡的目标时，你可以摸一张牌并取消之。
    /// </remarks>
    public class Rumiya : Charactor
    {
        public Rumiya()
        {
            MaxHP = 4;
            HP = 4;
            Country = Remilia.CountryNameOfLeader;
            Skills.Add(new Rumiya_Skill_0());
        }

        public override Charactor Clone()
        {
            return new Rumiya();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "露米娅";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Rumiya");
            info.AbilityRadar = new AbilityRadar() { Attack = 1, Defence = 5, Control = 4, Auxiliary = 5, LastStages = 5, Difficulty = 5 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }
    }
    
    public class Rumiya_Skill_0 : Skill
    {
        public Rumiya_Skill_0()
        {
            Triggers.Add(new Rumiya_Skill_0_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Rumiya_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Rumiya_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "宵暗",
                Description = "当你或者你距离为1以内的角色成为你以外的角色使用的黑色卡的目标时，你可以摸一张牌并取消之。",
            };
        }
    }
   
    public class Rumiya_Skill_0_Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Rumiya_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Rumiya_Skill_0_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName => State.Handle;

        int ITriggerInState.StateStep => StateChangeEvent.Step_BeforeStart;

        string ITriggerAsk.Message => "是否要发动【宵暗】？";
       
        Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(state.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            ctx.World.DrawCard(skill.Owner, 1, ev_skill);
            state.Step = StateChangeEvent.Step_AfterEnd;
        }
    }

    public class Rumiya_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Rumiya_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner == null) return false;
            if (!ctx.World.IsInDistance(ctx, skill.Owner, state.Owner)) return false;
            if (!(state.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (ev.Reason is CardEvent) return false; // 当牌事件的原因也是牌事件，是一张牌响应另一张牌，不是主动使用牌。
            if (ev.Source != skill.Owner) return false;
            if (ev.Card?.CardColor?.SeemAs(Enum_CardColor.Black) != true) return false;
            return true;
        }

    }



}
