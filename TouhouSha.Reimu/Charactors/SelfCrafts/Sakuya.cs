using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;
using TouhouSha.Core.AIs;
using TouhouSha.Koishi.Cards;
using TouhouSha.Reimu.Charactors.Homos;

namespace TouhouSha.Reimu.Charactors.SelfCrafts
{
    /// <summary>
    /// 角色【十六夜咲夜】
    /// </summary>
    /// <remarks>
    /// 势力：自机（可替换为红魔） 3勾玉
    /// 【时停】：当你受到伤害时发动，这个回合的结束阶段，你将角色牌翻面，并进行一个额外的回合。
    /// 【飞刃】：出牌阶段限一次，你可以将任意两张牌当作【万箭齐发】使用。如果你的角色牌正面朝上，你也作为这张牌的目标，如果你的角色牌背面朝上，则本阶段你可以使用两次该技能。
    /// </remarks>
    public class Sakuya : Charactor
    {
        #region 技能【时停】

        public class Skill_0 : Skill
        {
            public const string DefaultKeyName = "时停";
            public const string Prepared = "准备时停";

            #region 受到伤害

            public class Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
            {
                public const string DefaultKeyName = "时停询问";

                public class TriggerCondition : ConditionFilterFromSkill
                {
                    public TriggerCondition(Skill _skill) : base(_skill)
                    {
                    }

                    public override bool Accept(Context ctx)
                    {
                        State state = ctx.World.GetCurrentState();
                        if (state == null) return false;
                        if (state.Owner != skill.Owner) return false;
                        if (!(state.Ev is DamageEvent)) return false;
                        DamageEvent ev = (DamageEvent)(state.Ev);
                        if (ev.Cancel) return false;
                        if (ev.DamageValue <= 0) return false;
                        if (ev.DamageType?.Equals(DamageEvent.Lost) == true) return false;
                        return true;
                    }
                }

                public Trigger_0(Skill _skill) : base(_skill)
                {
                    KeyName = DefaultKeyName;
                    Condition = new TriggerCondition(skill);
                }

                string ITriggerInState.StateKeyName { get => State.Damaged; }

                int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

                string ITriggerAsk.Message { get => "是否要发动【时停】？"; }

                Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }

                public override void Action(Context ctx)
                {
                    State state = ctx.World.GetCurrentState();
                    if (state == null) return;
                    Player source = state.Owner;
                    SkillEvent ev = new SkillEvent();
                    ev.Reason = state.Ev;
                    ev.Skill = skill;
                    ev.Source = source;
                    ev.Targets.Clear();
                    ev.Targets.Add(source);
                    ctx.World.InvokeEvent(ev);
                    if (ev.Cancel) return;
                    source.SetValue(Skill_0.Prepared, 1);
                }
            }

            #endregion

            #region 额外回合

            public class Trigger_1 : SkillTrigger, ITriggerInState
            {
                public class TriggerCondition : ConditionFilterFromSkill
                {
                    public TriggerCondition(Skill _skill) : base(_skill)
                    {

                    }

                    public override bool Accept(Context ctx)
                    {
                        if (skill.Owner.GetValue(Skill_0.Prepared) != 1) return false;
                        return true;
                    }
                }

                public Trigger_1(Skill _skill) : base(_skill)
                {
                    Condition = new TriggerCondition(skill);
                }

                string ITriggerInState.StateKeyName { get => State.End; }

                int ITriggerInState.StateStep { get => StateChangeEvent.Step_AfterEnd; }

                public override void Action(Context ctx)
                {
                    skill.Owner.SetValue(Skill_0.Prepared, 0);
                    SkillEvent ev0 = new SkillEvent();
                    ev0.Skill = skill;
                    ev0.Source = skill.Owner;
                    ev0.Targets.Clear();
                    ev0.Targets.Add(skill.Owner);
                    GiveExtraPhaseEvent ev1 = new GiveExtraPhaseEvent();
                    ev1.Reason = ev0;
                    ev1.StartState = new State();
                    ev1.StartState.Ev = ev1;
                    ev1.StartState.Owner = skill.Owner;
                    ev1.StartState.KeyName = State.Begin;
                    ev1.StartState.Step = 0;
                    ctx.World.InvokeEvent(ev1);
                }
            }

            #endregion

            public Skill_0()
            {
                KeyName = DefaultKeyName;
                Triggers.Add(new Trigger_0(this));
                Triggers.Add(new Trigger_1(this));
            }

            public override Skill Clone()
            {
                return new Skill_0();
            }

            public override Skill Clone(Card newcard)
            {
                return new Skill_0();
            }

            public override SkillInfo GetInfo()
            {
                return new SkillInfo()
                {
                    Name = "时停",
                    Description = "当你受到伤害时发动，这个回合的结束阶段，你将角色牌翻面，并无视翻面状态进行一个额外的回合。"
                };
            }
        }

        #endregion

