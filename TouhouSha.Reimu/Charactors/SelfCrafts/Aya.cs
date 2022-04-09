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

namespace TouhouSha.Reimu.Charactors.SelfCrafts
{
    /// <summary>
    /// 角色【射命丸文】
    /// </summary>
    /// <remarks>
    /// 势力：自机（可替换为天狗） 4勾玉
    /// 【风神】：你可以实行如下操作，并视作对一名其他角色使用一张无视距离的【杀】。
    ///     1. 回合开始时，将你的角色牌翻面。
    ///     2. 判定阶段开始时，当你判定区有牌时，弃置判定区所有的牌。
    ///     3. 摸牌阶段开始时，跳过你的摸牌阶段。    
    ///     4. 出牌阶段开始时，弃置一张装备牌并跳过你的出牌阶段。
    ///     5. 弃牌阶段开始时，弃置至少两张手牌并跳过你的弃牌阶段。
    ///     6. 回合结束前，失去一点体力。
    /// 【新闻】觉醒技，回合结束时，如果你背面朝上并且没有手牌，你减去一点体力上限，恢复一点体力并摸两张牌，获得技能【头条】。
    /// 【头条】一名角色的出牌阶段开始时，若其手牌数为全场最多（之一），你获取其一张手牌并展示，本回合其使用或者打出与这张牌相同类型的牌时，需要丢弃一张相同类型的牌，否则这张牌结算无效。
    /// </remarks>
    public class Aya : Charactor
    {
        #region 技能【风神】

        public class Skill_0 : Skill
        {
            #region 各阶段触发

