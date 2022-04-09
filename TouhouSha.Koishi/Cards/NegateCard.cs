using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;

namespace TouhouSha.Koishi.Cards
{
    public class NegateCard : Card
    {
        public const string Normal = "无懈可击";
        public const string HasBeenNegated = "被无懈可击掉";

        public NegateCard()
        {
            KeyName = Normal;
            UseCondition = new UseNegateCondition();
            TargetFilter = new UseNegateTargetFilter();
            CardType = new CardType(Enum_CardType.Spell, new CardSubType(Enum_CardSubType.Immediate));
        }
        public override Card Create()
        {
            return new NegateCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "当锦囊牌指定一个目标发动时，你可以打出这张卡取消对这个目标的结算。",
                Image = ImageHelper.LoadCardImage("Cards", "Negate")
            };
        }
        public override double GetWorthForTarget()
        {
            return 2;
        }
    }
    public class UseNegateCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用无懈可击的条件";

        public UseNegateCondition()
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            return false;
        }
    }
    
    public class UseNegateTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用无懈可击的目标";

        public UseNegateTargetFilter()
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
