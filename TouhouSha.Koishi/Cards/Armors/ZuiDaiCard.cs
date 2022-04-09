using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Triggers;

namespace TouhouSha.Koishi.Cards.Armors
{
    /// <summary>
    /// 防具【罪袋】
    /// </summary>
    /// <remarks>
    /// 赠送品。
    /// 你无法用【闪】响应【杀】。当你受到【杀】的伤害后，你可以将【罪袋】转移到一个合理的位置。
    /// </remarks>
    public class ZuiDaiCard : GiftArmor
    {
        public const string DefaultKeyName = "罪袋";

        public ZuiDaiCard()
        {
            KeyName = DefaultKeyName;
            Skills.Add(new ZuiDai_Skill(this));
        }
        public override Card Create()
        {
            return new ZuiDaiCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "赠送品。你无法用【闪】响应【杀】。当你受到【杀】的伤害后，你可以将【罪袋】放置到另一名没有装备防具的角色的装备栏上。",
                Image = ImageHelper.LoadCardImage("Cards", "ZuiDai")
            };
        }
        public override double GetWorthForTarget()
        {
            return -2;
        }
    }
    
    public class ZuiDai_Skill : Skill
    {
        public ZuiDai_Skill(Card _armor)
        {
            this.armor = _armor;
            IsLocked = true;
            Triggers.Add(new ZuiDai_Trigger_0(armor));
            Triggers.Add(new ZuiDai_Trigger_1(armor));
        }

        private Card armor;
        public Card Armor { get { return this.armor; } }

        public override Skill Clone()
        {
            return new ZuiDai_Skill(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new ZuiDai_Skill(newcard);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }
    }

    public class ZuiDai_Trigger_0 : CardTrigger, ITriggerInEvent
    {
        public ZuiDai_Trigger_0(Card _card) : base(_card)
        {
            Condition = new ZuiDai_Trigger_Condition_0(card);
        }
        
        string ITriggerInEvent.EventKeyName { get => UseTriggerEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is UseTriggerEvent)) return;
            UseTriggerEvent ev = (UseTriggerEvent)(ctx.Ev);
            if (!(ev.NewTrigger is MissKillTrigger)) return;
            ev.NewTrigger = new ZuiDai_CannotMissTrigger();
        }
    }

    public class ZuiDai_Trigger_Condition_0 : ConditionFilterFromCard
    {
        public ZuiDai_Trigger_Condition_0(Card _card) : base(_card) { }

        public override bool Accept(Context ctx)
        {
            if (card.Owner == null) return false;
            if (card.Zone?.KeyName?.Equals(Zone.Equips) != true) return false;
            if (!(ctx.Ev is UseTriggerEvent)) return false;
            UseTriggerEvent ev = (UseTriggerEvent)(ctx.Ev);
            if (!(ev.NewTrigger is MissKillTrigger)) return false;
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.KeyName?.Equals(State.Handle) != true) return false;
            if (state.Owner != card.Owner) return false;
            return true;
        }
    }

    public class ZuiDai_CannotMissTrigger : Trigger
    {

    }

    public class ZuiDai_Trigger_1 : CardTrigger, ITriggerInState
    {
        public ZuiDai_Trigger_1(Card _card) : base(_card)
        {
            Condition = new ZuiDai_Trigger_Condition_1(card);
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
            ctx.World.SelectPlayer(card.KeyName, "请将【罪袋】装备给一个目标。", source,
                new ZuiDai_AnotherTargetFilter(source), false, 15,
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

    public class ZuiDai_Trigger_Condition_1 : ConditionFilterFromCard
    {
        public ZuiDai_Trigger_Condition_1(Card _card) : base(_card) { }

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

    public class ZuiDai_AnotherTargetFilter : PlayerFilter
    {
        public ZuiDai_AnotherTargetFilter(Player _source) { this.source = _source; }

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
