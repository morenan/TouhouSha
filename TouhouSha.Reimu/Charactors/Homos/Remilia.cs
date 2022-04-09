using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Koishi.Cards;
using TouhouSha.Koishi.Cards.Weapons;
using TouhouSha.Koishi.Triggers;
using TouhouSha.Koishi.Calculators;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Homos
{
    /// <summary>
    /// 角色【蕾米莉亚】
    /// </summary>
    /// <remarks>
    /// 势力：红魔 4勾玉
    /// 【血族】：锁定技，当你造成伤害时，每有一点你可以选择其中一项：1.恢复一点体力，2.摸一张牌。
    /// 【神枪】：锁定技，当你没有装备武器时，视作你装备了【冈格尼尔】。
    /// 【红雾】：转化技，出牌阶段仅一次。
    ///     阳：你对自己造成一点伤害，失去一点体力上限，直到你的下一回合开始前，你不会受到火焰伤害以外的其他伤害。
    ///     阴：你增加一点体力上限，并对所有其他角色造成一点伤害。
    /// 【威严】：主公技，其他红魔角色的回合结束时，你可以视作对其使用一张【杀】。
    /// </remarks>
    public class Remilia : Charactor
    {
        public const string CountryNameOfLeader = "红魔";

        public Remilia()
        {
            HP = 4;
            MaxHP = 4;
            Country = CountryNameOfLeader;
            Skills.Add(new Remilia_Skill_0());
            Skills.Add(new Remilia_Skill_1());
            Skills.Add(new Remilia_Skill_2());
            Skills.Add(new Remilia_Skill_3());
        }

        public override Charactor Clone()
        {
            return new Remilia();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "蕾米莉亚";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Remilia");
            info.AbilityRadar = new AbilityRadar() { Attack = 3.5, Defence = 5, Control = 1, Auxiliary = 1, LastStages = 4, Difficulty = 5 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }

    }
  
    public class Remilia_Skill_0 : Skill
    {
        public Remilia_Skill_0()
        {
            IsLocked = true;
            Triggers.Add(new Remilia_Skill_0_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Remilia_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Remilia_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "血族",
                Description = "锁定技，当你造成伤害时，每有一点你可以选择其中一项：1.恢复一点体力，2.摸一张牌。"
            };
        }
    }
    
    public class Remilia_Skill_0_Trigger_0 : SkillTrigger, ITriggerInEvent
    {
        public const string Selection_0 = "恢复一点体力";
        public const string Selection_1 = "摸一张牌";

        public Remilia_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Remilia_Skill_0_Trigger_0_Condition(skill);
        }
        
        string ITriggerInEvent.EventKeyName { get => DamageEvent.DefaultKeyName; }
        
        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is DamageEvent)) return;
            DamageEvent ev = (DamageEvent)(ctx.Ev);
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            for (int i = 0; i < ev.DamageValue; i++)
            {
                string selection = Selection_1;
                if (skill.Owner.HP < skill.Owner.MaxHP)
                    ctx.World.ShowList(skill.KeyName, "请选择一项：", skill.Owner,
                        new List<object>() { Selection_0, Selection_1 },
                        1, false, 15,
                        (selecteds) => { selection = selecteds.FirstOrDefault().ToString(); },
                        null);
                switch (selection)
                {
                    case Selection_0:
                        ctx.World.Heal(skill.Owner, skill.Owner, 1, HealEvent.Normal, ev_skill);
                        break;
                    case Selection_1:
                        ctx.World.DrawCard(skill.Owner, 1, ev_skill);
                        break;
                }
            }
        }

    }

    public class Remilia_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Remilia_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is DamageEvent)) return false;
            DamageEvent ev = (DamageEvent)(ctx.Ev);
            if (ev.Source != skill.Owner) return false;
            if (ev.DamageType?.Equals(DamageEvent.Lost) == true) return false;
            return true;
        }
    }
   
    public class Remilia_Skill_1 : Skill
    {
        public Remilia_Skill_1()
        {
            IsLocked = true;
            Triggers.Add(new Remilia_Skill_1_Trigger_0(this));
            Calculators.Add(new Remilia_Skill_1_RangeCalculator(this));
        }

        public override Skill Clone()
        {
            return new Remilia_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Remilia_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "神枪",
                Description = "锁定技，当你没有装备武器时，视作你装备了【冈格尼尔】。"
            };
        }
    }

    public class Remilia_Skill_1_RangeCalculator : CalculatorFromSkill, ICalculatorProperty
    {
        string ICalculatorProperty.PropertyName { get => World.KillRange; }

        public Remilia_Skill_1_RangeCalculator(Skill _skill) : base(_skill)
        {
            
        }
        
        public override int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
        {
            switch (propertyname)
            {
                case World.KillRange:
                    if (skill.Owner != obj) return oldvalue;
                    EquipZone equipzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone is EquipZone) as EquipZone;
                    if (equipzone == null) return oldvalue;
                    EquipCell equipcell = equipzone.Cells.FirstOrDefault(_cell => _cell.E == Enum_CardSubType.Weapon);
                    if (equipcell == null) return oldvalue;
                    if (!equipcell.IsEnabled) return oldvalue;
                    if (equipcell.CardIndex >= 0) return oldvalue;
                    return oldvalue + 2;
            }
            return oldvalue;
        }
    }

    public class Remilia_Skill_1_Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Remilia_Skill_1_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Remilia_Skill_1_Trigger_0_Condition(skill);
        }
        
        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start + 1; }

        string ITriggerAsk.Message { get => "是否要发动【冈格尼尔】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return  skill.Owner; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(ctx.Ev);
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Source = ev.Source;
            ev_skill.Skill = skill;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(state.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            ctx.World.DiscardCardCanCancel(ev.Source, 2, ev_skill, true,
                (cards) => { ev.SetValue(MissKillTrigger.HasMissed, 0); },
                () => { });
        }
    }

    public class Remilia_Skill_1_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Remilia_Skill_1_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state?.KeyName?.Equals(State.Handle) != true) return false;
            if (!(state.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (!KillCard.IsKill(ev.Card)) return false;
            if (ev.GetValue(MissKillTrigger.HasMissed) == 0) return false;
            if (skill.Owner != ev.Source) return false;
            EquipZone equipzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone is EquipZone) as EquipZone;
            if (equipzone == null) return false;
            EquipCell equipcell = equipzone.Cells.FirstOrDefault(_cell => _cell.E == Enum_CardSubType.Weapon);
            if (equipcell == null) return false;
            if (!equipcell.IsEnabled) return false;
            if (equipcell.CardIndex >= 0) return false;
            return true;
        }
    }

    public class Remilia_Skill_2 : Skill, ISkillInitative
    {
        public const string Used = "该阶段已经使用了红雾";
        public const string Dark = "阴";
        public const string EvadeNotFire = "避免火焰以外的伤害";

        public Remilia_Skill_2()
        {
            this.usecondition = new Remilia_Skill_2_UseCondition(this);
            this.targetfilter = new Remilia_Skill_2_TargetFilter(this);
            this.costfilter = new Remilia_Skill_2_CostFilter(this);
            Triggers.Add(new Remilia_Skill_2_Trigger_0(this));
            Triggers.Add(new Remilia_Skill_2_Trigger_1(this));
        }

        private Remilia_Skill_2_UseCondition usecondition;
        public ConditionFilter UseCondition { get => usecondition; }

        private Remilia_Skill_2_TargetFilter targetfilter;
        public PlayerFilter TargetFilter { get => targetfilter; }
        
        private Remilia_Skill_2_CostFilter costfilter;
        public CardFilter CostFilter { get => costfilter; }
        
        public void Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
        {
            SkillEvent ev = new SkillEvent();
            ev.Skill = this;
            ev.Source = skilluser;
            ev.Targets.Clear();
            ev.Targets.AddRange(targets);
            ctx.World.InvokeEvent(ev);
            if (ev.Cancel) return;
            int dark = GetValue(Dark);
            SetValue(Dark, dark == 1 ? 0 : 1);
            switch (dark)
            {
                case 0:
                    ctx.World.Damage(skilluser, skilluser, 1, DamageEvent.Normal, ev);
                    ctx.World.ChangeMaxHp(skilluser, -1, ev);
                    Owner.SetValue(EvadeNotFire, 1);
                    break;
                case 1:
                    ctx.World.ChangeMaxHp(skilluser, 1, ev);
                    foreach (Player target in targets)
                        ctx.World.Damage(skilluser, target, 1, DamageEvent.Normal, ev);
                    break;
            }
        }

        public override Skill Clone()
        {
            return new Remilia_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new Remilia_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "红雾",
                Description = "转化技，出牌阶段仅一次。\n" +
                    "\t阳：你对自己造成一点伤害，失去一点体力上限，直到你的下一回合开始前，你不会受到火焰伤害以外的其他伤害。\n" +
                    "\t阴：你增加一点体力上限，并对所有其他角色造成一点伤害。"
            };
        }
    }

    public class Remilia_Skill_2_UseCondition : ConditionFilterFromSkill
    {
        public Remilia_Skill_2_UseCondition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.GetValue(Remilia_Skill_2.Used) != 0) return false;
            return true;
        }
    }

    public class Remilia_Skill_2_TargetFilter : PlayerFilterFromSkill
    {
        public Remilia_Skill_2_TargetFilter(Skill _skill) : base(_skill)
        {

        }

        public override Enum_PlayerFilterFlag GetFlag(Context ctx)
        {
            return Enum_PlayerFilterFlag.ForceAll;
        }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            switch (skill.GetValue(Remilia_Skill_2.Dark))
            {
                case 0:
                    return want == skill.Owner;
                case 1:
                    return want != skill.Owner;
            }
            return false;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return true;
        }
    }

    public class Remilia_Skill_2_CostFilter : CardFilterFromSkill
    {
        public Remilia_Skill_2_CostFilter(Skill _skill) : base(_skill)
        {
            
        }

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
    
    public class Remilia_Skill_2_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public Remilia_Skill_2_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Remilia_Skill_2_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Damaging; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_AfterEnd - 1; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (!(state.Ev is DamageEvent)) return;
            DamageEvent ev = (DamageEvent)(state.Ev);
            ev.DamageValue = 0;
        }
    }
 
    public class Remilia_Skill_2_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Remilia_Skill_2_Trigger_0_Condition(Skill _skill) : base(_skill) { }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            if (skill.Owner.GetValue(Remilia_Skill_2.EvadeNotFire) == 0) return false;
            if (!(state.Ev is DamageEvent)) return false;
            DamageEvent ev = (DamageEvent)(state.Ev);
            if (ev.DamageType?.Equals(DamageEvent.Fire) == true) return false;
            if (ev.DamageType?.Equals(DamageEvent.Lost) == true) return false;
            return true;
        }
    }

    public class Remilia_Skill_2_Trigger_1 : SkillTrigger, ITriggerInState
    {
        public Remilia_Skill_2_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Remilia_Skill_2_Trigger_1_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Begin; }
        
        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        public override void Action(Context ctx)
        {
            skill.Owner.SetValue(Remilia_Skill_2.EvadeNotFire, 0);
        }
    }

    public class Remilia_Skill_2_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Remilia_Skill_2_Trigger_1_Condition(Skill _skill) : base(_skill) { }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            if (skill.Owner.GetValue(Remilia_Skill_2.EvadeNotFire) == 0) return false;
            return true;
        }
    }

    public class Remilia_Skill_3 : Skill
    {
        public Remilia_Skill_3()
        {
            IsLeader = true;
            IsLeaderForLeader = true;
            IsLeaderForSlave = false;
            IsLocked = false;
            Triggers.Add(new Remilia_Skill_3_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Remilia_Skill_3();
        }

        public override Skill Clone(Card newcard)
        {
            return new Remilia_Skill_3();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "威严",
                Description = "主公技，其他红魔角色的回合结束时，你可以视作对其使用一张【杀】。"
            };
        }
    }

    public class Remilia_Skill_3_Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Remilia_Skill_3_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Remilia_Skill_3_Trigger_0_Condition(skill);    
        }

        string ITriggerInState.StateKeyName { get => State.End; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start - 1; }

        string ITriggerAsk.Message { get => "是否要发动【威严】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return  skill.Owner; }
        
        public override void Action(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return;
            if (state.Owner == null) return;
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(state.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            Card kill = ctx.World.GetCardInstance(KillCard.Normal);
            kill = kill.Clone();
            kill.CardColor = new CardColor(Enum_CardColor.None);
            kill.CardPoint = -1;
            kill.IsVirtual = true;
            CardEvent ev = new CardEvent();
            ev.Card = kill;
            ev.Source = ev_skill.Source;
            ev.Targets.Clear();
            ev.Targets.AddRange(ev_skill.Targets);
            ctx.World.InvokeEvent(ev);
        }
    }

    public class Remilia_Skill_3_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Remilia_Skill_3_Trigger_0_Condition(Skill _skill) : base(_skill) { }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Owner == null) return false;
            if (state.Owner == skill.Owner) return false;
            if (state.Owner.Country?.Equals(Remilia.CountryNameOfLeader) != true) return false;
            return true;
        }
    }
}
