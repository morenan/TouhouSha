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

namespace TouhouSha.Reimu.Charactors.SelfCrafts
{
    /// <summary>
    /// 角色【雾雨魔理沙】
    /// </summary>
    /// <remarks>
    /// 势力：自机（可替换为魔使） 3勾玉
    /// 【大盗】：出牌阶段限一次，你可以选择你以外X名角色，各获取其一张牌，然后丢弃X张牌。如果X不少于2，将你的角色牌翻面。
    /// 【火花】：当你以出牌阶段打出以外的方式，将牌置入弃牌堆时，指定一名角色，除非其弃置任意张牌使得花色都相符，否则受到你的1点伤害。
    /// </remarks>
    public class Marisa : Charactor
    {
        public const string CountryNameOfLeader = "魔使";

        public class Marisa_AskWorth : AskWorth
        {
            public override double GetWorthNo(Context ctx, Player controller, string keyname)
            {
                return 0.0d;
            }

            public override double GetWorthYes(Context ctx, Player controller, string keyname)
            {
                switch (keyname)
                {
                    case Marisa.Skill_1.DefaultKeyName:
                        return -ctx.World.GetAlivePlayers().Where(_player => _player != controller)
                            .Max(_player => -AITools.WorthAcc.GetWorthPerHp(ctx, controller, _player));
                }
                return 0.0d;
            }
        }

        public override AskWorth GetAskWorthAI()
        {
            return new Marisa_AskWorth();
        }

        #region 技能【大盗】

        public class Skill_0 : Skill, ISkillInitative
        {
            public const string Used = "使用大盗的次数";
            
            public class SkillUseCondition : ConditionFilter
            {
                public override bool Accept(Context ctx)
                {
                    State state = ctx.World.GetPlayerState();
                    if (state == null) return false;
                    return state.GetValue(Used) < 1;
                }
            }

            public class SkillTargetFilter : PlayerFilter
            {
                public SkillTargetFilter(Skill _skill) { this.skill = _skill; }

                private Skill skill;
                public Skill Skill { get { return this.skill; } }

                public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
                {
                    if (selecteds.Count() >= 2) return false;
                    if (want != skill.Owner) return false;
                    return true;
                }

                public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
                {
                    return selecteds.Count() > 0;
                }
            }

            public class SkillCostFilter : CardFilter
            {
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

            /// <summary>
            /// 自动AI，尽量偏向进攻性。
            /// </summary>
            /// <remarks>
            /// 保证能凑出四种花色并丢弃，造成两点伤害才能加快节奏。
            /// 否则摸一张慢消耗是比较好的策略。
            /// 场上如果有灵梦，可以忽略翻面损失。
            /// </remarks>
            public class SkillAuto : InitativeSkillAuto
            {
                /// <summary>
                /// 所有的花色，都丢弃能造成2点伤害。
                /// </summary>
                private Enum_CardColor[] allcolors = new Enum_CardColor[] 
                    { Enum_CardColor.Heart, Enum_CardColor.Diamond, Enum_CardColor.Spade, Enum_CardColor.Club };
                /// <summary>
                /// 手牌能丢弃的花色集。
                /// </summary>
                private Dictionary<Enum_CardColor, KeyValuePair<double, Card>> handcolors
                    = new Dictionary<Enum_CardColor, KeyValuePair<double, Card>>();
                /// <summary>
                /// 场上能看到的花色，捡起来丢弃。
                /// </summary>
                private Dictionary<Enum_CardColor, List<KeyValuePair<double, Card>>> sawcolors
                    = new Dictionary<Enum_CardColor, List<KeyValuePair<double, Card>>>();
                /// <summary>
                /// 当前检索的玩家表。
                /// </summary>
                private List<Player> playerlist = new List<Player>();
                /// <summary>
                /// 凑花色时，最优价值的目标选择序列。
                /// </summary>
                private List<Player> optselecteds = new List<Player>();
                /// <summary>
                /// 凑花色时的最优价值。
                /// </summary>
                private double optworth;

                public override bool GetSelection(Context ctx, Skill skill, Player user, List<Player> targets, List<Card> costs)
                {
                    bool isyes = false;
                    GetWorthAndSelection(ctx, skill, user, new List<Player>(), new List<Card>(), out isyes);
                    return isyes;
                }

