using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Homos
{
    /// <summary>
    /// 角色【小恶魔】
    /// </summary>
    /// <remarks>
    /// 势力：红魔 4勾玉
    /// 【司书】：当其他角色摸牌时，其可以选择交给你，然后你交给其等量的牌。
    /// 【使魔】：觉醒技，你的回合开始阶段，当你体力为全场最少（之一）时，你失去一点体力上限，恢复一点体力或者摸两张牌，获得技能【整理】。
    /// 【整理】：当你获得其他角色的牌时，你可以摸等量的牌，然后丢弃等量的牌。
    /// </remarks>
    public class DevilLily : Charactor
    {
        public DevilLily()
        {
            MaxHP = 4;
            HP = 4;
            Country = Remilia.CountryNameOfLeader;
            Skills.Add(new DevilLily_Skill_0());
            Skills.Add(new DevilLily_Skill_1());
        }

        public override Charactor Clone()
        {
            return new DevilLily();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "小恶魔";
            info.AbilityRadar = new AbilityRadar() { Attack = 1, Defence = 3, Control = 1, Auxiliary = 5, LastStages = 1, Difficulty = 1 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "DevilLily");
            info.Skills.Add(new SkillInfo()
            {
                Name = "司书",
                Description = "当其他角色摸牌时，其可以选择交给你，然后你交给其等量的牌。"
            });
            SkillInfo skill_1 = new SkillInfo()
            {
                Name = "使魔",
                Description = "觉醒技，你的回合开始阶段，当你体力为全场最少（之一）时，你失去一点体力上限，选择恢复一点体力或摸两张牌，获得技能【整理】。"
            };
            skill_1.AttachedSkills.Add(new SkillInfo()
            {
                Name = "整理",
                Description = "当你获得其他角色的牌时，你可以摸等量的牌，然后丢弃等量的牌。"
            });
            info.Skills.Add(skill_1);
            return info;
        }
    }

    public class DevilLily_Skill_0 : Skill
    {
        public DevilLily_Skill_0()
        {
            Triggers.Add(new DevilLily_Skill_0_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new DevilLily_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new DevilLily_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "司书",
                Description = "当其他角色摸牌时，其可以选择交给你，然后你交给其等量的牌。"
            };
        }
    }

    public class DevilLily_Skill_0_Trigger_0 : SkillTrigger, ITriggerInEvent, ITriggerAsk
    {
        public DevilLily_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new DevilLily_Skill_0_Trigger_0_Condition(skill);
        }
        
        string ITriggerInEvent.EventKeyName { get => MoveCardDoneEvent.DefaultKeyName; }
        
        string ITriggerAsk.Message { get => "是否选择交给小恶魔（技能【司书】）？"; }
        
        Player ITriggerAsk.GetAsked(Context ctx)
        {
            if (!(ctx.Ev is MoveCardDoneEvent)) return null;
            MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
            return ev.NewZone.Owner;
        }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is MoveCardDoneEvent)) return;
            MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(ev.NewZone.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            Zone hand0 = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            Zone hand1 = ev.NewZone.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            int cardnumber = ev.MovedCards.Count();
            ctx.World.MoveCards(ev.NewZone.Owner, ev.MovedCards, hand0, ev_skill);
            ctx.World.RequireCard(skill.KeyName, "请将等量的手牌还回。", skill.Owner,
                new FulfillNumberCardFilter(cardnumber, cardnumber)
                {
                    Allow_Hand = true,
                    Allow_Equiped = false,
                    Allow_Judging = false,
                }, false, 15,
                (cards) =>
                {
                    ctx.World.MoveCards(skill.Owner, cards, hand1, ev_skill);
                }, null);
        }

        
    }
   
    public class DevilLily_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public DevilLily_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is MoveCardDoneEvent)) return false;
            MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
            if (ev.OldZone?.KeyName?.Equals(Zone.Draw) != true) return false;
            if (ev.NewZone?.KeyName?.Equals(Zone.Hand) != true) return false;
            if (ev.NewZone.Owner == skill.Owner) return false;
            return true;
        }
    }

    public class DevilLily_Skill_1 : Skill
    {
        public DevilLily_Skill_1()
        {
            IsOnce = true;
            IsLocked = true;
            Triggers.Add(new DevilLily_Skill_1_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new DevilLily_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new DevilLily_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "使魔",
                Description = "觉醒技，你的回合开始阶段，当你体力为全场最少（之一）时，你失去一点体力上限，恢复一点体力或者摸两张牌，获得技能【整理】。"
            };
        }

    }

    public class DevilLily_Skill_1_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public const string Selection_0 = "摸两张牌";
        public const string Selection_1 = "恢复一点体力";

        public DevilLily_Skill_1_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new DevilLily_Skill_1_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Begin; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        public override void Action(Context ctx)
        {
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            ctx.World.ChangeMaxHp(skill.Owner, -1, ev_skill);
            ctx.World.ShowList(skill.KeyName, "请选择一项。", skill.Owner,
                new List<object>() { Selection_0, Selection_1 }, 1,
                false, 15,
                (selected) =>
                {
                    switch (selected.ToString())
                    {
                        case Selection_0:
                            ctx.World.DrawCard(skill.Owner, 2, ev_skill);
                            break;
                        case Selection_1:
                            ctx.World.Heal(skill.Owner, skill.Owner, 1, HealEvent.Normal, ev_skill);
                            break;
                    }
                }, null);
            Player owner = skill.Owner;
            owner.Skills.Remove(skill);
            owner.Skills.Add(new DevilLily_Skill_2());
        }
    }

    public class DevilLily_Skill_1_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public DevilLily_Skill_1_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            if (ctx.World.Players.FirstOrDefault(_player =>
            {
                if (_player == skill.Owner) return false;
                if (!_player.IsAlive) return false;
                if (_player.HP < skill.Owner.HP) return true;
                return false;
            }) != null) return false;
            return true;
        }

    }

    public class DevilLily_Skill_2 : Skill
    {
        public DevilLily_Skill_2()
        {
            IsLocked = false;
            Triggers.Add(new DevilLily_Skill_2_Trigger(this));
        }


        public override Skill Clone()
        {
            return new DevilLily_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new DevilLily_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "整理",
                Description = "当你获得其他角色的牌时，你可以摸等量的牌，然后丢弃等量的牌。"
            };
        }
    }
   
    public class DevilLily_Skill_2_Trigger : SkillTrigger, ITriggerInEvent, ITriggerAsk
    {
        public DevilLily_Skill_2_Trigger(Skill _skill) : base(_skill)
        {

        }
       
        string ITriggerInEvent.EventKeyName { get => MoveCardDoneEvent.DefaultKeyName; }

        string ITriggerAsk.Message { get => "是否要发动【整理】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is MoveCardDoneEvent)) return;
            MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
            SkillEvent ev_skill = new SkillEvent();
            int cardnumber = ev.MovedCards.Count();
            ev_skill.Reason = ev;
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            ctx.World.DrawCard(skill.Owner, cardnumber, ev_skill);
            ctx.World.DiscardCard(skill.Owner, cardnumber, ev_skill, false);
        }
    }

    public class DevilLily_Skill_2_Trigger_Condition : ConditionFilterFromSkill
    {
        public DevilLily_Skill_2_Trigger_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is MoveCardDoneEvent)) return false;
            MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
            if (ev.NewZone.KeyName?.Equals(Zone.Hand) != true) return false;
            if (ev.NewZone.Owner != skill.Owner) return false;
            return true;
        }
    }
    

   

}
