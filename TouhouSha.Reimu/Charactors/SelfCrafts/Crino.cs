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

namespace TouhouSha.Reimu.Charactors.SelfCrafts
{
    /// <summary>
    /// 角色【琪露诺】
    /// </summary>
    /// <remarks>
    /// 势力：自机（可替换为妖精） 3勾玉
    /// 【冰冻】：回合开始阶段，你可以将X张不同花色的牌当作【冰冻】，选择X名角色使用。当选择的数量不小于2时，将你的角色牌翻面。
    /// 【冰精】：锁定技，当你判定区没有【冰冻】时，将牌堆顶的一张牌视作【冰冻】放置到你的判定区。你受到的火焰以外的伤害-X，你的手牌上限+X（X为你判定区的牌的数量）。
    /// 【算术】：你的判定阶段开始时，你确认牌堆顶X+2张牌，可以获取其中任意张点数之和不超过9的牌，剩下的牌以任意顺序放入牌堆顶（X为场上所有判定区的牌的数量）。
    /// </remarks>

    public class Crino : Charactor
    {
        public const string CountryNameOfLeader = "妖精";


        public Crino()
        {
            MaxHP = 3;
            HP = 3;
            Country = Reimu.CountryNameOfLeader;
            OtherCountries.Add(Crino.CountryNameOfLeader);
            Skills.Add(new Crino_Skill_0());
            Skills.Add(new Crino_Skill_1());
            Skills.Add(new Crino_Skill_2());

        }

        public override Charactor Clone()
        {
            return new Crino();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "琪露诺";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Crino");
            info.AbilityRadar = new AbilityRadar() { Attack = 2, Defence = 4, Control = 5, Auxiliary = 3, LastStages = 4, Difficulty = 4 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }
    }

    public class Crino_Skill_0 : Skill
    {
        public Crino_Skill_0()
        {
            Triggers.Add(new Crino_Skill_0_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Crino_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Crino_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "冰冻",
                Description = "回合开始阶段，你可以将X张不同花色的牌当作【冻青蛙】，选择X名角色使用。当选择的数量不小于2时，将你的角色牌翻面。",
            };
        }
    }

    public class Crino_Skill_0_Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Crino_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Crino_Skill_0_Trigger_0_Condition(skill);
        }    

        string ITriggerInState.StateKeyName { get => State.Begin; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        string ITriggerAsk.Message { get => "是否要发动【冰冻】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return  skill.Owner; }

        public override void Action(Context ctx)
        {
            int targetmax = ctx.World.Players.Count(
                (_player) =>
                {
                    if (!_player.IsAlive) return false;
                    if (_player == skill.Owner) return false;
                    Zone judgezone = _player.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
                    if (judgezone == null) return false;
                    if (judgezone.Cards.FirstOrDefault(_card => _card.KeyName?.Equals(HungerCard.Normal) == true) != null) return false;
                    return true;
                });
            if (targetmax == 0) return;
            ctx.World.RequireCard(skill.KeyName, "请选择要做为【冰冻】使用的卡。", skill.Owner,
                new Crino_Skill_0_Trigger_0_CostFilter(targetmax),
                true, 15,
                (cards) =>
                {
                    ctx.World.SelectPlayer(skill.KeyName, "请选择使用的目标。", skill.Owner,
                        new Crino_Skill_0_Trigger_0_TargetFilter(skill.Owner, cards.Count()),
                        true, 15,
                        (targets) =>
                        {
                            List<Card> cardlist = cards.ToList();
                            List<Player> targetlist = targets.ToList();
                            SkillEvent ev_skill = new SkillEvent();
                            ev_skill.Skill = skill;
                            ev_skill.Source = skill.Owner;
                            ev_skill.Targets.Clear();
                            ev_skill.Targets.AddRange(targetlist);
                            ctx.World.InvokeEvent(ev_skill);
                            if (ev_skill.Cancel) return;
                            targetlist = ev_skill.Targets.ToList();
                            for (int i = 0; i < Math.Min(cardlist.Count(), targetlist.Count()); i++)
                            {
                                Card origincard = cardlist[i];
                                Player target = targetlist[i];
                                Zone targetjudgezone = target.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
                                Card seemascard = ctx.World.GetCardInstance(HungerCard.Normal);
                                seemascard.Clone(origincard);
                                ctx.World.MoveCard(skill.Owner, seemascard, targetjudgezone, ev_skill);
                            }
                        }, null);
                }, null);
        }
    }

    public class Crino_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Crino_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill)
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
    
