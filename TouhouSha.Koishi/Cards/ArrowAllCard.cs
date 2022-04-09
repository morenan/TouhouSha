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
    public class ArrowAllCard : Card
    {
        public const string Normal = "万箭齐发";

        public ArrowAllCard()
        {
            KeyName = Normal;
            UseCondition = new UseArrowAllCondition();
            TargetFilter = new UseArrowAllTargetFilter();
            CardType = new CardType(Enum_CardType.Spell, new CardSubType(Enum_CardSubType.Immediate));
        }

        public override Card Create()
        {
            return new ArrowAllCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "出牌阶段，对除你以外的所有角色使用。目标依次结算，需打出一张【闪】字段卡，否则受到你造成的1点伤害。",
                Image = ImageHelper.LoadCardImage("Cards", "ArrowAll")
            };
        }
        public override double GetWorthForTarget()
        {
            return -1;
        }

        public override CardTargetFilterWorth GetWorthAI()
        {
            return new UseArrowAllWorthAI(this);
        }
    }

    public class UseArrowAllCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用万箭齐发的条件";

        public UseArrowAllCondition()
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            return true;
        }
    }

    public class UseArrowAllTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用万箭齐发的目标";

        public UseArrowAllTargetFilter()
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
            if (player == want) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return true;
        }
    }

    public class UseArrowAllWorthAI : CardWorthSumAI
    {
        public UseArrowAllWorthAI(Card _thiscard) : base(_thiscard)
        {

        }

        public override double GetWorth(Context ctx, Card card, Player user, IEnumerable<Player> selecteds, Player want)
        {
            double missrate = AITools.CardGausser.GetProbablyOfCardKey(ctx, want, MissCard.Normal);
            Card misscard = ctx.World.GetCardInstance(MissCard.Normal).Clone();
            Zone handzone1 = want.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            misscard.Zone = handzone1;
            try
            {
                double worth = 0;
                double damagevalue = 1;
                worth += missrate * (-AITools.WorthAcc.GetWorth(ctx, user, misscard, Enum_CardBehavior.Handle));
                worth += (1 - missrate) * (-AITools.WorthAcc.GetWorthPerHp(ctx, user, want)) * damagevalue;
                return worth;
            }
            catch (Exception)
            {
                return 0.0d;
            }
            finally
            {
                misscard.Zone = null;
            }
        }

    }
}
