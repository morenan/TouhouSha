using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Ghosts
{
    /// <summary>
    /// 角色【西行寺幽幽子】
    /// </summary>
    /// <remarks>
    /// 幽灵 0勾玉
    /// 【春雪】锁定技，游戏开始时，你增加X点体力上限并回复X点体力（X为场上角色限定技的数量）。
    ///     当场上角色使用限定技时，结算完毕后你减少1点体力上限。
    ///     当场上角色限定技恢复到未使用状态时，你增加1点体力上限并回复1点体力。
    /// 【反魂】限定技，当场上角色进入濒死状态时，你可以令该角色将体力恢复至体力上限，并摸和恢复数量相等的牌。
    /// 【死蝶】限定技，当场上角色进入濒死状态时，你可以指定场上一名角色，其流失3点体力。
    /// 【胥梦】限定技，你可以将一名角色翻面，该角色的下一个回合开始阶段，其流失2点体力。
    /// 【墨樱】主公技，出牌阶段限一次，选择场上的你以外的幽灵角色的一个已经发动的限定技，丢弃一张手卡，将其恢复到未使用状态。
    /// </remarks>
    public class Yoyoko : Charactor
    {
        public const string CountryNameOfLeader = "幽灵";

        public Yoyoko()
        {
            HP = 0;
            MaxHP = 0;
            Country = CountryNameOfLeader;
            Skills.Add(new Yoyoko_Skill_0());
            Skills.Add(new Yoyoko_Skill_1());
            Skills.Add(new Yoyoko_Skill_2());
            Skills.Add(new Yoyoko_Skill_3());
            Skills.Add(new Yoyoko_Skill_4());
        }

        public override Charactor Clone()
        {
            return new Yoyoko();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "西行寺幽幽子";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Yuyuko");
            info.AbilityRadar = new AbilityRadar() { Attack = 1, Defence = 1, Control = 5, Auxiliary = 3, LastStages = 1, Difficulty = 5 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }

    }
    
    public class Yoyoko_Skill_0 : Skill
    {
        public Yoyoko_Skill_0()
        {
            IsLocked = true;
            Triggers.Add(new Yoyoko_Skill_0_Trigger_0(this));
            Triggers.Add(new Yoyoko_Skill_0_Trigger_1(this));
            Triggers.Add(new Yoyoko_Skill_0_Trigger_2(this));
        }

        public override Skill Clone()
        {
            return new Yoyoko_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yoyoko_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "春雪",
                Description = "锁定技，游戏开始时，你增加X点体力上限并回复X点体力（X为场上角色限定技的数量）。" +
                    "当场上角色使用限定技时，结算完毕后你减少1点体力上限。" +
                    "当场上角色限定技恢复到未使用状态时，你增加1点体力上限并回复1点体力。"
            };
        }
    }

    public class Yoyoko_Skill_0_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public Yoyoko_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Yoyoko_Skill_0_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.GameStart; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_BeforeStart; }

        public override void Action(Context ctx)
        {
            int number = 0;
            foreach (Player player in ctx.World.Players)
            {
                if (!(player.IsAlive)) continue;
                foreach (Skill playerskill in player.Skills)
                {
                    if (!playerskill.IsOnce) continue;
                    if (playerskill.IsLocked) continue;
                    number++;
                }
            }
            if (number > 0)
            {
                SkillEvent ev = new SkillEvent();
                ev.Skill = skill;
                ev.Source = skill.Owner;
                ev.Targets.Clear();
                ev.Targets.Add(skill.Owner);
                ctx.World.ChangeMaxHp(skill.Owner, number, ev);
                ctx.World.Heal(skill.Owner, skill.Owner, number, HealEvent.Normal, ev);
            }
        }

    }

    public class Yoyoko_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Yoyoko_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill) { }

        public override bool Accept(Context ctx)
        {
            return true;
        }
    }

    public class Yoyoko_Skill_0_Trigger_1 : SkillTrigger, ITriggerInEvent
    {
        public Yoyoko_Skill_0_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Yoyoko_Skill_0_Trigger_1_Condition(skill);
        }
        
        string ITriggerInEvent.EventKeyName { get => SkillDoneEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            ctx.World.ChangeMaxHp(skill.Owner, -1, ctx.Ev);
        }
    }

    public class Yoyoko_Skill_0_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Yoyoko_Skill_0_Trigger_1_Condition(Skill _skill) : base(_skill)
        {
        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is SkillEvent)) return false;
            SkillEvent ev = (SkillEvent)(ctx.Ev);
            if (!ev.Skill.IsOnce) return false;
            if (ev.Skill.IsLocked) return false;
            return true;
        }
    }

    public class Yoyoko_Skill_0_Trigger_2 : SkillTrigger, ITriggerInEvent
    {
        public Yoyoko_Skill_0_Trigger_2(Skill _skill) : base(_skill)
        {
            Condition = new Yoyoko_Skill_0_Trigger_2_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => SkillOnceResetEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            ctx.World.ChangeMaxHp(skill.Owner, 1, ctx.Ev);
        }

    }

    public class Yoyoko_Skill_0_Trigger_2_Condition : ConditionFilterFromSkill
    {
        public Yoyoko_Skill_0_Trigger_2_Condition(Skill _skill) : base(_skill)
        {
        }

        public override bool Accept(Context ctx)
        {
            return true;
        }
    }
    
    public class Yoyoko_Skill_1 : Skill
    {
        public const string Used = "已经被使用过";
        
        public Yoyoko_Skill_1()
        {
            Triggers.Add(new Yoyoko_Skill_1_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Yoyoko_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yoyoko_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "反魂",
                Description = "限定技，当场上角色进入濒死状态时，你可以令该角色将体力恢复至体力上限，恢复被废除的装备栏，并摸6-X张牌（X为恢复的装备栏的数量）。",
            };
        }
    }
   
    public class Yoyoko_Skill_1_Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Yoyoko_Skill_1_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Yoyoko_Skill_1_Trigger_0_Condition(skill);
        }
        
        string ITriggerInState.StateKeyName { get => State.Dying; }
        
        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        string ITriggerAsk.Message { get => "是否要发动【返魂】"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return  skill.Owner; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            SkillEvent ev = new SkillEvent();
            skill.SetValue(Yoyoko_Skill_1.Used, 1);
            ev.Skill = skill;
            ev.Source = skill.Owner;
            ev.Targets.Clear();
            ev.Targets.Add(state.Owner);
            ctx.World.InvokeEvent(ev);
            if (ev.Cancel) return;
            ctx.World.Heal(skill.Owner, state.Owner, state.Owner.MaxHP - state.Owner.HP, HealEvent.Normal, ev);
            SkillDoneEvent ev_done = new SkillDoneEvent(ev);
            ctx.World.InvokeEvent(ev_done);
        }
    }
    
    public class Yoyoko_Skill_1_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Yoyoko_Skill_1_Trigger_0_Condition(Skill _skill) : base(_skill) { }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner == null) return false;
            if (state.Owner.HP > 0) return false;
            return skill.GetValue(Yoyoko_Skill_1.Used) == 0;
        }
    }

    public class Yoyoko_Skill_2 : Skill
    {
        public const string Used = "已经被使用过";

        public Yoyoko_Skill_2()
        {
            Triggers.Add(new Yoyoko_Skill_2_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Yoyoko_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yoyoko_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "死蝶",
                Description = "限定技，当场上角色进入濒死状态时，你可以指定场上一名角色，废除其一个技能。",
            };
        }
    }

    public class Yoyoko_Skill_2_Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Yoyoko_Skill_2_Trigger_0(Skill _skill) : base(_skill)
        {

        }

        string ITriggerInState.StateKeyName { get => State.Dying; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        string ITriggerAsk.Message { get => "是否要发动【死蝶】"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return  skill.Owner; }

        public override void Action(Context ctx)
        {
            ctx.World.SelectPlayer(skill.KeyName, "请选择一个对象。", skill.Owner,
                new FulfillNumberPlayerFilter(1, 1, skill.Owner),
                true, 15,
                (players) =>
                {
                    SkillEvent ev = new SkillEvent();
                    skill.SetValue(Yoyoko_Skill_2.Used, 1);
                    ev.Skill = skill;
                    ev.Source = skill.Owner;
                    ev.Targets.Clear();
                    ev.Targets.AddRange(players);
                    ctx.World.InvokeEvent(ev);
                    if (ev.Cancel) return;
                    ctx.World.Damage(skill.Owner, players.FirstOrDefault(), 3, DamageEvent.Lost, ev);
                    SkillDoneEvent ev_done = new SkillDoneEvent(ev);
                    ctx.World.InvokeEvent(ev_done);
                },
                () => { }); 
        }
    }


    public class Yoyoko_Skill_2_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Yoyoko_Skill_2_Trigger_0_Condition(Skill _skill) : base(_skill) { }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner == null) return false;
            if (state.Owner.HP > 0) return false;
            return skill.GetValue(Yoyoko_Skill_2.Used) == 0;
        }
    }
    
    public class Yoyoko_Skill_3 : Skill, ISkillInitative
    {
        public const string Used = "已经使用过";
        public const string Targeted = "成为胥梦的对象";

        public Yoyoko_Skill_3()
        {
            IsOnce = true;
            IsLocked = false;
            this.usecondition = new Yoyoko_Skill_3_UseCondition(this);
            this.targetfilter = new Yoyoko_Skill_3_TargetFilter(this);
            this.costfilter = new Yoyoko_Skill_3_CostFilter(this);
            Triggers.Add(new Yoyoko_Skill_3_Trigger_0(this));
        }

        private Yoyoko_Skill_3_UseCondition usecondition;
        public ConditionFilter UseCondition { get => usecondition; }

        private Yoyoko_Skill_3_TargetFilter targetfilter;
        public PlayerFilter TargetFilter { get => targetfilter; }

        private Yoyoko_Skill_3_CostFilter costfilter;
        public CardFilter CostFilter { get => costfilter; }
        
        public void Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
        {
            SetValue(Used, 1);
            SkillEvent ev = new SkillEvent();
            ev.Skill = this;
            ev.Source = Owner;
            ev.Targets.Clear();
            ev.Targets.AddRange(targets);
            ctx.World.InvokeEvent(ev);
            if (ev.Cancel) return;
            foreach (Player target in ev.Targets)
            {
                ctx.World.FaceClip(Owner, target, ev);
                target.SetValue(Targeted, 1);
            }
            SkillDoneEvent ev_done = new SkillDoneEvent(ev);
            ctx.World.InvokeEvent(ev_done);
        }

        public override Skill Clone()
        {
            return new Yoyoko_Skill_3();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yoyoko_Skill_3();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "胥梦",
                Description = "限定技，当场上角色进入濒死状态时，你可以指定场上一名角色，其丢弃所有手牌并将角色牌翻面。",
            };
        }
    }

    public class Yoyoko_Skill_3_UseCondition : ConditionFilterFromSkill
    {
        public Yoyoko_Skill_3_UseCondition(Skill _skill) : base(_skill) { }

        public override bool Accept(Context ctx)
        {
            return skill.GetValue(Yoyoko_Skill_3.Used) == 0;
        }
    }

    public class Yoyoko_Skill_3_TargetFilter : PlayerFilterFromSkill
    {
        public Yoyoko_Skill_3_TargetFilter(Skill _skill) : base(_skill) { }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            if (selecteds.Count() >= 1) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() >= 1;
        }
    }

    public class Yoyoko_Skill_3_CostFilter : CardFilterFromSkill
    {
        public Yoyoko_Skill_3_CostFilter(Skill _skill) : base(_skill) { }

        public override Enum_CardFilterFlag GetFlag(Context ctx)
        {
            return Enum_CardFilterFlag.ForceAll;
        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            return false;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return true;
        }
    }
    
    public class Yoyoko_Skill_3_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public Yoyoko_Skill_3_Trigger_0(Skill _skill) : base(_skill)
        {

        }
        
        string ITriggerInState.StateKeyName { get => State.Begin; }
        
        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }
        
        public override void Action(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return;
            if (state.Owner == null) return;
            state.Owner.SetValue(Yoyoko_Skill_3.Targeted, 0);
            ctx.World.Damage(skill.Owner, state.Owner, 2, DamageEvent.Lost, null);
        }
    }
     
    public class Yoyoko_Skill_3_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Yoyoko_Skill_3_Trigger_0_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Owner == null) return false;
            if (state.Owner.GetValue(Yoyoko_Skill_3.Targeted) == 0) return false;
            return true;
        }

    }

    public class Yoyoko_Skill_4 : Skill, ISkillInitative
    {
        public const string Used = "本阶段已经使用过墨樱";

        public Yoyoko_Skill_4()
        {
            IsLeader = true;
            IsLeaderForLeader = true;
            IsLeaderForSlave = false;
            IsLocked = false;
        }

        private Yoyoko_Skill_4_UseCondition usecondition;
        public ConditionFilter UseCondition { get => usecondition; }

        private Yoyoko_Skill_4_TargetFilter targetfilter;
        public PlayerFilter TargetFilter { get => targetfilter; }

        private Yoyoko_Skill_4_CostFilter costfilter;
        public CardFilter CostFilter { get => costfilter; }


        public void Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return;
            state.SetValue(Used, 1);
            SkillEvent ev = new SkillEvent();
            ev.Skill = this;
            ev.Source = skilluser;
            ev.Targets.Clear();
            ev.Targets.AddRange(targets);
            ctx.World.InvokeEvent(ev);
            if (ev.Cancel) return;
            Player target = targets.FirstOrDefault();
            List<Skill> skilllist = target.Skills.Where(_skill =>
            {
                if (!_skill.IsOnce) return false;
                if (_skill.IsLocked) return false;
                if (!(_skill is ISkillOnceKnowned)) return false;
                if (_skill.GetValue(((ISkillOnceKnowned)_skill).UsedProperty) == 0) return false;
                return true;
            }).ToList();
            ctx.World.ShowList(KeyName, "请选择一个技能。", Owner,
                skilllist, 1, false, 15,
                (selecteds) =>
                {
                    Skill selectedskill = selecteds.FirstOrDefault() as Skill;
                    if (selectedskill == null) return;
                    SkillOnceResetEvent ev_reset = new SkillOnceResetEvent();
                    ev_reset.Skill = selectedskill;
                    ev_reset.Source = Owner;
                    ev_reset.Target = target;
                    ctx.World.InvokeEvent(ev_reset);
                }, null);
        }

        public override Skill Clone()
        {
            return new Yoyoko_Skill_4();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yoyoko_Skill_4();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "墨樱",
                Description = "主公技，出牌阶段限一次，选择场上的你以外的幽灵角色的一个已经发动的限定技，丢弃一张手卡，将其恢复到未使用状态。",
            };
        }
    }

    public class Yoyoko_Skill_4_UseCondition : ConditionFilterFromSkill
    {
        public Yoyoko_Skill_4_UseCondition(Skill _skill) : base(_skill) { }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.GetValue(Yoyoko_Skill_4.Used) != 0) return false;
            return true;
        }
    }

    public class Yoyoko_Skill_4_TargetFilter : PlayerFilterFromSkill
    {
        public Yoyoko_Skill_4_TargetFilter(Skill _skill) : base(_skill) { }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            if (selecteds.Count() >= 1) return false;
            if (want == skill.Owner) return false;
            if (want.Skills.FirstOrDefault(_skill =>
            {
                if (!_skill.IsOnce) return false;
                if (_skill.IsLocked) return false;
                if (!(_skill is ISkillOnceKnowned)) return false;
                if (_skill.GetValue(((ISkillOnceKnowned)_skill).UsedProperty) == 0) return false;
                return true;
            }) == null) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() >= 1;
        }
    }

    public class Yoyoko_Skill_4_CostFilter : CardFilterFromSkill
    {
        public Yoyoko_Skill_4_CostFilter(Skill _skill) : base(_skill) { }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() >= 1) return false;
            if (want.Zone?.KeyName?.Equals(Zone.Hand) != true) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return (selecteds.Count() >= 1);
        }
    }
}
