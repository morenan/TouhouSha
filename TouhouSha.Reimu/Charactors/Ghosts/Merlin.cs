using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Ghosts
{
    /// <summary>
    /// 角色【梅露兰·普利兹姆利巴】
    /// </summary>
    /// <remarks>
    ///  幽灵 4勾玉
    /// 【管冥】：限定技，当你使用一张牌指定多个目标时，你可以取消X个目标然后选剩余目标中的一个多结算X次（X至多为3）。
    /// 【管灵】：限定技，当你使用一张牌指定唯一目标时，你可以将场上你与该目标以外的其他角色都作为这张卡的目标。
    /// 【合葬】：限定技，你的回合结束时，当你的【管冥】和【管灵】均发动过时发动，你将【管冥】和【管灵】恢复为未使用状态，并令所有你使用牌指定过目标的角色选择一项：
    ///     1. 丢弃X张牌（X为你使用牌指定其目标的次数）。
    ///     2. 将角色牌翻面。
    /// </remarks>
    public class Merlin : Charactor
    {
        public class Skill_0 : Skill
        {
            public override Skill Clone()
            {
                return new Skill_0();
            }

            public override Skill Clone(Card newcard)
            {
                return new Skill_0();
            }

            public override SkillInfo GetInfo()
            {
                SkillInfo info = new SkillInfo()
                {
                    Name = "管冥",
                    Description = "限定技，当你使用一张牌指定多个目标时，你可以取消X个目标然后选剩余目标中的一个多结算X次（X至多为3）。"
                };
                return info;
            }
        }

        public class Skill_1 : Skill
        {
            public override Skill Clone()
            {
                return new Skill_1();
            }

            public override Skill Clone(Card newcard)
            {
                return new Skill_1();
            }

            public override SkillInfo GetInfo()
            {
                SkillInfo info = new SkillInfo()
                {
                    Name = "管灵",
                    Description = "限定技，当你使用一张牌指定唯一目标时，你可以将场上你与该目标以外的其他角色都作为这张卡的目标。"
                };
                return info;
            }
        }

        public class Skill_2 : Skill
        {
            public override Skill Clone()
            {
                return new Skill_2();
            }

            public override Skill Clone(Card newcard)
            {
                return new Skill_2();
            }

            public override SkillInfo GetInfo()
            {
                SkillInfo info = new SkillInfo()
                {
                    Name = "合葬",
                    Description = "限定技，你的回合结束时，当你的【管冥】和【管灵】均发动过时发动，你将【管冥】和【管灵】恢复为未使用状态，并令所有你使用牌指定过目标的角色选择一项："+ 
                        "1. 丢弃X张牌（X为你使用牌指定其目标的次数）。" +
                        "2. 将角色牌翻面。"
                };
                return info;
            }
        }

        public Merlin()
        {
            MaxHP = 4;
            HP = 4;
            Country = Yoyoko.CountryNameOfLeader;
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
            Skills.Add(new Skill_2());
        }

        public override Charactor Clone()
        {
            return new Merlin();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "梅露兰";
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            info.AbilityRadar = new AbilityRadar() { Attack = 5, Defence = 1, Control = 4, Auxiliary = 3, LastStages = 1, Difficulty = 2 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Merlin");
            return info;
        }
    }
}
