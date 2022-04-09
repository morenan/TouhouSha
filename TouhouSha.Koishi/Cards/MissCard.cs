using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;

namespace TouhouSha.Koishi.Cards
{
    public class MissCard : Card
    {
        public const string Normal = "闪";

        public MissCard()
        {
            KeyName = Normal;
            UseCondition = new UseMissCondition();
            TargetFilter = new UseMissTargetFilter();
            CardType = new CardType(Enum_CardType.Base, null);
        }
        public override Card Create()
        {
            return new LiqureCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "当你成为【杀】字段卡的目标时，你可以打出这张卡使【杀】无效。",
                Image = ImageHelper.LoadCardImage("Cards", "Miss")
            };
        }

        public override double GetWorthForTarget()
        {
            return 1.0d;
        }
    }

    public class UseMissCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用闪的条件";

        public UseMissCondition()
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            return false;
        }
    }


    public class UseMissTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用闪的目标";

        public UseMissTargetFilter()
        {
            KeyName = DefaultKeyName;
        }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            return false;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return true;
        }
    }
}