                public override double GetWorth(Context ctx, Skill skill, Player user)
                {
                    bool isyes = false;
                    return GetWorthAndSelection(ctx, skill, user, new List<Player>(), new List<Card>(), out isyes);
                }

                public double GetWorthAndSelection(Context ctx, Skill skill, Player user, List<Player> targets, List<Card> costs, out bool isyes)
                {
                    Player reimu = ctx.World.GetAlivePlayers().FirstOrDefault(_player => _player.Charactors.FirstOrDefault(_char => _char is Reimu) != null);
                    Zone handzone = user.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                    PlayerFilter targetfilter = ctx.World.TryReplaceNewPlayerFilter(((ISkillInitative)skill).TargetFilter, null);
                    CardFilter costfilter = ctx.World.TryReplaceNewCardFilter(((ISkillInitative)skill).CostFilter, null);
                    double damageworth = ctx.World.GetAlivePlayers()
                        .Where(_player => _player != user)
                        .Max(_player => -AITools.WorthAcc.GetWorthPerHp(ctx, user, _player));
                    handcolors.Clear();
                    sawcolors.Clear();
                    foreach (Card card in handzone.Cards)
                    {
                        double worth = -AITools.WorthAcc.GetWorth(ctx, user, card);
                        KeyValuePair<double, Card> kvp0 = default(KeyValuePair<double, Card>);
                        KeyValuePair<double, Card> kvp1 = new KeyValuePair<double, Card>(worth, card);
                        if (!handcolors.TryGetValue(card.CardColor.E, out kvp0))
                            handcolors.Add(card.CardColor.E, kvp1);
                        else if (kvp0.Key < kvp1.Key)
                            handcolors[card.CardColor.E] = kvp1;
                    }
                    // 有灵梦就直接快节奏一波。
                    // 有值得集火的对象就快节奏放倒。
                    #region 慢节奏
                    if (reimu == null && damageworth <= 1)
                    {
                        double optworth = double.MinValue; 
                        Player opttarget = null;
                        foreach (Player target in ctx.World.GetAlivePlayers())
                        {
                            double suboptworth = double.MinValue;
                            if (targets.Contains(target)) continue;
                            if (!targetfilter.CanSelect(ctx, targets, target)) continue;
                            foreach (Zone zone in target.Zones)
                            {
                                switch (zone.KeyName)
                                {
                                    case Zone.Hand:
                                    case Zone.Equips:
                                    case Zone.Judge:
                                        foreach (Card card in zone.Cards)
                                            suboptworth = Math.Max(suboptworth, -AITools.WorthAcc.GetWorth(ctx, user, card));
                                        break;
                                }
                            }
                            if (suboptworth > optworth)
                            {
                                optworth = suboptworth;
                                opttarget = target;
                            }
                        }
                        if (opttarget != null)
                        {
                            isyes = true;
                            targets.Add(opttarget);
                            return optworth;
                        }
                    }
                    #endregion
                    // 尽量凑齐四种花色，伤害拉满。
                    #region 快节奏
                    else
                    {
                        #region 搜集看到的花色集合
                        foreach (Player target in ctx.World.GetAlivePlayers())
                        {
                            if (!targetfilter.CanSelect(ctx, new Player[] { }, target)) continue;
                            Zone equipzone = target.Zones.FirstOrDefault(_zone => _zone is EquipZone);
                            Zone judgezone = target.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
                            List<Player> playerlist = new List<Player>();
                            if (equipzone != null)
                            {
                                foreach (Card card in equipzone.Cards)
                                {
                                    double sheepworth = -AITools.WorthAcc.GetWorth(ctx, user, card);
                                    KeyValuePair<double, Card> kvp = new KeyValuePair<double, Card>(sheepworth, card);
                                    List<KeyValuePair<double, Card>> list = null;
                                    if (!sawcolors.TryGetValue(card.CardColor.E, out list))
                                    {
                                        list = new List<KeyValuePair<double, Card>>() { kvp };
                                        sawcolors.Add(card.CardColor.E, list);
                                        continue;
                                    }
                                    list.Add(kvp);
                                }
                            }
                            if (judgezone != null)
                            {
                                foreach (Card card in judgezone.Cards)
                                {
                                    double sheepworth = -AITools.WorthAcc.GetWorth(ctx, user, card);
                                    KeyValuePair<double, Card> kvp = new KeyValuePair<double, Card>(sheepworth, card);
                                    List<KeyValuePair<double, Card>> list = null;
                                    if (!sawcolors.TryGetValue(card.CardColor.E, out list))
                                    {
                                        list = new List<KeyValuePair<double, Card>>() { kvp };
                                        sawcolors.Add(card.CardColor.E, list);
                                    }
                                    list.Add(kvp);
                                }
                            }
                        }
                        foreach (List<KeyValuePair<double, Card>> list in sawcolors.Values)
                            list.Sort((p0, p1) => -(p0.Key.CompareTo(p1.Key)));
                        #endregion
                        #region 满足花色的条件下，最优化选择
                        playerlist.Clear();
                        optselecteds.Clear();
                        optworth = double.MinValue;
                        GetOptimize(0, 0);
                        targets.AddRange(optselecteds);
                        #endregion
                        #region 剩下的目标选择
                        double worth = optworth;
                        foreach (Player target in ctx.World.GetAlivePlayers())
                        {
                            double optworth = double.MinValue;
                            if (targets.Contains(target)) continue;
                            if (!targetfilter.CanSelect(ctx, targets, target)) continue;
                            foreach (Zone zone in target.Zones)
                            {
                                switch (zone.KeyName)
                                {
                                    case Zone.Hand:
                                    case Zone.Equips:
                                    case Zone.Judge:
                                        foreach (Card card in zone.Cards)
                                            optworth = Math.Max(optworth, -AITools.WorthAcc.GetWorth(ctx, user, card));
                                        break;
                                }
                            }
                            // 满足花色的情况下进行价值估算。
                            // 没有则先尽量满足花色。
                            if (optworth > 0 || handcolors.Count() + optselecteds.Count() < 4)
                                targets.Add(target);
                        }
                        #endregion
                        if (targets.Count() > 0)
                        {
                            isyes = true;
                            return worth;
                        }
                    }
                    #endregion
                    isyes = false;
                    return -10000;
                }

