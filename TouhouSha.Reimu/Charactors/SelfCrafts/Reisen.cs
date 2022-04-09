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
using TouhouSha.Reimu.Charactors.Forevers;

namespace TouhouSha.Reimu.Charactors.SelfCrafts
{
    /// <summary>
    /// 角色【铃仙·优谭华院·因幡】
    /// </summary>
    /// <remarks>
    /// 势力：自机（可替换为永夜） 3
    /// 勾玉
    /// 【狂瞳】：出牌阶段，你可以失去一点体力或者将你正面朝上的角色牌翻面，摸一张牌，并选择一名其他角色，其选择一项：
    ///     1. 将一张♥手牌交给你。
    ///     2. 指定另一名角色，视作对其使用了一张【决斗】。
    /// 【国士】：锁定技，当你成为【桃】或者【酒】的目标时，你的下一次受到的伤害-1，你的下一次造成的伤害+1（可叠加）。
    /// </remarks>
    public class Reisen : Charactor 
    {
        #region 技能【狂瞳】

        public class Skill_0 : Skill, ISkillInitative
        {
            public const string Selection_0 = "失去一点体力。";
            public const string Selection_1 = "翻面。";
            public const string Selection_2 = "交给铃仙一张♥手牌。";
            public const string Selection_3 = "让铃仙指定一名目标，你对其发动【决斗】。";
            
            public class SkillUseCondition : ConditionFilterFromSkill
            {
                public SkillUseCondition(Skill _skill) : base(_skill)
                {

                }

                public override bool Accept(Context ctx)
                {
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

            public class GiveHeart : CardFilter
            {
                public class CardWorth : CardFilterWorth
                {
                    public override double GetWorth(Context ctx, SelectCardBoardCore core, IEnumerable<Card> selecteds, Card want)
                    {
                        return 0.0d;
                    }

                    public override double GetWorthNo(Context ctx, SelectCardBoardCore core)
                    {
                        return 0.0d;
                    }
                }

                public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
                {
                    if (selecteds.Count() >= 1) return false;
                    if (want.Zone == null) return false;
                    if (want.Zone.KeyName?.Equals(Zone.Hand) != true) return false;
                    if (want.CardColor?.SeemAs(Enum_CardColor.Heart) != true) return false;
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

            public class SkillWorth : InitativeSkillWorth
            {
                public override double GetWorthSelect(Context ctx, Skill skill, Player user, IEnumerable<Player> selecteds, Player want)
                {
                    double worth = 0;
                    double killprob = AITools.CardGausser.GetProbablyOfCardKey(ctx, want, "Kill");
                    double heartprob = AITools.CardGausser.GetProbablyOfCardColor(ctx, want, Enum_CardColor.Heart);
                    double damageworth = -4;
                    Card killcard = ctx.World.GetCardInstance(KillCard.Normal);
                    foreach (Player other in ctx.World.GetAlivePlayers())
                    {
                        if (other == want) continue;
                        damageworth = Math.Max(damageworth, -AITools.WorthAcc.GetWorthPerHp(ctx, user, other));
                    }
                    killcard = killcard.Clone();
                    killcard.Zone = want.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                    // 挑选另外一个决斗对象，造成伤害一定要价值最大化。
                    // 简化决斗模型为：出杀就可对对方造成伤害，不出杀会对自己造成伤害。

                    // 如果是敌人，更倾向于给红桃（废牌）。
                    if (AITools.WorthAcc.GetHatred(ctx, user, want) < 0)
                    {
                        heartprob *= 0.8;
                        worth += heartprob *
                            (-AITools.WorthAcc.GetWorthExpected(ctx, user, want.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true))
                            + AITools.WorthAcc.GetWorthExpected(ctx, user, user.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true)));
                        worth += (1 - heartprob) * killprob * damageworth;
                        worth += (1 - heartprob) * (1 - killprob) * AITools.WorthAcc.GetWorthPerHp(ctx, user, want);
                    }
                    // 如果是队友，更倾向于给团队敌人造成伤害。
                    else
                    {
                        worth += killprob * damageworth;
                        worth += (1 - killprob) * AITools.WorthAcc.GetWorthPerHp(ctx, user, want);
                    }
                    killcard.Zone = null; 
                    return worth;
                }

                public override double GetWorthSelect(Context ctx, Skill skill, Player user, IEnumerable<Card> selecteds, Card want)
                {
                    return 0.0d;
                }
            }

            public Skill_0()
            {
                this.usecondition = new SkillUseCondition(this);
                this.targetfilter = new SkillTargetFilter(this);
                this.costfilter = new SkillCostFilter(this);
            }

            private SkillUseCondition usecondition;
            ConditionFilter ISkillInitative.UseCondition { get => usecondition; }

            private SkillTargetFilter targetfilter;
            PlayerFilter ISkillInitative.TargetFilter { get => targetfilter; }

            private SkillCostFilter costfilter;
            CardFilter ISkillInitative.CostFilter { get => costfilter; }

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
                    Name = "狂瞳",
                    Description = "出牌阶段，你可以失去一点体力或者将你正面朝上的角色牌翻面，摸一张牌，选择一名其他角色令其选择一项：\n" +
                        "\t1. 将一张♥手牌交给你。\n" +
                        "\t2. 指定另一名角色，视作对其使用了一张【决斗】。",
                };
            }

