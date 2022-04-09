using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Calculators;

namespace TouhouSha.Koishi.Cards.Weapons
{
    /// <summary>
    /// 武器【死神镰刀】
    /// </summary>
    /// <remarks>
    /// 武器范围：2
    /// 你对手牌为0的角色打出的杀造成的伤害+1。
    /// </remarks>
    public class SickleCard : SelfWeapon
    {
        public const string DefaultKeyName = "死神镰刀";
        
        public SickleCard()
        {
            KeyName = DefaultKeyName;
        }

        public override Card Create()
        {
            return new SickleCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "攻击距离2。你对手牌为0的角色打出的杀造成的伤害+1。",
                Image = ImageHelper.LoadCardImage("Cards", "Sickle")
            };
        }
        public override double GetWorthForTarget()
        {
            return 3;
        }

    }
}
