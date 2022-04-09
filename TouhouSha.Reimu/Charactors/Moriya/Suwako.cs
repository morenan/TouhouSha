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
using TouhouSha.Reimu.Filters;
using TouhouSha.Reimu.Triggers;

namespace TouhouSha.Reimu.Charactors.Moriya
{
    /// <summary>
    /// 角色【洩矢诹访子】
    /// </summary>
    /// <remarks>
    /// HP:1 守矢
    /// 【信灵】锁定技：你的回合结束阶段，你将体力上限减为1，并摸减去的数量的牌。
    /// 【崇神】锁定技：你拼点使用牌堆顶部的牌来代替。你与其他角色的拼点，若你没赢，你增加一点体力上限并回复一点体力。
    /// 【蛇祸】每回合限一次，当你成为一名其他角色使用牌的目标后，你与该角色进行一次拼点，若你赢，你可以摸一张牌，本回合你可以将一张牌当作任意基本牌使用或者打出。
    /// 【地坤】出牌阶段，你可以与一名角色进行拼点，若你赢，回合结束前你计算与其的距离-1，若你没赢，你选择一项：1.你失去2点体力，2.本回合你不能再发动【地坤】。
    /// </remarks>
    public class Suwako : Charactor
    {
        public Suwako()
        {
            MaxHP = 1;
            HP = 1;
            Country = Kanako.CountryNameOfLeader;
            Skills.Add(new Suwako_Skill_0());
            Skills.Add(new Suwako_Skill_1());
            Skills.Add(new Suwako_Skill_2());
            Skills.Add(new Suwako_Skill_3());
        }

        public override Charactor Clone()
        {
            return new Suwako();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "洩矢诹访子";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Suwako");
            info.AbilityRadar = new AbilityRadar() { Attack = 5, Defence = 2, Control = 1, Auxiliary = 2, LastStages = 4, Difficulty = 2 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }
    }

    public class Suwako_Skill_0 : Skill
    {
        public Suwako_Skill_0()
        {
            IsLocked = true;
            Triggers.Add(new Suwako_Skill_0_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Suwako_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Suwako_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "信灵",
                Description = "锁定技：你的回合结束阶段，你将体力上限减为1，并摸减去的数量的牌。"
            };
        }
    }

