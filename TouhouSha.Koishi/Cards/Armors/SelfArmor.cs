using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.AIs;
using TouhouSha.Koishi.AIs;

namespace TouhouSha.Koishi.Cards.Armors
{
    public abstract class SelfArmor : Card
    {
        public SelfArmor()
        {
            UseCondition = new SelfArmorUseCondition(this);
            TargetFilter = new SelfArmorTargetFilter(this);
            CardType = new CardType(Enum_CardType.Equip, new CardSubType(Enum_CardSubType.Armor));
        }

        public override CardTargetFilterWorth GetWorthAI()
        {
            return new UseSelfArmorWorthAI(this);
        }
    }

    public class SelfArmorUseCondition : ConditionFilterFromCard
    {
        public SelfArmorUseCondition(Card _card) : base(_card)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner == null) return false;
            EquipZone equipzone = state.Owner.Zones.FirstOrDefault(_zone => _zone is EquipZone) as EquipZone;
            if (equipzone == null) return false;
            EquipCell armorcell = equipzone.Cells.FirstOrDefault(_cell => _cell.E == Enum_CardSubType.Armor);
            if (armorcell == null) return false;
            if (!armorcell.IsEnabled) return false;
            return true;
        }
    }
    
    public class SelfArmorTargetFilter : PlayerFilterFromCard
    {
        public SelfArmorTargetFilter(Card _card) : base(_card)
        {

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

    public class UseSelfArmorWorthAI : CardWorthSumAI
    {
        public UseSelfArmorWorthAI(Card _thiscard) : base(_thiscard)
        {

        }

        public override double GetUseWorth(Context ctx, Card card, Player user, PlayerFilter targetfilter)
        {
            return GetWorth(ctx, card, user, new Player[] { }, user);
        }

        public override double GetWorth(Context ctx, Card card, Player user, IEnumerable<Player> selecteds, Player want)
        {
            double worth = 0;
            EquipZone equipzone = user.Zones.FirstOrDefault(_zone => _zone is EquipZone) as EquipZone;
            EquipCell armorcell = equipzone?.Cells?.FirstOrDefault(_cell => _cell.E == Enum_CardSubType.Armor);
            Card armor = null;
            if (armorcell == null || !armorcell.IsEnabled) return 0;
            if (armorcell.CardIndex >= 0 && armorcell.CardIndex < equipzone.Cards.Count())
                armor = equipzone.Cards[armorcell.CardIndex];
            Zone oldzone = card.Zone;
            card.Zone = equipzone;
            worth += AITools.WorthAcc.GetWorth(ctx, user, card);
            if (armor != null) worth -= AITools.WorthAcc.GetWorth(ctx, user, armor);
            card.Zone = oldzone;
            return worth;
        }
    }


}
