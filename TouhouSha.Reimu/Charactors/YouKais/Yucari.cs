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
    /// 角色【八云紫】
    /// </summary>
    /// <remarks>
    /// 妖怪 4勾玉
    /// 【隙间】：你的回合开始时，你进行一次判定，如果角色牌上没有和判定同花色的牌，将判定牌放置在角色牌上作为【隙】，然后可以选择再发动【隙间】。
    /// 【裂隙】：锁定技，其他角色计算与你的距离时+X，X为你的【隙】的数量。
    /// 【神隐】：觉醒技，当你【隙】的数量不小于四张时，你减少一点体力上限，并获得【境界】。
    /// 【境界】：出牌阶段，你可以将两张【隙】加入手牌，直到回合结束，所有与第一张【隙】相同名称的牌视为第二张【隙】。
    /// 【跋扈】：主公技，其他妖怪势力造成伤害后，可令你进行一次判定，如果角色牌上没有和判定同花色的牌，将判定牌放置在角色牌上作为【隙】。
    /// </remarks>
    public class Yucari : Charactor
    {
        public const string CountryNameOfLeader = "妖怪";
        public const string Zone_Rune = "隙";
        
        public Yucari()
        {
            MaxHP = 4;
            HP = 4;
            Country = CountryNameOfLeader;
            Skills.Add(new Yucari_Skill_0());
            Skills.Add(new Yucari_Skill_1());
            Skills.Add(new Yucari_Skill_2());
        }

        public override Charactor Clone()
        {
            return new Yucari();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "八云紫";
            info.AbilityRadar = new AbilityRadar() { Attack = 4, Defence = 4, Control = 4, Auxiliary = 2, LastStages = 5, Difficulty = 4 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Yucari");
            info.Skills.Add(new SkillInfo()
            {
                Name = "隙间",
                Description = "你的回合开始时，你进行一次判定，如果角色牌上没有和判定同花色的【隙】，将判定牌作为【隙】放置在角色牌上，然后可以选择再发动【隙间】。"
            });
            info.Skills.Add(new SkillInfo()
            {
                Name = "神隐",
                Description = "锁定技，其他角色计算与你的距离时+X，X为你的【隙】的数量。"
            });
            SkillInfo skill_2 = new SkillInfo()
            {
                Name = "贤者",
                Description = "觉醒技，当你【隙】的数量不小于四张时，你减少一点体力上限，并获得【境界】。"
            };
            skill_2.AttachedSkills.Add(new SkillInfo()
            {
                Name = "境界",
                Description = "出牌阶段，你可以将两张【隙】加入手牌，直到回合结束，场上所有与第一张【隙】相同名称的牌视作第二张【隙】。"
            });
            info.Skills.Add(skill_2);
            info.Skills.Add(new SkillInfo()
            {
                Name = "跋扈",
                Description = "主公技，其他妖怪势力造成伤害后，可令你进行一次判定，如果角色牌上没有和判定同花色的牌，将判定牌放置在角色牌上作为【隙】。"
            });
            return info;
        }

    }

    public class Yucari_Skill_0 : Skill
    {
        public Yucari_Skill_0()
        {
            Triggers.Add(new Yucari_Skill_0_Trigger_0(this));
            Triggers.Add(new Yucari_Skill_0_Trigger_1(this));
            Triggers.Add(new Yucari_Skill_0_Trigger_2(this));
        }

        public override Skill Clone()
        {
            return new Yucari_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yucari_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "隙间",
                Description = "你的回合开始时，你进行一次判定，如果角色牌上没有和判定同花色的牌，将判定牌放置在角色牌上作为【隙】，然后可以选择再发动【隙间】。"
            };
        }
    }

    public class Yucari_Skill_0_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public Yucari_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Yucari_Skill_0_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.GameStart; }

        int ITriggerInState.StateStep { get => 0; }

        public override void Action(Context ctx)
        {
            Zone runezone = new Zone();
            runezone.Owner = skill.Owner;
            runezone.KeyName = Yucari.Zone_Rune;
            runezone.Flag = Enum_ZoneFlag.LabelOnPlayer;
            runezone.Owner.Zones.Add(runezone);
        }
    }

    public class Yucari_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Yucari_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            return true;
        }
    }
    
    public class Yucari_Skill_0_Trigger_1 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Yucari_Skill_0_Trigger_1(Skill _skill) : base(_skill)
        {
            KeyName = skill.KeyName;
            Condition = new Yucari_Skill_0_Trigger_1_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Begin; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        string ITriggerAsk.Message { get => "是否要发动【隙间】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return  skill.Owner; }

        public override void Action(Context ctx)
        {
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            JudgeEvent ev_judge = new JudgeEvent();
            ev_judge.Reason = ev_skill;
            ev_judge.JudgeNumber = 1;
            ev_judge.JudgeTarget = skill.Owner;
            ctx.World.InvokeEvent(ev_judge);
        }
    }

    public class Yucari_Skill_0_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Yucari_Skill_0_Trigger_1_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            return true;
        }
    }

    public class Yucari_Skill_0_Trigger_2 : SkillTrigger, ITriggerInState
    {
        public Yucari_Skill_0_Trigger_2(Skill _skill) : base(_skill)
        {
            KeyName = skill.KeyName;
            Condition = new Yucari_Skill_0_Trigger_2_Condition(skill);
        }
        
        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (state.Owner != skill.Owner) return;
            if (!(state.Ev is JudgeEvent)) return;
            JudgeEvent ev_judge = (JudgeEvent)(state.Ev);
            if (ev_judge.JudgeCards.Count() == 0) return;
            Zone runezone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Yucari.Zone_Rune) == true);
            if (runezone == null) return;
            if (runezone.Cards.FirstOrDefault(_card => _card.CardColor?.E == ev_judge.JudgeCards[0].CardColor?.E) != null) return;
            ctx.World.MoveCard(skill.Owner, ev_judge.JudgeCards[0], runezone, ev_judge);
            if (ctx.World.Ask(skill.Owner, skill.KeyName, "是否要再次发动【隙间】？"))
            {
                JudgeEvent ev_nextjudge = new JudgeEvent();
                ev_nextjudge.Reason = ev_judge.Reason;
                ev_nextjudge.JudgeTarget = ev_judge.JudgeTarget;
                ev_nextjudge.JudgeNumber = 1;
                ev_nextjudge.JudgeCards.Clear();
                ctx.World.InvokeEventAfterEvent(ev_nextjudge, ev_judge);
            }
        }
    }

    public class Yucari_Skill_0_Trigger_2_Condition : ConditionFilterFromSkill
    {
        public Yucari_Skill_0_Trigger_2_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            if (!(state.Ev is JudgeEvent)) return false;
            JudgeEvent ev_judge = (JudgeEvent)(state.Ev);
            if (!(ev_judge.Reason is SkillEvent)) return false;
            SkillEvent ev_skill = (SkillEvent)(state.Ev);
            if (ev_skill.Skill != skill) return false;
            return true;
        }
    }
    
    public class Yucari_Skill_1 : Skill
    {
        public Yucari_Skill_1()
        {
            IsLocked = true;
            Calculators.Add(new Yucari_Skill_1_Calculator(this));
        }

        public override Skill Clone()
        {
            return new Yucari_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yucari_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "裂隙",
                Description = "锁定技，其他角色计算与你的距离时+X，X为你的【隙】的数量。"
            };
        }
    }
   
    public class Yucari_Skill_1_Calculator : CalculatorFromSkill, ICalculatorProperty
    {
        public Yucari_Skill_1_Calculator(Skill _skill) : base(_skill)
        {
                    
        }

        string ICalculatorProperty.PropertyName { get => World.DistancePlus; }

        public override int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
        {
            if (obj != skill.Owner) return oldvalue;
            Zone runezone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Yucari.Zone_Rune) == true);
            if (runezone == null) return oldvalue;
            return oldvalue + runezone.Cards.Count();
        }
    }

    public class Yucari_Skill_2 : Skill
    {
        public Yucari_Skill_2()
        {
            IsOnce = true;
            IsLocked = true;
            Triggers.Add(new Yucari_Skill_2_Trigger_0(this));
        }
        public override Skill Clone()
        {
            return new Yucari_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yucari_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "神隐",
                Description = "觉醒技，当你【隙】的数量不小于四张时，你减少一点体力上限，并获得【境界】。"
            };
        }
    }

    public class Yucari_Skill_2_Trigger_0 : SkillTrigger, ITriggerInEvent
    {
        public Yucari_Skill_2_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Yucari_Skill_2_Trigger_0_Condition(skill);    
        }

        string ITriggerInEvent.EventKeyName { get => MoveCardDoneEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            Player player = skill.Owner;
            player.MaxHP--;
            player.Skills.Remove(skill);
            player.Skills.Add(new Yucari_Skill_3());
        }
    }

    public class Yucari_Skill_2_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Yucari_Skill_2_Trigger_0_Condition(Skill _skill) : base(_skill)
        {
               
        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is MoveCardDoneEvent)) return false;
            MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
            if (ev.NewZone.KeyName?.Equals(Yucari.Zone_Rune) != true) return false;
            if (ev.NewZone.Owner != skill.Owner) return false;
            if (ev.NewZone.Cards.Count() < 3) return false;
            return true;
        }
    }
    
    public class Yucari_Skill_3 : Skill, ISkillInitative
    {
        public Yucari_Skill_3()
        {
            IsLocked = false;
            this.usecondition = new Yucari_Skill_3_UseCondition(this);
            this.targetfilter = new Yucari_Skill_3_PlayerFilter(this);
            this.costfilter = new Yucari_Skill_3_CostFilter(this);
        }
        
        private Yucari_Skill_3_UseCondition usecondition;
        public ConditionFilter UseCondition { get => this.usecondition; }

        private Yucari_Skill_3_PlayerFilter targetfilter;
        public PlayerFilter TargetFilter { get => this.targetfilter; }

        private Yucari_Skill_3_CostFilter costfilter;
        public CardFilter CostFilter { get => this.costfilter; }

        public void Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
        {
            Zone runezone = Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Yucari.Zone_Rune) == true);
            if (runezone == null) return;
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = this;
            ev_skill.Source = skilluser;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skilluser);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            FulfillNumberCardFilter cardfilter = new FulfillNumberCardFilter(1, 1);
            cardfilter.Allow_Equiped = false;
            cardfilter.Allow_Hand = false;
            cardfilter.Allow_Judging = false;
            cardfilter.Allow_OtherZones.Add(runezone);
            ctx.World.RequireCard(KeyName, "请选择第一张卡（被转化的卡）。", Owner,
                cardfilter, false, 15,
                (card0s) =>
                {
                    Card card0 = card0s.FirstOrDefault();
                    ctx.World.RequireCard(KeyName, "请选择第二张卡（转化成的卡）。", Owner,
                        new Yucari_Skill_3_SelectConvertTo(card0), false, 15,
                        (card1s) =>
                        {
                            Card card1 = card1s.FirstOrDefault();
                            Zone handzone = Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                            if (handzone == null) return;
                            ctx.World.MoveCards(Owner, new List<Card>() { card0, card1 }, handzone, null);
                            
                        }, null);
                }, null);
        }
        public override Skill Clone()
        {
            return new Yucari_Skill_3();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yucari_Skill_3();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "境界",
                Description = "出牌阶段，你可以将两张【隙】加入手牌，直到回合结束，所有与第一张【隙】相同名称的牌视为第二张【隙】。"
            };
        }
    }

    public class Yucari_Skill_3_UseCondition : ConditionFilterFromSkill
    {
        public Yucari_Skill_3_UseCondition(Skill _skill) : base(_skill) { }

        public override bool Accept(Context ctx)
        {
            Zone runezone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Yucari.Zone_Rune) == true);
            if (runezone == null) return false;
            if (runezone.Cards.Count() < 2) return false;
            return true;
        }
    }

    public class Yucari_Skill_3_PlayerFilter : PlayerFilterFromSkill
    {
        public Yucari_Skill_3_PlayerFilter(Skill _skill) : base(_skill) { }

        public override Enum_PlayerFilterFlag GetFlag(Context ctx)
        {
            return Enum_PlayerFilterFlag.ForceAll;
        }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            return want == skill.Owner;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return true;
        }
    }

    public class Yucari_Skill_3_CostFilter : CardFilterFromSkill
    {
        public Yucari_Skill_3_CostFilter(Skill _skill) : base(_skill) { }

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
    
    public class Yucari_Skill_3_SelectConvertTo : CardFilter
    {
        public Yucari_Skill_3_SelectConvertTo(Card _firstcard) { this.firstcard = _firstcard; }

        private Card firstcard;
        public Card FirstCard { get { return this.firstcard; } }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() >= 1) return false;
            if (want == firstcard) return false;
            if (want.Zone?.KeyName?.Equals(Yucari.Zone_Rune) != true) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return selecteds.Count() >= 1;
        }
    }

    public class Yucari_Skill_3H : Skill, ISkillCardConverter
    {
        static public readonly Dictionary<string, Card> SeemAs = new Dictionary<string, Card>();

        public Yucari_Skill_3H()
        {
            IsHidden = true;
            IsLocked = true;
            this.usecondition = new Yucari_Skill_3H_UseCondition(this);
            this.cardfilter = new Yucari_Skill_3H_CardFilter(this);
            this.cardconverter = new Yucari_Skill_3H_CardConverter(this);
        }

        private Yucari_Skill_3H_UseCondition usecondition;
        ConditionFilter ISkillCardConverter.UseCondition { get => usecondition; }

        private Yucari_Skill_3H_CardFilter cardfilter;
        CardFilter ISkillCardConverter.CardFilter { get => cardfilter; }

        private Yucari_Skill_3H_CardConverter cardconverter;
        CardCalculator ISkillCardConverter.CardConverter { get => cardconverter; }

        public override Skill Clone()
        {
            return new Yucari_Skill_3H();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yucari_Skill_3H();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }
    }

    public class Yucari_Skill_3H_UseCondition : ConditionFilterFromSkill
    {
        public Yucari_Skill_3H_UseCondition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            return Yucari_Skill_3H.SeemAs.Count() > 0;
        }
    }

    public class Yucari_Skill_3H_CardFilter : CardFilterFromSkill
    {
        public Yucari_Skill_3H_CardFilter(Skill _skill) : base(_skill)
        {

        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() >= 1) return false;
            if (want.OriginCards.Count() > 0) return false;
            if (!Yucari_Skill_3H.SeemAs.ContainsKey(want.KeyName)) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return selecteds.Count() >= 1;
        }
    }
    
    public class Yucari_Skill_3H_CardConverter : CardCalculatorFromSkill
    {
        public Yucari_Skill_3H_CardConverter(Skill _skill) : base(_skill)
        {

        }

        public override Card GetValue(Context ctx, Card oldvalue)
        {
            Card seemas = null;
            if (!Yucari_Skill_3H.SeemAs.TryGetValue(oldvalue.KeyName, out seemas)) return oldvalue;
            seemas = seemas.Clone(oldvalue);
            return seemas;
        }
    }

    public class Yucari_Skill_4 : Skill
    {
        public Yucari_Skill_4()
        {
            IsLeader = true;
            IsLeaderForLeader = false;
            IsLeaderForSlave = true;
            Triggers.Add(new Yucari_Skill_4_Trigger_0(this));
            Triggers.Add(new Yucari_Skill_4_Trigger_1(this));
        }

        public override Skill Clone()
        {
            return new Yucari_Skill_4();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yucari_Skill_4();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "跋扈",
                Description = "主公技，其他妖怪势力造成伤害后，可令你进行一次判定，如果角色牌上没有和判定同花色的牌，将判定牌放置在角色牌上作为【隙】。"
            };
        }
    }

    public class Yucari_Skill_4_Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Yucari_Skill_4_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Yucari_Skill_4_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Damaged; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        string ITriggerAsk.Message { get => "是否要发动【跋扈】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            SkillEvent ev_skill = new SkillEvent();
            Player leader = ctx.World.Players.FirstOrDefault(_player => _player.Charactors.FirstOrDefault(_char => _char is Yucari) != null);
            if (leader == null) return;
            ev_skill.Reason = state.Ev;
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(leader);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            JudgeEvent ev_judge = new JudgeEvent();
            ev_judge.Reason = ev_skill;
            ev_judge.JudgeTarget = leader;
            ev_judge.JudgeNumber = 1;
            ctx.World.InvokeEvent(ev_judge);
        }
    }

    public class Yucari_Skill_4_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Yucari_Skill_4_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (!(state.Ev is DamageEvent)) return false;
            DamageEvent ev = (DamageEvent)(state.Ev);
            if (ev.Source != skill.Owner) return false;
            if (ev.DamageValue <= 0) return false;
            if (ev.DamageType?.Equals(DamageEvent.Lost) == true) return false;
            return true;
        }
    }

    public class Yucari_Skill_4_Trigger_1 : SkillTrigger, ITriggerInState
    {
        public Yucari_Skill_4_Trigger_1(Skill _skill) : base(_skill)
        {
            KeyName = skill.KeyName;
            Condition = new Yucari_Skill_0_Trigger_2_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (!(state.Ev is JudgeEvent)) return;
            JudgeEvent ev_judge = (JudgeEvent)(state.Ev);
            if (ev_judge.JudgeCards.Count() == 0) return;
            Zone runezone = state.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Yucari.Zone_Rune) == true);
            if (runezone == null) return;
            if (runezone.Cards.FirstOrDefault(_card => _card.CardColor?.E == ev_judge.JudgeCards[0].CardColor?.E) != null) return;
            ctx.World.MoveCard(skill.Owner, ev_judge.JudgeCards[0], runezone, ev_judge);
        }
    }

    public class Yucari_Skill_4_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Yucari_Skill_4_Trigger_1_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (!(state.Ev is JudgeEvent)) return false;
            JudgeEvent ev_judge = (JudgeEvent)(state.Ev);
            if (!(ev_judge.Reason is SkillEvent)) return false;
            SkillEvent ev_skill = (SkillEvent)(state.Ev);
            if (ev_skill.Skill != skill) return false;
            if (ev_skill.Source != skill.Owner) return false;
            return true;
        }
    }

}
