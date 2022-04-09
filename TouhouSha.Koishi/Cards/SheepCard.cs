using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Koishi.AIs;

namespace TouhouSha.Koishi.Cards
{
    public class SheepCard : Card
    {
        public const string Normal = "顺手牵羊";

        public SheepCard()
        {
            KeyName = Normal;
            UseCondition = new UseSheepCondition();
            TargetFilter = new UseSheepTargetFilter();
            CardType = new CardType(Enum_CardType.Spell, new CardSubType(Enum_CardSubType.Immediate));
        }

        public override Card Create()
        {
            return new SheepCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "①：出牌阶段，选择距离范围为1以内的一名其他角色发动。将目标手牌·装备·判定区的一张牌加入你的手牌。",
                Image = ImageHelper.LoadCardImage("Cards", "Sheep")
            };
        }
        public override double GetWorthForTarget()
        {
            return -2;
        }
    }

    public class UseSheepCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用顺手牵羊的条件";

        public UseSheepCondition()
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            return true;
        }
    }

    public class UseSheepTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用顺手牵羊的目标";

        public UseSheepTargetFilter()
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
            if (selecteds.Count() > 0) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() > 0;
        }
    }
    public class UseSheepWorthAI : CardWorthSumAI
    {
        public UseSheepWorthAI(Card _thiscard) : base(_thiscard)
        {

        }

        public override double GetWorth(Context ctx, Card card, Player user, IEnumerable<Player> selecteds, Player want)
        {
            Card optcard = null;
            double optworth = double.MinValue;
            Zone handzone = user.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Hand) == true);
            foreach (Zone zone in want.Zones)
            {
                switch (zone.KeyName)
                {
                    case Zone.Hand:
                    case Zone.Equips:
                    case Zone.Judge:
                        foreach (Card sheepcard in zone.Cards)
                        {
                            double sheepworth = -AITools.WorthAcc.GetWorth(ctx, user, sheepcard);
                            Zone oldzone = sheepcard.Zone;
                            sheepcard.Zone = handzone;
                            sheepworth += AITools.WorthAcc.GetWorth(ctx, user, sheepcard);
                            sheepcard.Zone = oldzone;
                            if (sheepworth > optworth)
                            {
                                optworth = sheepworth;
                                optcard = sheepcard;
                            }
                        }
                        break;
                }
            }
            return optworth;
        }
    }
}