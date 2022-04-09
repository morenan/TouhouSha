using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Calculators;

namespace TouhouSha.Koishi.Cards.Weapons
{
    /// <summary>
    /// 武器【河童机枪】
    /// </summary>
    /// <remarks>
    /// 武器范围：1
    /// 你出牌阶段打出的杀没有数量限制。
    /// </remarks>
    public class AkCard : SelfWeapon
    {
        public const string Normal = "河童机枪";
        
        public AkCard()
        {
            KeyName = Normal;
            Skills.Add(new SkillAk(this));
        }

        public override Card Create()
        {
            return new AkCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "攻击距离1。出牌阶段，你使用【杀】字段没有次数限制。",
                Image = ImageHelper.LoadCardImage("Cards", "Ak")
            };
        }

        public override double GetWorthForTarget()
        {
            return 5;
        }
    }
    
    public class SkillAk : Skill
    {
        public SkillAk(Card _weapon)
        {
            this.weapon = _weapon;
            IsLocked = true;
            Calculators.Add(new WeaponKillRangePlusCalculator(_weapon, 0));
            Calculators.Add(new UnimateKillMaximumCalculator(_weapon));
        }

        private Card weapon;
        public Card Weapon { get { return this.weapon; } }

        public override Skill Clone()
        {
            return new SkillAk(null);
        }

        public override Skill Clone(Card newcard)
        {
            return new SkillAk(newcard);
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo();
        }

    }

    public class UnimateKillMaximumCalculator : CalculatorFromCard, ICalculatorProperty
    {
        string ICalculatorProperty.PropertyName { get => KillCard.KillMaxUse; }

        public UnimateKillMaximumCalculator(Card _weapon) : base(_weapon)
        {
        }
        
        public override int GetValue(Context ctx, ShaObject obj, string propertyname, int oldvalue)
        {
            if (card == null) return oldvalue;
            switch (propertyname)
            {
                case KillCard.KillMaxUse:
                    if (card.Zone?.KeyName?.Equals(Zone.Equips) != true) return oldvalue;
                    if (!(obj is State)) return oldvalue;
                    if (card.Zone.Owner != ((State)obj).Owner) return oldvalue;
                    if (!card.Zone.Cards.Contains(card)) return oldvalue;
                    return 9999;
            }
            return oldvalue;
        }
    }

    
}