    public class Suwako_Skill_0_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public Suwako_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Suwako_Skill_0_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.End; }

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
            int delta = skill.Owner.MaxHP - 1;
            ctx.World.ChangeMaxHp(skill.Owner, -delta, ev_skill);
            ctx.World.DrawCard(skill.Owner, delta, ev_skill);
        }
    }
    
    public class Suwako_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Suwako_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            if (state.Owner.MaxHP <= 1) return false;
            return true;
            
        }
    }

    public class Suwako_Skill_1 : Skill
    {
        public Suwako_Skill_1()
        {
            IsLocked = true;
            Triggers.Add(new Suwako_Skill_1_Trigger_0(this));
            Triggers.Add(new Suwako_Skill_1_Trigger_1(this));
            Triggers.Add(new Suwako_Skill_1_Trigger_2(this));
        }
        public override Skill Clone()
        {
            return new Suwako_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Suwako_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "崇神",
                Description = "锁定技：你拼点使用牌堆顶部的牌来代替。你与其他角色的拼点，若你没赢，你增加一点体力上限并回复一点体力。"
            };
        }

    }
   
    public class Suwako_Skill_1_Trigger_0 : SkillTrigger, ITriggerInEvent
    {
        public Suwako_Skill_1_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Suwako_Skill_1_Trigger_0_Condition(skill);
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
                ev.NewFilter = new Suwako_Skill_1_PointBattleCondition();
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
    
    public class Suwako_Skill_1_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Suwako_Skill_1_Trigger_0_Condition(Skill _skill) : base(_skill)
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

    public class Suwako_Skill_1_PointBattleCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            return true;
        }
    }
    
    public class Suwako_Skill_1_Trigger_1 : SkillTrigger, ITriggerInEvent
    {
        public Suwako_Skill_1_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Suwako_Skill_1_Trigger_1_Condition(skill);
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

    public class Suwako_Skill_1_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Suwako_Skill_1_Trigger_1_Condition(Skill _skill) : base(_skill)
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
    
    public class Suwako_Skill_1_Trigger_2 : SkillTrigger
    {
        public Suwako_Skill_1_Trigger_2(Skill _skill) : base(_skill)
        {
            Condition = new Suwako_Skill_1_Trigger_2_Condition(skill);    
        }

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
            ctx.World.ChangeMaxHp(skill.Owner, 1, ev_skill);
            ctx.World.Heal(skill.Owner, skill.Owner, 1, HealEvent.Normal, ev_skill);
        }
    }

    public class Suwako_Skill_1_Trigger_2_Condition : ConditionFilterFromSkill
    {
        public Suwako_Skill_1_Trigger_2_Condition(Skill _skill) : base(_skill)
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

    public class Suwako_Skill_2 : Skill, ISkillCardMultiConverter, ISkillCardMultiConverter2
    {
        public const string Used = "已经使用过蛇祸";
        public const string Actived = "本回合蛇祸的转换有效";

        public Suwako_Skill_2()
        {
            IsLocked = false;
            Triggers.Add(new Suwako_Skill_2_Trigger_0(this));
            Triggers.Add(new Suwako_Skill_2_Trigger_1(this));
            this.condition = new Suwako_Skill_2_Condition(this);
            this.cardfilter = new Suwako_Skill_2_CardFilter(this);
            this.cardconverter = new Suwako_Skill_2_CardConverter(this);
        }
        
        private Suwako_Skill_2_Condition condition;
        ConditionFilter ISkillCardConverter.UseCondition
        {
            get { return this.condition; }
        }

        private Suwako_Skill_2_CardFilter cardfilter;
        CardFilter ISkillCardConverter.CardFilter
        {
            get { return this.cardfilter; }
        }

        private Suwako_Skill_2_CardConverter cardconverter;
        CardCalculator ISkillCardConverter.CardConverter
        {
            get { return this.cardconverter; }
        }


        IEnumerable<string> ISkillCardMultiConverter.GetCardTypes(Context ctx)
        {
            return enabledcardtypes ?? ctx.World.GetBaseCardKeyNames();
        }

        void ISkillCardMultiConverter.SetSelectedCardType(Context ctx, string cardtype)
        {
            this.selectedcardtype = cardtype;
            cardconverter.SeemAs = ctx.World.GetCardInstance(cardtype);
        }

        private string selectedcardtype;
        string ISkillCardMultiConverter.SelectedCardType
        {
            get
            {
                return this.selectedcardtype;
            }
        }

        private List<string> enabledcardtypes;
        public IEnumerable<string> EnabledCardTypes
        {
            get
            {
                return this.enabledcardtypes;
            }
        }

        void ISkillCardMultiConverter2.SetEnabledCardTypes(Context ctx, IEnumerable<string> cardtypes)
        {
            List<string> typelist = ctx.World.GetBaseCardKeyNames().ToList();
            this.enabledcardtypes = new List<string>();
            foreach (string cardtype in cardtypes)
            {
                if (!typelist.Contains(cardtype)) continue;
                enabledcardtypes.Add(cardtype);
            }
        }

        void ISkillCardMultiConverter2.CancelEnabledCardTypes(Context ctx)
        {
            this.enabledcardtypes = null;
        }
        public override Skill Clone()
        {
            return new Suwako_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new Suwako_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "蛇祸",
                Description = "当你成为一名其他角色使用的牌的目标后，你与该角色进行一次拼点，若你赢，你可以摸一张牌，本回合你可以将一张牌当作任意基本牌使用或者打出。"
            };
        }
    }

    public class Suwako_Skill_2_Trigger_0 : SkillTrigger, ITriggerInEvent
    {
        public Suwako_Skill_2_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Suwako_Skill_0_Trigger_0_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => CardPreviewEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return;
            if (state.Ev == null) return;
            if (!(ctx.Ev is CardPreviewEvent)) return;
            CardPreviewEvent ev = (CardPreviewEvent)(ctx.Ev);
            ctx.Ev.SetValue(Suwako_Skill_2.Used, 1);
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            PointBattleBeginEvent ev_battle = new PointBattleBeginEvent();
            ev_battle.Reason = ev_skill;
            ev_battle.Source = skill.Owner;
            ev_battle.Target = ev.Source;
            ctx.World.InvokeEvent(ev_battle);
        }
    }
    
    public class Suwako_Skill_2_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Suwako_Skill_2_Trigger_0_Condition(Skill _skill) : base(_skill)
        {
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Ev == null) return false;
            //if (state.Ev.GetValue(Suwako_Skill_2.Used) == 1) return false;
            if (!(ctx.Ev is CardPreviewEvent)) return false;
            CardPreviewEvent ev = (CardPreviewEvent)(ctx.Ev);
            if (ev.Source == skill.Owner) return false;
            if (!ev.Targets.Contains(skill.Owner)) return false;
            return true;
        }
    }
    
    public class Suwako_Skill_2_Trigger_1 : SkillTrigger, ITriggerInEvent
    {
        public Suwako_Skill_2_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Suwako_Skill_2_Trigger_1_Condition(skill);
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
                state.Ev.SetValue(Suwako_Skill_2.Actived, 1);
                ctx.World.DrawCard(skill.Owner, 1, ev.Reason);   
            }
        }
    }

    public class Suwako_Skill_2_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Suwako_Skill_2_Trigger_1_Condition(Skill _skill) : base(_skill)
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
    
    public class Suwako_Skill_2_Condition : ConditionFilterFromSkill
    {
        public Suwako_Skill_2_Condition(Skill _skill) : base(_skill)
        {
        }

        public override bool Accept(Context ctx)
        {
            if (!(Skill is ISkillCardMultiConverter)) return false;
            ISkillCardMultiConverter multiconv = (ISkillCardMultiConverter)Skill;
            if (multiconv.GetCardTypes(ctx).Count() == 0) return false;
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Ev == null) return false;
            if (state.Ev.GetValue(Suwako_Skill_2.Actived) != 1) return false;
            return true;
        }
    }

    public class Suwako_Skill_2_CardFilter : CardFilterFromSkill
    {
        public Suwako_Skill_2_CardFilter(Skill _skill) : base(_skill) { }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() >= 1) return false;
            switch (want.Zone?.KeyName)
            {
                case Zone.Hand:
                case Zone.Equips:
                    break;
                default:
                    return false;
            }
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return selecteds.Count() == 1;
        }
    }

    public class Suwako_Skill_2_CardConverter : CardCalculatorFromSkill
    {
        public Suwako_Skill_2_CardConverter(Skill _skill) : base(_skill) { }

        private Card seemas;
        public Card SeemAs
        {
            get { return this.seemas; }
            set { this.seemas = value; }
        }

        public override Card GetValue(Context ctx, Card oldvalue)
        {
            if (seemas == null) return oldvalue;
            Card newcard = seemas.Clone(oldvalue);
            return newcard;
        }
    }

    public class Suwako_Skill_3 : Skill, ISkillInitative
    {
        public const string DistMinus = "地坤计算距离减成";
        public const string CannotActive = "不能再发动地坤";

        public Suwako_Skill_3()
        {
            IsLocked = false;
            this.condition = new Suwako_Skill_3_UseCondition(this);
            this.targetfilter = new Suwako_Skill_3_TargetFilter(this);
            this.costfilter = new Suwako_Skill_3_CostFilter(this);
            Calculators.Add(new Suwako_Skill_3_DistMinus(this));
        }

        private Suwako_Skill_3_UseCondition condition;
        ConditionFilter ISkillInitative.UseCondition { get => condition; }
        
        private Suwako_Skill_3_TargetFilter targetfilter;
        PlayerFilter ISkillInitative.TargetFilter { get => targetfilter; }

        private Suwako_Skill_3_CostFilter costfilter;
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
            ev_battle.Target = targets.FirstOrDefault();
            ctx.World.InvokeEvent(ev_battle);
        }
        public override Skill Clone()
        {
            return new Suwako_Skill_3();
        }

        public override Skill Clone(Card newcard)
        {
            return new Suwako_Skill_3();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "地坤",
                Description = "出牌阶段，你可以与一名角色进行拼点，若你赢，回合结束前你计算与其的距离-1，若你没赢，你选择一项：1.你失去2点体力，2.本回合你不能再发动【地坤】。"
            };
        }
    }

    public class Suwako_Skill_3_UseCondition : ConditionFilterFromSkill
    {
        public Suwako_Skill_3_UseCondition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            if (state.Ev == null) return false;
            if (state.Ev.GetValue(Suwako_Skill_3.CannotActive) == 1) return false;
            return true;
        }
    }
    
    public class Suwako_Skill_3_TargetFilter : PointBattleTargetSelector
    {
        public Suwako_Skill_3_TargetFilter(Skill _skill) : base(_skill)
        {

        }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            if (want.MaxHP <= skill.Owner.MaxHP) return false;   
            return base.CanSelect(ctx, selecteds, want);
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return base.Fulfill(ctx, selecteds);
        }
    }

    public class Suwako_Skill_3_CostFilter : CardFilterFromSkill
    {
        public Suwako_Skill_3_CostFilter(Skill _skill) : base(_skill)
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
    
    public class Suwako_Skill_3_Trigger_0 : SkillTrigger, ITriggerInEvent
    {
        public const string Selection_0 = "失去一点体力。";
        public const string Selection_1 = "本回合不能发动【地坤】。";

        public Suwako_Skill_3_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Suwako_Skill_3_Trigger_0_Condition(skill);
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
                string distminus_propname = String.Format("{0}_{1}", ev.Target.KeyName, Suwako_Skill_3.DistMinus);
                int distminus = state.Ev.GetValue(distminus_propname);
                state.Ev.SetValue(distminus_propname, distminus + 1);
            }
            else
            {
                ctx.World.ShowList("地坤拼输选择项。", "请选择一项。", skill.Owner,
                    new List<string>() { Selection_0, Selection_1 }, 1,
                    false, 15,
                    (selected) =>
                    {
                        switch (selected.ToString())
                        {
                            case Selection_0:
                                ctx.World.Damage(skill.Owner, skill.Owner, 2, DamageEvent.Lost, ev.Reason);
                                break;
                            case Selection_1:
                                state.Ev.SetValue(Suwako_Skill_3.CannotActive, 1);
                                break;
                        }
                    }, null);
            }
            base.Action(ctx);
        }
    }

    public class Suwako_Skill_3_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Suwako_Skill_3_Trigger_0_Condition(Skill _skill) : base(_skill)
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

    public class Suwako_Skill_3_DistMinus : CalculatorFromSkill
    {
        public Suwako_Skill_3_DistMinus(Skill _skill) : base(_skill)
        {
            
        }

        public override int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
        {
            if (obj != skill.Owner) return oldvalue;
            State state = ctx.World.GetPlayerState();
            if (state == null) return oldvalue;
            if (state.Ev == null) return oldvalue;
            switch (propertyname)
            {
                //case World.DistanceMinus:
                //    return oldvalue + state.Ev.GetValue(Suwako_Skill_3.DistMinus);
                default:
                    if (propertyname.EndsWith(World.DistanceMinus))
                    {
                        string target_keyname = propertyname.Substring(0, propertyname.Length - World.DistanceMinus.Length);
                        string distminus_propname = target_keyname + Suwako_Skill_3.DistMinus;
                        return oldvalue + state.Ev.GetValue(distminus_propname);
                    }
                    break;
            }
            return oldvalue;
        }
    }
}