                /// <summary>
                /// 凑花色并保证最优解的搜索算法。
                /// </summary>
                /// <param name="ci"></param>
                /// <param name="worth"></param>
                protected void GetOptimize(int ci, double worth)
                {
                    List<KeyValuePair<double, Card>> list = null;
                    if (ci >= 4)
                    {
                        if (worth > optworth)
                        {
                            optworth = worth;
                            optselecteds.Clear();
                            optselecteds.AddRange(playerlist);
                        }
                        return;
                    }
                    if (handcolors.ContainsKey(allcolors[ci])
                     || !sawcolors.TryGetValue(allcolors[ci], out list))
                    {
                        GetOptimize(ci + 1, worth);
                        return;
                    }
                    foreach (KeyValuePair<double, Card> kvp in list)
                    {
                        if (kvp.Value.Owner != null && playerlist.Contains(kvp.Value.Owner)) continue;
                        if (kvp.Value.Owner != null) playerlist.Add(kvp.Value.Owner);
                        GetOptimize(ci + 1, worth + kvp.Key);
                        if (kvp.Value.Owner != null) playerlist.Remove(kvp.Value.Owner);
                    }
                }
            }

            public Skill_0()
            {
                IsLocked = false;
                this.usecondition = new SkillUseCondition();
                this.targetfilter = new SkillTargetFilter(this);
                this.costfilter = new SkillCostFilter();
            }

            private SkillUseCondition usecondition;
            ConditionFilter ISkillInitative.UseCondition { get => usecondition; }

            private SkillTargetFilter targetfilter;
            PlayerFilter ISkillInitative.TargetFilter { get => targetfilter; }

            private SkillCostFilter costfilter;
            CardFilter ISkillInitative.CostFilter { get => costfilter; }

            public void Action(Context ctx, Player skilluser, IEnumerable<Player> targets, IEnumerable<Card> costs)
            {
                SkillEvent ev = new SkillEvent();
                ev.Skill = this;
                ev.Source = Owner;
                ev.Targets.Clear();
                ev.Targets.AddRange(targets);
                ctx.World.InvokeEvent(ev);
                if (ev.Cancel) return;
                int stealcount = 0;
                foreach (Player target in ev.Targets)
                    stealcount += ctx.World.StealTargetCard(Owner, target, 1, ev, true, true);
                ctx.World.DiscardCard(Owner, stealcount, ev, true);
                if (stealcount >= 2)
                    ctx.World.FaceClip(Owner, Owner, ev);
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
                    Name = "大盗",
                    Description = "出牌阶段限一次，你可以选择你以外X名角色，各获取其一张牌，然后丢弃X张牌。如果X不少于2，将你的角色牌翻面。"
                };
            }

