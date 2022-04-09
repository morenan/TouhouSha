using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.AIs;
using TouhouSha.Koishi.AIs;
using TouhouSha.Koishi.Calculators;

namespace TouhouSha.Koishi.Cards.Weapons
{
    public abstract class SelfWeapon : Card
    {
        public SelfWeapon()
        {
            UseCondition = new SelfWeaponCondition(this);
            TargetFilter = new SelfWeaponTargetFilter(this);
            CardType = new CardType(Enum_CardType.Equip, new CardSubType(Enum_CardSubType.Weapon));
        }

        public virtual int GetWeaponRange()
        {
            foreach (Skill skill in Skills)
                foreach (Calculator calc in skill.Calculators)
                {
                    if (!(calc is WeaponKillRangePlusCalculator)) continue;
                    WeaponKillRangePlusCalculator rangecalc = (WeaponKillRangePlusCalculator)calc;
                    return rangecalc.KillRange + 1;
                }
            return 1;
        }
        
        public override double GetWorthForTarget()
        {
            return GetWeaponRange();
        }

        public override CardTargetFilterWorth GetWorthAI()
        {
            return new UseSelfWeaponWorthAI(this);
        }
    }

    public class SelfWeaponCondition : ConditionFilterFromCard
    {
        public SelfWeaponCondition(Card _card) : base(_card)
        {

        }

        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return false;
            if (state.Owner == null) return false;
            EquipZone equipzone = state.Owner.Zones.FirstOrDefault(_zone => _zone is EquipZone) as EquipZone;
            if (equipzone == null) return false;
            EquipCell weaponcell = equipzone.Cells.FirstOrDefault(_cell => _cell.E == Enum_CardSubType.Weapon);
            if (weaponcell == null) return false;
            if (!weaponcell.IsEnabled) return false;
            return true;
        }
    }

    public class SelfWeaponTargetFilter : PlayerFilterFromCard
    {
        public SelfWeaponTargetFilter(Card _card) : base(_card)
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

    public class UseSelfWeaponWorthAI : CardWorthSumAI
    {
        public UseSelfWeaponWorthAI(Card _thiscard) : base(_thiscard)
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
            EquipCell weaponcell = equipzone?.Cells?.FirstOrDefault(_cell => _cell.E == Enum_CardSubType.Weapon);
            Card weapon = null;
            if (weaponcell == null || !weaponcell.IsEnabled) return 0;
            if (weaponcell.CardIndex >= 0 && weaponcell.CardIndex < equipzone.Cards.Count())
                weapon = equipzone.Cards[weaponcell.CardIndex];
            Zone oldzone = card.Zone;
            card.Zone = equipzone;
            worth += AITools.WorthAcc.GetWorth(ctx, user, card);
            if (weapon != null) worth -= AITools.WorthAcc.GetWorth(ctx, user, weapon);
            card.Zone = oldzone;
            return worth;
        }
    }
}