            void ISkillInitative.Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
            {
                SkillEvent ev_skill = new SkillEvent();
                ev_skill.Skill = this;
                ev_skill.Source = skilluser;
                ev_skill.Targets.Clear();
                ev_skill.Targets.AddRange(targets);
                ctx.World.InvokeEvent(ev_skill);
                if (ev_skill.Cancel) return;
                if (skilluser.IsFacedDown)
                {
                    ActionAfterSelected(ctx, skilluser, targets.FirstOrDefault(), Selection_0, ev_skill);
                    return;
                }
                ctx.World.ShowList("选择一项发动狂瞳", "请选择一项。", skilluser,
                    new List<object>() { Selection_0, Selection_1 }, 1, true, 15,
                    (selected) => ActionAfterSelected(ctx, skilluser, targets.FirstOrDefault(), selected.ToString(), ev_skill), null);
            }

            protected void ActionAfterSelected(Context ctx, Player skilluser, Player target, string selectedconfig, SkillEvent ev_skill)
            {
                bool hasgived = false;
                Zone handzone = target.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                switch (selectedconfig)
                {
                    case Selection_0:
                        ctx.World.Damage(skilluser, skilluser, 1, DamageEvent.Lost, null);
                        break;
                    case Selection_1:
                        ctx.World.FaceClip(skilluser, skilluser, null);
                        break;
                }
                ctx.World.DrawCard(skilluser, 1, ev_skill);
                if (handzone != null && handzone.Cards.FirstOrDefault(_card => _card.CardColor?.SeemAs(Enum_CardColor.Heart) == true) != null)
                {
                    ctx.World.ShowList("狂瞳的目标选择一项", "铃仙发动了【狂瞳】，请选择一项。", target,
                        new List<object>() { Selection_2, Selection_3 }, 1,
                        false, Config.GameConfig.Timeout_Handle,
                        (selected) =>
                        {
                            ctx.World.RequireCard("狂瞳交红桃", "请选择一张♥交给铃仙。", target,
                                new GiveHeart(),
                                true, Config.GameConfig.Timeout_Handle,
                                (cards) =>
                                {
                                    hasgived = true;
                                    ctx.World.MoveCards(target, cards, handzone, ev_skill, Enum_MoveCardFlag.FaceUp);
                                }, null);
                        }, null);
                }
                if (hasgived) return;
                ctx.World.SelectPlayer("狂瞳选择决斗对象", "请选择一个对象来发动【决斗】。", skilluser,
                    new FulfillNumberPlayerFilter(1, 1, target),
                    false, Config.GameConfig.Timeout_Handle,
                    (targets) =>
                    {
                        Card duelcard = ctx.World.GetCardInstance(DuelCard.Normal);
                        duelcard.IsVirtual = true;
                        duelcard.CardColor = new CardColor(Enum_CardColor.None);
                        duelcard.CardPoint = -1;
                        CardEvent ev = new CardEvent();
                        ev.Reason = ev_skill;
                        ev.Card = duelcard;
                        ev.Source = target;
                        ev.Targets.Clear();
                        ev.Targets.AddRange(targets);
                        ctx.World.InvokeEvent(ev);
                    }, null);
            }

            public override InitativeSkillWorth GetWorthAI()
            {
                return new SkillWorth();
            }
        }

        #endregion

        #region 技能【国士】

        public class Skill_1 : Skill
        {
            public const string DamageMinus = "下次伤害减成";
            public const string DamagePlus = "下次伤害加成";

            public class Trigger_0 : SkillTrigger, ITriggerInEvent
            {
                public class TriggerCondition : ConditionFilterFromSkill
                {
                    public TriggerCondition(Skill _skill) : base(_skill)
                    {

                    }

                    public override bool Accept(Context ctx)
                    {
                        if (!(ctx.Ev is CardDoneEvent)) return false;
                        CardDoneEvent ev = (CardDoneEvent)(ctx.Ev);
                        switch (ev.Card?.KeyName)
                        {
                            case PeachCard.Normal:
                            case LiqureCard.Normal:
                                break;
                            default:
                                return false;
                        }
                        if (!ev.Targets.Contains(skill.Owner)) return false;
                        return true;
                    }

                }

                public Trigger_0(Skill _skill) : base(_skill)
                {
                    Condition = new TriggerCondition(skill);
                }