        #region 技能【飞刃】

        public class Skill_1 : Skill, ISkillCardConverter
        {
            public const string Used = "使用过飞刃的次数";
            public const string Symbol = "这张牌是通过飞刃打出的";

            public class SkillUseCondition : ConditionFilterFromSkill
            {
                public SkillUseCondition(Skill _skill) : base(_skill)
                {

                }

                public override bool Accept(Context ctx)
                {
                    State state = ctx.World.GetPlayerState();
                    if (state?.Owner == null) return false;
                    if (state.GetValue(Skill_1.Used) >= 1 + (state.Owner.IsFacedDown ? 1 : 0)) return false;
                    Zone handzone = state.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                    if (handzone == null) return false;
                    if (handzone.Cards.Count() < 2) return false;
                    return true;
                }
            }

            public class SkillCardFilter : CardFilterFromSkill
            {
                public SkillCardFilter(Skill _skill) : base(_skill)
                {

                }

                public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
                {
                    if (selecteds.Count() >= 2) return false;
                    if (want.Zone?.KeyName?.Equals(Zone.Hand) != true) return false;
                    return true;
                }

                public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
                {
                    return selecteds.Count() >= 2;
                }

            }

            public class SkillCardCalculator : CardCalculatorFromSkill
            {
                public SkillCardCalculator(Skill _skill) : base(_skill)
                {

                }

                public override Card GetCombine(Context ctx, IEnumerable<Card> oldvalue)
                {
                    Card arrowall = ctx.World.GetCardInstance(ArrowAllCard.Normal);
                    arrowall = arrowall.Clone(oldvalue);
                    arrowall.SetValue(Skill_1.Symbol, 1);
                    return arrowall;
                }
            }

            #region 增加自己为目标

            public class Trigger_0 : SkillTrigger, ITriggerInEvent
            {
                public class TriggerCondition : ConditionFilterFromSkill
                {
                    public TriggerCondition(Skill _skill) : base(_skill) { }

                    public override bool Accept(Context ctx)
                    {
                        if (skill.Owner.IsFacedDown) return false;
                        if (!(ctx.Ev is CardEvent)) return false;
                        CardEvent ev = (CardEvent)(ctx.Ev);
                        if (ev.Card?.GetValue(Skill_1.Symbol) != 1) return false;
                        return true;
                    }
                }

                public Trigger_0(Skill _skill) : base(_skill) { }

                public override void Action(Context ctx)
                {
                    if (!(ctx.Ev is CardEvent)) return;
                    CardEvent ev = (CardEvent)(ctx.Ev);
                    ev.Targets.Add(skill.Owner);
                }

                string ITriggerInEvent.EventKeyName { get => CardEvent.DefaultKeyName; }

            }

            #endregion

            public Skill_1()
            {
                this.usecondition = new SkillUseCondition(this);
                this.cardfilter = new SkillCardFilter(this);
                this.cardconverter = new SkillCardCalculator(this);
                Triggers.Add(new Trigger_0(this));
            }

            public override Skill Clone()
            {
                return new Skill_1();
            }

            public override Skill Clone(Card newcard)
            {
                return new Skill_1();
            }

            public override SkillInfo GetInfo()
            {
                return new SkillInfo()
                {
                    Name = "飞刃",
                    Description = "出牌阶段限一次，你可以将任意两张牌当作【万箭齐发】使用。如果你的角色牌正面朝上，你也作为这张牌的目标，如果你的角色牌背面朝上，则本阶段你可以使用两次该技能。"
                };
            }

            private SkillUseCondition usecondition;
            ConditionFilter ISkillCardConverter.UseCondition => usecondition;

            private SkillCardFilter cardfilter;
            CardFilter ISkillCardConverter.CardFilter => cardfilter;

            private SkillCardCalculator cardconverter;
            CardCalculator ISkillCardConverter.CardConverter => cardconverter;
        }

        #endregion 

        public class Sakuya_AskWorth : AskWorth
        {
            public override double GetWorthNo(Context ctx, Player controller, string keyname)
            {
                return 0;
            }

            public override double GetWorthYes(Context ctx, Player controller, string keyname)
            {
                switch (keyname)
                {
                    case Skill_0.Trigger_0.DefaultKeyName: return 2;
                }
                return 0;
            }
        }

        public Sakuya()
        {
            HP = 3;
            MaxHP = 3;
            Country = Reimu.CountryNameOfLeader;
            OtherCountries.Add(Remilia.CountryNameOfLeader);
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
        }

        public override Charactor Clone()
        {
            return new Sakuya();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "十六夜咲夜";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Sakuya");
            info.AbilityRadar = new AbilityRadar() { Attack = 4, Defence = 4, Control = 1, Auxiliary = 1, LastStages = 1, Difficulty = 4 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }

        public override AskWorth GetAskWorthAI()
        {
            return new Sakuya_AskWorth();
        }
    }
   

}
