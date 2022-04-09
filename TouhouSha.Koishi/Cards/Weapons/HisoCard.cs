using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Calculators;

namespace TouhouSha.Koishi.Cards.Weapons
{
    /// <summary>
    /// 武器【绯想之剑】
    /// </summary>
    /// <remarks>
    /// 武器范围：2
    /// 你的杀无视防具。
    /// </remarks>
    public class HisoCard : SelfWeapon
    {
        static public bool AssertEnvironment(Context ctx, Card weapon, Card armor, Event reason)
        {
            State state = ctx.World.GetCurrentState();
            CardEvent ev_card = null;
            CardPreviewEvent ev_preview = null;
            CardDoneEvent ev_done = null;

            if (armor?.CardType?.SubType?.E != Enum_CardSubType.Armor) return false;
            if (armor.Owner == null) return false;
            if (armor.Zone?.KeyName?.Equals(Zone.Equips) != true) return false;
            //if (weapon?.CardType?.SubType?.E != Enum_CardSubType.Weapon) return false;
            if (weapon.Owner == null) return false;
            if (weapon.Zone?.KeyName?.Equals(Zone.Equips) != true) return false;

            if (state != null)
            {
                ev_card = state.Ev as CardEvent;
                ev_preview = state.Ev as CardPreviewEvent;
                ev_done = state.Ev as CardDoneEvent;
            }
            if (reason != null)
            {
                if (reason.Reason is CardEvent) ev_card = (CardEvent)(reason.Reason);
                if (reason.Reason is CardPreviewEvent) ev_preview = (CardPreviewEvent)(reason.Reason);
                if (reason.Reason is CardDoneEvent) ev_done = (CardDoneEvent)(reason.Reason);
            }

            Card usedcard = ev_card?.Card ?? ev_preview?.Card ?? ev_done?.Card;
            Player source = ev_card?.Source ?? ev_preview?.Source ?? ev_done?.Source;
            List<Player> targets = new List<Player>();
            if (ev_card != null) targets.AddRange(ev_card.Targets);
            if (ev_preview != null) targets.AddRange(ev_preview.Targets);
            if (ev_done != null) targets.AddRange(ev_done.Targets);
            if (usedcard == null) return false;
            if (source == null) return false;
            if (!KillCard.IsKill(usedcard)) return false;
            if (source != weapon.Owner) return false;
            if (!targets.Contains(armor.Owner)) return false;
            return true;
        }

        public const string Normal = "绯想之剑";

