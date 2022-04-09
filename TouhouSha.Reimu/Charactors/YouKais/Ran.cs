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
    /// 角色【八云蓝】
    /// </summary>
    /// <remarks>
    /// 妖怪 4勾玉
    /// 【超算】：当一名角色进行判定前，你确认牌堆顶的5张牌，你可以获得至多一张牌，将其中任意牌以任意顺序置于牌堆顶，剩下的牌以任意顺序置于牌堆底。
    /// 【九尾】：觉醒技，你的濒死阶段。你扣除一点体力上限，将体力回复至体力上限，并获得【式神】。
    /// 【式神】：一名角色的回合开始阶段，你可以选择令其进行一次判定：
    ///     ♠：其受到一点雷电伤害。
    ///     ♣：其选择一至三张手牌交给你。
    ///     ♥：其恢复一点体力。
    ///     ♦：你将一至三张手牌交给该角色。
    /// </remarks>
    public class Ran : Charactor
    {
        public Ran()
        {
            MaxHP = 4;
            HP = 4;
            Country = Yucari.CountryNameOfLeader;
            Skills.Add(new Ran_Skill_0());
            Skills.Add(new Ran_Skill_1());
        }

        public override Charactor Clone()
        {
            return new Ran();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "八云蓝";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Ran");
            info.AbilityRadar = new AbilityRadar() { Attack = 2, Defence = 5, Control = 3, Auxiliary = 5, LastStages = 5, Difficulty = 3 };
            info.Skills.Add(new SkillInfo()
            {
                Name = "超算",
                Description = "当一名角色进行判定前，你确认牌堆顶的5张牌，你可以获得至多一张牌，将其中任意牌以任意顺序置于牌堆顶，剩下的牌以任意顺序置于牌堆底。"
            });
            SkillInfo skill_1 = new SkillInfo()
            {
                Name = "九尾",
                Description = "觉醒技，你的濒死阶段。你扣除一点体力上限，将体力回复至体力上限，并获得【式神】。"
            };
            skill_1.AttachedSkills.Add(new SkillInfo()
            {
                Name = "式神",
                Description = "一名角色的回合开始阶段，你可以选择令其进行一次判定：\n" +
                    "\t♠：其受到一点雷电伤害。\n" +
                    "\t♣：其选择一至三张手牌交给你。\n" +
                    "\t♥：其恢复一点体力。\n" +
                    "\t♦：你将一至三张手牌交给该角色。\n"
            });
            info.Skills.Add(skill_1);
            return info;
        }
    }
    
    public class Ran_Skill_0 : Skill
    {
        public Ran_Skill_0()
        {
            Triggers.Add(new Ran_Skill_0_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Ran_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Ran_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "超算",
                Description = "当一名角色进行判定前，你确认牌堆顶的5张牌，你可以获得至多一张牌，将其中任意牌以任意顺序置于牌堆顶，剩下的牌以任意顺序置于牌堆底。"
            };
        }
    }

    public class Ran_Skill_0_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public Ran_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Ran_Skill_0_Trigger_0_Condition(skill);    
        }

        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start - 1; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(state.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            DesktopCardBoardCore desktop_core = new DesktopCardBoardCore();
            List<IList<Card>> cardlists = new List<IList<Card>>();
            desktop_core.CardFilter = new FulfillNumberCardFilter(0, 1);
            desktop_core.Flag = Enum_DesktopCardBoardFlag.CannotNo;

            DesktopCardBoardZone totop = new DesktopCardBoardZone(desktop_core);
            cardlists.Add(ctx.World.GetDrawTops(5).ToArray());
            totop.Message = "牌堆顶的牌";
            desktop_core.Zones.Add(totop);

            DesktopCardBoardZone tobottom = new DesktopCardBoardZone(desktop_core);
            cardlists.Add(new List<Card>());
            tobottom.Message = "牌堆底的牌";
            desktop_core.Zones.Add(tobottom);

            DesktopCardBoardZone tohand = new DesktopCardBoardZone(desktop_core);
            cardlists.Add(new List<Card>());
            tohand.Message = "获得的牌";
            tohand.Flag = Enum_DesktopZoneFlag.AsSelected;
            desktop_core.Zones.Add(tohand);

            ctx.World.ShowDesktop(skill.Owner, desktop_core, cardlists, true, ev_skill);
            if (desktop_core.IsYes)
            {
                Zone drawzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Draw) == true);
                if (drawzone == null) return;
                Zone handzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                if (handzone == null) return;
                ctx.World.MoveCards(skill.Owner, totop.Cards, drawzone, ev_skill);
                ctx.World.MoveCards(skill.Owner, tobottom.Cards, drawzone, ev_skill, Enum_MoveCardFlag.MoveToFirst);
                ctx.World.MoveCards(skill.Owner, tohand.Cards, handzone, ev_skill);
            }   
        }
    }

    public class Ran_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Ran_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (!(state.Ev is JudgeEvent)) return false;
            return true;
        }
    }

    public class Ran_Skill_1 : Skill
    {
        public Ran_Skill_1()
        {
            IsOnce = true;
            IsLocked = true;
            Triggers.Add(new Ran_Skill_1_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Ran_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Ran_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "九尾",
                Description = "觉醒技，你的濒死阶段。你扣除一点体力上限，将体力回复至体力上限，并获得【式神】。"
            };
        }
    }
    
    public class Ran_Skill_1_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public Ran_Skill_1_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Ran_Skill_1_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Dying; }
        
        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        public override void Action(Context ctx)
        {
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            ctx.World.Heal(skill.Owner, skill.Owner, skill.Owner.MaxHP - skill.Owner.HP, HealEvent.Normal, ev_skill);
            Player owner = skill.Owner;
            owner.Skills.Remove(skill);
            owner.Skills.Add(new Ran_Skill_2());
        }
    }

    public class Ran_Skill_1_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Ran_Skill_1_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            if (state.Owner.HP > 0) return false;
            return true;
        }

    }

    public class Ran_Skill_2 : Skill
    {
        public Ran_Skill_2()
        {
            Triggers.Add(new Ran_Skill_2_Trigger_0(this));
            Triggers.Add(new Ran_Skill_2_Trigger_1(this));
        }

        public override Skill Clone()
        {
            return new Ran_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new Ran_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "式神",
                Description = "一名角色的回合开始阶段，你可以选择令其进行一次判定：\n" +
                    "\t\t♠：其受到一点雷电伤害。\n" +
                    "\t\t♣：其选择一至三张手牌交给你。\n" +
                    "\t\t♥：其恢复一点体力。\n" +
                    "\t\t♦：你将一至三张手牌交给该角色。\n"
            };
        }
    }

    public class Ran_Skill_2_Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Ran_Skill_2_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Ran_Skill_2_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Begin; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        string ITriggerAsk.Message { get => "是否要发动【式神】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return  skill.Owner; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return;
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(state.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            JudgeEvent ev_judge = new JudgeEvent();
            ev_judge.Reason = ev_skill;
            ev_judge.JudgeTarget = state.Owner;
            ev_judge.JudgeNumber = 1;
            ctx.World.InvokeEvent(ev_judge);
        }
    }

    public class Ran_Skill_2_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Ran_Skill_2_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            return true;
        }

    }


    public class Ran_Skill_2_Trigger_1 : SkillTrigger, ITriggerInState
    {
        public Ran_Skill_2_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Ran_Skill_2_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Begin; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }
        
        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (!(state.Ev is JudgeEvent)) return;
            JudgeEvent ev = (JudgeEvent)(state.Ev);
            Card result = ev.JudgeCards.FirstOrDefault();
            if (result == null) return;
            switch (result.CardColor?.E)
            {
                case Enum_CardColor.Spade:
                    ctx.World.Damage(skill.Owner, state.Owner, 1, DamageEvent.Thunder, ev);
                    break;
                case Enum_CardColor.Club:
                    if (state.Owner == skill.Owner) break;
                    ctx.World.RequireCard(skill.KeyName, "请交给八云蓝1至3张手牌。", state.Owner,
                        new FulfillNumberCardFilter(1, 3)
                        {
                            Allow_Hand = true,
                            Allow_Judging = false,
                            Allow_Equiped = false,
                        },
                        false, 15,
                        (cards) =>
                        {
                            Zone handzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                            if (handzone == null) return;
                            ctx.World.MoveCards(state.Owner, cards, handzone, ev);
                        }, null);
                    break;
                case Enum_CardColor.Heart:
                    ctx.World.Heal(skill.Owner, state.Owner, 1, HealEvent.Normal, ev);
                    break;
                case Enum_CardColor.Diamond:
                    if (state.Owner == skill.Owner) break;
                    ctx.World.RequireCard(skill.KeyName, "请交给其1至3张手牌。", skill.Owner,
                        new FulfillNumberCardFilter(1, 3)
                        {
                            Allow_Hand = true,
                            Allow_Judging = false,
                            Allow_Equiped = false,
                        },
                        false, 15,
                        (cards) =>
                        {
                            Zone handzone = state.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                            if (handzone == null) return;
                            ctx.World.MoveCards(skill.Owner, cards, handzone, ev);
                        }, null);
                    break;
            }
        }
    }

    public class Ran_Skill_2_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Ran_Skill_2_Trigger_1_Condition(Skill _skill) : base(_skill)
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
