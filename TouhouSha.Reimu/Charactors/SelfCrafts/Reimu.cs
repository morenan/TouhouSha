using System.Collections.Generic;
using System.Linq;
using TouhouSha.Core;
using TouhouSha.Core.AIs;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.AIs;
using TouhouSha.Koishi.Cards;

namespace TouhouSha.Reimu.Charactors.SelfCrafts
{
    /// <summary>
    /// 角色【博丽灵梦】
    /// </summary>
    /// <remarks>
    /// 势力：自机 3勾玉
    /// 【巫女】：游戏开始时，你将牌堆顶三张卡放置在你的武将牌上，作为【灵符】。场上的判定牌生效前，你可以打出一张卡替换，将被替换的牌放置为【灵符】。
    /// 【二重】：你可以将一个【灵符】去除，当作任意基本牌来使用或打出。这张牌结算前，你进行一次判定，红则将场上一位背面朝上的角色翻面，黑则将场上一位正面朝上的角色翻面。
    /// 【奉纳】：主公技，其他自机角色的出牌阶段限一次，可以丢弃一张手牌，对你进行一次判定，如果判定的牌和丢弃的牌同花色，你获得这个丢弃的牌。
    /// </remarks>
    public class Reimu : Charactor 
    {
        public const string CountryNameOfLeader = "自机";
        public const string Zone_Rune = "灵符";
        public const string Card_Rune = "灵符";

        public class Reimu_AskWorth : AskWorth
        {
            public override double GetWorthNo(Context ctx, Player controller, string keyname)
            {
                switch (keyname)
                {
                    case Skill_0.Trigger_1.DefaultKeyName:
                        ChangeJudgeCardWorth judgeworth = new ChangeJudgeCardWorth();
                        SelectCardBoardCore virtcore = new SelectCardBoardCore() { Controller = controller };
                        return judgeworth.GetWorthNo(ctx, virtcore);
                }
                return 0.0d;
            }

            public override double GetWorthYes(Context ctx, Player controller, string keyname)
            {
                switch (keyname)
                {
                    case Skill_0.Trigger_1.DefaultKeyName:
                        ChangeJudgeCardWorth judgeworth = new ChangeJudgeCardWorth();
                        SelectCardBoardCore virtcore = new SelectCardBoardCore() { Controller = controller };
                        double optworth = -10000;
                        foreach (Zone zone in controller.Zones)
                        {
                            switch (zone.KeyName)
                            {
                                case Zone.Hand:
                                case Zone.Equips:
                                    foreach (Card card in zone.Cards)
                                        optworth = judgeworth.GetWorth(ctx, virtcore, new Card[] { }, card) + 2;
                                    break;
                            }
                        }
                        return judgeworth.GetWorthNo(ctx, virtcore);
                }
                return 0.0d;
            }
        }

        public override AskWorth GetAskWorthAI()
        {
            return new Reimu_AskWorth();
        }

        #region 技能【神道】

        public class Skill_0 : Skill
        {
            #region 触发器【创建灵符区】
            public class Trigger_0 : SkillTrigger, ITriggerInState
            {
                public const string DefaultKeyName = "游戏开始创建灵符区";
                
                public class TriggerCondition : ConditionFilterFromSkill
                {
                    public TriggerCondition(Skill _skill) : base(_skill) { }

                    public override bool Accept(Context ctx)
                    {
                        return true;
                    }
                }

                public Trigger_0(Skill _skill) : base(_skill)
                {
                    KeyName = DefaultKeyName;
                    Condition = new TriggerCondition(skill);
                }

                string ITriggerInState.StateKeyName { get => State.GameStart; }

                int ITriggerInState.StateStep { get => 0; }

                public override void Action(Context ctx)
                {
                    Zone runezone = new Zone();
                    runezone.Owner = skill.Owner;
                    runezone.KeyName = Reimu.Zone_Rune;
                    runezone.Flag = Enum_ZoneFlag.LabelOnPlayer;
                    runezone.Owner.Zones.Add(runezone);
                    List<Card> cards = ctx.World.GetDrawTops(3);
                    SkillEvent ev = new SkillEvent();
                    ev.Skill = skill;
                    ev.Source = skill.Owner;
                    ev.Targets.Clear();
                    ev.Targets.Add(skill.Owner);
                    ctx.World.InvokeEvent(ev);
                    if (ev.Cancel) return;
                    ctx.World.MoveCards(skill.Owner, cards, runezone, ev);
                }
            }

