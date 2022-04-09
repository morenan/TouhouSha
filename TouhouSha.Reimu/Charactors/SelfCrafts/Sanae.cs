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
using TouhouSha.Reimu.Charactors.Moriya;
using TouhouSha.Reimu.AIs;

namespace TouhouSha.Reimu.Charactors.SelfCrafts
{
    /// <summary>
    /// 角色【东风谷早苗】
    /// </summary>
    /// <remarks>
    /// 势力：自机（可替换为守矢） 3勾玉
    /// 【风祝】：游戏开始时，你将牌堆顶三张卡放置在你的武将牌上，作为【信仰】。场上的拼点牌生效前，你可以打出一张手卡替换其中一张，将被替换的牌放置为【信仰】。
    /// 【奇迹】：当你的角色牌正面朝上时，你可以将一个【信仰】去除，当作任意锦囊牌来使用或打出。若如此做，你和一名其他角色进行拼点，若你赢你将一张手牌交给对方或者丢弃，若你输则将角色牌翻面。
    /// 【人神】：限定技，当你进入濒死阶段时，你可以移去全部的【信仰】，将体力上限调整为移除的【信仰】的数量（至少为3），回复移除数量的体力并摸等量的牌。
    /// </remarks>
    public class Sanae : Charactor
    {
        public const string Zone_Rune = "信仰";
        public const string Card_Rune = "信仰";

        #region 技能【风祝】

        public class Skill_0 : Skill
        {
            #region 创建信仰区

            public class Trigger_0 : SkillTrigger, ITriggerInState
            {
                public class TriggerCondition : ConditionFilterFromSkill
                {
                    public TriggerCondition(Skill _skill) : base(_skill)
                    {

                    }

                    public override bool Accept(Context ctx)
                    {
                        return true;
                    }
                }

                public Trigger_0(Skill _skill) : base(_skill)
                {
                    Condition = new TriggerCondition(skill);
                }

                string ITriggerInState.StateKeyName { get => State.GameStart; }

                int ITriggerInState.StateStep { get => 0; }

                public override void Action(Context ctx)
                {
                    Zone runezone = new Zone();
                    runezone.Owner = skill.Owner;
                    runezone.KeyName = Sanae.Zone_Rune;
                    runezone.Flag = Enum_ZoneFlag.LabelOnPlayer;
                    runezone.Owner.Zones.Add(runezone);
                    List<Card> cards = ctx.World.GetDrawTops(3);
                    SkillEvent ev = new SkillEvent();
                    ev.Skill = skill;
                    ev.Source = skill.Owner;
                    ev.Targets.Clear();
                    ev.Targets.Add(skill.Owner);
                    ctx.World.MoveCards(skill.Owner, cards, runezone, ev);
                }
            }

            #endregion

            #region 改双人拼点

            public class Trigger_1 : SkillTrigger, ITriggerInEvent, ITriggerAsk
            {
                public const string DefaultKeyName = "风祝改拼";

                static public bool ReplaceAction(Context ctx, Player controller, AITools aitools, out Card replaced, out Card toreplace)
                {
                    replaced = null;
                    toreplace = null;
                    Zone rune = controller.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Sanae.Zone_Rune) == true);
                    if (rune == null) return false;
                    Zone hand = controller.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                    if (hand == null) return false;

