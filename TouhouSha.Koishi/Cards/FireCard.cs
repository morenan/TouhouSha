using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Koishi.AIs;

namespace TouhouSha.Koishi.Cards
{
    public class FireCard : Card
    {
        public const string Normal = "火攻";

        public FireCard()
        {
            KeyName = Normal;
            UseCondition = new UseFireCondition();
            TargetFilter = new UseFireTargetFilter();
            CardType = new CardType(Enum_CardType.Spell, new CardSubType(Enum_CardSubType.Immediate));
        }
        public override Card Create()
        {
            return new FireCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "出牌阶段对一名有手牌的角色为对象发动。目标展示一张手牌，可以选择丢弃相同花色的手牌，对目标造成一点火焰伤害。",
                Image = ImageHelper.LoadCardImage("Cards", "Fire")
            };
        }
        public override double GetWorthForTarget()
        {
            return -2;
        }
    }

    public class UseFireCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用火攻的条件";

        public UseFireCondition()
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            return true;
        }
    }

    public class UseFireTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用火攻的目标";

        public UseFireTargetFilter()
        {
            KeyName = DefaultKeyName;
        }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            Player player = state.Owner;
            if (player == null) return false;
            if (player == want) return false;
            if (selecteds.Count() > 0) return false;
            Zone handzone = want.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return false;
            if (handzone.Cards.Count() == 0) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() > 0;
        }
    }

    public class UseFireWorthAI : CardWorthSumAI
    {
        public UseFireWorthAI(Card _thiscard) : base(_thiscard)
        {

        }

        public override double GetWorth(Context ctx, Card card, Player user, IEnumerable<Player> selecteds, Player want)
        {
            Zone handzone = user.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return -10000;
            if (handzone.Cards.Count() <= 1) return -10000;
            Dictionary<Enum_CardColor, double> handcolors = new Dictionary<Enum_CardColor, double>();
            Dictionary<Enum_CardColor, double> targetprobs = new Dictionary<Enum_CardColor, double>();
            double probsum = 0;
            double worth = 0;
            double damageworth = -AITools.WorthAcc.GetWorthPerHp(ctx, user, want);
            foreach (Player other in ctx.World.GetAlivePlayers())
            {
                if (other == want) continue;
                if (!other.IsChained) continue;
                damageworth += (-AITools.WorthAcc.GetWorthPerHp(ctx, user, other));
            }
            foreach (Card othercard in handzone.Cards)
            {
                if (othercard == card) continue;
                if (othercard.CardColor == null) continue;
                double lostworth = -AITools.WorthAcc.GetWorth(ctx, user, othercard);
                if (!handcolors.ContainsKey(othercard.CardColor.E))
                    handcolors.Add(othercard.CardColor.E, lostworth);
                else if (handcolors[othercard.CardColor.E] < lostworth)
                    handcolors[othercard.CardColor.E] = lostworth;
            }
            foreach (Enum_CardColor color in new Enum_CardColor[] { Enum_CardColor.Heart, Enum_CardColor.Diamond, Enum_CardColor.Spade, Enum_CardColor.Club })
            {
                double prob = AITools.CardGausser.GetProbablyOfCardColor(ctx, want, color);
                targetprobs.Add(color, prob);
                probsum += prob;
            }
            foreach (KeyValuePair<Enum_CardColor, double> kvp in targetprobs)
            {
                double handworth = 0.0d;
                if (handcolors.TryGetValue(kvp.Key, out handworth))
                    worth += kvp.Value / probsum * (damageworth - handworth);
            }
            return worth;
        }
    }
}
