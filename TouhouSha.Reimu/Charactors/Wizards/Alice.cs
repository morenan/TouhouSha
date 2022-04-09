using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.UIs;
using TouhouSha.Core.Filters;
using TouhouSha.Koishi.Cards;
using TouhouSha.Koishi.Cards.Horses;
using TouhouSha.Reimu.Charactors.SelfCrafts;

namespace TouhouSha.Reimu.Charactors.Wizards
{
    /// <summary>
    /// 角色【爱丽丝】
    /// </summary>
    /// <remarks>
    /// 魔使 HP:3 
    /// 【人偶】出牌阶段，每项限一次：
    ///     1. 你可以将一张红色牌当作【上海】置于你的装备栏上。
    ///     2. 你可以将一张黑色牌当作【蓬莱】置于你的装备栏上。
    /// 以此法放置的牌在回合结束阶段弃置。
    /// 【军势】锁定技：当你失去装备栏里的牌时：
    ///     1. 武器：选择一名角色，造成一点伤害。
    ///     2. 防具：你回复一点体力。
    ///     3. UFO：你摸两张牌。
    /// </remarks>
    public class Alice : Charactor
    {
        public Alice()
        {
            HP = 3;
            MaxHP = 3;
            Country = Marisa.CountryNameOfLeader;
            Skills.Add(new Alice_Skill_0());
            Skills.Add(new Alice_Skill_1());
        }

        public override Charactor Clone()
        {
            return new Alice();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "爱丽丝";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Alice");
            info.AbilityRadar = new AbilityRadar() { Attack = 4, Defence = 4, Control = 2, Auxiliary = 1.5, LastStages = 4.5, Difficulty = 4 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }


    }

    public class Alice_Skill_0 : Skill, ISkillCardConverter
    {
        public const string UFO_0_Used = "已经装备过上海";
        public const string UFO_1_Used = "已经装备过蓬莱";
        public const string Marked = "使用人偶装备的";
      
        public Alice_Skill_0()
        {
            IsLocked = false;
            this.usecondition = new Alice_Skill_0_UseCodition(this);
            this.cardfilter = new Alice_Skill_0_CardFilter(this);
            this.cardconverter = new Alice_Skill_0_CardConverter(this);
            Triggers.Add(new Alice_Skill_0_Trigger_0(this));
            Triggers.Add(new Alice_Skill_0_Trigger_1(this));
            
        }

        private Alice_Skill_0_UseCodition usecondition;
        public ConditionFilter UseCondition { get { return this.usecondition; } }

        private Alice_Skill_0_CardFilter cardfilter;
        public CardFilter CardFilter { get { return this.cardfilter; } }

        private Alice_Skill_0_CardConverter cardconverter;
        public CardCalculator CardConverter { get { return this.cardconverter; } }

        public override Skill Clone()
        {
            return new Alice_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Alice_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            SkillInfo skillinfo = new SkillInfo()
            {
                Name = "人偶",
                Description = "出牌阶段，每项限一次：\n" +
                    "\t1. 你可以将一张红色牌当作【上海】置于你的装备栏上。\n" +
                    "\t2. 你可以将一张黑色牌当作【蓬莱】置于你的装备栏上。\n" +
                    "以此法放置的牌在回合结束阶段弃置。"
            };
            skillinfo.AttachedSkills.Add(new SkillInfo()
            {
                Name = "上海",
                Description = "攻击型UFO。你计算与其他角色的距离时-1。你使用【杀】时可以额外指定一个目标。",
            });
            skillinfo.AttachedSkills.Add(new SkillInfo()
            {
                Name = "蓬莱",
                Description = "防御型UFO。其他角色计算与你的距离时+1。当你回复体力时，可以弃置装备栏里的这张牌，并摸两张牌。",
            });
            return skillinfo;
        }
    }

    public class Alice_Skill_0_UseCodition : ConditionFilterFromSkill
    {
        public Alice_Skill_0_UseCodition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (state.Owner != skill.Owner) return false;
            if (state.KeyName?.Equals(State.UseCard) != true) return false;
            if (state.GetValue(Alice_Skill_0.UFO_0_Used) == 1
             && state.GetValue(Alice_Skill_0.UFO_1_Used) == 1) return false;
            return true;
        }
    }