            public override InitativeSkillAuto GetAutoAI()
            {
                return new SkillAuto();
            }
        }

        #endregion

        #region 技能【火花】

        public class Skill_1 : Skill
        {
            public const string DefaultKeyName = "火花";
            public const string Marked = "打出的牌准备火花";

            public class Trigger_0 : Trigger, ITriggerInEvent
            {
                public class TriggerCondition : ConditionFilter
                {
                    public TriggerCondition(Skill _skill)
                    {
                        this.skill = _skill;
                    }

                    private Skill skill;
                    public Skill Skill { get { return this.skill; } }

                    public override bool Accept(Context ctx)
                    {
                        if (!(ctx.Ev is MoveCardDoneEvent)) return false;
                        MoveCardDoneEvent ev0 = (MoveCardDoneEvent)(ctx.Ev);
                        // 弃置：由持有方直接进入弃牌。
                        if (ev0.OldZone?.Owner == skill.Owner
                         && ev0.NewZone?.KeyName?.Equals(Zone.Discard) == true) return true;
                        // 打出：Reason是CardEvent，其Reason也是CardEvent。
                        // 不会立即进入弃牌，会先进入桌面，所以要作好标记。
                        if (ev0.OldZone?.Owner == skill.Owner
                         && ev0.NewZone?.KeyName?.Equals(Zone.Desktop) == true
                         && ev0.Reason is CardEventBase
                         && ev0.Reason.Reason is CardEventBase) return true;
                        // 带标记的卡移动，选择去标记或者发动技能。
                        if (ev0.MovedCards.FirstOrDefault(_card => _card.GetValue(Marked) == 1) != null)
                            return true;
                        // 使用：不触发技能。
                        return false;
                    }
                }

                public class TargetSelector : PlayerFilter
                {
                    public class TargetWorth : PlayerFilterWorth
                    {
                        public override double GetWorth(Context ctx, SelectPlayerBoardCore core, IEnumerable<Player> selecteds, Player want)
                        {
                            return -AITools.WorthAcc.GetWorthPerHp(ctx, core.Controller, want);
                        }

                        public override double GetWorthNo(Context ctx, SelectPlayerBoardCore core)
                        {
                            return 0.0d;
                        }
                    }

                    public TargetSelector(Skill _skill)
                    {
                        this.skill = _skill;
                    }

                    private Skill skill;
                    public Skill Skill { get { return this.skill; } }

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

                    public override PlayerFilterWorth GetWorthAI()
                    {
                        return new TargetWorth();
                    }
                }

                public class DiscardRequire : CardFilter
                {
                    public class DiscardWorth : CardFilterWorth
                    {
                        public DiscardWorth(int _damagevalue)
                        {
                            this.damagevalue = _damagevalue;
                        }

                        private int damagevalue;
                        
                        public override double GetWorth(Context ctx, SelectCardBoardCore core, IEnumerable<Card> selecteds, Card want)
                        {
                            return -AITools.WorthAcc.GetWorth(ctx, core.Controller, want);    
                        }

                        public override double GetWorthNo(Context ctx, SelectCardBoardCore core)
                        {
                            return -AITools.WorthAcc.GetWorthPerHp(ctx, core.Controller, core.Controller) * damagevalue;
                        }
                    }

                    public DiscardRequire(Skill _skill, IEnumerable<Enum_CardColor> _colors, int _damagevalue)
                    {
                        this.skill = _skill;
                        this.colors = _colors.ToList();
                        this.damagevalue = _damagevalue;
                    }

                    private Skill skill;
                    public Skill Skill { get { return this.skill; } }

                    private List<Enum_CardColor> colors = new List<Enum_CardColor>();
                    public List<Enum_CardColor> Colors { get { return this.colors; } }

                    private int damagevalue;
                    public int DamageValue { get { return this.damagevalue; } }

