using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Chirendens
{
    /// <summary>
    /// 角色【火焰猫燐】
    /// </summary>
    /// <remarks>
    /// HP:4/4 地灵
    /// 【收尸】当场上其他角色死亡时，你可以获得其所有的牌，增加一点体力上限并回复一点体力。
    /// </remarks>
    public class NikoRin : Charactor
    {
        public NikoRin()
        {
            MaxHP = 3;
            HP = 3;
            Country = Satori.CountryNameOfLeader;
        }

        public override Charactor Clone()
        {
            return new NikoRin();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "火焰猫燐";
            info.Skills.Add(new SkillInfo()
            {
                Name = "收尸",
                Description = "当场上其他角色死亡时，你可以获得其所有的牌，增加一点体力上限并回复一点体力。"
            });
            info.Skills.Add(new SkillInfo()
            {
                Name = "怨灵",
                Description = "当你成为点数不超过你的手牌的卡的目标时，你可以取消对你的结算。"
            });
            info.AbilityRadar = new AbilityRadar() { Attack = 1, Defence = 2, Control = 1, Auxiliary = 1, LastStages = 4, Difficulty = 5 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "NikoRin");
            return info;
        }
    }
}
