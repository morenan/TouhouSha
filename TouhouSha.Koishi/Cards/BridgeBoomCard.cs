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
    public class BridgeBoomCard : Card
    {
        public const string Normal = "过河拆桥";

        public BridgeBoomCard()
        {
            KeyName = Normal;
            UseCondition = new UseBridgeBoomCondition();
            TargetFilter = new UseBridgeBoomTargetFilter();
            CardType = new CardType(Enum_CardType.Spell, new CardSubType(Enum_CardSubType.Immediate));
        }
        public override Card Create()
        {
            return new BridgeBoomCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "出牌阶段，选择除你以外的一名角色为对象发动。弃置对象手牌·装备·判定区里的一张牌。",
                Image = ImageHelper.LoadCardImage("Cards", "Bridge")
            };
        }
        public override double GetWorthForTarget()
        {
            return -1;
        }

        public override CardTargetFilterWorth GetWorthAI()
        {
            return new UseBridgeBoomWorthAI(this);
        }
    }

    public class UseBridgeBoomCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用过河拆桥的条件";

        public UseBridgeBoomCondition()
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            return true;
        }
    }

    public class UseBridgeBoomTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用过河拆桥的目标";

        public UseBridgeBoomTargetFilter()
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

    public class UseBridgeBoomWorthAI : CardWorthSumAI
    {
        public UseBridgeBoomWorthAI(Card _thiscard) : base(_thiscard)
        {

        }

        public override double GetWorth(Context ctx, Card card, Player user, IEnumerable<Player> selecteds, Player want)
        {
            Card optcard = null;
            double optworth = double.MinValue;
            foreach (Zone zone in want.Zones)
            {
                switch (zone.KeyName)
                {
                    case Zone.Hand:
                    case Zone.Equips:
                    case Zone.Judge:
                        foreach (Card boomcard in zone.Cards)
                        {
                            double boomworth = -AITools.WorthAcc.GetWorth(ctx, user, boomcard);
                            if (boomworth > optworth)
                            {
                                optworth = boomworth;
                                optcard = boomcard;
                            }
                        }
                        break;
                }
            }
            return optworth;
        }
    }
}
