using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.UIs;
using TouhouSha.Core.AIs;
using TouhouSha.Koishi.Calculators;
using TouhouSha.Koishi.Cards;
using TouhouSha.Koishi.Cards.Weapons;
using TouhouSha.Koishi.Triggers;
using TouhouSha.Reimu.Charactors.Ghosts;

namespace TouhouSha.Reimu.Charactors.SelfCrafts
{

    /// <summary>
    /// 角色【魂魄妖梦】
    /// </summary>
    /// <remarks>
    /// 势力：自机（可替换为幽灵） 4勾玉
    /// 【双刀】：你可以将武器牌装到防具栏上。当你装备两把武器时，攻击范围以最大的来计算，出牌阶段你可以额外打出一张【杀】。
    /// 【名刀】：限定技，出牌阶段，从所有牌中获得【楼观剑】和【白楼剑】。
    /// 【永劫】：限定技，出牌阶段，当你角色牌正面朝上并装备【白楼剑】时，你可以将角色牌翻面，视作对一名角色使用了三张【杀】。
    /// 【现世】：限定技，出牌阶段，当你角色牌正面朝上并装备【楼观剑】时，你可以将角色牌翻面，视作对至多三名角色各使用了一张【杀】。
    /// </remarks>
    public class YouMon : Charactor
    {
        #region 技能【双刀】
        public class Skill_0 : Skill
        {
            #region 替换【装备一个种类多张时弃置其他张】的触发器为【装备武器两张以上再选择弃置】的触发器
            public class Trigger_0 : SkillTrigger, ITriggerInEvent
            {
                public const string DefaultKeyName = "双刀装备触发器";
                public const string Ask0 = "武器装备到防具槽0";
                public const string Ask1 = "武器装备到防具槽1";

                public class TriggerCondition : ConditionFilterFromSkill
                {
                    public TriggerCondition(Skill _skill) : base(_skill) { }

                    public override bool Accept(Context ctx)
                    {
                        if (!(ctx.Ev is UseTriggerEvent)) return false;
                        UseTriggerEvent ev0 = (UseTriggerEvent)(ctx.Ev);
                        if (!(ev0.Reason is MoveCardDoneEvent)) return false;
                        if (!(ev0.NewTrigger is EquipReplaceTrigger)) return false;
                        MoveCardDoneEvent ev1 = (MoveCardDoneEvent)(ev0.Reason);
                        if (ev1.NewZone?.KeyName?.Equals(Zone.Equips) != true) return false;
                        if (ev1.NewZone.Owner != skill.Owner) return false;
                        if (ev1.MovedCards.FirstOrDefault(_card => _card.CardType?.SubType?.E == Enum_CardSubType.Armor) != null) return false;
                        EquipZone equipzone = ev1.NewZone as EquipZone;
                        if (equipzone == null) return false;
                        if (!equipzone.Cells.FirstOrDefault(_cell => _cell.E == Enum_CardSubType.Armor).IsEnabled) return false;
                        if (equipzone.Cards.Count(_card => _card.CardType?.SubType?.E == Enum_CardSubType.Weapon) < 2) return false;
                        return true;
                    }
                }

                #region 触发器【装备武器两张以上再选择弃置】

                public class EquipDoubleWeaponTrigger : Trigger
                {
                    public class TriggerCondition : ConditionFilter
                    {
                        public override bool Accept(Context ctx)
                        {
                            if (!(ctx.Ev is MoveCardDoneEvent)) return false;
                            MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
                            if (ev.NewZone?.KeyName?.Equals(Zone.Equips) != true) return false;
                            return true;
                        }
                    }

                    public EquipDoubleWeaponTrigger()
                    {
                        Condition = new TriggerCondition();
                    }

