using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Koishi.AIs;
using TouhouSha.Koishi.Triggers;

namespace TouhouSha.Koishi.Cards
{
    public class PeachCard : Card
    {
        public const string Normal = "桃";
        
        public PeachCard()
        {
            KeyName = Normal;
            UseCondition = new UsePeachCondition();
            TargetFilter = new UsePeachTargetFilter();
            CardType = new CardType(Enum_CardType.Base, null);
        }
        public override Card Create()
        {
            return new PeachCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "①：出牌阶段，若自己已受伤，对自己使用。回复1点体力。\n②：场上角色的濒死阶段使用，进入这个阶段的角色回复一点体力。",
                Image = ImageHelper.LoadCardImage("Cards", "Peach")
            };
        }

        public override double GetWorthForTarget()
        {
            return 2;
        }
    }

    public class UsePeachCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用桃的条件";

        public UsePeachCondition()
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            Player player = state.Owner;
            if (player == null) return false;
            return player.HP < player.MaxHP;
        }
    }

    public class UsePeachTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用桃的目标";

        public UsePeachTargetFilter()
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
            return player == want;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() > 0;
        }
    }

    public class UsePeachWorthAI : CardWorthSumAI
    {
        public UsePeachWorthAI(Card _thiscard) : base(_thiscard)
        {
            
        }

        public override double GetWorth(Context ctx, Card card, Player user, IEnumerable<Player> selecteds, Player want)
        {
            double hpworth = AITools.WorthAcc.GetWorthPerHp(ctx, user, want);
            if (want != user) return hpworth;
            Zone handzone = user.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            int cardcapacity = user.HP;
            cardcapacity += ctx.World.CalculateValue(ctx, user, DiscardTrigger.ExtraHandCapacity);
            int cardtodiscard = Math.Max(0, handzone.Cards.Count() - cardcapacity);
            return hpworth - Math.Min(2, cardtodiscard) * AITools.WorthAcc.GetWorthExpected(ctx, user, handzone);
        }
    }
}
