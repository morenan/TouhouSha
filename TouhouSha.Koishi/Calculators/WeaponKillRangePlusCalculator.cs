using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;

namespace TouhouSha.Koishi.Calculators
{
    public class WeaponKillRangePlusCalculator : Calculator, ICalculatorProperty
    {
        string ICalculatorProperty.PropertyName { get => World.KillRange; }

        public WeaponKillRangePlusCalculator(Card _weapon, int _killrange)
        {
            this.weapon = _weapon;
            this.killrange = _killrange;
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        private int killrange;
        public int KillRange { get { return this.killrange; } }

        public override int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
        {
            if (weapon == null) return oldvalue;
            switch (propertyname)
            {
                case World.KillRange:
                    if (weapon.Zone?.KeyName?.Equals(Zone.Equips) != true) return oldvalue;
                    if (weapon.Zone.Owner != obj) return oldvalue;
                    if (!weapon.Zone.Cards.Contains(weapon)) return oldvalue;
                    return oldvalue + killrange;
            }
            return oldvalue;
        }

    }
}
