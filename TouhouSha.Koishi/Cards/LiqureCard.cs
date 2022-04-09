using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.AIs;
using TouhouSha.Koishi.AIs;

namespace TouhouSha.Koishi.Cards
{
    public class LiqureCard : Card
    {
        public const string Normal = "酒";
        public const string BullUp = "喝过酒";
        public const string UsedBull = "出过的酒";
        public const string BullMaxUse = "酒的最大使用数量";

        public LiqureCard()
        {
            KeyName = Normal;
            UseCondition = new UseLiqureCondition();
            TargetFilter = new UseLiqureTargetFilter();
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
                Description = "①：出牌阶段限一次，对自己使用。下一张的【杀】字段卡伤害+1。\n②：濒死阶段，对自己使用。你回复一点体力。",
                Image = ImageHelper.LoadCardImage("Cards", "Liqure")
            };
        }

        public override double GetWorthForTarget()
        {
            return 1;
        }
    }

    public class UseLiqureCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用酒的条件";

        public UseLiqureCondition()
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            Player player = state.Owner;
            if (player == null) return false;
            int bullup = ctx.World.CalculateValue(ctx, player, LiqureCard.BullUp);
            if (bullup > 0) return false;
            return true;
        }
    }

    public class UseLiqureTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用酒的目标";

        public UseLiqureTargetFilter()
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

    public class UseLiqureWorthAI : CardWorthSumAI
    {
        public UseLiqureWorthAI(Card _thiscard) : base(_thiscard)
        {

        }

        public override double GetWorth(Context ctx, Card card, Player user, IEnumerable<Player> selecteds, Player want)
        {
            double missrate = AITools.CardGausser.GetProbablyOfCardKey(ctx, want, MissCard.Normal);
            Zone handzone0 = user.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            Zone handzone1 = want.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            Card killcard = handzone0.Cards.FirstOrDefault(_card => KillCard.IsKill(_card));
            Card misscard = ctx.World.GetCardInstance(MissCard.Normal).Clone();
            misscard.Zone = handzone1;
            try
            {
                if (killcard == null) return 0;
                ConditionFilter killcan = killcard.UseCondition;
                killcan = ctx.World.TryReplaceNewCondition(killcan, ctx.Ev);
                if (killcan == null || !killcan.Accept(ctx)) return 0;
                missrate = Math.Min(missrate, 1.0d);
                double worth = 0;
                worth += (1 - missrate) * (-AITools.WorthAcc.GetWorthPerHp(ctx, user, want));
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
