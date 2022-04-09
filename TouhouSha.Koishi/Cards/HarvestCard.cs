using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.AIs;
using TouhouSha.Core.Events;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.AIs;

namespace TouhouSha.Koishi.Cards
{
    public class HarvestCard : Card
    {
        public const string Normal = "五谷丰登";
        public const string DesktopBoard = "五谷丰登选卡";
        public static readonly Stack<DesktopCardBoardCore> DesktopStack = new Stack<DesktopCardBoardCore>();

        public HarvestCard()
        {
            KeyName = Normal;
            UseCondition = new UseHarvestCondition();
            TargetFilter = new UseHarvestTargetFilter();
            CardType = new CardType(Enum_CardType.Spell, new CardSubType(Enum_CardSubType.Immediate));
        }
        public override Card Create()
        {
            return new HarvestCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "出牌阶段对所有角色使用。从牌堆将X张牌置于桌面（X为目标数），目标角色依次选择并获得其中的一张，剩余的送去墓地。",
                Image = ImageHelper.LoadCardImage("Cards", "Harvest")
            };
        }
        public override double GetWorthForTarget()
        {
            return 1;
        }

        public override CardTargetFilterWorth GetWorthAI()
        {
            return new UseHarvestWorthAI(this);
        }
    }

    public class UseHarvestCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用五谷丰登的条件";

        public UseHarvestCondition()
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            return true;
        }
    }

    public class UseHarvestTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用五谷丰登的目标";

        public UseHarvestTargetFilter()
        {
            KeyName = DefaultKeyName;
        }

        public override Enum_PlayerFilterFlag GetFlag(Context ctx)
        {
            return Enum_PlayerFilterFlag.ForceAll;
        }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            Player player = state.Owner;
            if (player == null) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return true;
        }
    }
    public class UseHarvestWorthAI : CardWorthSumAI
    {
        public UseHarvestWorthAI(Card _thiscard) : base(_thiscard)
        {

        }

        public override double GetWorth(Context ctx, Card card, Player user, IEnumerable<Player> selecteds, Player want)
        {
            Zone handzone = want.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            if (handzone == null) return 0;
            return AITools.WorthAcc.GetWorthExpected(ctx, user, handzone);
        }
    }
}
