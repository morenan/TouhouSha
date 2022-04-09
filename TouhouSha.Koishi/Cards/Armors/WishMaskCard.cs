using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.UIs;

namespace TouhouSha.Koishi.Cards.Armors
{
    /// <summary>
    /// 防具【希望之面】
    /// </summary>
    /// <remarks>
    /// 赠送品。
    /// 你受到的锦囊牌的伤害+1。当你受到锦囊牌的伤害后，你可以将【希望之面】转移到一个合理的位置。
    /// </remarks>
    public class WishMaskCard : GiftArmor
    {
        public const string DefaultKeyName = "希望之面";
        
        public WishMaskCard()
        {
            KeyName = DefaultKeyName;
            Skills.Add(new WishMask_Skill(this));
        }
        public override Card Create()
        {
            return new WishMaskCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "赠送品。你受到的锦囊牌的伤害+1。当你受到锦囊牌的伤害后，你可以将【希望之面】放置到另一名没有装备防具的角色的装备栏上。",
                Image = ImageHelper.LoadCardImage("Cards", " WishMask")
            };
        }
        public override double GetWorthForTarget()
        {
            return -2;
        }
    }
    
    public class WishMask_Skill : Skill
    {
        public WishMask_Skill(Card _armor)
        {
            this.armor = _armor;
            IsLocked = true;
            Triggers.Add(new WishMask_Trigger_0(armor));
            Triggers.Add(new WishMask_Trigger_1(armor));
        }

        private Card armor;
        public Card Armor { get { return this.armor; } }

        public override Skill Clone()
        {
            return new WishMask_Skill(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new WishMask_Skill(newcard);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }

    }
    
    public class WishMask_Trigger_0 : CardTrigger, ITriggerInState
    {
        public WishMask_Trigger_0(Card _card) : base(_card)
        {
            Condition = new WishMask_Trigger_Condition_0(card);
        }

        string ITriggerInState.StateKeyName { get => State.Damaging; }
        
        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state.Ev is DamageEvent)) return;
            DamageEvent ev = (DamageEvent)(state.Ev);
            ev.DamageValue++;
        }
    }

    public class WishMask_Trigger_Condition_0 : ConditionFilterFromCard
    {
        public WishMask_Trigger_Condition_0(Card _card) : base(_card) { }

        public override bool Accept(Context ctx)
        {
            if (card.Owner == null) return false;
            if (card.Zone?.KeyName?.Equals(Zone.Equips) != true) return false;
            State state = ctx.World.GetCurrentState();
            if (!(state.Ev is DamageEvent)) return false;
            DamageEvent ev = (DamageEvent)(state.Ev);
            if (!(ev.Reason is CardEvent)) return false;
            if (ev.Target != card.Owner) return false;
            CardEvent ev1 = (CardEvent)(ev.Reason);
            if (ev1.Card.CardType?.E != Enum_CardType.Spell) return false;
            return true;
        }

    }

    public class WishMask_Trigger_1 : CardTrigger, ITriggerInState
    {
        public WishMask_Trigger_1(Card _card) : base(_card)
        {
            Condition = new WishMask_Trigger_Condition_1(card);
        }
        
        string ITriggerInState.StateKeyName { get => State.Damaged; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_End; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return;
            Player source = state.Owner;
            if (source == null) return;
            if (!(state.Ev is DamageEvent)) return;
            DamageEvent ev_damage = (DamageEvent)(state.Ev);
            ctx.World.SelectPlayer(card.KeyName, "请将【希望之面】装备给一个目标。", source,
                new WishMask_AnotherTargetFilter(source), false, 15,
                (players) => 
                {
                    Player target = players.FirstOrDefault();
                    if (target == null) return;
                    EquipZone equipzone = target.Zones.FirstOrDefault(_zone => _zone is EquipZone) as EquipZone;
                    if (equipzone == null) return;
                    ctx.World.MoveCard(source, card, equipzone, ev_damage);
                },
                () => { });
        }
    }

    public class WishMask_Trigger_Condition_1 : ConditionFilterFromCard
    {
        public WishMask_Trigger_Condition_1(Card _card) : base(_card) { }

        public override bool Accept(Context ctx)
        {
            if (card.Owner == null) return false;
            if (card.Zone?.KeyName?.Equals(Zone.Equips) != true) return false;
            State state = ctx.World.GetCurrentState();
            if (!(state.Ev is DamageEvent)) return false;
            DamageEvent ev = (DamageEvent)(state.Ev);
            if (!(ev.Reason is CardEvent)) return false;
            if (ev.Target != card.Owner) return false;
            CardEvent ev1 = (CardEvent)(ev.Reason);
            if (ev1.Card.CardType?.E != Enum_CardType.Spell) return false;
            return true;
        }
    }

    public class WishMask_AnotherTargetFilter : PlayerFilter
    {
        public WishMask_AnotherTargetFilter(Player _source) { this.source = _source; }

        private Player source;
        public Player Source { get { return this.source; } }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            if (selecteds.Count() > 0) return false;
            if (want == source) return false;
            EquipZone equipzone = want.Zones.FirstOrDefault(_zone => _zone is EquipZone) as EquipZone;
            if (equipzone == null) return false;
            EquipCell equipcell = equipzone.Cells.FirstOrDefault(_cell => _cell.E == Enum_CardSubType.Armor);
            if (equipcell == null) return false;
            if (!equipcell.IsEnabled) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() > 0;
        }
    }
}
