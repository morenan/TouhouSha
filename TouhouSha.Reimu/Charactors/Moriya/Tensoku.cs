using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Triggers;
using TouhouSha.Koishi.Cards;
using TouhouSha.Reimu.Filters;
using TouhouSha.Reimu.Triggers;

namespace TouhouSha.Reimu.Charactors.Moriya
{
    /// <summary>
    /// 角色【非想天则】
    /// </summary>
    /// <remarks>
    /// HP:4 守矢
    /// 【机甲】锁定技，游戏开始时，你将牌堆顶4张牌放置到你的角色牌上，视作【能量】。你与其他角色拼点时，只能使用【能量】来作为拼点的牌。
    /// 【充能】出牌阶段开始时，你可以放弃出牌阶段，将牌堆顶的牌放置为【能量】直到4个。
    /// 【锁定】出牌阶段，你选择一名角色进行拼点，若你赢，本回合其非锁定技失效，并且不能使用或打出与你拼点的牌不同花色的牌。
    /// 【霰弹】出牌阶段，你选择一名角色进行拼点，若你赢，视作你对其无视距离使用了一张【杀】。此【杀】不计入出杀的次数。
    /// </remarks>
    public class Tensoku : Charactor
    {
        public const string Zone_Power = "能量";

        static public Zone GetPowerZone(Player player)
        {
            Zone powerzone = player.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone_Power) == true);
            if (powerzone == null)
            {
                powerzone = new Zone();
                powerzone.KeyName = Zone_Power;
                powerzone.Flag |= Enum_ZoneFlag.LabelOnPlayer;
                powerzone.Owner = player;
                player.Zones.Add(powerzone);
            }
            return powerzone;
        }

        public Tensoku()
        {
            MaxHP = 4;
            HP = 4;
            Skills.Add(new Tensoku_Skill_0());
            Skills.Add(new Tensoku_Skill_1());
            Skills.Add(new Tensoku_Skill_2());
            Skills.Add(new Tensoku_Skill_3());
        }

        public override Charactor Clone()
        {
            return new Tensoku();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "非想天则";
            info.AbilityRadar = new AbilityRadar() { Attack = 5, Defence = 1, Control = 3, Auxiliary = 1, LastStages = 3, Difficulty = 3 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }
    }

    public class Tensoku_Skill_0 : Skill
    {
        public Tensoku_Skill_0()
        {
            IsLocked = true;
            Triggers.Add(new Tensoku_Skill_0_Trigger_0(this));
            Triggers.Add(new Tensoku_Skill_0_Trigger_1(this));
            Triggers.Add(new Tensoku_Skill_0_Trigger_2(this));
        }

        public override Skill Clone()
        {
            return new Tensoku_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Tensoku_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "机甲",
                Description = "锁定技：你的回合结束阶段，你将体力上限减为1，并摸减去的数量的牌。"
            };
        }
    }

    public class Tensoku_Skill_0_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public Tensoku_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Tensoku_Skill_0_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.GameStart; }
        
        int ITriggerInState.StateStep { get => StateChangeEvent.Step_BeforeStart; }

        public override void Action(Context ctx)
        {
            Zone powerzone = Tensoku.GetPowerZone(skill.Owner);
            ctx.World.MoveCards(skill.Owner, ctx.World.GetDrawTops(4), powerzone, null);
        }
    }

    public class Tensoku_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Tensoku_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            return true;
        }




    }
    
    public class Tensoku_Skill_0_Trigger_1 : SkillTrigger, ITriggerInEvent
    {
        public Tensoku_Skill_0_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Tensoku_Skill_0_Trigger_1_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => UseFilterEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            base.Action(ctx);
            if (!(ctx.Ev is UseFilterEvent)) return;
            UseFilterEvent ev = (UseFilterEvent)(ctx.Ev);
            if (ev.NewFilter is PointBattleConditionFilter)
            {
                PointBattleConditionFilter filter = (PointBattleConditionFilter)(ev.NewFilter);
                ev.NewFilter = new Tensoku_Skill_0_PointBattleCondition(skill);
                return;
            }
            if (ev.NewFilter is PointBattleTargetSelector)
            {
                PointBattleTargetSelector filter = (PointBattleTargetSelector)(ev.NewFilter);
                ev.NewFilter = new PointBattleTargetSelector2() { OldFilter = filter };
                return;
            }
        }

    }

    public class Tensoku_Skill_0_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Tensoku_Skill_0_Trigger_1_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is UseFilterEvent)) return false;
            UseFilterEvent ev = (UseFilterEvent)(ctx.Ev);
            if (ev.NewFilter is PointBattleConditionFilter)
            {
                PointBattleConditionFilter filter = (PointBattleConditionFilter)(ev.NewFilter);
                if (filter.Skill.Owner != skill.Owner) return false;
                return true;
            }
            if (ev.NewFilter is PointBattleTargetSelector)
            {
                PointBattleTargetSelector filter = (PointBattleTargetSelector)(ev.NewFilter);
                return true;
            }
            return false;
        }
    }

    public class Tensoku_Skill_0_PointBattleCondition : ConditionFilterFromSkill
    {
        public Tensoku_Skill_0_PointBattleCondition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            Zone powerzone = Tensoku.GetPowerZone(skill.Owner);
            if (powerzone.Cards.Count() == 0) return false;
            return true;
        }
    }
    
    public class Tensoku_Skill_0_Trigger_2 : SkillTrigger, ITriggerInEvent
    {
        public Tensoku_Skill_0_Trigger_2(Skill _skill) : base(_skill)
        {
            Condition = new Tensoku_Skill_0_Trigger_2_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => UseTriggerEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is UseTriggerEvent)) return;
            UseTriggerEvent ev = (UseTriggerEvent)(ctx.Ev);
            PointBattleTrigger2 newtrigger = new PointBattleTrigger2();
            newtrigger.OldTrigger = ev.NewTrigger;
            ev.NewTrigger = newtrigger;
        }
    }

    public class Tensoku_Skill_0_Trigger_2_Condition : ConditionFilterFromSkill
    {
        public Tensoku_Skill_0_Trigger_2_Condition(Skill _skill) : base(_skill)
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
                    if (overrides is PointBattleTrigger2) return false;
                    if (overrides.OldTrigger is PointBattleTrigger) return true;
                    overrides = overrides.OldTrigger as OverrideTrigger;
                }
            }
            return false;
        }
    }
    
    public class Tensoku_Skill_1 : Skill
    {
        public Tensoku_Skill_1()
        {
            Triggers.Add(new Tensoku_Skill_1_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Tensoku_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Tensoku_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "充能",
                Description = "出牌阶段开始时，你可以放弃出牌阶段，将牌堆顶的牌放置为【能量】直到4个。"
            };
        }
    }

    public class Tensoku_Skill_1_Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Tensoku_Skill_1_Trigger_0(Skill _skill) : base(_skill)
        {

        }

        string ITriggerInState.StateKeyName { get => State.UseCard; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_BeforeStart; }

        string ITriggerAsk.Message { get => "是否要发动【充能】？"; }
       
        Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            state.Step = StateChangeEvent.Step_AfterEnd;
            Zone powerzone = Tensoku.GetPowerZone(skill.Owner);
            ctx.World.MoveCards(skill.Owner, ctx.World.GetDrawTops(4 - powerzone.Cards.Count()), powerzone, null);
        }
    }

    public class Tensoku_Skill_1_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Tensoku_Skill_1_Trigger_0_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            Zone powerzone = Tensoku.GetPowerZone(skill.Owner);
            if (powerzone.Cards.Count() >= 4) return false;
            return true;
        }
    }
    
    public class Tensoku_Skill_2 : Skill, ISkillInitative
    {
        public const string SkillDisabled = "的非锁定技本回合失效";
        public const string CardColorOnly = "仅能打出此种花色的牌";
        
        public Tensoku_Skill_2()
        {
            this.usecondition = new Tensoku_Skill_0_PointBattleCondition(this);
            this.targetfilter = new PointBattleTargetSelector(this);
            this.costfilter = new Tensoku_Skill_2_CostFilter(this);
            Triggers.Add(new Tensoku_Skill_2_Trigger_0(this));
            Triggers.Add(new Tensoku_Skill_2_Trigger_1(this));
            Triggers.Add(new Tensoku_Skill_2_Trigger_2(this));
            Triggers.Add(new Tensoku_Skill_2_Trigger_3(this));
        }

        private Tensoku_Skill_0_PointBattleCondition usecondition;
        ConditionFilter ISkillInitative.UseCondition { get => usecondition; }

        private PointBattleTargetSelector targetfilter;
        PlayerFilter ISkillInitative.TargetFilter { get => targetfilter; }

        private Tensoku_Skill_2_CostFilter costfilter;
        CardFilter ISkillInitative.CostFilter { get => costfilter; }
        
        void ISkillInitative.Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
        {
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = this;
            ev_skill.Source = skilluser;
            ev_skill.Targets.Clear();
            ev_skill.Targets.AddRange(targets);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            PointBattleBeginEvent ev_battle = new PointBattleBeginEvent();
            ev_battle.Reason = ev_skill;
            ev_battle.Source = skilluser;
            ev_battle.Target = ev_skill.Targets.FirstOrDefault();
            if (ev_battle.Target == null) return;
            ctx.World.InvokeEvent(ev_battle);
        }
        public override Skill Clone()
        {
            return new Tensoku_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new Tensoku_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "锁定",
                Description = "出牌阶段，你选择一名角色进行拼点，若你赢，本回合其非锁定技失效，并且不能使用或打出与你拼点的牌不同花色的牌。"
            };
        }
    }
    
    public class Tensoku_Skill_2_CostFilter : CardFilterFromSkill
    {
        public Tensoku_Skill_2_CostFilter(Skill _skill) : base(_skill)
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

    public class Tensoku_Skill_2_Trigger_0 : SkillTrigger, ITriggerInEvent
    {
        public Tensoku_Skill_2_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Tensoku_Skill_2_Trigger_0_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => PointBattleDoneEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return;
            if (state.Ev == null) return;
            if (!(ctx.Ev is PointBattleDoneEvent)) return;
            PointBattleDoneEvent ev = (PointBattleDoneEvent)(ctx.Ev);
            if (ev.IsWin(ev.Source))
            {
                state.Ev.SetValue(ev.Target.KeyName + Tensoku_Skill_2.SkillDisabled, 1);
                if (ev.SourceCard?.CardColor != null)
                    state.Ev.SetValue(ev.Target.KeyName + Tensoku_Skill_2.CardColorOnly, (int)(ev.SourceCard.CardColor.E)); 
            }
        }
    }
   
    public class Tensoku_Skill_2_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Tensoku_Skill_2_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Ev == null) return false;
            if (!(ctx.Ev is PointBattleDoneEvent)) return false;
            PointBattleDoneEvent ev = (PointBattleDoneEvent)(ctx.Ev);
            if (!(ev.Reason is SkillEvent)) return false;
            SkillEvent ev_skill = (SkillEvent)(ev.Reason);
            if (ev_skill.Skill != skill) return false;
            return true;
        }
    }

    public class Tensoku_Skill_2_Trigger_1 : SkillTrigger, ITriggerInEvent
    {
        public Tensoku_Skill_2_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Tensoku_Skill_2_Trigger_1_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => SkillEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is SkillEvent)) return;
            SkillEvent ev = (SkillEvent)(ctx.Ev);
            ev.Cancel = true;
        }
    }
    
    public class Tensoku_Skill_2_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Tensoku_Skill_2_Trigger_1_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Ev == null) return false;
            if (!(ctx.Ev is SkillEvent)) return false;
            SkillEvent ev = (SkillEvent)(ctx.Ev);
            if (ev.Skill.IsLocked) return false;
            if (state.Ev.GetValue(ev.Skill.Owner.KeyName + Tensoku_Skill_2.SkillDisabled) != 1) return false;
            return true;
        }
    }
    
    public class Tensoku_Skill_2_Trigger_2 : SkillTrigger, ITriggerInEvent
    {
        public Tensoku_Skill_2_Trigger_2(Skill _skill) : base(_skill)
        {
            Condition = new Tensoku_Skill_2_Trigger_2_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => UseCalculatorEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is UseCalculatorEvent)) return;
            UseCalculatorEvent ev = (UseCalculatorEvent)(ctx.Ev);
            ev.NewCalculator = new Tensoku_Skill_2_ReturnOldValue();
        }
    }

    public class Tensoku_Skill_2_Trigger_2_Condition : ConditionFilterFromSkill
    {
        public Tensoku_Skill_2_Trigger_2_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Ev == null) return false;
            if (!(ctx.Ev is UseCalculatorEvent)) return false;
            UseCalculatorEvent ev = (UseCalculatorEvent)(ctx.Ev);
            if (ev.NewCalculator is CalculatorFromSkill)
            {
                CalculatorFromSkill skillcalc = (CalculatorFromSkill)(ev.NewCalculator);
                if (skillcalc.Skill.IsLocked) return false;
                if (state.Ev.GetValue(skillcalc.Skill.Owner.KeyName + Tensoku_Skill_2.SkillDisabled) != 1) return false;
                return true;
            }
            return false;
        }
    }

    public class Tensoku_Skill_2_Trigger_3 : SkillTrigger, ITriggerInEvent
    {
        public Tensoku_Skill_2_Trigger_3(Skill _skill) : base(_skill)
        {
            Condition = new Tensoku_Skill_2_Trigger_3_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => UseFilterEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return;
            if (state.Ev == null) return;
            if (!(ctx.Ev is UseFilterEvent)) return;
            UseFilterEvent ev = (UseFilterEvent)(ctx.Ev);
            if (ev.NewFilter is ConditionFilterFromSkill)
            {
                ev.NewFilter = new Tensoku_Skill_2_ReturnFalse();
                return;
            }
            if (ev.NewFilter is CardFilterFromSkill)
            {
                CardFilterFromSkill oldfilter = (CardFilterFromSkill)(ev.NewFilter);
                Enum_CardColor color = (Enum_CardColor)(state.Ev.GetValue(oldfilter.Skill.Owner.KeyName + Tensoku_Skill_2.CardColorOnly));
                ev.NewFilter = new Tensoku_Skill_2_CardColorOnly(color) { OldFilter = oldfilter };
                return;
            }
            if (ev.NewFilter is CardFilterFromCard)
            {
                CardFilterFromCard oldfilter = (CardFilterFromCard)(ev.NewFilter);
                Enum_CardColor color = (Enum_CardColor)(state.Ev.GetValue(oldfilter.Card.Owner.KeyName + Tensoku_Skill_2.CardColorOnly));
                ev.NewFilter = new Tensoku_Skill_2_CardColorOnly(color) { OldFilter = oldfilter };
                return;
            }
        }
    }

    public class Tensoku_Skill_2_Trigger_3_Condition : ConditionFilterFromSkill
    {
        public Tensoku_Skill_2_Trigger_3_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Ev == null) return false;
            if (!(ctx.Ev is UseFilterEvent)) return false;
            UseFilterEvent ev = (UseFilterEvent)(ctx.Ev);
            if (ev.NewFilter is ConditionFilterFromSkill)
            {
                ConditionFilterFromSkill filterskill = (ConditionFilterFromSkill)(ev.NewFilter);
                if (filterskill.Skill.IsLocked) return false;
                if (state.Ev.GetValue(filterskill.Skill.Owner.KeyName + Tensoku_Skill_2.SkillDisabled) != 1) return false;
                return true;
            }
            if (ev.NewFilter is CardFilterFromSkill)
            {
                CardFilterFromSkill filterskill = (CardFilterFromSkill)(ev.NewFilter);
                if (filterskill.Skill?.Owner == null) return false;
                if (state.Ev.GetValue(filterskill.Skill.Owner.KeyName + Tensoku_Skill_2.CardColorOnly) == 0) return false;
                if ((filterskill.GetFlag(ctx) & Enum_CardFilterFlag.ToUse) == Enum_CardFilterFlag.None
                 && (filterskill.GetFlag(ctx) & Enum_CardFilterFlag.ToHandle) == Enum_CardFilterFlag.None) return false;
                return true;
            }
            if (ev.NewFilter is CardFilterFromCard)
            {
                CardFilterFromCard filtercard = (CardFilterFromCard)(ev.NewFilter);
                if (filtercard.Card?.Owner == null) return false;
                if (state.Ev.GetValue(filtercard.Card.Owner.KeyName + Tensoku_Skill_2.CardColorOnly) == 0) return false;
                if ((filtercard.GetFlag(ctx) & Enum_CardFilterFlag.ToUse) == Enum_CardFilterFlag.None
                 && (filtercard.GetFlag(ctx) & Enum_CardFilterFlag.ToHandle) == Enum_CardFilterFlag.None) return false;
                return true;
            }
            return false;
        }
    }

    public class Tensoku_Skill_2_ReturnOldValue : Calculator
    {
        public override int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
        {
            return oldvalue;
        }
    }

    public class Tensoku_Skill_2_ReturnFalse : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            return false;
        }
    }

    public class Tensoku_Skill_2_CardColorOnly : OverrideCardFilter
    {
        public Tensoku_Skill_2_CardColorOnly(Enum_CardColor _color)
        {
            this.color = _color;
        }

        private Enum_CardColor color;
        public Enum_CardColor Color { get { return this.color; } }

        public override Enum_CardFilterFlag GetFlag(Context ctx)
        {
            return OldFilter?.GetFlag(ctx) ?? Enum_CardFilterFlag.None;
        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (want.CardColor?.SeemAs(color) != true) return false;
            return OldFilter?.CanSelect(ctx, selecteds, want) ?? false;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return OldFilter?.Fulfill(ctx, selecteds) ?? false;
        }
    }

    public class Tensoku_Skill_3 : Skill, ISkillInitative
    {
        public Tensoku_Skill_3()
        {
            this.usecondition = new Tensoku_Skill_0_PointBattleCondition(this);
            this.targetfilter = new PointBattleTargetSelector(this);
            this.costfilter = new Tensoku_Skill_2_CostFilter(this);
            Triggers.Add(new Tensoku_Skill_3_Trigger_0(this));
        }

        private Tensoku_Skill_0_PointBattleCondition usecondition;
        ConditionFilter ISkillInitative.UseCondition { get => usecondition; }

        private PointBattleTargetSelector targetfilter;
        PlayerFilter ISkillInitative.TargetFilter { get => targetfilter; }

        private Tensoku_Skill_2_CostFilter costfilter;
        CardFilter ISkillInitative.CostFilter { get => costfilter; }

        void ISkillInitative.Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
        {
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = this;
            ev_skill.Source = skilluser;
            ev_skill.Targets.Clear();
            ev_skill.Targets.AddRange(targets);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            PointBattleBeginEvent ev_battle = new PointBattleBeginEvent();
            ev_battle.Reason = ev_skill;
            ev_battle.Source = skilluser;
            ev_battle.Target = ev_skill.Targets.FirstOrDefault();
            if (ev_battle.Target == null) return;
            ctx.World.InvokeEvent(ev_battle);
        }
        public override Skill Clone()
        {
            return new Tensoku_Skill_3();
        }

        public override Skill Clone(Card newcard)
        {
            return new Tensoku_Skill_3();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "霰弹",
                Description = "出牌阶段，你选择一名角色进行拼点，若你赢，视作你对其无视距离使用了一张【杀】。此【杀】不计入出杀的次数。"
            };
        }
    }

    public class Tensoku_Skill_3_Trigger_0 : SkillTrigger, ITriggerInEvent
    {
        public Tensoku_Skill_3_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Tensoku_Skill_3_Trigger_0_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => PointBattleDoneEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is PointBattleDoneEvent)) return;
            PointBattleDoneEvent ev = (PointBattleDoneEvent)(ctx.Ev);
            if (ev.IsWin(ev.Source))
            {
                Card killcard = ctx.World.GetCardInstance(KillCard.Normal);
                killcard = killcard.Clone();
                killcard.IsVirtual = true;
                killcard.CardColor = new CardColor(Enum_CardColor.None);
                killcard.CardPoint = -1;
                CardEvent ev_card = new CardEvent();
                ev_card.Reason = ev.Reason;
                ev_card.Card = killcard;
                ev_card.Source = ev.Source;
                ev_card.Targets.Clear();
                ev_card.Targets.Add(ev.Target);
                ctx.World.InvokeEvent(ev_card);
            }
        }
    }

    public class Tensoku_Skill_3_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Tensoku_Skill_3_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Ev == null) return false;
            if (!(ctx.Ev is PointBattleDoneEvent)) return false;
            PointBattleDoneEvent ev = (PointBattleDoneEvent)(ctx.Ev);
            if (!(ev.Reason is SkillEvent)) return false;
            SkillEvent ev_skill = (SkillEvent)(ev.Reason);
            if (ev_skill.Skill != skill) return false;
            return true;
        }
    }

}