                    if (ctx.Ev is PointBattleEventBase)
                    {
                        PointBattleEventBase ev = (PointBattleEventBase)(ctx.Ev);
                        PointBattleCardWorth worthacc = new PointBattleCardWorth(ev);
                        double worthwin = worthacc.GetWorthWin(ctx, controller);
                        double worthlose = worthacc.GetWorthLose(ctx, controller);
                        if (rune.Cards.Count() > 0
                         && ((worthwin >= worthlose && ev.SourceCard.CardPoint > ev.TargetCard.CardPoint)
                         || (worthwin <= worthlose && ev.SourceCard.CardPoint <= ev.TargetCard.CardPoint)))
                        {
                            replaced = null;
                            toreplace = null;
                            return false;
                        }
                        else if (worthwin > worthlose)
                        {
                            Card optcard = null;
                            double optworth = -10000;
                            int point0 = Math.Min(ev.SourceCard.CardPoint, ev.TargetCard.CardPoint);
                            int point1 = Math.Max(ev.SourceCard.CardPoint, ev.TargetCard.CardPoint);
                            foreach (Card card in hand.Cards)
                            {
                                if (card.CardPoint >= point0
                                 && card.CardPoint <= point1) continue;
                                double lostworth = -aitools.WorthAcc.GetWorth(ctx, controller, card);
                                if (lostworth > optworth)
                                {
                                    optcard = card;
                                    optworth = lostworth;
                                }
                            }
                            if (optcard == null)
                            {
                                replaced = null;
                                toreplace = null;
                                return false;
                            }
                            else if (optcard.CardPoint > ev.TargetCard.CardPoint)
                            {
                                replaced = ev.SourceCard;
                                toreplace = optcard;
                                return true;
                            }
                            else
                            {
                                replaced = ev.TargetCard;
                                toreplace = optcard;
                                return true;
                            }
                        }
                        else
                        {
                            Card optcard = null;
                            double optworth = -10000;
                            int point0 = Math.Min(ev.SourceCard.CardPoint, ev.TargetCard.CardPoint);
                            int point1 = Math.Max(ev.SourceCard.CardPoint, ev.TargetCard.CardPoint);
                            foreach (Card card in hand.Cards)
                            {
                                if (card.CardPoint > point0
                                 && card.CardPoint < point1) continue;
                                double lostworth = -aitools.WorthAcc.GetWorth(ctx, controller, card);
                                if (lostworth > optworth)
                                {
                                    optcard = card;
                                    optworth = lostworth;
                                }
                            }
                            if (optcard == null)
                            {
                                replaced = null;
                                toreplace = null;
                                return false;
                            }
                            else if (optcard.CardPoint >= ev.SourceCard.CardPoint)
                            {
                                replaced = ev.TargetCard;
                                toreplace = optcard;
                                return true;
                            }
                            else
                            {
                                replaced = ev.SourceCard;
                                toreplace = optcard;
                                return true;
                            }
                        }
                    }
                    
                    if (ctx.Ev is PointMultiBattleEventBase)
                    {
                        PointMultiBattleEventBase ev = (PointMultiBattleEventBase)(ctx.Ev);
                        PointMultiBattleCardWorth worthacc = new PointMultiBattleCardWorth(ev);
                    }

                    return false;
                }

                public class TriggerCondition : ConditionFilterFromSkill
                {
                    public TriggerCondition(Skill _skill) : base(_skill)
                    {

                    }

                    public override bool Accept(Context ctx)
                    {
                        Zone handzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                        if (handzone == null) return false;
                        if (handzone.Cards.Count() == 0) return false;
                        return true;
                    }
                }

                public class Auto0 : DesktopCardFilterAuto
                {
                    public override bool GetNo(Context ctx, DesktopCardBoardCore core)
                    {
                        return false;
                    }

                    public override List<Card> GetSelection(Context ctx, DesktopCardBoardCore core)
                    {
                        Card replaced = null;
                        Card toreplace = null;
                        ReplaceAction(ctx, core.Controller, AITools, out replaced, out toreplace);
                        return new List<Card>() { replaced };
                    }
                }

                public class Auto1 : CardFilterAuto
                {
                    public override bool GetNo(Context ctx, SelectCardBoardCore core)
                    {
                        return false;
                    }

                    public override List<Card> GetSelection(Context ctx, SelectCardBoardCore core)
                    {
                        Card replaced = null;
                        Card toreplace = null;
                        ReplaceAction(ctx, core.Controller, AITools, out replaced, out toreplace);
                        return new List<Card>() { toreplace };
                    }
                }

                public Trigger_1(Skill _skill) : base(_skill)
                {
                    KeyName = DefaultKeyName;
                    Condition = new TriggerCondition(skill);
                }

                string ITriggerInEvent.EventKeyName { get => PointBattlePreviewEvent.DefaultKeyName; }

                string ITriggerAsk.Message { get => "是否要发动【风祝】？"; }

                Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }

