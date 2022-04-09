using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;
using TouhouSha.Reimu.Charactors.Onis;

namespace TouhouSha.Reimu.Charactors.Chirendens
{
    /// <summary>
    /// 角色【星熊勇仪】
    /// </summary>
    /// <remarks>
    /// HP:4/4 地灵（可以替换为鬼族）
    /// 【鬼王】锁定技，你的锦囊牌均视作【酒】，你出牌阶段使用【酒】没有次数限制。
    /// 【怪力】摸牌阶段开始时，你可亮出牌堆顶的三张牌，然后你可以获得其中的【杀】和装备牌，剩余的牌当作【酒】来使用。若如此做，你放弃摸牌。
    /// 【乱神】当你使用【酒】影响的【杀】被【闪】抵消时，你将目标翻面。
    /// </remarks>
    public class Yugi : Charactor
    {
        public Yugi()
        {
            MaxHP = 4;
            HP = 4;
            Country = Satori.CountryNameOfLeader;
            OtherCountries.Add(Kasen.CountryNameOfLeader);
        }

        public override Charactor Clone()
        {
            return new Yugi();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "星熊勇仪";
            info.Skills.Add(new SkillInfo()
            {
                Name = "鬼王",
                Description = "锁定技，你的锦囊牌均视作【酒】，你出牌阶段使用【酒】没有次数限制。"
            });
            info.Skills.Add(new SkillInfo()
            {
                Name = "怪力",
                Description = "摸牌阶段开始时，你可亮出牌堆顶的三张牌，然后你可以获得其中的【杀】和装备牌，剩余的牌当作【酒】来使用。若如此做，你放弃摸牌。"
            });
            info.Skills.Add(new SkillInfo()
            {
                Name = "乱神",
                Description = "当你使用【酒】影响的【杀】被【闪】抵消时，你将目标翻面。"
            });
            info.AbilityRadar = new AbilityRadar() { Attack = 5, Defence = 3, Control = 5, Auxiliary = 1, LastStages = 3, Difficulty = 4 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Yugi");
            return info;
        }
    }
}
