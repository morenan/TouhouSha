using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.AnotherLands
{
    /// <summary>
    /// 角色【四季映姬】
    /// </summary>
    /// <remarks>
    /// HP:3/3 彼岸
    /// 【罪论】一名角色的判定牌生效前，你可以打出一张牌替代之，被替代的牌作为【罪】放置到其角色牌上。
    ///     当你成为其他角色使用的卡的目标时，你将该角色一张牌作为【罪】放置到其角色牌上。
    /// 【审判】当场上有【罪】的角色造成伤害时，你可获得这张【罪】以及该角色的一张牌。
    ///     当场上有【罪】的角色受到伤害时，你可令该角色获得这张【罪】并摸两张牌。
    /// 【阎官】主公技，当场上角色死亡时，你获得其所有的【罪】。
    /// </remarks>
    public class Shiki : Charactor
    {
        public const string CountryNameOfLeader = "彼岸";

        public Shiki()
        {
            MaxHP = 3;
            HP = 3;
            Country = CountryNameOfLeader;
            Skills.Add(new Shiki_Skill_0());
            Skills.Add(new Shiki_Skill_1());
        }

        public override Charactor Clone()
        {
            return new Shiki();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "四季映姬";
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            info.Image = ImageHelper.LoadCardImage("Charactors", "Shiki");
            info.AbilityRadar = new AbilityRadar() { Attack = 2, Defence = 5, Auxiliary = 5, Control = 5, LastStages = 5, Difficulty = 5 };
            return info;
        }
    }
   
    public class Shiki_Skill_0 : Skill
    {
        public override Skill Clone()
        {
            return new Shiki_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Shiki_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "罪论",
                Description = "一名角色的判定牌生效前，你可以打出一张牌替代之，被替代的牌作为【罪】放置到其角色牌上。" +
                   "当你成为其他角色使用的卡的目标时，你将该角色一张牌作为【罪】放置到其角色牌上。"
            };
        }

    }

    public class Shiki_Skill_1 : Skill
    {
        public override Skill Clone()
        {
            return new Shiki_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Shiki_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "审判",
                Description = "当场上有【罪】的角色造成伤害时，你可获得这张【罪】以及该角色的一张牌。" +
                    "当场上有【罪】的角色受到伤害时，你可令该角色获得这张【罪】并摸两张牌。"
            };
        }
    }

    public class Shiki_Skill_2 : Skill
    {
        public override Skill Clone()
        {
            return new Shiki_Skill_2();
        }

        public override Skill Clone(Card newcard)
        {
            return new Shiki_Skill_2();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "阎官",
                Description = "主公技，当场上角色死亡时，你获得其所有的【罪】。"
            };
        }
    }
}