            public class Trigger_0 : SkillTrigger, ITriggerInAnyState
            {
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
                        Zone judgezone = state.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
                        Zone handzone = state.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                        Zone equipzone = state.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Equips) == true);
                        switch (state?.KeyName)
                        {
                            case State.Begin:
                                if (state.Step != StateChangeEvent.Step_BeforeStart) return false;
                                return true;
                            case State.Judge:
                                if (state.Step != StateChangeEvent.Step_BeforeStart) return false;
                                if (judgezone == null) return false;
                                if (judgezone.Cards.Count() == 0) return false;
                                return true;
                            case State.Draw:
                                if (state.Step != StateChangeEvent.Step_BeforeStart + 1) return false;
                                return true;
                            case State.UseCard:
                                if (state.Step != StateChangeEvent.Step_BeforeStart + 1) return false;
                                if (equipzone != null && equipzone.Cards.Count() > 0) return true;
                                if (handzone != null && handzone.Cards.FirstOrDefault(_card => _card.CardType?.E == Enum_CardType.Equip) != null) return true;
                                return false;
                            case State.Discard:
                                if (state.Step != StateChangeEvent.Step_BeforeStart + 1) return false;
                                if (handzone == null) return false;
                                if (handzone.Cards.Count() < 2) return false;
                                return true;
                            case State.End:
                                if (state.Step != StateChangeEvent.Step_BeforeStart) return false;
                                return true;
                        }
                        return false;
                    }
                }

                public class SelectEquip : CardFilter
                {
                    public class CardWorth : CardFilterWorth
                    {
                        public override double GetWorth(Context ctx, SelectCardBoardCore core, IEnumerable<Card> selecteds, Card want)
                        {
                            return -AITools.WorthAcc.GetWorth(ctx, core.Controller, want);
                        }

                        public override double GetWorthNo(Context ctx, SelectCardBoardCore core)
                        {
                            return 0.0d;
                        }
                    }

                    public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
                    {
                        if (selecteds.Count() >= 1) return false;
                        if (want.CardType?.E != Enum_CardType.Equip) return false;
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
                        return selecteds.Count() >= 1;
                    }

                    public override CardFilterWorth GetWorthAI()
                    {
                        return new CardWorth();
                    }
                }

                public Trigger_0(Skill _skill) : base(_skill)
                {
                    Condition = new TriggerCondition(skill);
                }

                public override void Action(Context ctx)
                {
                    State state = ctx.World.GetCurrentState();
                    if (state == null) return;
                    Zone judgezone = state.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
                    Zone handzone = state.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                    Zone equipzone = state.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Equips) == true);
                    Zone discardzone = state.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
                    switch (state.KeyName)
                    {
                        case State.Begin:
                            if (!ctx.World.Ask(skill.Owner, "风神1", "是否要翻面来出杀？")) return;
                            break;
                        case State.Judge:
                            if (!ctx.World.Ask(skill.Owner, "风神2", "是否丢弃所有判定区的牌来出杀？")) return;
                            break;
                        case State.Draw:
                            if (!ctx.World.Ask(skill.Owner, "风神3", "是否放弃摸牌来出杀？")) return;
                            break;
                        case State.UseCard:
                            if (!ctx.World.Ask(skill.Owner, "风神4", "是否放弃出牌并丢弃装备来出杀？")) return;
                            break;
                        case State.Discard:
                            if (!ctx.World.Ask(skill.Owner, "风神5", "是否弃牌来出杀？")) return;
                            break;
                        case State.End:
                            if (!ctx.World.Ask(skill.Owner, "风神6", "是否卖血来出杀？")) return;
                            break;
                    }
                    Card killcard = ctx.World.GetCardInstance(KillCard.Normal);
                    killcard = killcard.Clone();
                    killcard.IsVirtual = true;
                    killcard.CardColor = new CardColor(Enum_CardColor.None);
                    killcard.CardPoint = -1;
                    UseKillTargetFilterIgnoreRange targetfilter = new UseKillTargetFilterIgnoreRange(killcard);
                    ctx.World.SelectPlayer("风神选择目标", "请选择一个目标。", skill.Owner, targetfilter, false, Config.GameConfig.Timeout_Handle,
                        (targets) =>
                        {
                            SkillEvent ev_skill = new SkillEvent();
                            ev_skill.Skill = skill;
                            ev_skill.Source = skill.Owner;
                            ev_skill.Targets.Clear();
                            ev_skill.Targets.AddRange(targets);
                            ctx.World.InvokeEvent(ev_skill);
                            if (ev_skill.Cancel) return;
                            CardEvent ev_card = new CardEvent();
                            ev_card.Reason = ev_skill;
                            ev_card.Card = killcard;
                            ev_card.Source = ev_skill.Source;
                            ev_card.Targets.Clear();
                            ev_card.Targets.AddRange(ev_skill.Targets);
                            switch (state.KeyName)
                            {
                                case State.Begin:
                                    ctx.World.FaceClip(skill.Owner, skill.Owner, ev_skill);
                                    ctx.World.InvokeEvent(ev_card);
                                    break;
                                case State.Judge:
                                    ctx.World.MoveCards(skill.Owner, judgezone.Cards, discardzone, ev_skill);
                                    ctx.World.InvokeEvent(ev_card);
                                    break;
                                case State.Draw:
                                    state.Step = StateChangeEvent.Step_End;
                                    ctx.World.InvokeEvent(ev_card);
                                    break;
                                case State.UseCard:
                                    ctx.World.RequireCard("风神丢弃装备", "请丢弃一张装备卡。", skill.Owner,
                                        new SelectEquip(), true, Config.GameConfig.Timeout_Handle,
                                        (cards) =>
                                        {
                                            ctx.World.MoveCards(skill.Owner, cards, discardzone, ev_skill);
                                            state.Step = StateChangeEvent.Step_End;
                                            ctx.World.InvokeEvent(ev_card);
                                        }, null);
                                    break;
                                case State.Discard:
                                    ctx.World.RequireCard("风神丢弃手卡", "请丢弃至少两张手卡。", skill.Owner,
                                        new FulfillNumberCardFilter(2, 9999) 
                                        { 
                                            Allow_Hand = true, 
                                            Allow_Equiped = false, 
                                            Allow_Judging = false 
                                        },
                                        true, Config.GameConfig.Timeout_Handle,
                                        (cards) =>
                                        {
                                            ctx.World.MoveCards(skill.Owner, cards, discardzone, ev_skill);
                                            state.Step = StateChangeEvent.Step_End;
                                            ctx.World.InvokeEvent(ev_card);
                                        }, null);
                                    break;
                                case State.End:
                                    ctx.World.Damage(skill.Owner, skill.Owner, 1, DamageEvent.Lost, ev_skill);
                                    ctx.World.InvokeEvent(ev_card);
                                    break;
                            }
                        }, null);
                }
            }

            #endregion 

            public Skill_0()
            {
                Triggers.Add(new Trigger_0(this));
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
                    Name = "风神",
                    Description = "你可以实行如下操作，并视作对一名其他角色使用一张无视距离的【杀】。\n" +
                    "\t1. 回合开始时，将你的角色牌翻面。\n" +
                    "\t2. 判定阶段开始时，当你判定区有牌时，弃置判定区所有的牌并。\n" +
                    "\t3. 摸牌阶段开始时，跳过你的摸牌阶段。\n" +
                    "\t4. 出牌阶段开始时，弃置一张装备牌并跳过你的出牌阶段。\n" +
                    "\t5. 弃牌阶段开始时，弃置至少两张手牌并跳过你的弃牌阶段。\n" +
                    "\t6. 回合结束前，失去一点体力。"
                };
            }
        }

        #endregion

        #region 技能【新闻】

        

        #endregion

        #region 技能【头条】

        #endregion 

        public Aya()
        {
            MaxHP = 4;
            HP = 4;
            Country = Reimu.CountryNameOfLeader;
            Skills.Add(new Skill_0());
            Skills.Add(new Aya_Skill_1());
        }

        public override Charactor Clone()
        {
            return new Aya();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "射命丸文";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Aya");
            info.AbilityRadar = new AbilityRadar() { Attack = 5, Defence = 2, Control = 5, Auxiliary = 1, LastStages = 2, Difficulty = 2 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }
    }
  

    public class Aya_Skill_1 : Skill
    {
        public Aya_Skill_1()
        {
            IsOnce = true;
            IsLocked = true;
            Triggers.Add(new Aya_Skill_1_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Aya_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Aya_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            SkillInfo info = new SkillInfo()
            {
                Name = "新闻",
                Description = "觉醒技，回合结束时，如果你背面朝上并且没有手牌，你减去一点体力上限，恢复一点体力并摸两张牌，获得技能【头条】。"
            };
            info.AttachedSkills.Add(new SkillInfo()
            {
                Name = "头条",
                Description = "一名角色的出牌阶段开始时，若其手牌数为全场最多（之一），你获取其一张手牌并展示，本回合其使用或者打出与这张牌相同类型的牌时，需要丢弃一张相同类型的牌，否则这张牌结算无效。"
            });
            return info;
        }
    }

    public class Aya_Skill_1_Trigger_0 : SkillTrigger, ITriggerInState
    {
        public Aya_Skill_1_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Aya_Skill_1_Trigger_0_Condition(skill);    
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
            ctx.World.ChangeMaxHp(skill.Owner, -1, ev_skill);
            ctx.World.Heal(skill.Owner, skill.Owner, 1, HealEvent.Normal, ev_skill);
            ctx.World.DrawCard(skill.Owner, 2, ev_skill);
        }
    }

    public class Aya_Skill_1_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Aya_Skill_1_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            Zone handzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return false;
            if (handzone.Cards.Count() > 0) return false;
            if (!skill.Owner.IsFacedDown) return false;
            return true;
        }


    }

    public class Aya_Skill_2 : Skill
    {
        public const string ShowedCardType = "头条展示过的牌的类型";

        public Aya_Skill_2()
        {
            Triggers.Add(new Aya_Skill_2_Trigger_0(this));
            Triggers.Add(new Aya_Skill_2_Trigger_1(this));
        }

        public override Skill Clone()
        {
            return new Aya_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new Aya_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "头条",
                Description = "一名角色的出牌阶段开始时，若其手牌数为全场最多（之一），你获取其一张手牌并展示，本回合其使用或者打出与这张牌相同类型的牌时，需要丢弃一张相同类型的牌，否则这张牌结算无效。"
            };
        }
    }

    public class Aya_Skill_2_Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Aya_Skill_2_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Aya_Skill_2_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName => State.UseCard;

        int ITriggerInState.StateStep => StateChangeEvent.Step_Start - 1;

        string ITriggerAsk.Message => "是否要发动【头条】？";

        Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (state.Owner == null) return;
            if (state.Ev == null) return;
            Zone handzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return;
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(state.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            ctx.World.SelectTargetCard(skill.Owner, state.Owner, 1, false, false,
                (cards) =>
                {
                    Card card = cards.FirstOrDefault();
                    state.Ev.SetValue(Aya_Skill_2.ShowedCardType, (int)(card.CardType.E) + 1);
                    ctx.World.MoveCards(skill.Owner, cards, handzone, ev_skill, Enum_MoveCardFlag.FaceUp);
                }, null);
        }
    }
   
    public class Aya_Skill_2_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Aya_Skill_2_Trigger_0_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Ev == null) return false;
            if (state.Owner == null) return false;
            Zone handzone0 = state.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone0 == null) return false;
            foreach (Player other in ctx.World.Players)
            {
                if (!other.IsAlive) continue;
                if (other == state.Owner) continue;
                Zone handzone1 = other.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                if (handzone1 == null) continue;
                if (handzone0.Cards.Count() < handzone1.Cards.Count()) return false;
            }
            return true;
        }
    }
   
    public class Aya_Skill_2_Trigger_1 : SkillTrigger, ITriggerInEvent
    {
        public Aya_Skill_2_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Aya_Skill_2_Trigger_1_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => CardPreviewEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is CardPreviewEvent)) return;
            CardPreviewEvent ev = (CardPreviewEvent)(ctx.Ev);
            State state = ctx.World.GetPlayerState();
            if (state == null) return;
            if (state.Owner == null) return;
            if (state.Ev == null) return;
            int ct = state.Ev.GetValue(Aya_Skill_2.ShowedCardType);
            if (ct == 0) return;
            Enum_CardType cardtype = (Enum_CardType)(ct - 1);
            Zone discardzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
            if (discardzone == null) return;
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(state.Owner);
            ctx.World.RequireCard("头条丢弃相同类型", "请丢弃一张相同类型的牌，否则该牌无效。", state.Owner,
                new Aya_Skill_2_TargetedCardType(cardtype), true, 15,
                (cards) => { ctx.World.MoveCards(state.Owner, cards, discardzone, ev_skill); },
                () => { ev.Cancel = true; });
        }
    }

    public class Aya_Skill_2_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Aya_Skill_2_Trigger_1_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Owner == null) return false;
            if (state.Ev == null) return false;
            int ct = state.Ev.GetValue(Aya_Skill_2.ShowedCardType);
            if (ct == 0) return false;
            if (!(ctx.Ev is CardPreviewEvent)) return false;
            CardPreviewEvent ev = (CardPreviewEvent)(ctx.Ev);
            if (ev.Source != state.Owner) return false;
            if (ev.Card.CardType.E != (Enum_CardType)(ct - 1)) return false;
            return true;
        }
    }

    public class Aya_Skill_2_TargetedCardType : CardFilter
    {
        public Aya_Skill_2_TargetedCardType(Enum_CardType _cardtype) { this.cardtype = _cardtype; }

        private Enum_CardType cardtype;
        public Enum_CardType CardType { get { return this.cardtype; } }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() >= 1) return false;
            if (want.Zone?.KeyName?.Equals(Zone.Hand) != true) return false;
            if (want.CardType?.E != cardtype) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return selecteds.Count() >= 1;
        }
    }
}
