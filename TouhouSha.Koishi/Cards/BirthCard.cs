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
    public class BirthCard : Card
    {
        public const string Normal = "无中生有";

        public BirthCard()
        {
            KeyName = Normal;
            UseCondition = new UsePeachCondition();
            TargetFilter = new UsePeachTargetFilter();
            CardType = new CardType(Enum_CardType.Spell, new CardSubType(Enum_CardSubType.Immediate));
        }
        public override Card Create()
        {
            return new BirthCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "①：出牌阶段，对自己使用。你摸两张牌。",
                Image = ImageHelper.LoadCardImage("Cards", "Birth")
            };
        }
        public override double GetWorthForTarget()
        {
            return 2;
        }

        public override CardTargetFilterWorth GetWorthAI()
        {
            return new UseBirthWorthAI(this);
        }
    }


    public class UseBirthWorthAI : CardWorthSumAI
    {
        public UseBirthWorthAI(Card _thiscard) : base(_thiscard)
        {

        }

        public override double GetWorth(Context ctx, Card card, Player user, IEnumerable<Player> selecteds, Player want)
        {
            Zone handzone = user.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            return AITools.WorthAcc.GetWorthExpected(ctx, user, handzone) * 2;
        }
    }
}
