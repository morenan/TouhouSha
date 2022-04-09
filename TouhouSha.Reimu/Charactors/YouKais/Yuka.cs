using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.YouKais
{
    /// <summary>
    /// 角色【风见幽香】
    /// </summary>
    /// <remarks>
    /// 妖怪 HP:4
    /// 【花开】锁定技，游戏开始时，你将牌堆顶一张牌置于你的角色牌上，作为【花】。
    /// 【春樱】锁定技，当你的【花】为♣，并且♥牌被弃置于弃牌堆时，你将这张牌替换为【花】，并回复一点体力。
    /// 【夏葵】锁定技，当你的【花】为♥，并且♦牌被弃置于弃牌堆时，你将这张牌替换为【花】，并摸两张牌。
    /// 【秋菊】锁定技，当你的【花】为♦，并且♠牌被弃置于弃牌堆时，你将这张牌替换为【花】，并弃置场上所有其他角色的一张牌（此法弃置的牌不触发你的技能）。
    /// 【冬梅】锁定技，当你的【花】为♠，并且♣牌被弃置于弃牌堆时，你将这张牌替换为【花】，并对场上所有其他角色造成一点伤害。
    /// </remarks>
    public class Yuka : Charactor
    { 
        public const string Zone_Flower = "花";
      
        public Yuka()
        {
            MaxHP = 4;
            HP = 4;
            Country = Yucari.CountryNameOfLeader;
            Skills.Add(new Yuka_Skill_0());
            Skills.Add(new Yuka_Skill_1());
            Skills.Add(new Yuka_Skill_2());
            Skills.Add(new Yuka_Skill_3());
            Skills.Add(new Yuka_Skill_4());
        }

        public override Charactor Clone()
        {
            return new Yuka();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "风见幽香";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Yuka");
            info.AbilityRadar = new AbilityRadar() { Attack = 3, Defence = 4, Control = 4, Auxiliary = 1, LastStages = 2, Difficulty = 5 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }
    }

    public class Yuka_Skill_0 : Skill
    {
        public Yuka_Skill_0()
        {
            IsLocked = true;
            Triggers.Add(new Yuka_Skill_0_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Yuka_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yuka_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "花开",
                Description = "锁定技，游戏开始时，你将牌堆顶一张牌置于你的角色牌上，作为【花】。"
            };
        }
    }
    
    public class Yuka_Skill_0_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public Yuka_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Yuka_Skill_0_Trigger_0_Condition(skill);
        }
        
        string ITriggerInState.StateKeyName { get => State.GameStart; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_BeforeStart; }

        public override void Action(Context ctx)
        {
            Zone flower_zone = new Zone();
            flower_zone.KeyName = Yuka.Zone_Flower;
            flower_zone.Owner = skill.Owner;
            flower_zone.Flag = Enum_ZoneFlag.LabelOnPlayer;
            skill.Owner.Zones.Add(flower_zone);
            ctx.World.MoveCards(skill.Owner, ctx.World.GetDrawTops(1), flower_zone, null);
        }

    }
    
    public class Yuka_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Yuka_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            return true;
        }

    }
    
    public abstract class Yuka_Skill_Trigger_About_Flower : SkillTrigger, ITriggerInEvent
    {
        public Yuka_Skill_Trigger_About_Flower(Skill _skill, Enum_CardColor _oldcolor, Enum_CardColor _newcolor) : base(_skill)
        {
            Condition = new Yuka_Skill_Trigger_Condition_About_Flower(_skill, _oldcolor, _newcolor);
            this.oldcolor = _oldcolor;
            this.newcolor = _newcolor;
        }

        private Enum_CardColor oldcolor;
        public Enum_CardColor OldColor { get { return this.oldcolor; } }

        private Enum_CardColor newcolor;
        public Enum_CardColor NewColor { get { return this.newcolor; } }

        string ITriggerInEvent.EventKeyName { get => MoveCardDoneEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is MoveCardDoneEvent)) return;
            MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
            Card heartcard = null;
            foreach (Card card in ev.MovedCards)
            {
                foreach (Card initcard in card.GetInitialCards())
                {
                    if (initcard.CardColor?.SeemAs(newcolor) == true) heartcard = initcard;
                    if (heartcard != null) break;
                }
                if (heartcard != null) break;
            }
            if (heartcard == null) return;
            Zone flower_zone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Yuka.Zone_Flower) == true);
            if (flower_zone == null) return;
            if (flower_zone.Cards.Count() == 0) return;
            Zone discard_zone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
            if (discard_zone == null) return;
            SkillEvent ev_skill = CreateSkillEvent(ctx, ev);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            ActionAfter(ctx, ev_skill);
        }
        
        protected virtual SkillEvent CreateSkillEvent(Context ctx, Event reason)
        {
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = reason;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            return ev_skill;
        }

        protected abstract void ActionAfter(Context ctx, SkillEvent ev_skill);
    }

    public class Yuka_Skill_Trigger_Condition_About_Flower : ConditionFilterFromSkill
    {
        public Yuka_Skill_Trigger_Condition_About_Flower(Skill _skill, Enum_CardColor _oldcolor, Enum_CardColor _newcolor) : base(_skill)
        {
            this.oldcolor = _oldcolor;
            this.newcolor = _newcolor;
        }

        private Enum_CardColor oldcolor;
        public Enum_CardColor OldColor { get { return this.oldcolor; } }

        private Enum_CardColor newcolor;
        public Enum_CardColor NewColor { get { return this.newcolor; } }
        
        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is MoveCardDoneEvent)) return false;
            MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
            if (ev.NewZone.KeyName?.Equals(Zone.Discard) != true) return false;
            bool existheart = false;
            foreach (Card card in ev.MovedCards)
            {
                if (card.GetValue(Yuka_Skill_3.Marked) == 1) continue;
                foreach (Card initcard in card.GetInitialCards())
                {
                    if (initcard.GetValue(Yuka_Skill_3.Marked) == 1) continue;
                    if (initcard.CardColor?.SeemAs(newcolor) == true) existheart = true;
                    if (existheart) break;
                }
                if (existheart) break;
            }
            if (!existheart) return false;
            Zone flower_zone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Yuka.Zone_Flower) == true);
            if (flower_zone == null) return false;
            if (flower_zone.Cards.Count() == 0) return false;
            if (flower_zone.Cards[0].CardColor?.SeemAs(oldcolor) != true) return false;
            return true;
        }
    }

    public class Yuka_Skill_1 : Skill
    {
        public Yuka_Skill_1()
        {
            IsLocked = true;
            Triggers.Add(new Yuka_Skill_1_Trigger_0(this));
        }
        public override Skill Clone()
        {
            return new Yuka_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yuka_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "春樱",
                Description = "锁定技，当你的【花】为♣，并且♥牌被弃置于弃牌堆时，你将这张牌替换为【花】，并回复一点体力。"
            };
        }
    }

    public class Yuka_Skill_1_Trigger_0 : Yuka_Skill_Trigger_About_Flower
    {
        public Yuka_Skill_1_Trigger_0(Skill _skill) : base(_skill, Enum_CardColor.Spade, Enum_CardColor.Heart)
        {
        }

        protected override void ActionAfter(Context ctx, SkillEvent ev_skill)
        {
            ctx.World.Heal(skill.Owner, skill.Owner, 1, HealEvent.Normal, ev_skill);
        }
    }
    
    public class Yuka_Skill_2 : Skill
    {

        public Yuka_Skill_2()
        {
            IsLocked = true;
            Triggers.Add(new Yuka_Skill_2_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Yuka_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yuka_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "夏葵",
                Description = "锁定技，当你的【花】为♥，并且♦牌被弃置于弃牌堆时，你将这张牌替换为【花】，并摸两张牌。"
            };
        }
    }

    public class Yuka_Skill_2_Trigger_0 : Yuka_Skill_Trigger_About_Flower
    {
        public Yuka_Skill_2_Trigger_0(Skill _skill) : base(_skill, Enum_CardColor.Heart, Enum_CardColor.Diamond)
        {
        }

        protected override void ActionAfter(Context ctx, SkillEvent ev_skill)
        {
            ctx.World.DrawCard(skill.Owner, 2, ev_skill);
        }
    }

    public class Yuka_Skill_3 : Skill
    {
        public const string Marked = "被秋菊的效果弃置";

        public Yuka_Skill_3()
        {
            IsLocked = true;
            Triggers.Add(new Yuka_Skill_3_Trigger_0(this));
        }
        public override Skill Clone()
        {
            return new Yuka_Skill_3();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yuka_Skill_3();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "秋菊",
                Description = "锁定技，当你的【花】为♦，并且♠牌被弃置于弃牌堆时，你将这张牌替换为【花】，并弃置场上所有其他角色的一张牌（此法弃置的牌不触发你的技能）。"
            };
        }
    }

    public class Yuka_Skill_3_Trigger_0 : Yuka_Skill_Trigger_About_Flower
    {
        public Yuka_Skill_3_Trigger_0(Skill _skill) : base(_skill, Enum_CardColor.Diamond, Enum_CardColor.Club)
        {

        }

        protected override SkillEvent CreateSkillEvent(Context ctx, Event reason)
        {
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = reason;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.AddRange(ctx.World.GetAlivePlayersStartHere(skill.Owner));
            ev_skill.Targets.Remove(skill.Owner);
            return ev_skill;
        }

        protected override void ActionAfter(Context ctx, SkillEvent ev_skill)
        {
            Zone discardzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
            if (discardzone == null) return;
            foreach (Player target in ev_skill.Targets)
            {
                ctx.World.SelectTargetCard(skill.Owner, target, 1, true, true,
                    (cards) => 
                    {
                        if (cards.Count() == 0) return;
                        foreach (Card card in cards) card.SetValue(Yuka_Skill_3.Marked, 1);
                        ctx.World.MoveCards(target, cards, discardzone, ev_skill);
                    }, null);
            }
        }
    }

    public class Yuka_Skill_4 : Skill
    {
        public Yuka_Skill_4()
        {
            IsLocked = true;
            Triggers.Add(new Yuka_Skill_4_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Yuka_Skill_4();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yuka_Skill_4();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "冬梅",
                Description = "锁定技，当你的【花】为♠，并且♣牌被弃置于弃牌堆时，你将这张牌替换为【花】，并对场上所有其他角色造成一点伤害。"
            };
        }
    }

    public class Yuka_Skill_4_Trigger_0 : Yuka_Skill_Trigger_About_Flower
    {
        public Yuka_Skill_4_Trigger_0(Skill _skill) : base(_skill, Enum_CardColor.Diamond, Enum_CardColor.Club)
        {

        }

        protected override SkillEvent CreateSkillEvent(Context ctx, Event reason)
        {
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = reason;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.AddRange(ctx.World.GetAlivePlayersStartHere(skill.Owner));
            ev_skill.Targets.Remove(skill.Owner);
            return ev_skill;
        }

        protected override void ActionAfter(Context ctx, SkillEvent ev_skill)
        {
            foreach (Player target in ev_skill.Targets)
                ctx.World.Damage(skill.Owner, target, 1, DamageEvent.Normal, ev_skill);
        }
    }
}
