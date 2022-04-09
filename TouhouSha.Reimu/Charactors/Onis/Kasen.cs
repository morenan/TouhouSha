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
    /// 角色【茨木华扇】
    /// </summary>
    /// <remarks>
    /// HP:3/3 鬼族
    /// 【右臂】转换技，出牌阶段限一次：
    ///     阳：你失去技能【驭龙】和【雷灵】，并获得技能【鬼王】和【邪智】。
    ///     阴：你失去技能【鬼王】和【邪智】，并获得技能【驭龙】和【雷灵】。
    /// 【驭龙】你可以将你的红色牌当作【杀】来使用或打出，你使用或者打出♥【杀】时或者成为♥【杀】的目标时，你可以选择摸3-X张牌（X为你本回合发动【驭龙】的次数且至多为2）。你使用♦【杀】无距离限制且不可闪避。
    /// 【雷灵】你可以将你的黑色牌当作【闪】来使用或打出，你打出黑色【闪】抵消【杀】或者【万剑齐发】后，结算完毕后根据【闪】的花色发动以下效果：
    ///     ♠：你对【杀】或者【万剑齐发】使用者造成两点雷电伤害。
    ///     ♣：你对【杀】或者【万剑齐发】使用者造成一点雷电伤害，你回复一点体力。
    /// 【鬼王】锁定技，你的锦囊牌均视作【酒】，你出牌阶段使用【酒】没有次数限制。
    /// 【邪智】出牌阶段限一次，你可以将两张【酒】当作任意非延时锦囊牌来使用。
    ///     如果这两张【酒】的花色相同，结算前你可以选择摸两张牌。
    ///     如果不同，这张牌你可以额外指定一个原本不包含的合理的目标。
    /// 【华宴】主公技，当你需要使用或打出【酒】时，你可以令其他鬼族势力角色选择是否打出一张【酒】。濒死阶段使用此【酒】额外回复一点体力。
    /// </remarks>
    public class Kasen : Charactor
    {
        public const string CountryNameOfLeader = "鬼族";

        public Kasen()
        {
            MaxHP = 3;
            HP = 3;
            Country = CountryNameOfLeader;
        }

        public override Charactor Clone()
        {
            return new Kasen();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore core = new CharactorInfoCore(this);
            core.Name = "茨木华扇";
            SkillInfo skill0 = new SkillInfo()
            {
                Name = "右臂",
                Description = "转换技，出牌阶段限一次：\n"
                   + "\t阳：你失去技能【驭龙】和【雷灵】，并获得技能【鬼王】和【邪智】。\n"
                   + "\t阴：你失去技能【鬼王】和【邪智】，并获得技能【驭龙】和【雷灵】。"
            };
            skill0.AttachedSkills.Add(new SkillInfo()
            {
                Name = "鬼王",
                Description = "锁定技，你的锦囊牌均视作【酒】，你出牌阶段使用【酒】没有次数限制。"
            });
            skill0.AttachedSkills.Add(new SkillInfo()
            {
                Name = "邪智",
                Description = "出牌阶段限一次，你可以将两张【酒】当作任意非延时锦囊牌来使用。"
                    + "如果这两张【酒】的花色相同，结算前你可以选择摸两张牌。"
                    + "如果不同，这张牌你可以额外指定一个原本不包含的合理的目标。"
            });
            core.Skills.Add(skill0);
            core.Skills.Add(new SkillInfo()
            {
                Name = "驭龙",
                Description = "你可以将你的红色牌当作【杀】来使用或打出，"
                    + "你使用或者打出♥【杀】时或者成为♥【杀】的目标时，你可以选择摸3-X张牌（X为你本回合发动【驭龙】的次数且至多为2）。"
                    + "你使用♦【杀】无距离限制且不可闪避。"
            });
            core.Skills.Add(new SkillInfo()
            {
                Name = "雷灵",
                Description = "你可以将你的黑色牌当作【闪】来使用或打出，你打出黑色【闪】抵消【杀】或者【万剑齐发】后，结算完毕后根据【闪】的花色发动以下效果："
                    + "♠：你对【杀】或者【万剑齐发】使用者造成两点雷电伤害。"
                    + "♣：你对【杀】或者【万剑齐发】使用者造成一点雷电伤害，你回复一点体力。"
            });
            core.Skills.Add(new SkillInfo()
            {
                Name = "华宴",
                Description = "主公技，当你需要使用或打出【酒】时，你可以令其他鬼族势力角色选择是否打出一张【酒】。濒死阶段使用此【酒】额外回复一点体力。"
            });
            core.AbilityRadar = new AbilityRadar() { Attack = 4, Defence = 4, Control = 3, Auxiliary = 1, LastStages = 4, Difficulty = 3 };
            core.Image = ImageHelper.LoadCardImage("Charactors", "Kasen");
            return core;
        }

    }
}
