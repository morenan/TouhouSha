using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Onis
{
    /// <summary>
    /// 角色【伊吹萃香】
    /// </summary>
    /// <remarks>
    /// HP:4/4 鬼族
    /// 【鬼王】锁定技，你的锦囊牌均视作【酒】，你出牌阶段使用【酒】没有次数限制。
    /// 【酒宴】出牌阶段限一次，你可以使用一张手牌与场上至多四名其他角色同时进行拼点，你每赢一次拼点，视作你使用了一张【酒】。
    /// 【碎月】当你使用【酒】影响的【杀】被【闪】抵消时，你弃置目标至多2张牌。
    /// </remarks>
    public class Suika : Charactor
    {
        public Suika()
        {
            MaxHP = 4;
            HP = 4;
            Country = Kasen.CountryNameOfLeader;
        }

        public override Charactor Clone()
        {
            return new Suika();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "伊吹萃香";
            info.Skills.Add(new SkillInfo()
            {
                Name = "鬼王",
                Description = "锁定技，你的锦囊牌均视作【酒】，你出牌阶段使用【酒】没有次数限制。"
            });
            info.Skills.Add(new SkillInfo()
            {
                Name = "酒宴",
                Description = "出牌阶段限一次，你可以使用一张手牌与至多三名其他角色同时进行拼点，你每赢一次拼点，视作你使用了一张【酒】。"
            });
            info.Skills.Add(new SkillInfo()
            {
                Name = "碎月",
                Description = "当你使用【酒】影响的【杀】被【闪】抵消时，你弃置目标至多2张牌。"
            });
            info.AbilityRadar = new AbilityRadar() { Attack = 5, Defence = 3, Control = 3, Auxiliary = 1, LastStages = 3, Difficulty = 4 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Suika");
            return info;
        }

    }
}
