using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;

namespace TouhouSha.Koishi.Cards.Armors
{
    public abstract class GiftArmor : Card
    {
        public GiftArmor()
        {
            UseCondition = new GiftArmorUseCondition(this);
            TargetFilter = new GiftArmorTargetFilter(this);
            CardType = new CardType(Enum_CardType.Equip, new CardSubType(Enum_CardSubType.Armor));
        }
    }

    public class GiftArmorUseCondition : ConditionFilterFromCard
    {
        public GiftArmorUseCondition(Card _card) : base(_card)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            Player source = state.Owner;
            if (source == null) return false;
            return true;
        }
    }

    public class GiftArmorTargetFilter : PlayerFilterFromCard
    {
        public GiftArmorTargetFilter(Card _card) : base(_card)
        {

        }
        
        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            if (selecteds.Count() > 0) return false;
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            Player source = state.Owner;
            if (source == null) return false;
            if (!ctx.World.IsInDistance(ctx, source, want)) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() > 0;
        }
    }
}