        public HisoCard()
        {
            KeyName = Normal;
            Skills.Add(new SkillHiso(this));
        }
        public override Card Create()
        {
            return new HisoCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "攻击距离2。你的杀无视防具。",
                Image = ImageHelper.LoadCardImage("Cards", "Hiso")
            };
        }
        public override double GetWorthForTarget()
        {
            return 3;
        }
    }
    
    public class SkillHiso : Skill
    {
        public SkillHiso(Card _weapon)
        {
            this.weapon = _weapon;
            IsLocked = true;
            Calculators.Add(new WeaponKillRangePlusCalculator(weapon, 1));
            Triggers.Add(new Hiso_ArmorEmptyTrigger_Trigger(weapon));
            Triggers.Add(new Hiso_ArmorCalculatorReturnOldValue_Trigger(weapon));
            Triggers.Add(new Hiso_ArmorCardCalculator_Trigger(weapon));
            Triggers.Add(new Hiso_ArmorFilterTrigger(weapon));
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        public override Skill Clone()
        {
            return new SkillHiso(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new SkillHiso(newcard);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }
    }
    
    public class Hiso_ArmorEmptyTrigger_Trigger : CardTrigger, ITriggerInEvent
    {
        public Hiso_ArmorEmptyTrigger_Trigger(Card _weapon) : base(_weapon)
        {
            Condition = new Hiso_ArmorEmptyTrigger_Condition(card);
        }
        
        string ITriggerInEvent.EventKeyName { get => UseTriggerEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is UseTriggerEvent)) return;
            UseTriggerEvent ev = (UseTriggerEvent)(ctx.Ev);
            if (ev.NewTrigger is Hiso_ArmorEmptyTrigger) return;
            if (ev.NewTrigger is OverrideTrigger)
            {
                OverrideTrigger overridetrigger = (OverrideTrigger)(ev.NewTrigger);
                while (overridetrigger != null)
                {
                    if (overridetrigger.OldTrigger is Hiso_ArmorEmptyTrigger) return;
                    overridetrigger = overridetrigger.OldTrigger as OverrideTrigger;
                }
            }
            Hiso_ArmorEmptyTrigger newtrigger = new Hiso_ArmorEmptyTrigger();
            newtrigger.OldTrigger = ev.NewTrigger;
            ev.NewTrigger = newtrigger;
        }
    }

    public class Hiso_ArmorEmptyTrigger_Condition : ConditionFilterFromCard
    {
        public Hiso_ArmorEmptyTrigger_Condition(Card _weapon) : base(_weapon)
        {

        }
        
        public override bool Accept(Context ctx)
        {
            UseTriggerEvent ev = (UseTriggerEvent)(ctx.Ev);
            if (!(ev.NewTrigger is ICardTrigger)) return false;
            ICardTrigger ct = (ICardTrigger)(ev.NewTrigger);
            return HisoCard.AssertEnvironment(ctx, card, ct.Card, ev);
        }
    }

    public class Hiso_ArmorEmptyTrigger : OverrideTrigger
    {
        
    }
    
    public class Hiso_ArmorCalculatorReturnOldValue_Trigger : CardTrigger, ITriggerInEvent
    {
        public Hiso_ArmorCalculatorReturnOldValue_Trigger(Card _weapon) : base(_weapon)
        {
            Condition = new Hiso_ArmorCalculatorReturnOldValue_Condition(weapon);
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        string ITriggerInEvent.EventKeyName { get => UseCalculatorEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is UseCalculatorEvent)) return;
            UseCalculatorEvent ev = (UseCalculatorEvent)(ctx.Ev);
            if (ev.NewCalculator is Hiso_ArmorCalculatorReturnOldValue) return;
            if (ev.NewCalculator is OverrideCalculator)
            {
                OverrideCalculator overridecalculator = (OverrideCalculator)(ev.NewCalculator);
                while (overridecalculator != null)
                {
                    if (overridecalculator.OldCalculator is Hiso_ArmorCalculatorReturnOldValue) return;
                    overridecalculator = overridecalculator.OldCalculator as OverrideCalculator;
                }
            }
            Hiso_ArmorCalculatorReturnOldValue newcalculator = new Hiso_ArmorCalculatorReturnOldValue();
            newcalculator.OldCalculator = ev.NewCalculator;
            ev.NewCalculator = newcalculator;
        }
    }

    public class Hiso_ArmorCalculatorReturnOldValue_Condition : ConditionFilterFromCard
    {
        public Hiso_ArmorCalculatorReturnOldValue_Condition(Card _weapon) : base(_weapon)
        {

        }
        
        public override bool Accept(Context ctx)
        {
            UseCalculatorEvent ev = (UseCalculatorEvent)(ctx.Ev);
            if (!(ev.NewCalculator is ICalculatorFromCard)) return false;
            ICalculatorFromCard fromcard = (ICalculatorFromCard)(ev.NewCalculator);
            return HisoCard.AssertEnvironment(ctx, card, fromcard.Card, ev);
        }
    }

    public class Hiso_ArmorCalculatorReturnOldValue : OverrideCalculator
    {
        public override int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
        {
            return oldvalue;
        }
    }

    public class Hiso_ArmorCardCalculator_Trigger : CardTrigger, ITriggerInEvent
    {
        public Hiso_ArmorCardCalculator_Trigger(Card _weapon) : base(_weapon)
        {
            Condition = new Hiso_ArmorCardCalculator_Condition(card);
        }
        
        string ITriggerInEvent.EventKeyName { get => UseCalculatorEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is UseCardCalculatorEvent)) return;
            UseCardCalculatorEvent ev = (UseCardCalculatorEvent)(ctx.Ev);
            if (ev.NewCalculator is Hiso_ArmorCardCalculator) return;
            if (ev.NewCalculator is OverrideCardCalculator)
            {
                OverrideCardCalculator overridecalculator = (OverrideCardCalculator)(ev.NewCalculator);
                while (overridecalculator != null)
                {
                    if (overridecalculator.OldCalculator is Hiso_ArmorCardCalculator) return;
                    overridecalculator = overridecalculator.OldCalculator as OverrideCardCalculator;
                }
            }
            Hiso_ArmorCardCalculator newcalculator = new Hiso_ArmorCardCalculator();
            newcalculator.OldCalculator = ev.NewCalculator;
            ev.NewCalculator = newcalculator;
        }
    }

    public class Hiso_ArmorCardCalculator_Condition : ConditionFilterFromCard
    {
        public Hiso_ArmorCardCalculator_Condition(Card _weapon) : base(_weapon)
        {
            
        }
        
        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is UseCardCalculatorEvent)) return false;
            UseCardCalculatorEvent ev = (UseCardCalculatorEvent)(ctx.Ev);
            if (!(ev.NewCalculator is ICalculatorFromCard)) return false;
            ICalculatorFromCard cardcalc = (ICalculatorFromCard)(ev.NewCalculator);
            return HisoCard.AssertEnvironment(ctx, card, cardcalc.Card, ev);
        }

    }

    public class Hiso_ArmorCardCalculator : OverrideCardCalculator
    {
        public override Card GetValue(Context ctx, Card oldvalue)
        {
            return oldvalue;
        }
    }

    public class Hiso_ArmorFilterTrigger : CardTrigger, ITriggerInEvent
    {
        public Hiso_ArmorFilterTrigger(Card _weapon) : base(_weapon)
        {
            Condition = new Hiso_ArmorCardCalculator_Condition(_weapon);
        }

        string ITriggerInEvent.EventKeyName { get => UseFilterEvent.DefaultKeyName; }
        
        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is UseFilterEvent)) return;
            UseFilterEvent ev = (UseFilterEvent)(ctx.Ev);
            if (ev.NewFilter is ConditionFilter)
            {
                ev.NewFilter = new Hiso_ArmorCondition_AlwaysFalse(card);
                return;
            }
            if (ev.NewFilter is OverridePlayerFilter)
            {
                OverridePlayerFilter overrides = (OverridePlayerFilter)(ev.NewFilter);
                UseKillTargetFilter killtargeter = null;
                List<PlayerFilter> ancpath0s = new List<PlayerFilter>();
                List<PlayerFilter> ancpath1s = new List<PlayerFilter>();
                while (overrides != null)
                {
                    ancpath0s.Add(overrides);
                    if (!(overrides is IFilterFromCard)
                     || !HisoCard.AssertEnvironment(ctx, card, ((IFilterFromCard)overrides).Card, ev))
                        ancpath1s.Add(overrides);
                    if (overrides.OldFilter is UseKillTargetFilter)
                        killtargeter = (UseKillTargetFilter)(overrides.OldFilter);
                    overrides = overrides.OldFilter as OverridePlayerFilter;
                }
                if (killtargeter != null
                 && ancpath0s.Count() > ancpath1s.Count())
                {
                    ancpath0s.Add(killtargeter);
                    ancpath1s.Add(killtargeter);
                    Hiso_TargetFilter_RemoveAboutArmor newfilter = new Hiso_TargetFilter_RemoveAboutArmor(card);
                    newfilter.OldFilter = ev.NewFilter as OverridePlayerFilter;
                    newfilter.AncPath_Olds.AddRange(ancpath0s);
                    newfilter.AncPath_News.AddRange(ancpath1s);
                    ev.NewFilter = newfilter;
                }
                return;
            }
        }
    }
    
    public class Hiso_ArmorFilterTrigger_Condition : ConditionFilterFromCard
    {
        public Hiso_ArmorFilterTrigger_Condition(Card _weapon) : base(_weapon)
        {

        }

        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is UseFilterEvent)) return false;
            UseFilterEvent ev = (UseFilterEvent)(ctx.Ev);
            if (ev.NewFilter is ConditionFilter 
             && ev.NewFilter is IFilterFromCard)
            {
                IFilterFromCard filtercard = (IFilterFromCard)(ev.NewFilter);
                if (HisoCard.AssertEnvironment(ctx, card, filtercard.Card, ev)) return true;
            }
            if (ev.NewFilter is OverridePlayerFilter)
            {
                OverridePlayerFilter overrides = (OverridePlayerFilter)(ev.NewFilter);
                UseKillTargetFilter killtargeter = null;
                List<PlayerFilter> ancpath0s = new List<PlayerFilter>();
                List<PlayerFilter> ancpath1s = new List<PlayerFilter>();
                while (overrides != null)
                {
                    ancpath0s.Add(overrides);
                    if (!(overrides is IFilterFromCard)
                     || !HisoCard.AssertEnvironment(ctx, card, ((IFilterFromCard)overrides).Card, ev))
                        ancpath1s.Add(overrides);
                    if (overrides.OldFilter is UseKillTargetFilter)
                        killtargeter = (UseKillTargetFilter)(overrides.OldFilter);
                    overrides = overrides.OldFilter as OverridePlayerFilter;
                }
                if (killtargeter != null
                 && ancpath0s.Count() > ancpath1s.Count())
                    return true;
            }
            return false;
        }
    }

    public class Hiso_ArmorCondition_AlwaysFalse : ConditionFilterFromCard
    {
        public Hiso_ArmorCondition_AlwaysFalse(Card _card) : base(_card)
        {
            
        }
        
        public override bool Accept(Context ctx)
        {
            return false;
        }
    }
   
    public class Hiso_TargetFilter_RemoveAboutArmor : OverridePlayerFilter, IFilterFromCard
    { 
        public Hiso_TargetFilter_RemoveAboutArmor(Card _card)
        {
            this.card = _card;
        }

        private Card card;
        public Card Card { get { return this.card; } }

        private List<PlayerFilter> ancpath_olds = new List<PlayerFilter>();
        public List<PlayerFilter> AncPath_Olds { get { return this.ancpath_olds; } }

        private List<PlayerFilter> ancpath_news = new List<PlayerFilter>();
        public List<PlayerFilter> AncPath_News { get { return this.ancpath_news; } }


        protected void SetAncPath_News()
        {
            OldFilter = ancpath_news.FirstOrDefault();
            for (int i = 0; i < ancpath_news.Count() - 1; i++)
                if (ancpath_news[i] is OverridePlayerFilter)
                    ((OverridePlayerFilter)ancpath_news[i]).OldFilter = ancpath_news[i + 1];
        }

        protected void SetAncPath_Olds()
        {
            OldFilter = ancpath_olds.FirstOrDefault();
            for (int i = 0; i < ancpath_olds.Count() - 1; i++)
                if (ancpath_olds[i] is OverridePlayerFilter)
                    ((OverridePlayerFilter)ancpath_olds[i]).OldFilter = ancpath_olds[i + 1];
        }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            SetAncPath_News();
            try
            {
                return OldFilter?.CanSelect(ctx, selecteds, want) ?? false;  
            }
            catch (Exception exce)
            {
                throw exce;
            }
            finally
            {
                SetAncPath_Olds();
            }
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            SetAncPath_News();
            try
            {
                return OldFilter?.Fulfill(ctx, selecteds) ?? false;
            }
            catch (Exception exce)
            {
                throw exce;
            }
            finally
            {
                SetAncPath_Olds();
            }
        }


    }

    
}
