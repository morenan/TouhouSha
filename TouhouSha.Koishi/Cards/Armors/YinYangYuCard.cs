using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.UIs;

namespace TouhouSha.Koishi.Cards.Armors
{
    /// <summary>
    /// 防具【阴阳玉】
    /// </summary>
    /// <remarks>
    /// 当你成为【杀】的目标时，进行一次判定，结果为红视为使用了一张【闪】。
    /// </remarks>
    public class YinYangYuCard : SelfArmor
    {
        public const string DefaultKeyName = "阴阳玉";

        public YinYangYuCard()
        {
            KeyName = DefaultKeyName;
            Skills.Add(new WishMask_Skill(this));
        }
        public override Card Create()
        {
            return new YinYangYuCard();
        }

        public override CardInfo GetInfo()
        {
            return new CardInfo()
            {
                Name = KeyName,
                Description = "当你成为【杀】的目标时，进行一次判定，结果为红视为使用了一张【闪】。",
                Image = ImageHelper.LoadCardImage("Cards", "YinYangYu")
            };
        }


        public override double GetWorthForTarget()
        {
            return 3;
        }
    }
}