                string ITriggerInEvent.EventKeyName { get => CardDoneEvent.DefaultKeyName; }

                public override void Action(Context ctx)
                {
                    int minus = skill.GetValue(Skill_1.DamageMinus);
                    int plus = skill.GetValue(Skill_1.DamagePlus);
                    skill.SetValue(Skill_1.DamageMinus, minus + 1);
                    skill.SetValue(Skill_1.DamagePlus, plus + 1);
                }
            }

            public class Trigger_1 : SkillTrigger, ITriggerInState
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
                        if (!(state.Ev is DamageEvent)) return false;
                        DamageEvent ev = (DamageEvent)(state.Ev);
                        if (ev.DamageValue <= 0) return false;
                        if (ev.DamageType?.Equals(DamageEvent.Lost) == true) return false;
                        if (ev.Target == skill.Owner) return true;
                        if (ev.Source == skill.Owner) return true;
                        return false;
                    }
                }

                public Trigger_1(Skill _skill) : base(_skill)
                {
                    Condition = new TriggerCondition(skill);
                }

                string ITriggerInState.StateKeyName { get => State.Damaging; }

                int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

                public override void Action(Context ctx)
                {
                    State state = ctx.World.GetCurrentState();
                    if (state == null) return;
                    if (!(state.Ev is DamageEvent)) return;
                    DamageEvent ev = (DamageEvent)(state.Ev);
                    if (ev.Target == skill.Owner)
                    {
                        int minus = skill.GetValue(Skill_1.DamageMinus);
                        if (minus == 0) return;
                        ev.DamageValue = Math.Max(0, ev.DamageValue - minus);
                        skill.SetValue(Skill_1.DamageMinus, 0);
                        return;
                    }
                    if (ev.Source == skill.Owner)
                    {
                        int plus = skill.GetValue(Skill_1.DamagePlus);
                        if (plus == 0) return;
                        ev.DamageValue += plus;
                        skill.SetValue(Skill_1.DamagePlus, 0);
                        return;
                    }
                }
            }

            public Skill_1()
            {
                IsLocked = true;
                Triggers.Add(new Trigger_0(this));
                Triggers.Add(new Trigger_1(this));
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
                    Name = "国士",
                    Description = "锁定技，当你成为【桃】或者【酒】的目标时，你的下一次受到的伤害-1，你的下一次造成的伤害+1（可叠加）。"
                };
            }
        }

        #endregion

        public class Reisen_ListWorth : ListSelectWorth
        {
            public override double GetWorthSelect(Context ctx, ListBoardCore core, string want)
            {
                switch (core.KeyName)
                {
                    #region 选择一项发动狂瞳
                    case "选择一项发动狂瞳":
                        switch (want)
                        {
                            case Skill_0.Selection_0:
                                return core.Controller.IsFacedDown ? 10.0d : 0.0d;
                            case Skill_0.Selection_1:
                                return core.Controller.IsFacedDown ? 0.0d : 10.0d;
                        }
                        break;
                    #endregion
                    case "狂瞳的目标选择一项":
                        switch (want)
                        {
                            case Skill_0.Selection_2:
                                {
                                    Player reisen = ctx.World.Players.FirstOrDefault(_player => _player.Charactors.FirstOrDefault(_char => _char is Reisen) != null);
                                    if (reisen == null) return 0.0d;
                                    Player opttarget = null;
                                    double optworth = -10000;
                                    foreach (Player target in ctx.World.GetAlivePlayers())
                                    {
                                        if (target == core.Controller) continue;
                                        double worth = -AITools.WorthAcc.GetWorthPerHp(ctx, reisen, target);
                                        if (worth > optworth)
                                        {
                                            optworth = worth;
                                            opttarget = target;
                                        }
                                    }
                                    if (opttarget == null) return 0.0d;
                                    return -AITools.WorthAcc.GetWorthPerHp(ctx, core.Controller, opttarget);
                                }
                            case Skill_0.Selection_3:
                                {
                                    CardFilterWorth worthobject = new Skill_0.GiveHeart.CardWorth();
                                    worthobject.AITools = AITools;
                                    return worthobject.GetWorthNo(ctx, new SelectCardBoardCore() { Controller = core.Controller });
                                }
                        }
                        break;
                }
                return 0.0d;
            }
        }

        public Reisen()
        {
            MaxHP = 3;
            HP = 3;
            Country = Reimu.CountryNameOfLeader;
            OtherCountries.Add(Kaguya.CountryNameOfLeader);
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
        }

        public override Charactor Clone()
        {
            return new Reisen();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "铃仙";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Reisen");
            info.AbilityRadar = new AbilityRadar() { Attack = 5, Defence = 3, Control = 2, Auxiliary = 1, LastStages = 1, Difficulty = 3 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }

        public override ListSelectWorth GetListWorthAI()
        {
            return new Reisen_ListWorth();
        }
    }

}
