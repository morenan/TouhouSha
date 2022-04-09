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
    public class AttackAllCard : Card
    {
        public const string Normal = "南蛮入侵";

        public AttackAllCard()
        {
            KeyName = Normal;
            UseCondition = new UseAttackAllCondition();
            TargetFilter = new UseAttackAllTargetFilter();
            CardType = new CardType(Enum_CardType.Spell, new CardSubType(Enum_CardSubType.Immediate));
        }

        public override Card Create()
        {
            return new AttackAllCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "出牌阶段，对除你以外的所有角色使用。目标依次结算，需打出一张【杀】字段卡，否则受到你造成的1点伤害。",
                Image = ImageHelper.LoadCardImage("Cards", "AttackAll")
            };
        }
        public override double GetWorthForTarget()
        {
            return -1;
        }
        public override CardTargetFilterWorth GetWorthAI()
        {
            return new UseAttackAllWorthAI(this);
        }
    }

    public class UseAttackAllCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用南蛮入侵的条件";

        public UseAttackAllCondition()
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            return true;
        }
    }

    public class UseAttackAllTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用南蛮入侵的目标";

        public UseAttackAllTargetFilter()
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

    public class UseAttackAllWorthAI : CardWorthSumAI
    {
        public UseAttackAllWorthAI(Card _thiscard) : base(_thiscard)
        {

        }

        public override double GetWorth(Context ctx, Card card, Player user, IEnumerable<Player> selecteds, Player want)
        {
            double killrate = AITools.CardGausser.GetProbablyOfCardKey(ctx, want, "Kill");
            Card killcard = ctx.World.GetCardInstance(KillCard.Normal).Clone();
            Zone handzone1 = want.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            killcard.Zone = handzone1;
            try
            {
                double worth = 0;
                double damagevalue = 1;
                worth += killrate * (-AITools.WorthAcc.GetWorth(ctx, user, killcard, Enum_CardBehavior.Handle));
                worth += (1 - killrate) * (-AITools.WorthAcc.GetWorthPerHp(ctx, user, want)) * damagevalue;
                return worth;
            }
            catch (Exception)
            {
                return 0.0d;
            }
            finally
            {
                killcard.Zone = null;
            }
        }

    }
}
