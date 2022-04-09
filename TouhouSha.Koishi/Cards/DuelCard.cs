using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.AIs;
using TouhouSha.Core.Events;
using TouhouSha.Koishi.AIs;

namespace TouhouSha.Koishi.Cards
{
    public class DuelCard : Card
    {
        public const string Normal = "决斗";

        public DuelCard()
        {
            KeyName = Normal;
            UseCondition = new UseDuelCondition();
            TargetFilter = new UseDuelTargetFilter();
            CardType = new CardType(Enum_CardType.Spell, new CardSubType(Enum_CardSubType.Immediate));
        }
        public override Card Create()
        {
            return new DuelCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "出牌阶段，选择你以外一名角色为对象使用。对象开始轮流打出【杀】字段卡，直到一方不选择出【杀】字段卡并受到对方的一点伤害。",
                Image = ImageHelper.LoadCardImage("Cards", "Duel")
            };
        }
        public override double GetWorthForTarget()
        {
            return -2;
        }

        public override CardTargetFilterWorth GetWorthAI()
        {
            return new UseDuelWorthAI(this);
        }
    }
    
    public class UseDuelCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用决斗的条件";

        public UseDuelCondition()
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            return true;
        }
    }

    public class UseDuelTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用决斗的目标";

        public UseDuelTargetFilter()
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
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() > 0;
        }
    }

    public class UseDuelWorthAI : CardWorthSumAI
    {
        public UseDuelWorthAI(Card _thiscard) : base(_thiscard)
        {

        }

        public override double GetWorth(Context ctx, Card card, Player user, IEnumerable<Player> selecteds, Player want)
        {
            Zone handzone = user.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return double.MinValue;
            int killcount = handzone.Cards.Count(_card => KillCard.IsKill(_card));
            double[] killprobs = AITools.CardGausser.GetProbablyArrayOfCardKey(ctx, want, "Kill");
            double loserate = 0.0d;
            double worth = 0.0d;
            if (killcount < killprobs.Length) loserate = killprobs[killcount];
            worth += (1 - loserate) * -AITools.WorthAcc.GetWorthPerHp(ctx, user, want);
            worth += loserate * -AITools.WorthAcc.GetWorthPerHp(ctx, user, user);
            return worth;
        }
    }
}