    public class Alice_Skill_0_CardFilter : CardFilterFromSkill
    {
        public Alice_Skill_0_CardFilter(Skill _skill) : base(_skill)
        {

        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (selecteds.Count() >= 1) return false;
            if (want.Zone.KeyName?.Equals(Zone.Hand) != true) return false;
            if (want.CardColor?.SeemAs(Enum_CardColor.Red) == true
             && state.GetValue(Alice_Skill_0.UFO_0_Used) == 0)
                return true;
            if (want.CardColor?.SeemAs(Enum_CardColor.Black) == true
             && state.GetValue(Alice_Skill_0.UFO_1_Used) == 0)
                return true;
            return false;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return selecteds.Count() >= 1;
        }

    }
   
    public class Alice_Skill_0_CardConverter : CardCalculatorFromSkill
    {
        public Alice_Skill_0_CardConverter(Skill _skill) : base(_skill)
        {
            
        }

        public override Card GetValue(Context ctx, Card oldvalue)
        {
            if (oldvalue.CardColor?.SeemAs(Enum_CardColor.Red) == true)
            {
                Card card = new ShangHai();
                card = card.Clone(oldvalue);
                card.SetValue(Alice_Skill_0.Marked, 1);
                return card;
            }
            if (oldvalue.CardColor?.SeemAs(Enum_CardColor.Red) == true)
            {
                Card card = new PengLai();
                card = card.Clone(oldvalue);
                card.SetValue(Alice_Skill_0.Marked, 1);
                return card;
            }
            return oldvalue;
        }
    }
    
    public class Alice_Skill_0_Trigger_0 : SkillTrigger, ITriggerInEvent
    {
        public Alice_Skill_0_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Alice_Skill_0_Trigger_0_Condition(skill);            
        }