                    public override void Action(Context ctx)
                    {
                        if (!(ctx.Ev is MoveCardDoneEvent)) return;
                        MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
                        Dictionary<Enum_CardSubType, List<Card>> cardofsubs = new Dictionary<Enum_CardSubType, List<Card>>();
                        HashSet<Card> handleds = new HashSet<Card>();
                        List<Card> discards = new List<Card>();
                        foreach (Card card in ev.MovedCards.Concat(ev.NewZone.Cards))
                        {
                            List<Card> sublist = null;
                            if (handleds.Contains(card)) continue;
                            handleds.Add(card);
                            if (card.CardType?.SubType == null) continue;
                            if (cardofsubs.TryGetValue(card.CardType.SubType.E, out sublist))
                            {
                                sublist = new List<Card>();
                                cardofsubs.Add(card.CardType.SubType.E, sublist);
                            }
                            sublist.Add(card);
                        }
                        bool isdoubleweapon = cardofsubs.ContainsKey(Enum_CardSubType.Weapon) && cardofsubs[Enum_CardSubType.Weapon].Count() >= 2;
                        bool isnewarmor = ev.MovedCards.FirstOrDefault(_card => _card.CardType?.SubType?.E == Enum_CardSubType.Armor) != null;
                        foreach (KeyValuePair<Enum_CardSubType, List<Card>> kvp in cardofsubs)
                        {
                            // 处理武器槽的占用问题。
                            if (kvp.Key == Enum_CardSubType.Weapon)
                            {
                                // 装备新防具，防具槽只能放置防具。
                                if (isnewarmor && kvp.Value.Count() > 1)
                                    discards.AddRange(kvp.Value.GetRange(1, kvp.Value.Count - 1));
                                // 允许防具槽放置武器。
                                else if (!isnewarmor && kvp.Value.Count() > 2)
                                    discards.AddRange(kvp.Value.GetRange(2, kvp.Value.Count - 2));
                            }
                            // 处理防具槽的占用问题。
                            else if (kvp.Key == Enum_CardSubType.Armor)
                            {
                                // 装备新防具，旧的防具被替换。
                                if (isnewarmor && kvp.Value.Count() > 1)
                                    discards.AddRange(kvp.Value.GetRange(1, kvp.Value.Count - 1));
                                // 防具槽没有被放置武器，只能存在一个防具。
                                else if (!isnewarmor && !isdoubleweapon && kvp.Value.Count() > 1)
                                    discards.AddRange(kvp.Value.GetRange(1, kvp.Value.Count - 1));
                                // 防具槽有被放置武器，不能存在一个防具。
                                else if (!isnewarmor && isdoubleweapon && kvp.Value.Count() > 0)
                                    discards.AddRange(kvp.Value);
                            }
                            // 处理其他槽（UFO）的占用问题。
                            else
                            {
                                if (kvp.Value.Count() <= 1) continue;
                                discards.AddRange(kvp.Value.GetRange(1, kvp.Value.Count() - 1));
                            }
                        }
                        if (discards.Count() > 0)
                        {
                            Zone discardzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
                            if (discardzone != null) ctx.World.MoveCards(ev.NewZone.Owner, discards, discardzone, ev);
                        }
                    }
                }

                #endregion

                public Trigger_0(Skill _skill) : base(_skill)
                {
                    KeyName = DefaultKeyName;
                    Condition = new TriggerCondition(skill);
                }

                string ITriggerInEvent.EventKeyName { get => UseFilterEvent.DefaultKeyName; }

                public override void Action(Context ctx)
                {
                    if (!(ctx.Ev is UseTriggerEvent)) return;
                    UseTriggerEvent ev0 = (UseTriggerEvent)(ctx.Ev);
                    if (!(ev0.Reason is MoveCardDoneEvent)) return;
                    MoveCardDoneEvent ev1 = (MoveCardDoneEvent)(ev0.Reason);
                    if (ev1.MovedCards.FirstOrDefault(_card => _card.CardType?.SubType?.E == Enum_CardSubType.Weapon) != null)
                    {
                        List<Card> weaponlist = ev1.NewZone.Cards.Where(_card => _card.CardType?.SubType?.E == Enum_CardSubType.Weapon).ToList();
                        if (weaponlist.Count() == 2)
                        {
                            if (!ctx.World.Ask(skill.Owner, Ask0, "是否要将武器装备于防具栏？"))
                                return;
                        }
                        else if (weaponlist.Count() == 3)
                        {
                            if (!ctx.World.Ask(skill.Owner, Ask1, "是否要将武器装备于防具栏？"))
                            {
                                // 将最新的武器和第一个武器（在武器槽）交换位置，这样武器槽的武器就会被触发器弃置。
                                int i0 = ev1.NewZone.Cards.IndexOf(weaponlist[0]);
                                int i1 = ev1.NewZone.Cards.IndexOf(weaponlist[2]);
                                ev1.NewZone.Cards[i0] = weaponlist[2];
                                ev1.NewZone.Cards[i1] = weaponlist[0];
                            }
                        }
                    }
                    ev0.NewTrigger = new EquipDoubleWeaponTrigger();
                }
            }

