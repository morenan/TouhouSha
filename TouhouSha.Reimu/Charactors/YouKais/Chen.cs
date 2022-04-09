using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.YouKais
{
    /// <summary>
    /// 角色【橙】
    /// </summary>
    /// <remarks>
    /// 妖怪 4勾玉
    /// 【二尾】：转化技：
    ///     阳：当你使用或打出一张红色牌时，你进行一次判定，若为红色，则你回复一点体力，若为黑色，你摸一张牌。
    ///     阴：当你使用或打出一张黑色牌时，你进行一次判定，若为黑色，则你选择一名角色造成一点伤害，若为红色，你摸一张牌。
    /// </remarks>
    public class Chen : Charactor
    {
        public Chen()
        {
            MaxHP = 4;
            HP = 4;
            Country = Yucari.CountryNameOfLeader;
            Skills.Add(new Chen_Skill_0());
        }

        public override Charactor Clone()
        {
            return new Chen();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "橙";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Chen");
            info.AbilityRadar = new AbilityRadar() { Attack = 3.5, Defence = 4, Control = 1, Auxiliary = 1, LastStages = 2, Difficulty = 5 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }
    }

    public class Chen_Skill_0 : Skill
    {
        public const string Dark = "阴";

        public Chen_Skill_0()
        {
            Triggers.Add(new Chen_Skill_0_Trigger_0(this));
            Triggers.Add(new Chen_Skill_0_Trigger_1(this));
        }

        public override Skill Clone()
        {
            return new Chen_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Chen_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "二尾",
                Description = "转化技：\n" +
                    "\t阳：当你使用或打出一张红色牌时，你可以进行一次判定，若为红色，则你回复一点体力，若为黑色，你摸一张牌。\n" +
                    "\t阴：当你使用或打出一张黑色牌时，你可以进行一次判定，若为黑色，则你选择一名角色造成一点伤害，若为红色，你摸一张牌。"
            };
        }
    }

    public class Chen_Skill_0_Trigger_0 : SkillTrigger, ITriggerInEvent, ITriggerAsk
    {
        public Chen_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Chen_Skill_0_Trigger_0_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => CardEvent.DefaultKeyName; }

        string ITriggerAsk.Message { get => "是否要发动【二尾】？"; }
       
        Player ITriggerAsk.GetAsked(Context ctx) { return  skill.Owner; }

        public override void Action(Context ctx)
        {
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = ctx.Ev;
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;

            JudgeEvent ev_judge = new JudgeEvent();
            ev_judge.Reason = ev_skill;
            ev_judge.JudgeTarget = skill.Owner;
            ev_judge.JudgeNumber = 1;
            ctx.World.InvokeEvent(ev_judge);
        }
    }

    public class Chen_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Chen_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(ctx.Ev);
            if (ev.Source != skill.Owner) return false;
            if (skill.GetValue(Chen_Skill_0.Dark) == 1)
                return ev.Card.CardColor?.SeemAs(Enum_CardColor.Black) == true;
            else
                return ev.Card.CardColor?.SeemAs(Enum_CardColor.Red) == true;
        }
    }

    public class Chen_Skill_0_Trigger_1 : SkillTrigger, ITriggerInState
    {
        public Chen_Skill_0_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Chen_Skill_0_Trigger_1_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get =>State.Handle; }
        
        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (!(state.Ev is JudgeEvent)) return;
            JudgeEvent ev = (JudgeEvent)(state.Ev);
            Card judge = ev.JudgeCards.FirstOrDefault();
            if (judge == null) return;
            if (skill.GetValue(Chen_Skill_0.Dark) == 1)
            {
                if (judge.CardColor?.SeemAs(Enum_CardColor.Red) == true)
                    ctx.World.Heal(skill.Owner, skill.Owner, 1, HealEvent.Normal, ev);
                else
                    ctx.World.DrawCard(skill.Owner, 1, ev);
            }
            else
            {
                if (judge.CardColor?.SeemAs(Enum_CardColor.Black) == true)
                    Damage(ctx, ev);
                else
                    ctx.World.DrawCard(skill.Owner, 1, ev);
            }
        }

        protected void Damage(Context ctx, Event reason)
        {
            ctx.World.SelectPlayer(skill.KeyName, "请选择一名角色，造成一点伤害。", skill.Owner,
                new FulfillNumberPlayerFilter(1, 1, null),
                false, 15,
                (targets) => { ctx.World.Damage(skill.Owner, targets.FirstOrDefault(), 1, DamageEvent.Normal, reason); },
                null);
        }
    }

    public class Chen_Skill_0_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Chen_Skill_0_Trigger_1_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (!(state.Ev is JudgeEvent)) return false;
            JudgeEvent ev0 = (JudgeEvent)(state.Ev);
            if (!(ev0.Reason is SkillEvent)) return false;
            SkillEvent ev1 = (SkillEvent)(ev0.Reason);
            if (ev1.Skill != skill) return false;
            return true;
        }
    }


}
