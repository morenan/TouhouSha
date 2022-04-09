using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Koishi.Cards;

namespace TouhouSha.Koishi.Triggers
{
    public class FireTrigger : Trigger, ITriggerInState
    {
        public const string DefaultKeyName = "火攻响应";
        public const string FireShowCard = "火攻展示牌";
        public const string FireDiscard = "火攻丢弃牌";

        public FireTrigger()
        {
            KeyName = DefaultKeyName;
            Condition = new FireCondition();
        }

        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(state.Ev);
            ctx.World.RequireCard(FireShowCard, "请展示一张牌（火攻）。",
                state.Owner,
                new FulfillNumberCardFilter(1, 1)
                {
                    Allow_Hand = true,
                    Allow_Equiped = false,
                    Allow_Judging = false
                },
                false,
                Config.GameConfig.Timeout_Handle,
                (showcards) =>
                {
                    Card showcard = showcards.FirstOrDefault();
                    ctx.World.ShowHand(state.Owner, showcards);
                    ctx.World.RequireCard(FireDiscard, "请丢弃一张相同花色的牌。",
                        ev.Source,
                        new FireSameColor(showcard.CardColor),
                        true,
                        Config.GameConfig.Timeout_Handle,
                        (discards) =>
                        {
                            Zone discardzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
                            if (discardzone == null) return;
                            ctx.World.MoveCards(ev.Source, discards, discardzone, ev);
                            DamageEvent damageevent = ctx.World.GetDamageEvent(ev.Source, state.Owner, 1, DamageEvent.Fire, ev);
                            ctx.World.InvokeEventAfterState(damageevent, state);
                        },
                        null);
                    ctx.World.HideHand(state.Owner);
                },
                () =>
                {
                });
        }
    }

    public class FireCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return false;
            if (state.GetValue(NegateCard.HasBeenNegated) == 1) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (ev.Card.KeyName?.Equals(FireCard.Normal) != true) return false;
            return true;
        }
    }

    public class FireSameColor : CardFilter
    {
        public FireSameColor(CardColor _color)
        {
            this.color = _color;
        }

        private CardColor color;

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() >= 1) return false;
            if (want.Zone?.KeyName?.Equals(Zone.Hand) != true) return false;
            if (want.CardColor?.SeemAs(color.E) != true) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return selecteds.Count() >= 1;
        }
    }
}
