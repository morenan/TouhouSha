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

namespace TouhouSha.Reimu.Charactors.Forevers
{
    /// <summary>
    /// 角色【蓬莱山辉夜】
    /// </summary>
    /// <remarks>
    /// HP:3 竹林
    /// 【蓬莱】锁定技，你的回合开始阶段，若你已受伤，你回复2点体力，若你未受伤，你增加一点体力上限并回复一点体力。
    /// 【难题】锁定技，当你体力上限不超过10并且回复体力时，你选择增加1至3点体力上限，当体力上限因此变为以下数的整数倍时：
    /// 2. 你减一点体力上限并摸两张牌。
    /// 3. 你减一点体力上限并回复一点体力（不触发【难题】）。
    /// 5. 你减一点体力上限并将全部手牌交给一名其他角色。
    /// 【须臾】主公技，觉醒技，当场上的竹林角色的体力上限总和不小于10时，你减少一点体力上限，回复一点体力并获得技能【永夜】。
    /// 【永夜】主公技，场上的竹林角色成为红色牌的目标时，你可以减少一点体力上限取消之。
    /// </remarks>
    public class Kaguya : Charactor
    {
        public const string CountryNameOfLeader = "竹林";

        public Kaguya()
        {
            MaxHP = 3;
            HP = 3;
            Country = CountryNameOfLeader;
            Skills.Add(new Skill_PengLai());
            Skills.Add(new Kaguya_Skill_1());
            Skills.Add(new Kaguya_Skill_2());
        }

        public override Charactor Clone()
        {
            return new Kaguya();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "蓬莱山辉夜";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Kaguya");
            info.AbilityRadar = new AbilityRadar() { Attack = 1, Defence = 4, Control = 2, Auxiliary = 5, LastStages = 5, Difficulty = 2 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }
    }

    public class Kaguya_Skill_1 : Skill
    {
        public const string DefaultKeyName = "难题";

        public Kaguya_Skill_1()
        {
            KeyName = DefaultKeyName;
            Triggers.Add(new Kaguya_Skill_1_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Kaguya_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Kaguya_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "难题",
                Description = "你体力上限不超过10时可以发动。当你回复体力时，你选择增加1至3点体力上限（非随机），当你受到伤害时，你选择减少1至3点体力上限（非随机）。" + 
                    "当体力上限因此变为以下数的整数倍时：\n" +
                    "\t2: 你减一点体力上限并摸两张牌。\n" +
                    "\t3: 你减一点体力上限并回复一点体力（不触发【难题】）。\n" +
                    "\t5: 你减一点体力上限并将全部手牌交给一名其他角色。",
            };
        }
    }
    