    public class Crino_Skill_0_Trigger_0_CostFilter : CardFilter
    {
        public Crino_Skill_0_Trigger_0_CostFilter(int _maxnumber)
        {
            this.maxnumber = _maxnumber;
        }

        private int maxnumber;
        public int MaxNumber { get { return this.maxnumber; } }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.FirstOrDefault(_card => _card.CardColor?.E == want.CardColor?.E) != null) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return selecteds.Count() >= 1;
        }
    }
    
    public class Crino_Skill_0_Trigger_0_TargetFilter : PlayerFilter
    {
        public Crino_Skill_0_Trigger_0_TargetFilter(Player _self, int _numberrequired)
        {
            this.self = _self;
            this.numberrequired = _numberrequired;
        }

        private Player self;
        public Player Self { get { return this.self; } }

        private int numberrequired;
        public int NumberRequired { get { return this.numberrequired; } }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            if (selecteds.Count() >= numberrequired) return false;
            if (want == self) return false;
            Zone judgezone = want.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
            if (judgezone == null) return false;
            if (judgezone.Cards.FirstOrDefault(_card => _card.KeyName?.Equals(HungerCard.Normal) == true) != null) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() >= numberrequired;
        }
    }

    public class Crino_Skill_1 : Skill
    {
        public Crino_Skill_1()
        {
            IsLocked = true;
            Triggers.Add(new Crino_Skill_1_Trigger_0(this));
            Triggers.Add(new Crino_Skill_1_Trigger_1(this));
            Calculators.Add(new Crino_Skill_1_HandCapacity(this));
        }
        public override Skill Clone()
        {
            return new Crino_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Crino_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "冰精",
                Description = "锁定技，当你判定区没有【冻青蛙】时，将牌堆顶的一张牌视作【冻青蛙】放置到你的判定区。你受到的火焰以外的伤害-X，你的手牌上限+X（X为你判定区的牌的数量）。",
            };
        }
    }
    
    public class Crino_Skill_1_Trigger_0 : SkillTrigger, ITriggerInEvent
    {
        public Crino_Skill_1_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Crino_Skill_1_Trigger_0_Condition(skill);           
        }

        string ITriggerInEvent.EventKeyName { get => MoveCardDoneEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is MoveCardDoneEvent)) return;
            MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
            Card topcard = ctx.World.GetDrawTops(1).FirstOrDefault();
            Card seemas = ctx.World.GetCardInstance(HungerCard.Normal);
            seemas = seemas.Clone(topcard);
            ctx.World.MoveCard(skill.Owner, seemas, ev.OldZone, ev);
        }
    }
    
    public class Crino_Skill_1_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Crino_Skill_1_Trigger_0_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is MoveCardDoneEvent)) return false;
            MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
            if (ev.OldZone.Owner != skill.Owner) return false;
            if (ev.OldZone.KeyName?.Equals(Zone.Judge) != true) return false;
            if (ev.MovedCards.FirstOrDefault(_card => _card.KeyName?.Equals(HungerCard.Normal) == true) == null) return false;
            return true;
        }
    }
    
    public class Crino_Skill_1_Trigger_1 : SkillTrigger, ITriggerInState
    {
        public Crino_Skill_1_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Crino_Skill_1_Trigger_1_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Damaging; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_AfterEnd - 1; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (!(state.Ev is DamageEvent)) return;
            DamageEvent ev = (DamageEvent)(state.Ev);
            Zone judgezone = state.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
            if (judgezone == null) return;
            ev.DamageValue = Math.Max(0, ev.DamageValue - judgezone.Cards.Count());
        }
    }

    public class Crino_Skill_1_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Crino_Skill_1_Trigger_1_Condition(Skill _skill) : base(_skill) { }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            if (!(state.Ev is DamageEvent)) return false;
            DamageEvent ev = (DamageEvent)(state.Ev);
            if (ev.DamageType?.Equals(DamageEvent.Fire) == true) return false;
            if (ev.DamageType?.Equals(DamageEvent.Lost) == true) return false;
            Zone judgezone = state.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
            if (judgezone == null) return false;
            if (judgezone.Cards.Count() == 0) return false;
            return true;
        }
    }

    public class Crino_Skill_1_HandCapacity : CalculatorFromSkill, ICalculatorProperty
    {
        public Crino_Skill_1_HandCapacity(Skill _skill) : base(_skill)
        {
            
        }

        string ICalculatorProperty.PropertyName { get => DiscardTrigger.ExtraHandCapacity; }

        public override int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
        {
            switch (propertyname)
            {
                case DiscardTrigger.ExtraHandCapacity:
                    if (obj != skill.Owner) return oldvalue;
                    Zone judgezone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
                    if (judgezone == null) return oldvalue;
                    return oldvalue + judgezone.Cards.Count();
            }
            return oldvalue;
        }
    }

    public class Crino_Skill_2 : Skill
    {
        public Crino_Skill_2()
        {
            Triggers.Add(new Crino_Skill_2_Trigger_0(this));
        }
        public override Skill Clone()
        {
            return new Crino_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new Crino_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "算术",
                Description = "你的判定阶段开始时，你确认牌堆顶X+2张牌，可以获取其中任意张点数之和不超过9的牌，剩下的牌以任意顺序放入牌堆顶（X为场上所有判定区的牌的数量）。",
            };
        }

    }

    public class Crino_Skill_2_Trigger_0 : SkillTrigger, ITriggerInState, ITriggerAsk
    {
        public Crino_Skill_2_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Crino_Skill_2_Trigger_0_Condition(skill);
        }

        string ITriggerInState.StateKeyName { get => State.Judge; }
        
        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start - 2; }

        string ITriggerAsk.Message { get => "是否要发送【算术】？"; }
        
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

            DesktopCardBoardCore desktop_core = new DesktopCardBoardCore();
            List<IList<Card>> cardlists = new List<IList<Card>>();
            int judgecount = ctx.World.Players.Sum(_player =>
            {
                if (!_player.IsAlive) return 0;
                Zone judgezone = _player.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
                if (judgezone == null) return 0;
                return judgezone.Cards.Count();
            });
            desktop_core.Flag |= Enum_DesktopCardBoardFlag.CannotNo;
            desktop_core.CardFilter = new Crino_Skill_2_CardFilter();

            DesktopCardBoardZone totop = new DesktopCardBoardZone(desktop_core);
            totop.Message = "牌堆顶的卡";
            desktop_core.Zones.Add(totop);
            cardlists.Add(ctx.World.GetDrawTops(judgecount + 2));

            DesktopCardBoardZone tohand = new DesktopCardBoardZone(desktop_core);
            tohand.Message = "要获得的卡";
            tohand.Flag |= Enum_DesktopZoneFlag.AsSelected;
            desktop_core.Zones.Add(tohand);
            cardlists.Add(new List<Card>());

            ctx.World.ShowDesktop(skill.Owner, desktop_core, cardlists, true, ev_skill);
            if (desktop_core.IsYes)
            {
                Zone drawzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Draw) == true);
                if (drawzone == null) return;
                Zone handzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                if (handzone == null) return;
                ctx.World.MoveCards(skill.Owner, totop.Cards, drawzone, ev_skill);
                ctx.World.MoveCards(skill.Owner, tohand.Cards, handzone, ev_skill);
            }
        }
    }

    public class Crino_Skill_2_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Crino_Skill_2_Trigger_0_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            return true;
        }
    }

    public class Crino_Skill_2_CardFilter : CardFilter
    {
        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() == 0) return want.CardPoint <= 9;
            return (selecteds.Sum(_card => _card.CardPoint) + want.CardPoint <= 9);
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return true;
        }

    }
    
}