            #endregion

            #region 计算器【两把武器的攻击范围取最大】

            public class MaxAttackRange_Calculator : CalculatorFromSkill, ICalculatorProperty
            {
                public MaxAttackRange_Calculator(Skill _skill) : base(_skill)
                {

                }

                string ICalculatorProperty.PropertyName { get => World.KillRange; }

                public override int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
                {
                    if (!(obj is Player)) return oldvalue;
                    Player player = (Player)obj;
                    if (player != skill.Owner) return oldvalue;
                    Zone equipzone = player.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Equips) == true);
                    if (equipzone == null) return oldvalue;
                    List<Card> weapons = equipzone.Cards.Where(_zone => _zone.CardType?.SubType?.E == Enum_CardSubType.Weapon).ToList();
                    if (weapons.Count() < 2) return oldvalue;
                    // 两个武器同时计算攻击范围时，减去最小的那个。
                    return oldvalue - weapons.Min(_weapon => (_weapon.Skills
                        .FirstOrDefault(_skill => _skill.Calculators.FirstOrDefault(_calc => _calc is WeaponKillRangePlusCalculator) != null)
                        ?.Calculators?.FirstOrDefault(_calc => _calc is WeaponKillRangePlusCalculator) as WeaponKillRangePlusCalculator)
                        ?.KillRange ?? 0);
                }
            }

            #endregion

            #region 计算器【能够额外打出一张杀】

            public class DoubleKill_Calculator : CalculatorFromSkill, ICalculatorProperty
            {
                public DoubleKill_Calculator(Skill _skill) : base(_skill)
                {

                }

                string ICalculatorProperty.PropertyName { get => KillCard.KillMaxUse; }


