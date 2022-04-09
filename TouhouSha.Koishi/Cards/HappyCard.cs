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
    public class HappyCard : Card
    {
        public const string Normal = "乐不思蜀";

        public HappyCard()
        {
            KeyName = Normal;
            UseCondition = new UseHappyCondition();
            TargetFilter = new UseHappyTargetFilter();
            CardType = new CardType(Enum_CardType.Spell, new CardSubType(Enum_CardSubType.Delay));
        }

        public override Card Create()
        {
            return new HappyCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "出牌阶段放置到一名其他角色没有同名卡的判定区。判定阶段进行判定，不为♥跳过本回合出牌阶段。结算完毕弃置此牌。",
                Image = ImageHelper.LoadCardImage("Cards", "Happy")
            };
        }
        public override double GetWorthForTarget()
        {
            return -5;
        }

        public override CardTargetFilterWorth GetWorthAI()
        {
            return new UseHappyWorthAI(this);
        }
    }

    public class UseHappyCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用乐不思蜀的条件";

        public UseHappyCondition()
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

    public class UseHappyTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用乐不思蜀的目标";

        public UseHappyTargetFilter()
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
            Zone judgezone = want.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
            if (judgezone == null) return false;
            Card happy = judgezone.Cards.FirstOrDefault(_card => _card.KeyName?.Equals(HappyCard.Normal) == true);
            if (happy != null) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return true;
        }
    }
    public class UseHappyWorthAI : CardWorthSumAI
    {
        public UseHappyWorthAI(Card _thiscard) : base(_thiscard)
        {

        }

        public override double GetWorth(Context ctx, Card card, Player user, IEnumerable<Player> selecteds, Player want)
        {
            return -AITools.WorthAcc.GetWorthInPhase(ctx, user, want);
        }
    }
}