            #endregion

            #region 触发器【改判】

            public class Trigger_1 : SkillTrigger, ITriggerInState, ITriggerAsk
            {
                public const string DefaultKeyName = "灵符改判";

                public class TriggerCondition : ConditionFilterFromSkill
                {
                    public TriggerCondition(Skill _skill) : base(_skill)
                    {
                    }

                    public override bool Accept(Context ctx)
                    {
                        State state = ctx.World.GetCurrentState();
                        if (!(state.Ev is JudgeEvent)) return false;
                        return true;
                    }
                }

                public Trigger_1(Skill _skill) : base(_skill)
                {
                    KeyName = DefaultKeyName;
                    Condition = new TriggerCondition(skill);
                }

                string ITriggerInState.StateKeyName { get => State.Handle; }

                int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

                string ITriggerAsk.Message { get => "是否要发动【神道】？"; }

                Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }

                public override void Action(Context ctx)
                {
                    State state = ctx.World.GetCurrentState();
                    if (state?.KeyName?.Equals(State.Handle) != true) return;
                    if (!(state.Ev is JudgeEvent)) return;
                    JudgeEvent ev = (JudgeEvent)(state.Ev);
                    Zone desktop = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Desktop) == true);
                    if (desktop == null) return;
                    Zone rune = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Reimu.Zone_Rune) == true);
                    if (rune == null) return;
                    SkillEvent ev_skill = new SkillEvent();
                    ev_skill.Skill = skill;
                    ev_skill.Source = skill.Owner;
                    ev_skill.Targets.Clear();
                    ev_skill.Targets.Add(ev.JudgeTarget);
                    ctx.World.InvokeEvent(ev_skill);
                    if (ev_skill.Cancel) return;
                    ctx.World.RequireCard(KeyName, "请使用一张牌替换判定牌。",
                        skill.Owner,
                        new FulfillNumberCardFilter(1, 1)
                        {
                            Allow_Hand = true,
                            Allow_Equiped = true,
                            Allow_Judging = false,
                            WorthAI = new TouhouSha.Reimu.AIs.ChangeJudgeCardWorth()
                        },
                        false, 15,
                        (cards) =>
                        {
                            List<Card> oldjudges = ev.JudgeCards.ToList();
                            ev.JudgeCards.Clear();
                            ev.JudgeCards.AddRange(cards);
                            ctx.World.MoveCards(skill.Owner, cards, desktop, ev_skill);
                            ctx.World.MoveCards(skill.Owner, oldjudges, rune, ev_skill);
                        },
                        () => { });
                }
            }

            #endregion 

            public Skill_0()
            {
                IsLocked = false;
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
                    Name = "神道",
                    Description = "游戏开始时，你将牌堆顶三张卡放置在你的武将牌上，作为【灵符】。场上的判定牌生效前，你可以打出一张卡替换，将被替换的牌放置为【灵符】。"
                };
            }
        }

        #endregion

        #region 技能【退治】

        public class Skill_1 : Skill, ISkillCardMultiConverter, ISkillCardMultiConverter2
        {
            public const string DefaultKeyName = "退治";

            public class SkillUseCondition : ConditionFilterFromSkill
            {
                public SkillUseCondition(Skill _skill) : base(_skill)
                {
                }

                public override bool Accept(Context ctx)
                {
                    if (!(Skill is ISkillCardMultiConverter)) return false;
                    ISkillCardMultiConverter multiconv = (ISkillCardMultiConverter)Skill;
                    if (multiconv.GetCardTypes(ctx).Count() == 0) return false;
                    Zone runezone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Sanae.Zone_Rune) == true);
                    if (runezone == null) return false;
                    if (runezone.Cards.Count() == 0) return false;
                    return true;
                }
            }

            public class SkillCardFilter : CardFilterFromSkill
            {
                public SkillCardFilter(Skill _skill) : base(_skill) { }

                public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
                {
                    if (selecteds.Count() >= 1) return false;
                    if (want.Zone.KeyName?.Equals(Reimu.Zone_Rune) != true) return false;
                    return true;
                }

                public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
                {
                    return selecteds.Count() == 1;
                }
            }

            public class SkillCardConverter : CardCalculatorFromSkill
            {
                public SkillCardConverter(Skill _skill) : base(_skill) { }

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
                    newcard.SetValue(Reimu.Card_Rune, 1);
                    return newcard;
                }
            }

            #region 结算前发起判定

            public class Trigger_0 : SkillTrigger, ITriggerInEvent
            {
                public const string DefaultKeyName = "退治发动前判定";

                public class TriggerCondition : ConditionFilterFromSkill
                {
                    public TriggerCondition(Skill _skill) : base(_skill)
                    {
                    }

                    public override bool Accept(Context ctx)
                    {
                        State state = ctx.World.GetPlayerState();
                        if (state?.Owner == skill.Owner) return false;
                        if (!(ctx.Ev is CardPreviewEvent)) return false;
                        CardPreviewEvent ev = (CardPreviewEvent)(ctx.Ev);
                        if (ev.Card == null) return false;
                        if (ev.Source != skill.Owner) return false;
                        if (ev.Card.GetValue(Reimu.Card_Rune) != 1) return false;
                        return true;
                    }
                }

                public Trigger_0(Skill _skill) : base(_skill)
                {
                    Condition = new TriggerCondition(skill);
                }

                string ITriggerInEvent.EventKeyName { get => CardPreviewEvent.DefaultKeyName; }

                public override void Action(Context ctx)
                {
                    State state = ctx.World.GetCurrentState();
                    if (state == null) return;
                    if (!(ctx.Ev is CardPreviewEvent)) return;
                    CardPreviewEvent ev = (CardPreviewEvent)(ctx.Ev);
                    State judgestate = state.Clone();
                    SkillEvent ev_skill = new SkillEvent();
                    ev_skill.Reason = ev;
                    ev_skill.Skill = skill;
                    ev_skill.Source = skill.Owner;
                    ev_skill.Targets.Clear();
                    ev_skill.Targets.Add(skill.Owner);
                    ctx.World.InvokeEvent(ev_skill);
                    if (ev_skill.Cancel) return;
                    JudgeEvent ev_judge = new JudgeEvent();
                    ev_judge.Reason = ev_skill;
                    ev_judge.JudgeNumber = 1;
                    ev_judge.JudgeTarget = ev.Source;
                    ev_judge.JudgeCards.Clear();
                    ctx.World.InvokeEvent(ev_judge);
                }
            }

            #endregion

            #region 判定效果

            public class Trigger_1 : SkillTrigger, ITriggerInState
            {
                public const string DefaultKeyName = "退治技能选择翻面或者乐";

                public class TriggerCondition : ConditionFilterFromSkill
                {
                    public TriggerCondition(Skill _skill) : base(_skill)
                    {
                    }

                    public override bool Accept(Context ctx)
                    {
                        State state = ctx.World.GetCurrentState();
                        if (state == null) return false;
                        if (!(state.Ev is JudgeEvent)) return false;
                        JudgeEvent ev0 = (JudgeEvent)(state.Ev);
                        if (ev0.JudgeCards.Count() == 0) return false;
                        if (!(ev0.Reason is SkillEvent)) return false;
                        SkillEvent ev1 = (SkillEvent)(ev0.Reason);
                        if (ev1.Skill != skill) return false;
                        return true;
                    }
                }

                public class SelectFaceDown : PlayerFilter
                {
                    public class WorthAI : PlayerFilterWorth
                    {
                        public override double GetWorthNo(Context ctx, SelectPlayerBoardCore core)
                        {
                            return 0;
                        }

                        public override double GetWorth(Context ctx, SelectPlayerBoardCore core, IEnumerable<Player> selecteds, Player want)
                        {
                            return AITools.WorthAcc.GetWorthInPhase(ctx, core.Controller, want);
                        }

                    }

                    public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
                    {
                        State state = ctx.World.GetCurrentState();
                        if (state == null) return false;
                        if (state.Owner == want) return false;
                        if (!want.IsFacedDown) return false;
                        return true;
                    }

                    public override PlayerFilterWorth GetWorthAI()
                    {
                        return new WorthAI();
                    }
                }


                public Trigger_1(Skill _skill) : base(_skill)
                {
                    Condition = new TriggerCondition(skill);
                }

                string ITriggerInState.StateKeyName { get => State.Handle; }

                int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

                public override void Action(Context ctx)
                {
                    State state = ctx.World.GetCurrentState();
                    if (state == null) return;
                    if (!(state.Ev is JudgeEvent)) return;
                    JudgeEvent ev0 = (JudgeEvent)(state.Ev);
                    if (ev0.JudgeCards.Count() == 0) return;
                    if (ev0.JudgeCards[0].CardColor?.SeemAs(Enum_CardColor.Black) == true)
                    {
                        Card happy = ctx.World.GetCardInstance(HappyCard.Normal);
                        Zone handzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                        happy.Zone = handzone;
                        ctx.World.SelectPlayer(KeyName, "请选择一个角色来放置封魔阵。",
                            skill.Owner,
                            happy.TargetFilter,
                            true, 15,
                            (players) =>
                            {
                                happy = happy.Clone(ev0.JudgeCards.FirstOrDefault());
                                CardEvent ev_card = new CardEvent();
                                ev_card.Card = happy;
                                ev_card.Source = skill.Owner;
                                ev_card.Targets.Clear();
                                ev_card.Targets.AddRange(players);
                                ctx.World.InvokeEvent(ev_card);
                            },
                            () => { });
                        return;
                    }
                    if (ev0.JudgeCards[0].CardColor?.SeemAs(Enum_CardColor.Red) == true)
                    {
                        ctx.World.SelectPlayer(KeyName, "请选择一个背面朝上的角色来复原。",
                            state.Owner,
                            new SelectFaceDown(),
                            true, 15,
                            (players) =>
                            {
                                foreach (Player player in players)
                                {
                                    SkillEvent ev_skill = new SkillEvent();
                                    ev_skill.Reason = ev0;
                                    ev_skill.Skill = skill;
                                    ev_skill.Source = skill.Owner;
                                    ev_skill.Targets.Clear();
                                    ev_skill.Targets.Add(player);
                                    ctx.World.InvokeEvent(ev_skill);
                                    if (ev_skill.Cancel) continue;
                                    ctx.World.FaceClip(skill.Owner, player, ev_skill);
                                }
                            },
                            () => { });
                        return;
                    }
                }
            }

            #endregion 

            public Skill_1()
            {
                KeyName = DefaultKeyName;
                this.condition = new SkillUseCondition(this);
                this.cardfilter = new SkillCardFilter(this);
                this.cardconverter = new SkillCardConverter(this);
                Triggers.Add(new Trigger_0(this));
                Triggers.Add(new Trigger_1(this));
            }

            private SkillUseCondition condition;
            ConditionFilter ISkillCardConverter.UseCondition
            {
                get { return this.condition; }
            }

            private SkillCardFilter cardfilter;
            CardFilter ISkillCardConverter.CardFilter
            {
                get { return this.cardfilter; }
            }

            private SkillCardConverter cardconverter;
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
                return new Skill_1();
            }

            public override Skill Clone(Card newcard)
            {
                return new Skill_1();
            }

            public override SkillInfo GetInfo()
            {
                SkillInfo skillinfo = new SkillInfo()
                {
                    Name = "退治",
                    Description = "你可以将一个【灵符】去除，当作任意基本牌来使用或打出。这张牌结算前，你进行一次判定，红则将选择一名背面朝上的角色翻面，黑则将该判定牌视作【封魔阵】来使用。"
                };
                skillinfo.AttachedSkills.Add(new SkillInfo()
                {
                    Name = "封魔阵",
                    Description = "放置到一名其他角色没有同名卡的判定区。判定阶段进行判定，不为♥跳过本回合出牌阶段。结算完毕弃置此牌。",
                });
                return skillinfo;
            }
        }

        #endregion

        #region 技能【奉纳】

        /// <summary>
        /// 【奉纳】：主公技，其他自机角色的出牌阶段限一次，可以丢弃一张手牌，对你进行一次判定，如果判定的牌和丢弃的牌同花色，你获得这个丢弃的牌。
        /// </summary>
        /// <remarks>
        /// 注意这时Skill.Owner为自机忠臣，不是主公。
        /// </remarks>
        public class Skill_2 : Skill, ISkillInitative
        {
            public const string Used = "使用奉纳的次数";

            public class SkillUseCondition : ConditionFilterFromSkill
            {
                public SkillUseCondition(Skill _skill) : base(_skill) { }

                public override bool Accept(Context ctx)
                {
                    State state = ctx.World.GetPlayerState();
                    if (state.Owner == null) return false;
                    if (!state.Owner.IsCountry(Reimu.CountryNameOfLeader)) return false;
                    if (state.GetValue(Skill_2.Used) >= 1) return false;
                    return true;
                }
            }

            public class SkillTargetFilter : PlayerFilterFromSkill
            {
                public SkillTargetFilter(Skill _skill) : base(_skill) { }

                public override Enum_PlayerFilterFlag GetFlag(Context ctx)
                {
                    return Enum_PlayerFilterFlag.ForceAll;
                }

                public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
                {
                    if (selecteds.Count() > 0) return false;
                    if (want?.Ass?.E != Enum_PlayerAss.Leader) return false;
                    return true;
                }

                public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
                {
                    return selecteds.Count() > 0;
                }
            }

            public class SkillCostFilter : CardFilterFromSkill
            {
                public SkillCostFilter(Skill _skill) : base(_skill) { }

                public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
                {
                    if (selecteds.Count() > 0) return false;
                    if (want?.Zone?.KeyName?.Equals(Zone.Hand) != true) return false;
                    return true;
                }

                public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
                {
                    return selecteds.Count() > 0;
                }
            }

            public class SkillWorth : InitativeSkillWorth
            {
                public double LeaderWorth => 1.5;

                public override double GetWorth(Context ctx, Skill skill, Player user)
                {
                    Zone handzone = user.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                    if (handzone == null) return double.MinValue;
                    if (handzone.Cards.Count() == 0) return double.MinValue;
                    return handzone.Cards.Max(_card => GetWorthSelect(ctx, skill, user, new Card[] { }, _card)) 
                        + GetWorthSelect(ctx, skill, user, new Player[] { }, ctx.World.Players.FirstOrDefault(_player => _player.Ass?.E == Enum_PlayerAss.Leader));
                }

                public override double GetWorthSelect(Context ctx, Skill skill, Player user, IEnumerable<Player> selecteds, Player want)
                {
                    if (want == null) return 0;
                    return 1.5 * AITools.WorthAcc.GetHatred(ctx, user, want);
                }

                public override double GetWorthSelect(Context ctx, Skill skill, Player user, IEnumerable<Card> selecteds, Card want)
                {
                    return -AITools.WorthAcc.GetWorth(ctx, user, want);
                }
            }

            #region 判定结果处理

            public class Trigger_0 : SkillTrigger, ITriggerInState
            {
                static public readonly Dictionary<JudgeEvent, Card> JudgeTos = new Dictionary<JudgeEvent, Card>();

                public const string DefaultKeyName = "奉纳可能拿牌";

                public class TriggerCondition : ConditionFilterFromSkill
                {
                    public TriggerCondition(Skill _skill) : base(_skill)
                    {
                    }

                    public override bool Accept(Context ctx)
                    {
                        State state = ctx.World.GetCurrentState();
                        if (state == null) return false;
                        if (!(state.Ev is JudgeEvent)) return false;
                        JudgeEvent ev0 = (JudgeEvent)(state.Ev);
                        if (ev0.JudgeCards.Count() == 0) return false;
                        if (!(ev0.Reason is SkillEvent)) return false;
                        SkillEvent ev1 = (SkillEvent)(ev0.Reason);
                        if (ev1.Skill != skill) return false;
                        return true;
                    }

                }

                public Trigger_0(Skill _skill) : base(_skill)
                {
                    KeyName = DefaultKeyName;
                    Condition = new TriggerCondition(skill);
                }

                string ITriggerInState.StateKeyName { get => State.Judge; }

                int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

                public override void Action(Context ctx)
                {
                    State state = ctx.World.GetCurrentState();
                    if (state == null) return;
                    if (!(state.Ev is JudgeEvent)) return;
                    JudgeEvent ev = (JudgeEvent)(state.Ev);
                    if (ev.JudgeCards.Count() == 0) return;
                    Card judgeto = null;
                    if (!JudgeTos.TryGetValue(ev, out judgeto)) return;
                    JudgeTos.Remove(ev);
                    if (judgeto.CardColor != null
                     && ev.JudgeCards[0].CardColor != null
                     && ev.JudgeCards[0].CardColor.E == judgeto.CardColor.E)
                    {
                        Zone hand = ev.JudgeTarget.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                        if (hand == null) return;
                        ctx.World.MoveCard(ev.JudgeTarget, judgeto, hand, ev);
                    }
                }
            }

            #endregion 

            public Skill_2()
            {
                IsLeader = true;
                IsLeaderForLeader = false;
                IsLeaderForSlave = true;
                this.usecondition = new SkillUseCondition(this);
                this.targetfilter = new SkillTargetFilter(this);
                this.costfilter = new SkillCostFilter(this);

            }

            private SkillUseCondition usecondition;
            ConditionFilter ISkillInitative.UseCondition { get { return this.usecondition; } }

            private SkillTargetFilter targetfilter;
            PlayerFilter ISkillInitative.TargetFilter { get { return this.targetfilter; } }

            private SkillCostFilter costfilter;
            CardFilter ISkillInitative.CostFilter { get { return this.costfilter; } }

            public void Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
            {
                State state = ctx.World.GetCurrentState();
                if (state == null) return;
                int used = state.GetValue(Used);
                state.SetValue(Used, used + 1);
                SkillEvent ev_skill = new SkillEvent();
                Zone discardzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
                if (discardzone == null) return;
                ev_skill.Skill = this;
                ev_skill.Source = Owner;
                ev_skill.Targets.Clear();
                ev_skill.Targets.AddRange(ctx.World.GetAlivePlayersStartHere(Owner).Where(_player => _player.Ass?.E == Enum_PlayerAss.Leader));
                if (ev_skill.Targets.Count() == 0) return;
                JudgeEvent ev_judge = new JudgeEvent();
                ev_judge.Reason = ev_skill;
                ev_judge.JudgeNumber = 1;
                ev_judge.JudgeTarget = ev_skill.Targets[0];
                Trigger_0.JudgeTos.Add(ev_judge, costs.FirstOrDefault());
                ctx.World.MoveCards(Owner, costs, discardzone, ev_skill);
                ctx.World.InvokeEvent(ev_judge);
            }

            public override Skill Clone()
            {
                return new Skill_2();
            }

            public override Skill Clone(Card newcard)
            {
                return new Skill_2();
            }

            public override SkillInfo GetInfo()
            {
                return new SkillInfo()
                {
                    Name = "奉纳",
                    Description = "主公技，其他自机角色的出牌阶段限一次，可以丢弃一张手牌，对你进行一次判定，如果判定的牌和丢弃的牌同花色，你获得这个丢弃的牌。"
                };
            }
            
            public override InitativeSkillWorth GetWorthAI()
            {
                return new SkillWorth();
            }
        }

        #endregion

        public Reimu()
        {
            Country = CountryNameOfLeader;
            HP = 3;
            MaxHP = 3;
            Skills.Clear();
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
            Skills.Add(new Skill_2());
        }

        public override Charactor Clone()
        {
            return new Reimu();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "博丽灵梦";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Reimu");
            info.AbilityRadar = new AbilityRadar() { Attack = 3, Defence = 5, Control = 5, Auxiliary = 5, LastStages = 5, Difficulty = 4 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }
    }
}
