using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;

namespace TouhouSha.Koishi.Cards
{
    public class PeachAllCard : Card
    {
        public const string Normal = "桃园结义";

        public PeachAllCard()
        {
            KeyName = Normal;
            UseCondition = new UsePeachAllCondition();
            TargetFilter = new UsePeachAllTargetFilter();
            CardType = new CardType(Enum_CardType.Spell, new CardSubType(Enum_CardSubType.Immediate));
        }

        public override Card Create()
        {
            return new PeachAllCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "出牌阶段，对所有角色使用。目标已受伤，则回复1点体力。",
                Image = ImageHelper.LoadCardImage("Cards", "PeachAll")
            };
        }
        public override double GetWorthForTarget()
        {
            return 2;
        }
    }

    public class UsePeachAllCondition : ConditionFilter
    {
        public const string DefaultKeyName = "使用桃园结义的条件";

        public UsePeachAllCondition()
        {
            KeyName = DefaultKeyName;
        }

        public override bool Accept(Context ctx)
        {
            return true;
        }
    }

    public class UsePeachAllTargetFilter : PlayerFilter
    {
        public const string DefaultKeyName = "使用桃园结义的目标";

        public UsePeachAllTargetFilter()
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
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return true;
        }
    }
}
