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
    public class KillCard : Card
    {
        public const string Normal = "杀";
        public const string Thunder = "雷杀";
        public const string Fire = "火杀";
        public const string KillMaxTarget = "杀的最大目标数量";
        public const string UsedKill = "出过的杀";
        public const string KillMaxUse = "杀的最大使用数量";

        static public bool IsKill(Card card)
        {
            switch (card?.KeyName)
            {
                case Normal:
                case Thunder:
                case Fire: return true;
                default: return false;
            }
        }
        
        public KillCard()
        {
            KeyName = Normal;
            UseCondition = new UseKillCondition(this);
            TargetFilter = new UseKillTargetFilter(this);
            CardType = new CardType(Enum_CardType.Base, null);
        }
        
        public override Card Create()
        {
            return new KillCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = String.Format("【杀】字段。出牌阶段同字段限一次，选择一名攻击范围内的其他角色为对象发动，其选择一项：1. 打出一张【闪】。2. 受到你的一点{0}伤害。",
                    Fire.Equals(KeyName) ? "火焰" : Thunder.Equals(KeyName) ? "雷电" : ""),
                Image = ImageHelper.LoadCardImage("Cards", Fire.Equals(KeyName) ? "FireKill" : Thunder.Equals(KeyName) ? "ThunderKill" : "Kill")
            };
        }

        public override double GetWorthForTarget()
        {
            return -1;
        }

        public override CardTargetFilterWorth GetWorthAI()
        {
            return new UseKillWorthAI(this);
        }
    }

    public class UseKillCondition : ConditionFilterFromCard
    {
        public const string DefaultKeyName = "使用杀的条件";

        public UseKillCondition(Card _card) : base(_card)
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            Player player = state.Owner;
            if (player == null) return false;
            int d0 = ctx.World.CalculateValue(ctx, state, KillCard.UsedKill);
            int d1 = ctx.World.CalculateValue(ctx, player, KillCard.KillMaxUse);
            return d0 < d1;
        }
    }

    public class UseKillTargetFilter : PlayerFilterFromCard
    {
        public const string DefaultKeyName = "使用杀的目标";

        public UseKillTargetFilter(Card _card) : base(_card)
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
            int killmax = ctx.World.CalculateValue(ctx, player, KillCard.KillMaxTarget);
            if (selecteds.Count() + 1 > killmax) return false;
            if (!ctx.World.IsInDistance2Kill(ctx, player, want)) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() > 0;
        }
    }

    public class UseKillTargetFilterIgnoreRange : PlayerFilterFromCard
    {
        public const string DefaultKeyName = "使用杀的目标(无视距离)";

        public UseKillTargetFilterIgnoreRange(Card _card) : base(_card)
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
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() > 0;
        }
    }

    public class UseKillWorthAI : CardWorthSumAI
    {
        public UseKillWorthAI(Card _thiscard) : base(_thiscard)
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
                damagevalue += ctx.World.CalculateValue(ctx, user, LiqureCard.BullUp);
                double damageworth = (-AITools.WorthAcc.GetWorthPerHp(ctx, user, want)) * damagevalue;
                if (want.IsChained && (card.KeyName?.Equals(KillCard.Fire) == true || card.KeyName?.Equals(KillCard.Thunder) == true))
                {
                    foreach (Player other in ctx.World.GetAlivePlayers())
                    {
                        if (other == want) continue;
                        if (!other.IsChained) continue;
                        damageworth += (-AITools.WorthAcc.GetWorthPerHp(ctx, user, other)) * damagevalue;
                    }
                }
                worth += missrate * (-AITools.WorthAcc.GetWorth(ctx, user, misscard, Enum_CardBehavior.Handle));
                worth += (1 - missrate) * damageworth;
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
