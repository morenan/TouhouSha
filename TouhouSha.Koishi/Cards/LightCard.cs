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
    public class LightCard : Card
    {
        public const string Normal = "闪电";

        public LightCard()
        {
            KeyName = Normal;
            UseCondition = new UseLightCondition();
            TargetFilter = new UseLightTargetFilter();
            CardType = new CardType(Enum_CardType.Spell, new CardSubType(Enum_CardSubType.Delay));
        }

        public override Card Create()
        {
            return new LightCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "出牌阶段放置到你的判定区。判定阶段进行判定，为♠2~9时受到三点雷电伤害并弃置此牌，否则移动到下一个角色的判定区。",
                Image = ImageHelper.LoadCardImage("Cards", "Light")
            };
        }
        public override double GetWorthForTarget()
        {
            return -1;
        }

        public override CardTargetFilterWorth GetWorthAI()
        {
            return new UseLightWorthAI(this);
        }
    }

    public class UseLightCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用闪电的条件";

        public UseLightCondition()
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            Player player = state.Owner;
            if (player == null) return false;
            Zone judgezone = player.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
            if (judgezone == null) return false;
            Card light = judgezone.Cards.FirstOrDefault(_card => _card.KeyName?.Equals(LightCard.Normal) == true);
            if (light != null) return false;
            return true;
        }
    }

    public class UseLightTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用闪电的目标";

        public UseLightTargetFilter()
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
            if (player != want) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return true;
        }
    }


    public class UseLightWorthAI : CardWorthSumAI
    {
        public UseLightWorthAI(Card _thiscard) : base(_thiscard)
        {

        }

        public override double GetWorth(Context ctx, Card card, Player user, IEnumerable<Player> selecteds, Player want)
        {
            double worth = 0;
            foreach (Player player in ctx.World.GetAlivePlayers())
                worth += -3 * AITools.WorthAcc.GetWorthPerHp(ctx, user, player);
            worth /= ctx.World.GetAlivePlayers().Count();
            return worth;
        }
    }
}
