using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Cards;

namespace TouhouSha.Koishi.Triggers
{
    public class EquipReplaceTrigger : Trigger, ITriggerInEvent
    {
        public EquipReplaceTrigger()
        {
            KeyName = "替换已经被装备的卡片";
            Condition = new EquipReplaceCondition();
        }

        string ITriggerInEvent.EventKeyName { get => MoveCardDoneEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is MoveCardDoneEvent)) return;
            MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
            Dictionary<Enum_CardSubType, List<Card>> cardofsubs = new Dictionary<Enum_CardSubType, List<Card>>();
            HashSet<Card> handleds = new HashSet<Card>();
            List<Card> discards = new List<Card>();
            foreach (Card card in ev.MovedCards.Concat(ev.NewZone.Cards))
            {
                List<Card> sublist = null;
                if (handleds.Contains(card)) continue;
                handleds.Add(card);
                if (card.CardType?.SubType == null) continue;
                if (cardofsubs.TryGetValue(card.CardType.SubType.E, out sublist))
                {
                    sublist = new List<Card>();
                    cardofsubs.Add(card.CardType.SubType.E, sublist);
                }
                sublist.Add(card);
            }
            foreach (KeyValuePair<Enum_CardSubType, List<Card>> kvp in cardofsubs)
            {
                if (kvp.Value.Count() <= 1) continue;
                discards.AddRange(kvp.Value.GetRange(1, kvp.Value.Count() - 1));
            }
            if (discards.Count() > 0)
            {
                Zone discardzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
                if (discardzone != null) ctx.World.MoveCards(ev.NewZone.Owner, discards, discardzone, ev);
            }
        }
    }

    public class EquipReplaceCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is MoveCardDoneEvent)) return false;
            MoveCardDoneEvent ev = (MoveCardDoneEvent)(ctx.Ev);
            if (ev.NewZone?.KeyName?.Equals(Zone.Equips) != true) return false;
            return true;
        }
    }



}