                    public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
                    {
                        if (want.Zone?.KeyName?.Equals(Zone.Hand) != true) return false;
                        return true;
                    }

                    public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
                    {
                        foreach (Enum_CardColor color in colors)
                            if (selecteds.FirstOrDefault(_card => _card.CardColor?.E == color) == null) return false;
                        return true;
                    }

                    public override CardFilterWorth GetWorthAI()
                    {
                        return new DiscardWorth(damagevalue);
                    }
                }

                public Trigger_0(Skill _skill)
                {
                    this.skill = _skill;
                    Condition = new TriggerCondition(skill);
                }

                private Skill skill;
                public Skill Skill { get { return this.skill; } }

                string ITriggerInEvent.EventKeyName { get => MoveCardDoneEvent.DefaultKeyName; }

                public override void Action(Context ctx)
                {
                    if (!(ctx.Ev is MoveCardDoneEvent)) return;
                    MoveCardDoneEvent ev0 = (MoveCardDoneEvent)(ctx.Ev);
                    #region 打出到桌面
                    if (ev0.NewZone?.KeyName?.Equals(Zone.Desktop) == true)
                    {
                        foreach (Card card in ev0.MovedCards)
                            card.SetValue(Marked, 1);
                    }
                    #endregion
                    #region 未进入弃牌
                    else if (ev0.NewZone?.KeyName?.Equals(Zone.Discard) != true)
                    {
                        foreach (Card card in ev0.MovedCards)
                            card.SetValue(Marked, 0);
                    }
                    #endregion
                    #region 触发技能
                    else if (ctx.World.Ask(skill.Owner, skill.KeyName, "是否要使用【火花】？"))
                    {
                        HashSet<Enum_CardColor> colors = new HashSet<Enum_CardColor>();
                        foreach (Card card in ev0.MovedCards)
                        {
                            if (ev0.OldZone?.Owner == skill.Owner
                             || card.GetValue(Marked) == 1)
                                if (!colors.Contains(card.CardColor.E))
                                    colors.Add(card.CardColor.E);
                            card.SetValue(Marked, 0);
                        }
                        ctx.World.SelectPlayer("火花目标选择", "请选择一个目标。", skill.Owner,
                            new TargetSelector(skill),
                            true, Config.GameConfig.Timeout_Handle,
                            (players) =>
                            {
                                SkillEvent ev1 = new SkillEvent();
                                ev1.Reason = ev0;
                                ev1.Skill = skill;
                                ev1.Source = skill.Owner;
                                ev1.Targets.Clear();
                                ev1.Targets.AddRange(players);
                                ctx.World.InvokeEvent(ev1);
                                if (ev1.Cancel) return;
                                foreach (Player target in ev1.Targets)
                                {
                                    ctx.World.RequireCard("火花受害者操作", "请丢弃手牌与魔理沙丢弃的牌花色相符，否则受到其一点伤害。",
                                        players.FirstOrDefault(),
                                        new DiscardRequire(skill, colors, colors.Count() >= 4 ? 2 : 1),
                                        true, Config.GameConfig.Timeout_Handle,
                                        (cards) => { ctx.World.MoveCards(skill.Owner, cards, ev0.NewZone, ev1); },
                                        () => { ctx.World.Damage(skill.Owner, players.FirstOrDefault(), colors.Count() >= 4 ? 2 : 1, DamageEvent.Normal, ev1); });
                                }
                            },
                            () => { });
                    }
                    #endregion
                }
            }

            public Skill_1()
            {
                IsLocked = false;
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
                    Name = "火花",
                    Description = "当你以使用该牌以外的方式，将牌置入弃牌堆时，指定一名你以外的其他角色，除非其弃置任意张牌使得花色都相符，否则受到你的1点伤害。如果你弃置的牌包含了所有花色，则此伤害+1。"
                };
            }
        }

        #endregion

        public Marisa()
        {
            HP = 3;
            MaxHP = 3;
            Country = Reimu.CountryNameOfLeader;
            OtherCountries.Add(CountryNameOfLeader);
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
        }

        public override Charactor Clone()
        {
            return new Marisa();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "雾雨魔理沙";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Marisa");
            info.AbilityRadar = new AbilityRadar() { Attack = 4, Defence = 4, Control = 3, Auxiliary = 2, LastStages = 4, Difficulty = 5 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }
    }

}