    public class Kaguya_Skill_1_Trigger_0 : SkillTrigger, ITriggerInEvent
    {
        public Kaguya_Skill_1_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Kaguya_Skill_1_Trigger_0_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => HealEvent.DefaultKeyName; }

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
            List<string> selections = new List<string>();
            for (int i = 1; i <= 3; i++)
                selections.Add(String.Format("增加{0}点体力上限。", i));
            ctx.World.ShowList("难题选择项", "请选择一项。", skill.Owner,
                selections, 1, false, 15,
                (selected) =>
                {
                    int i = selections.IndexOf(selected.ToString());
                    ctx.World.ChangeMaxHp(skill.Owner, i + 1, ev_skill);
                    int maxhp = skill.Owner.MaxHP;
                    if ((maxhp % 2) == 0)
                    {
                        ctx.World.DrawCard(skill.Owner, 2, ev_skill);
                    }
                    if ((maxhp % 3) == 0)
                    {
                        ctx.World.Heal(skill.Owner, skill.Owner, 1, skill.KeyName, ev_skill);
                    }
                    if ((maxhp % 5) == 0)
                    {
                        Zone handzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                        if (handzone != null && handzone.Cards.Count() > 0)
                        {
                            ctx.World.SelectPlayer("难题选择交牌", "请选择一名角色，将手牌都交给他。", skill.Owner,
                                new FulfillNumberPlayerFilter(1, 1, skill.Owner),
                                false, 15,
                                (targets) =>
                                {
                                    Player target = targets.FirstOrDefault();
                                    Zone target_handzone = target?.Zones?.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                                    if (target_handzone != null)
                                        ctx.World.MoveCards(skill.Owner, handzone.Cards.ToArray(), target_handzone, ev_skill);
                                }, null);
                        }
                    }
                }, null);
        }
    }

    public class Kaguya_Skill_1_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Kaguya_Skill_1_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is HealEvent)) return false;
            HealEvent ev = (HealEvent)(ctx.Ev);
            if (ev.Target != skill.Owner) return false;
            if (skill.Owner.MaxHP > 10) return false;
            if (ev.HealValue <= 0) return false;
            if (ev.HealType?.Equals(skill.KeyName) == true) return false;
            return true;
        }
    }

    public class Kaguya_Skill_2 : Skill
    {
        public override Skill Clone()
        {
            return new Kaguya_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new Kaguya_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            SkillInfo skillinfo = new SkillInfo()
            {
                Name = "须臾",
                Description = "主公技，觉醒技，当场上的竹林角色的体力上限总和不小于10时，你减少一点体力上限，回复一点体力并获得技能【永夜】。"
            };
            skillinfo.AttachedSkills.Add(new SkillInfo()
            {
                Name = "永夜",
                Description = "主公技，场上的竹林角色成为红色牌的目标时，你可以减少一点体力上限取消之。"
            });
            return skillinfo;
        }
    }

    public class Kaguya_Skill_2_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public Kaguya_Skill_2_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Kaguya_Skill_2_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.GameStart; }

        int ITriggerInState.StateStep { get => 0; }

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
            Player owner = skill.Owner;
            owner.Skills.Remove(skill);
            owner.Skills.Add(new Kaguya_Skill_3());
        }
    }

    public class Kaguya_Skill_2_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Kaguya_Skill_2_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            int maxhp_count = 0;
            foreach (Player player in ctx.World.Players)
            {
                if (!player.IsAlive) continue;
                if (player.Country?.Equals(Kaguya.CountryNameOfLeader) != true) continue;
                maxhp_count = player.MaxHP;
            }
            if (maxhp_count < 10) return false;
            return true;
        }
    }

    public class Kaguya_Skill_2_Trigger_1 : SkillTrigger, ITriggerInEvent
    {
        public Kaguya_Skill_2_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Kaguya_Skill_2_Trigger_1_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => PropChangeEvent.DefaultKeyName; }

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
            Player owner = skill.Owner;
            owner.Skills.Remove(skill);
            owner.Skills.Add(new Kaguya_Skill_3());
        }
    }

    public class Kaguya_Skill_2_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Kaguya_Skill_2_Trigger_1_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            int maxhp_count = 0;
            foreach (Player player in ctx.World.Players)
            {
                if (!player.IsAlive) continue;
                if (player.Country?.Equals(Kaguya.CountryNameOfLeader) != true) continue;
                maxhp_count = player.MaxHP;
            }
            if (maxhp_count < 10) return false;
            return true;
        }
    }

    public class Kaguya_Skill_3 : Skill
    {
        public Kaguya_Skill_3()
        {
            IsLeader = true;
            IsLeaderForLeader = true;
            IsLeaderForSlave = false;
            Triggers.Add(new Kaguya_Skill_3_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Kaguya_Skill_3();
        }

        public override Skill Clone(Card newcard)
        {
            return new Kaguya_Skill_3();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "永夜",
                Description = "主公技，场上的竹林角色成为红色牌的目标时，你可以减少一点体力上限取消之。"
            };
        }
    }

    public class Kaguya_Skill_3_Trigger_0 : SkillTrigger, ITriggerInEvent
    {
        public Kaguya_Skill_3_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Kaguya_Skill_3_Trigger_0_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => CardPreviewEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is CardPreviewEvent)) return;
            CardPreviewEvent ev = (CardPreviewEvent)(ctx.Ev);
            for (int i = 0; i < ev.Targets.Count(); i++)
            {
                Player target = ev.Targets[i];
                if (target.Country?.Equals(Kaguya.CountryNameOfLeader) != true) continue;
                if (skill.Owner.MaxHP <= 1) break;
                if (!ctx.World.Ask(skill.Owner, "询问是否用永夜取消目标", String.Format("是否扣除一点体力上限，取消{0}的目标结算？", target.Name))) continue;
                ev.Targets.RemoveAt(i--);
            }
        }
    }

    public class Kaguya_Skill_3_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Kaguya_Skill_3_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is CardPreviewEvent)) return false;
            CardPreviewEvent ev = (CardPreviewEvent)(ctx.Ev);
            if (ev.Card?.CardColor?.SeemAs(Enum_CardColor.Red) != true) return false;
            if (ev.Targets.FirstOrDefault(_target => _target.Country?.Equals(Kaguya.CountryNameOfLeader) == true) == null) return false;
            return true;
        }
    }
}