                public override int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
                {
                    if (!(obj is State)) return oldvalue;
                    State state = (State)obj;
                    if (state.KeyName?.Equals(State.UseCard) != true) return oldvalue;
                    if (state.Owner != skill.Owner) return oldvalue;
                    Zone equipzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Equips) == true);
                    if (equipzone == null) return oldvalue;
                    List<Card> weapons = equipzone.Cards.Where(_zone => _zone.CardType?.SubType?.E == Enum_CardSubType.Weapon).ToList();
                    if (weapons.Count() < 2) return oldvalue;
                    return oldvalue + 1;
                }
            }

            #endregion

            public Skill_0()
            {
                Triggers.Add(new Trigger_0(this));
                Calculators.Add(new MaxAttackRange_Calculator(this));
                Calculators.Add(new DoubleKill_Calculator(this));
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
                    Name = "双刀",
                    Description = "你可以将武器牌装到防具槽上。当你装备两把武器时，攻击范围以最大的来计算，出牌阶段你可以额外打出一张【杀】。"
                };
            }
        }

        #endregion

        #region 技能【名刀】

        public class Skill_1 : Skill, ISkillInitative
        {
            public const string Used = "已经被使用过";

            public class SkillUseCondition : ConditionFilterFromSkill
            {
                public SkillUseCondition(Skill _skill) : base(_skill)
                {

                }

                public override bool Accept(Context ctx)
                {
                    if (skill.GetValue(Skill_1.Used) == 1) return false;
                    return true;
                }
            }

            public class SkillTargetFilter : PlayerFilterFromSkill
            {
                public SkillTargetFilter(Skill _skill) : base(_skill)
                {

                }

                public override Enum_PlayerFilterFlag GetFlag(Context ctx)
                {
                    return Enum_PlayerFilterFlag.ForceAll;
                }

                public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
                {
                    if (want == skill.Owner) return false;
                    return true;
                }

                public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
                {
                    return true;
                }
            }

            public class SkillCostFilter : CardFilterFromSkill
            {
                public SkillCostFilter(Skill _skill) : base(_skill)
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

            public Skill_1()
            {
                this.usecondition = new SkillUseCondition(this);
                this.targetfilter = new SkillTargetFilter(this);
                this.costfilter = new SkillCostFilter(this);
                Triggers.Add(new SkillOnceResetTrigger(this, Used));
            }

            private SkillUseCondition usecondition;
            public ConditionFilter UseCondition => usecondition;

            private SkillTargetFilter targetfilter;
            public PlayerFilter TargetFilter => targetfilter;

            private SkillCostFilter costfilter;
            public CardFilter CostFilter => costfilter;

            public void Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
            {
                SetValue(Skill_1.Used, 1);
                SkillEvent ev0 = new SkillEvent();
                ev0.Skill = this;
                ev0.Source = skilluser;
                ev0.Targets.Clear();
                ev0.Targets.Add(skilluser);
                ctx.World.InvokeEvent(ev0);
                if (ev0.Cancel) return;
                Card weapon0 = null;
                Card weapon1 = null;
                foreach (Zone zone in ctx.World.CommonZones)
                    foreach (Card card in zone.Cards)
                    {
                        switch (card.KeyName)
                        {
                            case HakulouCard.Normal:
                                if (weapon0 == null) weapon0 = card;
                                break;
                            case LouKanCard.Normal:
                                if (weapon1 == null) weapon1 = card;
                                break;
                        }
                    }
                foreach (Player player in ctx.World.Players)
                    foreach (Zone zone in player.Zones)
                        foreach (Card card in zone.Cards)
                        {
                            switch (card.KeyName)
                            {
                                case HakulouCard.Normal:
                                    if (weapon0 == null) weapon0 = card;
                                    break;
                                case LouKanCard.Normal:
                                    if (weapon1 == null) weapon1 = card;
                                    break;
                            }
                        }
                Zone handzone = skilluser.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                if (handzone == null) return;
                List<Card> weapons = new List<Card>();
                if (weapon0 != null) weapons.Add(weapon0);
                if (weapon1 != null) weapons.Add(weapon1);
                if (weapons.Count() > 0) ctx.World.MoveCards(skilluser, weapons, handzone, ev0);
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
                    Name = "名刀",
                    Description = "限定技，出牌阶段，从所有牌中获得【楼观剑】和【白楼剑】。"
                };
            }
        }


        #endregion

        #region 技能【永劫】

        public class Skill_2 : Skill, ISkillInitative
        {
            public const string Used = "已经被使用过";

            public class SkillUseCondition : ConditionFilterFromSkill
            {
                public SkillUseCondition(Skill _skill) : base(_skill)
                {
                    SetValue(Skill_2.Used, 1);
                }

                public override bool Accept(Context ctx)
                {
                    if (skill.GetValue(Skill_2.Used) == 1) return false;
                    if (skill.Owner.IsFacedDown) return false;
                    Zone equipzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Equips) == true);
                    if (equipzone == null) return false;
                    Card weapon = equipzone.Cards.FirstOrDefault(_card => _card.KeyName?.Equals(HakulouCard.Normal) == true);
                    if (weapon == null) return false;
                    return true;
                }

                
            }

            public class SkillTargetFilter : PlayerFilterFromSkill
            {
                public SkillTargetFilter(Skill _skill) : base(_skill)
                {

                }

                public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
                {
                    if (selecteds.Count() >= 1) return false;
                    if (want == skill.Owner) return false;
                    return true;
                }

                public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
                {
                    return selecteds.Count() >= 1;
                }
            }

            public class SkillCostFilter : CardFilterFromSkill
            {
                public SkillCostFilter(Skill _skill) : base(_skill)
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

            public class SkillWorth : InitativeSkillWorth
            {
                public override double GetWorthSelect(Context ctx, Skill skill, Player user, IEnumerable<Player> selecteds, Player want)
                {
                    Card killcard = ctx.World.GetCardInstance(KillCard.Normal);
                    double worth = 0;
                    killcard = killcard.Clone();
                    killcard.Zone = user.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                    worth += killcard.GetWorthAI().GetWorth(ctx, killcard, user, new Player[] { }, want) * 1.5;
                    worth += AITools.WorthAcc.GetWorth(ctx, user, killcard) * 1.5;
                    worth += AITools.WorthAcc.GetWorthPerHp(ctx, user, want) * 1.5;
                    worth -= 2.88d;
                    killcard.Zone = null;
                    return worth;
                }

                public override double GetWorthSelect(Context ctx, Skill skill, Player user, IEnumerable<Card> selecteds, Card want)
                {
                    return 0.0d;
                }
            }

            public Skill_2()
            {
                this.usecondition = new SkillUseCondition(this);
                this.targetfilter = new SkillTargetFilter(this);
                this.costfilter = new SkillCostFilter(this);
                Triggers.Add(new SkillOnceResetTrigger(this, Used));
            }

            private SkillUseCondition usecondition;
            public ConditionFilter UseCondition => usecondition;

            private SkillTargetFilter targetfilter;
            public PlayerFilter TargetFilter => targetfilter;

            private SkillCostFilter costfilter;
            public CardFilter CostFilter => costfilter;

            public void Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
            {
                SetValue(Skill_2.Used, 1);
                SkillEvent ev0 = new SkillEvent();
                ev0.Skill = this;
                ev0.Source = skilluser;
                ev0.Targets.Clear();
                ev0.Targets.AddRange(targets);
                ctx.World.InvokeEvent(ev0);
                if (ev0.Cancel) return;
                ctx.World.FaceClip(skilluser, skilluser, ev0);
                Card killcard = ctx.World.GetCardInstance(KillCard.Normal);
                killcard = killcard.Clone();
                killcard.CardColor = new CardColor(Enum_CardColor.None);
                killcard.CardPoint = -1;
                killcard.IsVirtual = true;
                for (int i = 0; i < 3; i++)
                {
                    CardEvent ev1 = new CardEvent();
                    ev1.Card = killcard;
                    ev1.Source = ev0.Source;
                    ev1.Targets.Clear();
                    ev1.Targets.AddRange(ev0.Targets);
                    ctx.World.InvokeEvent(ev1);
                }
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
                    Name = "永劫",
                    Description = "限定技，出牌阶段，当你角色牌正面朝上并装备【白楼剑】时，你可以将角色牌翻面，视作对一名角色使用了三张【杀】（无视距离且不计入出杀次数）。"
                };
            }

            public override InitativeSkillWorth GetWorthAI()
            {
                return new SkillWorth();
            }
        }

        #endregion

        #region 技能【现世】

        public class Skill_3 : Skill, ISkillInitative
        {
            public const string Used = "已经被使用过";

            public class SkillUseCondition : ConditionFilterFromSkill
            {
                public SkillUseCondition(Skill _skill) : base(_skill)
                {
                    SetValue(Skill_3.Used, 1);
                }

                public override bool Accept(Context ctx)
                {
                    if (skill.GetValue(Skill_3.Used) == 1) return false;
                    if (skill.Owner.IsFacedDown) return false;
                    Zone equipzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Equips) == true);
                    if (equipzone == null) return false;
                    Card weapon = equipzone.Cards.FirstOrDefault(_card => _card.KeyName?.Equals(LouKanCard.Normal) == true);
                    if (weapon == null) return false;
                    return true;
                }
            }

            public class SkillTargetFilter : PlayerFilterFromSkill
            {
                public SkillTargetFilter(Skill _skill) : base(_skill)
                {

                }

                public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
                {
                    if (selecteds.Count() >= 3) return false;
                    if (want == skill.Owner) return false;
                    return true;
                }

                public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
                {
                    return selecteds.Count() >= 1;
                }
            }

            public class SkillCostFilter : CardFilterFromSkill
            {
                public SkillCostFilter(Skill _skill) : base(_skill)
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

            public class SkillWorth : InitativeSkillWorth
            {
                public override double GetWorthSelect(Context ctx, Skill skill, Player user, IEnumerable<Player> selecteds, Player want)
                {
                    Card killcard = ctx.World.GetCardInstance(KillCard.Normal);
                    double worth = 0;
                    killcard = killcard.Clone();
                    killcard.Zone = user.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                    worth += killcard.GetWorthAI().GetWorth(ctx, killcard, user, new Player[] { }, want);
                    worth += AITools.WorthAcc.GetWorth(ctx, user, killcard);
                    worth -= 2.88d;
                    killcard.Zone = null;
                    return worth;
                }

                public override double GetWorthSelect(Context ctx, Skill skill, Player user, IEnumerable<Card> selecteds, Card want)
                {
                    return 0.0d;
                }
            }

            public Skill_3()
            {
                this.usecondition = new SkillUseCondition(this);
                this.targetfilter = new SkillTargetFilter(this);
                this.costfilter = new SkillCostFilter(this);
                Triggers.Add(new SkillOnceResetTrigger(this, Used));
            }

            private SkillUseCondition usecondition;
            public ConditionFilter UseCondition => usecondition;

            private SkillTargetFilter targetfilter;
            public PlayerFilter TargetFilter => targetfilter;

            private SkillCostFilter costfilter;
            public CardFilter CostFilter => costfilter;

            public void Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
            {
                SetValue(Skill_3.Used, 1);
                SkillEvent ev0 = new SkillEvent();
                ev0.Skill = this;
                ev0.Source = skilluser;
                ev0.Targets.Clear();
                ev0.Targets.AddRange(targets);
                ctx.World.InvokeEvent(ev0);
                if (ev0.Cancel) return;
                ctx.World.FaceClip(skilluser, skilluser, ev0);
                Card killcard = ctx.World.GetCardInstance(KillCard.Normal);
                killcard = killcard.Clone();
                killcard.CardColor = new CardColor(Enum_CardColor.None);
                killcard.CardPoint = -1;
                killcard.IsVirtual = true;
                CardEvent ev1 = new CardEvent();
                ev1.Card = killcard;
                ev1.Source = ev0.Source;
                ev1.Targets.Clear();
                ev1.Targets.AddRange(ev0.Targets);
                ctx.World.InvokeEvent(ev1);
            }

            public override Skill Clone()
            {
                return new Skill_3();
            }

            public override Skill Clone(Card newcard)
            {
                return new Skill_3();
            }

            public override SkillInfo GetInfo()
            {
                return new SkillInfo()
                {
                    Name = "现世",
                    Description = "限定技，出牌阶段，当你角色牌正面朝上并装备【楼观剑】时，你可以将角色牌翻面，视作对至多三名角色各使用了一张【杀】（无视距离且不计入出杀次数）。"
                };
            }
        }

        #endregion

        public class YouMon_AskWorth : AskWorth
        {
            public override double GetWorthNo(Context ctx, Player controller, string keyname)
            {
                return 0;
            }

            public override double GetWorthYes(Context ctx, Player controller, string keyname)
            {
                switch (keyname)
                {
                    #region 将武器装备到防具栏上 
                    case Skill_0.Trigger_0.Ask0:
                        return 10.0d;
                    #endregion
                    #region 替换防具栏的武器             
                    case Skill_0.Trigger_0.Ask1:
                        EquipZone equipzone = controller.Zones.FirstOrDefault(_zone => _zone is EquipZone) as EquipZone;
                        List<Card> weapons = equipzone.Cards.Where(_card => _card.CardType?.SubType?.E == Enum_CardSubType.Weapon).ToList();
                        if (weapons.Count() < 2) return 10.0d;
                        return -AITools.WorthAcc.GetWorth(ctx, controller, weapons[1]) + AITools.WorthAcc.GetWorth(ctx, controller, weapons[0]);
                    #endregion
                }
                return 0.0d;
            }
        }

        public YouMon()
        {
            HP = 4;
            MaxHP = 4;
            Country = Reimu.CountryNameOfLeader;
            OtherCountries.Add(Yoyoko.CountryNameOfLeader);
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
            Skills.Add(new Skill_2());
            Skills.Add(new Skill_3());
        }

        public override Charactor Clone()
        {
            return new YouMon();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "魂魄妖梦";
            info.Image = ImageHelper.LoadCardImage("Charactors", "YouMon");
            info.AbilityRadar = new AbilityRadar() { Attack = 5, Defence = 1, Control = 1, Auxiliary = 1, LastStages = 1, Difficulty = 5 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }

        public override AskWorth GetAskWorthAI()
        {
            return new YouMon_AskWorth();
        }
    }
   

}
