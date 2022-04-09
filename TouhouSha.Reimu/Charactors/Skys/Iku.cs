using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Skys
{
    public class Iku : Charactor
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
                return new SkillInfo()
                {
                    Name = "龙雷",
                    Description = "当你成为【杀】的目标时，若来源的判定区没有【闪电】，你可以将一张牌作为【闪电】置于其判定区，视为你打出了一张【闪】。" + 
                        "否则，你获得这张【闪电】，并对其造成两点雷电伤害。"
                }; 
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
                return new SkillInfo()
                {
                    Name = "羽衣",
                    Description = "当你受到X点属性伤害前，可以选择你以外的X名角色，你防止这次伤害并摸X张牌，这些角色受到你造成的同属性的一点伤害。"
                };
            }
        }

        public Iku()
        {
            MaxHP = 3;
            HP = 3;
            Country = "天人";
        }

        public override Charactor Clone()
        {
            return new Iku();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "永江衣玖";
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            info.Image = ImageHelper.LoadCardImage("Charactors", "Iku");
            info.AbilityRadar = new AbilityRadar() { Attack = 5, Defence = 4, Auxiliary = 4, Control = 1, LastStages = 4, Difficulty = 5 };
            return info;
        }


    }
}