                public override void Action(Context ctx)
                {
                    if (!(ctx.Ev is PointBattleEventBase)) return;
                    PointBattleEventBase ev = (PointBattleEventBase)(ctx.Ev);
                    Zone desktop = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Desktop) == true);
                    if (desktop == null) return;
                    Zone rune = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Sanae.Zone_Rune) == true);
                    if (rune == null) return;

                    SkillEvent ev_skill = new SkillEvent();
                    ev_skill.Skill = skill;
                    ev_skill.Source = skill.Owner;
                    ev_skill.Targets.Clear();
                    ctx.World.InvokeEvent(ev_skill);
                    if (ev_skill.Cancel) return;

                    DesktopCardBoardCore desktop_core = new DesktopCardBoardCore();
                    DesktopCardBoardZone desktop_zone = new DesktopCardBoardZone(desktop_core);
                    desktop_core.Controller = skill.Owner;
                    desktop_core.IsAsync = false;
                    desktop_core.Flag = Enum_DesktopCardBoardFlag.SelectCardAndYes;
                    desktop_core.Flag |= Enum_DesktopCardBoardFlag.CannotNo;
                    desktop_core.CardFilter = new FulfillNumberCardFilter(1, 1) { DesktopAutoAI = new Auto0() };
                    desktop_core.Zones.Add(desktop_zone);
                    desktop_zone.Message = "请选择被替换的拼点牌";
                    ctx.World.ShowDesktop(skill.Owner, desktop_core, new List<IList<Card>>() { new List<Card>() { ev.SourceCard, ev.TargetCard } }, false, null);
                    Card selectedcard = desktop_core.SelectedCards.FirstOrDefault();

                    ctx.World.RequireCard(KeyName, "请使用一张手牌替换拼点牌。",
                        skill.Owner,
                        new FulfillNumberCardFilter(1, 1) 
                        { 
                            Allow_Equiped = false, 
                            Allow_Judging = false,
                            AutoAI = new Auto1(),
                        },
                        false, Config.GameConfig.Timeout_Handle,
                        (cards) =>
                        {
                            if (selectedcard == ev.SourceCard)
                                ev.SourceCard = cards.FirstOrDefault();
                            else
                                ev.TargetCard = cards.FirstOrDefault();
                            ctx.World.MoveCards(skill.Owner, cards, desktop, ev_skill);
                            ctx.World.MoveCard(skill.Owner, selectedcard, rune, ev_skill);
                        },
                        () => { });
                }
            }

            #endregion

            #region 改多人拼点

            public class Trigger_2 : SkillTrigger, ITriggerInEvent, ITriggerAsk
            {
                public const string DefaultKeyName = "风祝多人改拼";

                public class TriggerCondition : ConditionFilterFromSkill
                {
                    public TriggerCondition(Skill _skill) : base(_skill)
                    {

                    }

                    public override bool Accept(Context ctx)
                    {
                        Zone handzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                        if (handzone == null) return false;
                        if (handzone.Cards.Count() == 0) return false;
                        return true;
                    }
                }

                public Trigger_2(Skill _skill) : base(_skill)
                {
                    KeyName = DefaultKeyName;
                    Condition = new TriggerCondition(skill);
                }

                string ITriggerInEvent.EventKeyName { get => PointMultiBattlePreviewEvent.DefaultKeyName; }

                string ITriggerAsk.Message { get => "是否要发动【风祝】？"; }

                Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }

                public override void Action(Context ctx)
                {
                    if (!(ctx.Ev is PointMultiBattlePreviewEvent)) return;
                    PointMultiBattlePreviewEvent ev = (PointMultiBattlePreviewEvent)(ctx.Ev);
                    Zone desktop = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Desktop) == true);
                    if (desktop == null) return;
                    Zone rune = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Sanae.Zone_Rune) == true);
                    if (rune == null) return;

                    SkillEvent ev_skill = new SkillEvent();
                    ev_skill.Skill = skill;
                    ev_skill.Source = skill.Owner;
                    ev_skill.Targets.Clear();
                    ctx.World.InvokeEvent(ev_skill);
                    if (ev_skill.Cancel) return;

                    DesktopCardBoardCore desktop_core = new DesktopCardBoardCore();
                    DesktopCardBoardZone desktop_zone = new DesktopCardBoardZone(desktop_core);
                    desktop_core.Controller = skill.Owner;
                    desktop_core.IsAsync = false;
                    desktop_core.Flag = Enum_DesktopCardBoardFlag.SelectCardAndYes;
                    desktop_core.Flag |= Enum_DesktopCardBoardFlag.CannotNo;
                    desktop_core.CardFilter = new FulfillNumberCardFilter(1, 1) { DesktopAutoAI = new Trigger_1.Auto0() };
                    desktop_core.Zones.Add(desktop_zone);
                    desktop_zone.Message = "请选择被替换的拼点牌";
                    ctx.World.ShowDesktop(skill.Owner, desktop_core, new List<IList<Card>>() { new Card[] { ev.SourceCard }.Concat(ev.TargetCards).ToList() }, false, null);
                    Card selectedcard = desktop_core.SelectedCards.FirstOrDefault();

                    ctx.World.RequireCard(KeyName, "请使用一张手牌替换拼点牌。",
                        skill.Owner,
                        new FulfillNumberCardFilter(1, 1)
                        {
                            Allow_Equiped = false,
                            Allow_Judging = false,
                            AutoAI = new Trigger_1.Auto1()
                        },
                        false, 15,
                        (cards) =>
                        {
                            if (selectedcard == ev.SourceCard)
                                ev.SourceCard = cards.FirstOrDefault();
                            else
                            {
                                int i = ev.TargetCards.IndexOf(selectedcard);
                                ev.TargetCards[i] = cards.FirstOrDefault();
                            }
                            ctx.World.MoveCards(skill.Owner, cards, desktop, ev_skill);
                            ctx.World.MoveCard(skill.Owner, selectedcard, rune, ev_skill);
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
                Triggers.Add(new Trigger_2(this));
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
                    Name = "风祝",
                    Description = "游戏开始时，你将牌堆顶三张卡放置在你的武将牌上，作为【信仰】。场上的拼点牌生效前，你可以打出一张手卡替换其中一张，将被替换的牌放置为【信仰】。"
                };
            }
        }

        #endregion

        #region 技能【奇迹】

        public class Skill_1 : Skill, ISkillCardMultiConverter, ISkillCardMultiConverter2, ISkillCardConverterBefore
        {
            public const string DefaultKeyName = "奇迹";
            public const string Used = "奇迹的使用次数";

            public class SkillUseCondition : ConditionFilterFromSkill
            {
                public SkillUseCondition(Skill _skill) : base(_skill)
                {
                }

                public override bool Accept(Context ctx)
                {
                    State state = ctx.World.GetPlayerState();
                    if (state?.Ev == null) return false;
                    Zone runezone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Sanae.Zone_Rune) == true);
                    if (runezone == null) return false;
                    if (runezone.Cards.Count() == 0) return false;
                    Zone handzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                    if (handzone == null) return false;
                    if (handzone.Cards.Count() < 1 + Math.Max(0, state.Ev.GetValue(Used) - 2)) return false;
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
                    if (selecteds.Count() >= 1) return false;
                    if (want.Zone.KeyName?.Equals(Sanae.Zone_Rune) != true) return false;
                    return true;
                }

                public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
                {
                    return selecteds.Count() == 1;
                }
            }

            public class SkillCardConverter : CardCalculatorFromSkill
            {
                public SkillCardConverter(Skill _skill) : base(_skill)
                {

                }

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

            public class TargetWorth : PlayerFilterWorth
            {
                public TargetWorth(PointBattleBeginEvent _battleevent) { this.battleevent = _battleevent; }

                private PointBattleBeginEvent battleevent;
                public PointBattleBeginEvent BattleEvent { get { return this.battleevent; } }

                public override double GetWorth(Context ctx, SelectPlayerBoardCore core, IEnumerable<Player> selecteds, Player want)
                {
                    battleevent.Target = want;
                    double worth = 0;
                    worth += AITools.WorthAcc.GetWorthPointBattleLose(ctx, core.Controller, want, battleevent);
                    worth += AITools.WorthAcc.GetWorthPointBattleWin(ctx, core.Controller, want, battleevent);
                    battleevent.Target = null;
                    return worth;
                }

                public override double GetWorthNo(Context ctx, SelectPlayerBoardCore core)
                {
                    return 0.0d;
                }
            }

            public Skill_1()
            {
                this.condition = new SkillUseCondition(this);
                this.cardfilter = new SkillCardFilter(this);
                this.cardconverter = new SkillCardConverter(this);
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
                return enabledcardtypes ?? ctx.World.GetSpellCardKeyNames();
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
            IEnumerable<string> ISkillCardMultiConverter2.EnabledCardTypes
            {
                get
                {
                    return enabledcardtypes;
                }
            }

            void ISkillCardMultiConverter2.SetEnabledCardTypes(Context ctx, IEnumerable<string> cardtypes)
            {
                List<string> typelist = ctx.World.GetSpellCardKeyNames().ToList();
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

            void ISkillCardConverterBefore.Before(Context ctx, SkillEvent ev_skill, CardEvent ev_card)
            {
                if (ev_skill.Cancel) return;
                State state = ctx.World.GetPlayerState();
                int used = state.Ev.GetValue(Used) + 1;
                state.Ev.SetValue(Used, used);
                // 第三次发动翻面。
                if (used == 3) ctx.World.FaceClip(ev_skill.Source, ev_skill.Source, ev_skill);
                // 第四次以上发动弃牌。
                if (used >= 4) ctx.World.DiscardCard(ev_skill.Source, used - 3, ev_skill, false);
                // 发动技能前和一名角色进行拼点
                PointBattleBeginEvent ev_battle = new PointBattleBeginEvent();
                ev_battle.Reason = ev_skill;
                ev_battle.Source = ev_skill.Source;
                ctx.World.SelectPlayer("奇迹拼点对象", "请选择一名对象，发起拼点", ev_skill.Source,
                    new FulfillNumberPlayerFilter(1, 1, ev_skill.Source)
                    {
                        WorthAI = new TargetWorth(ev_battle)
                    },
                    false, Config.GameConfig.Timeout_Handle,
                    (targets) =>
                    {
                        ev_battle.Target = targets.FirstOrDefault();
                        ctx.World.InvokeEvent(ev_battle);
                        if (ev_battle.Cancel || !ev_battle.IsWin(ev_skill.Source))
                            ev_card.Cancel = true;
                    },
                    () =>
                    {
                        ev_card.Cancel = true;
                    });
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
                    Name = "奇迹",
                    Description = "你可以声明一个非延时锦囊牌并选择你的一张【信仰】，选择一名其他角色进行拼点，若你赢你将这张【信仰】当作该锦囊牌来使用或打出。"
                        + "若你是本回合第三次发动该技能，发动前将你的角色牌翻面。"
                        + "若你是本回合第四次或者以上发动该技能，发动前你需要丢弃X张手牌（X为之前发动的次数-2）。"
                };
            }
        }

        #endregion

        #region 技能【人神】

        public class Skill_2 : Skill
        {
            public const string Used = "使用过人神";

            public class Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
            {
                public class TriggerCondition : ConditionFilterFromSkill
                {
                    public TriggerCondition(Skill _skill) : base(_skill)
                    {

                    }

                    public override bool Accept(Context ctx)
                    {
                        State state = ctx.World.GetCurrentState();
                        if (state.KeyName?.Equals(State.Dying) != true) return false;
                        if (state.Owner != skill.Owner) return false;
                        if (state.Owner.GetValue(Skill_2.Used) == 1) return false;
                        Zone runezone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Sanae.Zone_Rune) == true);
                        if (runezone == null) return false;
                        if (runezone.Cards.Count() == 0) return false;
                        return true;
                    }
                }
                public Trigger_0(Skill _skill) : base(_skill)
                {
                    Condition = new TriggerCondition(skill);
                }

                string ITriggerInState.StateKeyName { get => State.Dying; }

                int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

                string ITriggerAsk.Message { get => "是否要发动【人神】？"; }

                Player ITriggerAsk.GetAsked(Context ctx) { return skill.Owner; }

                public override void Action(Context ctx)
                {
                    Zone runezone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Sanae.Zone_Rune) == true);
                    if (runezone == null) return;
                    int runecount = runezone.Cards.Count();
                    Zone discardzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
                    if (discardzone == null) return;
                    skill.Owner.SetValue(Skill_2.Used, 1);
                    SkillEvent ev_skill = new SkillEvent();
                    ev_skill.Skill = skill;
                    ev_skill.Source = skill.Owner;
                    ev_skill.Targets.Clear();
                    ev_skill.Targets.Add(skill.Owner);
                    ctx.World.InvokeEvent(ev_skill);
                    if (ev_skill.Cancel) return;
                    int maxhp = Math.Max(3, runecount);
                    if (skill.Owner.MaxHP != maxhp)
                        ctx.World.ChangeMaxHp(skill.Owner, maxhp - skill.Owner.MaxHP, ev_skill);
                    ctx.World.MoveCards(skill.Owner, runezone.Cards.ToList(), discardzone, ev_skill);
                    ctx.World.Heal(skill.Owner, skill.Owner, runecount, HealEvent.Normal, ev_skill);
                    ctx.World.DrawCard(skill.Owner, runecount, ev_skill);
                }
            }


            public Skill_2()
            {
                IsOnce = true;
                Triggers.Add(new Trigger_0(this));
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
                    Name = "人神",
                    Description = "限定技，当你进入濒死阶段时，你可以移去全部的【信仰】，将体力上限调整为移除的【信仰】的数量（至少为3），回复移除数量的体力并摸等量的牌。"
                };
            }
        }


        #endregion 

        public Sanae()
        {
            HP = 3;
            MaxHP = 3;
            Country = Reimu.CountryNameOfLeader;
            OtherCountries.Add(Kanako.CountryNameOfLeader);
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
            Skills.Add(new Skill_2());
        }

        public override Charactor Clone()
        {
            return new Sanae();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "东风谷早苗";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Sanae");
            info.AbilityRadar = new AbilityRadar() { Attack = 5, Defence = 4, Control = 4, Auxiliary = 5, LastStages = 1, Difficulty = 1 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }
    }

    

    
    
}
