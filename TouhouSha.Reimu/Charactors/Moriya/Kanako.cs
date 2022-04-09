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
using TouhouSha.Koishi.Triggers;

namespace TouhouSha.Reimu.Charactors.Moriya
{
    /// <summary>
    /// 角色【八坂神奈子】
    /// </summary>
    /// <remarks>
    /// HP:5 守矢
    /// 【神仪】锁定技，你与其他角色的拼点结算后，若你没赢并且你使用的拼点牌点数不小于3，你获得所有拼点的牌。
    /// 【天乾】锁定技，其他角色的回合开始阶段，你和其进行一次拼点：
    ///     若你赢，你选择一项：
    ///         1. 对其无视距离使用一张【杀】。
    ///         2. 直到回合结束，当你成为一张卡的唯一目标时，取消之。
    ///     若你没赢，对方选择一项：
    ///         1. 摸两张牌。
    ///         2. 获取你的一张牌。
    /// 【神威】主公技，你以外的其他守矢角色和你以外的角色进行拼点时，你可以代替该守矢角色打出拼点牌。
    /// </remarks>
    public class Kanako : Charactor
    {
        public const string CountryNameOfLeader = "守矢";

        public Kanako()
        {
            MaxHP = 5;
            HP = 5;
            Country = CountryNameOfLeader;
            Skills.Add(new Kanako_Skill_0());
            Skills.Add(new Kanako_Skill_1());
            Skills.Add(new Kanako_Skill_2());
        }

        public override Charactor Clone()
        {
            return new Kanako();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "八坂神奈子";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Kanako");
            info.AbilityRadar = new AbilityRadar() { Attack = 1, Defence = 4, Control = 2, Auxiliary = 5, LastStages = 1, Difficulty = 2 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }
    }
    
    public class Kanako_Skill_0 : Skill
    {
        public Kanako_Skill_0()
        {
            IsLocked = true;
            Triggers.Add(new Kanako_Skill_0_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Kanako_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Kanako_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "神仪",
                Description = "锁定技，你与其他角色的拼点结算后，若你没赢并且你使用的拼点牌点数不小于3，你获得所有拼点的牌。",
            };
        }
    }

    public class Kanako_Skill_0_Trigger_0 : SkillTrigger, ITriggerInEvent
    {
        public Kanako_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Kanako_Skill_0_Trigger_0_Condition(skill);
        }
        