        string ITriggerInEvent.EventKeyName { get => CardEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return;
            if (!(ctx.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(ctx.Ev);
            switch (ev.Card.KeyName)
            {
                case ShangHai.DefaultKeyName:
                    state.SetValue(Alice_Skill_0.UFO_0_Used, 1);
                    break;
                case PengLai.DefaultKeyName:
                    state.SetValue(Alice_Skill_0.UFO_1_Used, 1);
                    break;
            }
        }
    }

    public class Alice_Skill_0_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Alice_Skill_0_Trigger_0_Condition(Skill _skill) : base(_skill)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetPlayerState();
            if (state == null) return false;
            if (!(ctx.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(ctx.Ev);
            if (ev.Source != skill.Owner) return false;
            if (ev.Card.GetValue(Alice_Skill_0.Marked) != 1) return false;
            return true;
        }
    }
    
    public class Alice_Skill_0_Trigger_1 : SkillTrigger, ITriggerInState
    {
        public Alice_Skill_0_Trigger_1(Skill _skill) : base(_skill)
        {
            Condition = new Alice_Skill_0_Trigger_1_Condition(skill);   
        }
       
        string ITriggerInState.StateKeyName { get => State.End; }
        
        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        public override void Action(Context ctx)
        {
            Zone equipzone = skill.Owner.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Equips) == true);
            if (equipzone == null) return;
            List<Card> discards = equipzone.Cards.Where(_card => _card.GetValue(Alice_Skill_0.Marked) == 1).ToList();
            if (discards.Count() == 0) return;
            Zone discardzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
            if (discardzone == null) return;
            ctx.World.MoveCards(skill.Owner, discards, discardzone, null);
        }
    }

    public class Alice_Skill_0_Trigger_1_Condition : ConditionFilterFromSkill
    {
        public Alice_Skill_0_Trigger_1_Condition(Skill _skill) : base(_skill)
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

    public class Alice_Skill_1 : Skill
    {
        public Alice_Skill_1()
        {
            IsLocked = true;
            Triggers.Add(new Alice_Skill_1_Trigger_0(this));
        }

        public override Skill Clone()
        {
            return new Alice_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Alice_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "军势",
                Description = "锁定技：当你失去装备栏里的牌时：\n" +
                    "\t1. 武器：选择一名角色，造成一点伤害。\n" +
                    "\t2. 防具：你回复一点体力。\n" +
                    "\t3. UFO：你摸两张牌。"
            };
        }

    }

    public class Alice_Skill_1_Trigger_0 : SkillTrigger, ITriggerInEvent
    {
        public Alice_Skill_1_Trigger_0(Skill _skill) : base(_skill)
        {
            Condition = new Alice_Skill_1_Trigger_0_Condition(skill);
        }

        string ITriggerInEvent.EventKeyName { get => MoveCardDoneEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is MoveCardDoneEvent)) return;
            MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
            SkillEvent ev_skill = new SkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Skill = skill;
            ev_skill.Source = skill.Owner;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(skill.Owner);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            int drawnumber = 0;
            int damagenumber = 0;
            int healnumber = 0;
            foreach (Card card in ev.MovedCards)
            {
                switch (card.CardType?.SubType?.E)
                {
                    case Enum_CardSubType.Weapon:
                        damagenumber++;
                        break;
                    case Enum_CardSubType.Armor:
                        healnumber++;
                        break;
                    case Enum_CardSubType.HorsePlus:
                    case Enum_CardSubType.HorseMinus:
                        drawnumber += 2;
                        break;
                }
            }
            if (damagenumber > 0)
                ctx.World.SelectPlayer(skill.KeyName, "请选择一名角色，对其造成伤害。", skill.Owner,
                    new FulfillNumberPlayerFilter(1, 1, skill.Owner),
                    false, 15,
                    (targets) =>
                    {
                        foreach (Player target in targets)
                            ctx.World.Damage(skill.Owner, target, damagenumber, DamageEvent.Normal, ev_skill);
                    }, null);
            if (healnumber > 0)
                ctx.World.Heal(skill.Owner, skill.Owner, healnumber, HealEvent.Normal, ev_skill);
            if (drawnumber > 0)
                ctx.World.DrawCard(skill.Owner, drawnumber, ev_skill);
        }
    }
    
    public class Alice_Skill_1_Trigger_0_Condition : ConditionFilterFromSkill
    {
        public Alice_Skill_1_Trigger_0_Condition(Skill _skill) : base(_skill)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is MoveCardDoneEvent)) return false;
            MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
            if (ev.OldZone?.Owner != skill.Owner) return false;
            if (ev.OldZone.KeyName?.Equals(Zone.Equips) != true) return false;
            return true;
        }
    }


    /// <summary>
    /// 攻击UFO【上海】
    /// </summary>
    /// <remarks>
    /// 计算与其他角色距离时-1。
    /// 你使用【杀】时可以额外指定一个目标。
    /// </remarks>
    public class ShangHai : MinusHorseCard
    {
        public new const string DefaultKeyName = "上海";

        public ShangHai()
        {
            KeyName = DefaultKeyName;
            Skills.Add(new ShangHai_Skill(this));
        }
    }

    public class ShangHai_Skill : Skill
    {
        public ShangHai_Skill(Card _card)
        {
            this.card = _card;
            Triggers.Add(new ShangHai_Skill_Trigger_0(card));
        }

        private Card card;
        public Card Card { get { return this.card; } }

        public override Skill Clone()
        {
            return new ShangHai_Skill(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new ShangHai_Skill(card);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }

    }

    public class ShangHai_Skill_Trigger_0 : CardTrigger, ITriggerInEvent
    {
        public ShangHai_Skill_Trigger_0(Card _card) : base(_card)
        {
            Condition = new ShangHai_Skill_Trigger_0_Condition(card);
        }

        string ITriggerInEvent.EventKeyName { get => UseFilterEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is UseFilterEvent)) return;
            UseFilterEvent ev = (UseFilterEvent)(ctx.Ev);
            PlayerFilter oldfilter = ev.NewFilter as PlayerFilter;
            ev.NewFilter = new ShangHai_KillMoreTarget() { OldFilter = oldfilter };
        }
    }
    
    public class ShangHai_Skill_Trigger_0_Condition : ConditionFilterFromCard
    {
        public ShangHai_Skill_Trigger_0_Condition(Card _card) : base(_card)
        {
            
        }

        public override bool Accept(Context ctx)
        {
            if (card.Owner == null) return false;
            if (card.Zone.KeyName?.Equals(Zone.Equips) != true) return false;
            if (!(ctx.Ev is UseFilterEvent)) return false;
            UseFilterEvent ev = (UseFilterEvent)(ctx.Ev);
            UseKillTargetFilter originfilter = ev.NewFilter as UseKillTargetFilter;
            if (ev.NewFilter is OverridePlayerFilter)
            {
                OverridePlayerFilter overrides = (OverridePlayerFilter)(ev.NewFilter);
                while (overrides != null)
                {
                    if (overrides is ShangHai_KillMoreTarget)
                        return false;
                    if (originfilter == null)
                        originfilter = overrides.OldFilter as UseKillTargetFilter;
                    overrides = overrides.OldFilter as OverridePlayerFilter;
                }
            }
            if (originfilter == null) return false;
            if (originfilter.Card.Owner != card.Owner) return false;
            return true;
        }
    }

    public class ShangHai_KillMoreTarget : OverridePlayerFilter
    {
        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            if (OldFilter == null) return false;
            if (OldFilter.CanSelect(ctx, selecteds, want)) return true;
            List<Player> selectedlist = selecteds.ToList();
            if (selectedlist.Count() > 0)
            {
                selectedlist.RemoveAt(selectedlist.Count() - 1);
                if (OldFilter.CanSelect(ctx, selectedlist, want)) return true;
            }
            return false;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            if (OldFilter == null) return false;
            if (OldFilter.Fulfill(ctx, selecteds)) return true;
            List<Player> selectedlist = selecteds.ToList();
            if (selectedlist.Count() > 0)
            {
                selectedlist.RemoveAt(selectedlist.Count() - 1);
                if (OldFilter.Fulfill(ctx, selectedlist)) return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 防御UFO【蓬莱】
    /// </summary>
    /// <remarks>
    /// 你的回合内，你计算与其他角色距离时-1。
    /// 你的回合外，其他角色计算与你的距离时+1。
    /// </remarks>
    public class PengLai : PlusHorseCard
    {
        public new const string DefaultKeyName = "蓬莱";

        public PengLai()
        {
            KeyName = DefaultKeyName;
            Skills.Clear();
            Skills.Add(new PengLai_Skill(this));
        }
    }
   
    public class PengLai_Skill : Skill
    {
        public PengLai_Skill(Card _card)
        {
            this.card = _card;
            IsLocked = true;
            Calculators.Clear();
            Calculators.Add(new PlusDistanceCalculator(card));
            Calculators.Add(new MinusDistanceCalculator(card));
        }

        private Card card;
        public Card Card { get { return this.card; } }

        public override Skill Clone()
        {
            return new PengLai_Skill(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new PengLai_Skill(card);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }
    }

    public class PlusDistanceCalculator : CalculatorFromCard, ICalculatorProperty
    {
        public PlusDistanceCalculator(Card _card) : base(_card) { }

        string ICalculatorProperty.PropertyName { get => World.DistancePlus; }

        public override int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
        {
            State state = ctx.World.GetPlayerState();
            if (state.Owner == card.Owner) return oldvalue;
            if (!(obj is Player)) return oldvalue;
            if (!propertyname.Equals(World.DistancePlus)) return oldvalue;
            Player player = (Player)obj;
            if (player != card.Owner) return oldvalue;
            if (card.Zone?.KeyName?.Equals(Zone.Equips) != true) return oldvalue;
            return oldvalue + 1;
        }
    }

    public class MinusDistanceCalculator : CalculatorFromCard, ICalculatorProperty
    {
        public MinusDistanceCalculator(Card _card) : base(_card) { }

        string ICalculatorProperty.PropertyName { get => World.DistanceMinus; }

        public override int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
        {
            State state = ctx.World.GetPlayerState();
            if (state.Owner != card.Owner) return oldvalue;
            if (!(obj is Player)) return oldvalue;
            if (!propertyname.Equals(World.DistanceMinus)) return oldvalue;
            Player player = (Player)obj;
            if (player != card.Owner) return oldvalue;
            if (card.Zone?.KeyName?.Equals(Zone.Equips) != true) return oldvalue;
            return oldvalue + 1;
        }
    }
}
