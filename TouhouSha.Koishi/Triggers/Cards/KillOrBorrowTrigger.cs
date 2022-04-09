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
    class KillOrBorrowTrigger : Trigger, ITriggerInState
    {
        public const string DefaultKeyName = "出杀响应借刀杀人";
        public const string HasKilled = "杀了";

        public KillOrBorrowTrigger()
        {
            KeyName = DefaultKeyName;
            Condition = new KillOrBorrowCondition();
        }

        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(state.Ev);
            Player carduser = ev.Source;
            Player source = ev.Targets[0];
            Player target = ev.Targets[1];
            ctx.World.RequireCard(KeyName, String.Format("请对{0}打出一张杀，或者将武器交给{1}。", target.Name, carduser.Name),
                state.Owner,
                new TargetCardFilter(1, 1, KillCard.Normal, KillCard.Thunder, KillCard.Fire),
                true, 10,
                (cards) => 
                {
                    state.SetValue(HasKilled, 1);
                    ctx.World.UseCard(source, target, cards.FirstOrDefault(), ev);
                },
                () =>
                {
                    state.SetValue(HasKilled, 0);
                    Zone equipzone = source.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Equips) == true);
                    if (equipzone == null) return;
                    Card weapon = equipzone.Cards.FirstOrDefault(_card => _card.CardType?.E == Enum_CardType.Equip && _card.CardType.SubType?.E == Enum_CardSubType.Weapon);
                    if (weapon == null) return;
                    Zone handzone = carduser.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
                    if (handzone == null) return;
                    ctx.World.MoveCard(carduser, weapon, handzone, ev);
                });
        }
    }

    public class KillOrBorrowCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return false;
            if (state.GetValue(NegateCard.HasBeenNegated) == 1) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (ev.Card.KeyName?.Equals(BorrowKnifeCard.Normal) != true) return false;
            return true;
        }
    }
}