        string ITriggerInEvent.EventKeyName { get => PointBattleDoneEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is PointBattleDoneEvent)) return;
            PointBattleDoneEvent ev = (PointBattleDoneEvent)(ctx.Ev);
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            Zone handzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return;
            ctx.World.MoveCards(skill.Owner, new List<Card> { ev.SourceCard, ev.TargetCard }, handzone, ev_skill);
        }
    }

    public class Kanako_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Kanako_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is PointBattleDoneEvent)) return false;
            PointBattleDoneEvent ev = (PointBattleDoneEvent)(ctx.Ev);
            if (skill.Owner != ev.Source && skill.Owner != ev.Target) return false;
            if (ev.IsWin(skill.Owner)) return false;
            return true;
        }

    }

    public class Kanako_Skill_1 : Skill
    {
        public const string CancelTarget = "天乾取消目标";
       
        public Kanako_Skill_1()
        {
            IsLocked = true;
            Triggers.Add(new Kanako_Skill_1_Trigger_0(this));
            Triggers.Add(new Kanako_Skill_1_Trigger_1(this));
        }
        
        public override Skill Clone()
        {
            return new Kanako_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Kanako_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "天乾",
                Description = "锁定技，其他角色的回合开始阶段，你和其进行一次拼点：\n" +
                    "若你赢，你选择一项：\n" +
                    "\t1. 对其无视距离使用一张【杀】。\n" +
                    "\t2. 直到回合结束，当你成为一张卡的唯一目标时，取消之。\n" +
                    "若你没赢，对方选择一项：\n" +
                    "\t1. 摸两张牌。\n" +
                    "\t2. 获取你的一张牌。"
            };
        }
    }

    public class Kanako_Skill_1_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public Kanako_Skill_1_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Kanako_Skill_1_Trigger_0_Condition(skill);
        }
        
        string ITriggerInState.StateKeyName { get => State.Begin; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

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
            PointBattleBeginEvent ev_battle = new PointBattleBeginEvent();
            ev_battle.Reason = ev_skill;
            ev_battle.Source = skill.Owner;
            ev_battle.Target = state.Owner;
            ctx.World.InvokeEvent(ev_battle);
        }
    }
    
    public class Kanako_Skill_1_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Kanako_Skill_1_Trigger_0_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner == skill.Owner) return false;
            return true;
        }
    }

    public class Kanako_Skill_1_Trigger_1 : SkillTrigger, ITriggerInEvent
    {
        public const string Selection_0 = "对其无视距离使用一张【杀】。";
        public const string Selection_1 = "直到回合结束，当你成为一张卡的唯一目标时，取消之。";
        public const string Selection_2 = "摸两张牌。";
        public const string Selection_3 = "获取你的一张牌。";

        public Kanako_Skill_1_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Kanako_Skill_1_Trigger_1_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => PointBattleDoneEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is PointBattleDoneEvent)) return;
            PointBattleDoneEvent ev = (PointBattleDoneEvent)(ctx.Ev);
            if (ev.IsWin(ev.Source))
            {
                ctx.World.ShowList("天乾拼点赢", "请选择一项。", skill.Owner,
                    new List<object>() { Selection_0, Selection_1 }, 1,
                    false, 15,
                    (selected) =>
                    {
                        switch (selected.ToString())
                        {
                            case Selection_0:
                                ctx.World.RequireCard("请选择出杀", "请打出一张杀。", skill.Owner,
                                    new TargetCardFilter(1, 1, new string[] { KillCard.Normal, KillCard.Fire, KillCard.Thunder }),
                                    true, 15,
                                    (cards) =>
                                    {
                                        CardEvent ev_card = new CardEvent();
                                        ev_card.Reason = ev.Reason;
                                        ev_card.Card = cards.FirstOrDefault();
                                        ev_card.Source = skill.Owner;
                                        ev_card.Targets.Clear();
                                        ev_card.Targets.Add(ev.Target);
                                        ctx.World.InvokeEvent(ev_card);
                                    }, null);
                                break;
                            case Selection_1:
                                State state = ctx.World.GetPlayerState();
                                if (state == null) break;
                                if (state.Ev == null) break;
                                state.Ev.SetValue(Kanako_Skill_1.CancelTarget, 1);
                                break;
                        }
                    }, null);
            }
            else
            {
                ctx.World.ShowList("天乾拼点输", "请选择一项。", ev.Target,
                    new List<object> { Selection_2, Selection_3 }, 1,
                    false, 15,
                    (selected) =>
                    {
                        switch (selected.ToString())
                        {
                            case Selection_2:
                                ctx.World.DrawCard(ev.Target, 2, ev.Reason);
                                break;
                            case Selection_3:
                                ctx.World.StealTargetCard(ev.Target, skill.Owner, 1, ev.Reason, true, true);
                                break;
                        }
                    }, null);
            }
        }
    }

    public class Kanako_Skill_1_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Kanako_Skill_1_Trigger_1_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is PointBattleDoneEvent)) return false;
            PointBattleDoneEvent ev = (PointBattleDoneEvent)(ctx.Ev);
            if (!(ev.Reason is SkillEvent)) return false;
            SkillEvent ev_skill = (SkillEvent)(ev.Reason);
            if (ev_skill.Skill != skill) return false;
            return true;
        }

    }

    public class Kanako_Skill_2 : Skill
    {
        public Kanako_Skill_2()
        {
            IsLeader = true;
            IsLeaderForLeader = true;
            IsLeaderForSlave = false;
        }

        public override Skill Clone()
        {
            return new Kanako_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new Kanako_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "神威",
                Description = "主公技，你以外的其他守矢角色和你以外的角色进行拼点时，你可以代替该守矢角色打出拼点牌。",
            };
        }
    }
    
    public class Kanako_Skill_2_Trigger_0 : SkillTrigger, ITriggerInEvent, ITriggerAsk
    {
        public Kanako_Skill_2_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Kanako_Skill_2_Trigger_0_Condition(skill);
        }
        
        string ITriggerInEvent.EventKeyName { get => UseTriggerEvent.DefaultKeyName; }
        
        string ITriggerAsk.Message { get => "是否要发动【神威】？"; }
        
        Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is UseTriggerEvent)) return;
            UseTriggerEvent ev = (UseTriggerEvent)(ctx.Ev);
            Trigger oldtrigger = ev.NewTrigger;
            ev.NewTrigger = new Kanako_Skill_2_ProvidePointCardTrigger(skill) { OldTrigger = oldtrigger };
        }
    }

    public class Kanako_Skill_2_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Kanako_Skill_2_Trigger_0_Condition(Skill _skill) : base(_skill)
        {
        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is UseTriggerEvent)) return false;
            UseTriggerEvent ev = (UseTriggerEvent)(ctx.Ev);
            if (ev.NewTrigger is PointBattleTrigger) return true;
            if (ev.NewTrigger is OverrideTrigger)
            {
                OverrideTrigger overrides = (OverrideTrigger)(ev.NewTrigger);
                while (overrides != null)
                {
                    if (overrides is Kanako_Skill_2_ProvidePointCardTrigger)
                        return false;
                    if (overrides.OldTrigger is PointBattleTrigger)
                        return true;
                    overrides = overrides.OldTrigger as OverrideTrigger;
                }
            }
            return false;
        }
    }

    public class Kanako_Skill_2_ProvidePointCardTrigger : OverrideTrigger, ITriggerInEvent
    {
        public Kanako_Skill_2_ProvidePointCardTrigger(Skill _skill)
        {
            this.skill = _skill;
            Condition = new PointBattleCondition();
        }

        private Skill skill;
        public Skill Skill { get { return this.skill; } }

        string ITriggerInEvent.EventKeyName { get => PointBattleBeginEvent.DefaultKeyName; }


        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is PointBattleBeginEvent)) return;
            PointBattleBeginEvent ev = (PointBattleBeginEvent)(ctx.Ev);
            Card card0 = ev.SourceCard;
            Card card1 = ev.TargetCard;
            Task task0 = Task.Factory.StartNew(() =>
            {
                if (card0 != null) return;
                ctx.World.RequireCard("拼点使用的拼点牌", "请使用一张牌来拼点。", ev.Source == skill.Owner ? skill.Owner : ev.Source,
                    new FulfillNumberCardFilter(1, 1)
                    {
                        Allow_Hand = true,
                        Allow_Judging = false,
                        Allow_Equiped = false
                    },
                    false, 15,
                    (cards) => { card0 = cards.FirstOrDefault(); }, null);
            });
            Task task1 = Task.Factory.StartNew(() =>
            {
                if (card1 != null) return;
                ctx.World.RequireCard("拼点使用的拼点牌", "请使用一张牌来拼点。", ev.Target == skill.Owner ? skill.Owner : ev.Target,
                    new FulfillNumberCardFilter(1, 1)
                    {
                        Allow_Hand = true,
                        Allow_Judging = false,
                        Allow_Equiped = false
                    },
                    false, 15,
                    (cards) => { card1 = cards.FirstOrDefault(); }, null);
            });
            task0.Wait();
            task1.Wait();
            if (card0 == null) return;
            if (card1 == null) return;
            Zone desktopzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Desktop) == true);
            if (desktopzone == null) return;
            ctx.World.MoveCards(ev.Source, new List<Card>() { card0, card1 }, desktopzone, ev);
            PointBattlePreviewEvent ev1 = new PointBattlePreviewEvent();
            ev1.Reason = ev.Reason;
            ev1.Source = ev.Source;
            ev1.Target = ev.Target;
            ev1.SourceCard = card0;
            ev1.TargetCard = card1;
            ctx.World.InvokeEventAfterEvent(ev1, ev);
        }
    }
    
}


