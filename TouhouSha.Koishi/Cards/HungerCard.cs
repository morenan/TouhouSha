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
    public class HungerCard : Card
    {
        public const string Normal = "兵粮寸断";

        public HungerCard()
        {
            KeyName = Normal;
            UseCondition = new UseHungerCondition();
            TargetFilter = new UseHungerTargetFilter();
            CardType = new CardType(Enum_CardType.Spell, new CardSubType(Enum_CardSubType.Delay));
        }
        public override Card Create()
        {
            return new HungerCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "出牌阶段选择判定区没有同名卡的一名其他角色为对象发动，放置到其判定区内。判定阶段进行判定，不为♣跳过本回合摸牌阶段。结算完毕弃置此牌。",
                Image = ImageHelper.LoadCardImage("Cards", "Hunger")
            };
        }
        public override double GetWorthForTarget()
        {
            return -2;
        }

        public override CardTargetFilterWorth GetWorthAI()
        {
            return new UseHungerWorthAI(this);
        }
    }

    public class UseHungerCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用兵粮寸断的条件";

        public UseHungerCondition()
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            Player player = state.Owner;
            if (player == null) return false;
            return true;
        }
    }

    public class UseHungerTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用兵粮寸断的目标";

        public UseHungerTargetFilter()
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
            if (!ctx.World.IsInDistance(ctx, player, want)) return false;
            Zone judgezone = want.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
            if (judgezone == null) return false;
            Card hunger = judgezone.Cards.FirstOrDefault(_card => _card.KeyName?.Equals(HungerCard.Normal) == true);
            if (hunger != null) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return true;
        }
    }
    public class UseHungerWorthAI : CardWorthSumAI
    {
        public UseHungerWorthAI(Card _thiscard) : base(_thiscard)
        {

        }

        public override double GetWorth(Context ctx, Card card, Player user, IEnumerable<Player> selecteds, Player want)
        {
            Zone handzone = want.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return 0;
            return -AITools.WorthAcc.GetWorthExpected(ctx, user, handzone) * 2;
        }
    }
}
