using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Calculators;

namespace TouhouSha.Koishi.Cards.Weapons
{
    /// <summary>
    /// 武器【赖政之弓】
    /// </summary>
    /// <remarks>
    /// 武器范围：5
    /// 你的【杀】造成伤害前，你可以丢弃目标的一张【UFO】卡。
    /// </remarks>
    public class BowCard : SelfWeapon
    {
        public const string Normal = "赖政之弓";

        public BowCard()
        {
            KeyName = Normal;
            Skills.Add(new SkillBow(this));
        }
        public override Card Create()
        {
            return new BowCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "攻击距离5。你的【杀】造成伤害前，你可以丢弃目标的一张【UFO】卡。",
                Image = ImageHelper.LoadCardImage("Cards", "Bow")
            };
        }
        public override double GetWorthForTarget()
        {
            return 3;
        }
    }
    
    public class SkillBow : Skill
    {
        public SkillBow(Card _weapon)
        {
            this.weapon = _weapon;
            Calculators.Add(new WeaponKillRangePlusCalculator(weapon, 4));
            Triggers.Add(new BowTrigger(weapon, this));
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        public override Skill Clone()
        {
            return new SkillBow(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new SkillBow(newcard);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }
    }

    public class BowTrigger : CardTrigger, ITriggerInState, ITriggerAsk
    {
        public BowTrigger(Card _weapon, Skill _weaponskill) : base(_weapon)
        {
            this.weapon = _weapon;
            this.weaponskill = _weaponskill;
            Condition = new BowTriggerCondition(_weapon, _weaponskill);
        }
        
        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        private Skill weaponskill;
        public Skill WeaponSkill { get { return this.weaponskill; } }

        string ITriggerInState.StateKeyName { get => State.Damaged; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_BeforeStart; }

        string ITriggerAsk.Message { get => "是否要发动【月之弓】？"; }

        Player ITriggerAsk.GetAsked(Context ctx) { return  weapon.Owner; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            if (state.KeyName?.Equals(State.Damaged) != true) return;
            if (!(state.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(state.Ev);
            Player player = state.Owner;
            if (player == null) return;
            Zone equipzone = player.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Equips) == true);
            if (equipzone == null) return;
            Card card_horse_0 = equipzone.Cards.FirstOrDefault(_card => _card.CardType?.SubType?.E == Enum_CardSubType.HorsePlus);
            Card card_horse_1 = equipzone.Cards.FirstOrDefault(_card => _card.CardType?.SubType?.E == Enum_CardSubType.HorseMinus);
            if (card_horse_0 == null && card_horse_1 == null) return;
            CardSkillEvent ev_skill = new CardSkillEvent();
            ev_skill.Reason = ev;
            ev_skill.Card = weapon;
            ev_skill.Skill = weaponskill;
            ev_skill.Source = ev.Source;
            ev_skill.Targets.Clear();
            ev_skill.Targets.Add(player);
            ctx.World.InvokeEvent(ev_skill);
            if (ev_skill.Cancel) return;
            Action<IEnumerable<Card>> discard_horse = (cards) =>
            {
                Zone discardzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
                if (discardzone == null) return;
                ctx.World.MoveCards(player, cards, discardzone, ev_skill);
            };
            if (card_horse_0 != null && card_horse_1 != null)
            {
                DesktopCardBoardCore desktop_core = new DesktopCardBoardCore();
                desktop_core.Controller = ev.Source;
                desktop_core.IsAsync = false;
                DesktopCardBoardZone desktop_zone = new DesktopCardBoardZone(desktop_core);
                List<Card> card_house_list = new List<Card>();
                if (card_horse_0 != null) card_house_list.Add(card_horse_0);
                if (card_horse_1 != null) card_house_list.Add(card_horse_1);
                desktop_core.Zones.Add(desktop_zone);
                desktop_core.Flag = Enum_DesktopCardBoardFlag.SelectCardAndYes;
                desktop_core.Flag |= Enum_DesktopCardBoardFlag.CannotNo;
                desktop_core.CardFilter = new FulfillNumberCardFilter(1, 1);
                ctx.World.ShowDesktop(ev.Source, desktop_core, new List<IList<Card>>() { card_house_list }, false, null);
                if (desktop_core.SelectedCards.Count() > 0)
                    discard_horse?.Invoke(desktop_core.SelectedCards);
                return;
            }
            if (card_horse_0 != null)
            {
                discard_horse(new Card[] { card_horse_0 });
                return;
            }
            if (card_horse_1 != null)
            {
                discard_horse(new Card[] { card_horse_1 });
                return;
            }
        }
    }

    public class BowTriggerCondition : ConditionFilterFromCard
    {
        public BowTriggerCondition(Card _weapon, Skill _weaponskill) : base(_weapon)
        {
            this.weapon = _weapon;
            this.weaponskill = _weaponskill;
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        private Skill weaponskill;
        public Skill WeaponSkill { get { return this.weaponskill; } }
        
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.KeyName?.Equals(State.Damaged) != true) return false;
            if (!(state.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (!KillCard.IsKill(ev.Card)) return false;
            if (weapon.Zone?.KeyName?.Equals(Zone.Equips) != true) return false;
            if (weapon.Zone.Owner != ev.Source) return false;
            if (!weapon.Zone.Cards.Contains(weapon)) return false;
            Player player = state.Owner;
            if (player == null) return false;
            Zone equipzone = player.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Equips) == true);
            if (equipzone == null) return false;
            Card card_horse_0 = equipzone.Cards.FirstOrDefault(_card => _card.CardType?.SubType?.E == Enum_CardSubType.HorsePlus);
            if (card_horse_0 != null) return true;
            Card card_horse_1 = equipzone.Cards.FirstOrDefault(_card => _card.CardType?.SubType?.E == Enum_CardSubType.HorseMinus);
            if (card_horse_1 != null) return true;
            return false;
        }
    }
}
