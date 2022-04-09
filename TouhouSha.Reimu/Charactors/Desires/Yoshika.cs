using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Desires
{
    public class Yoshika : Charactor
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
                    Name = "崩坏",
                    Description = "锁定技，你不能恢复体力。你的回合结束，若你体力上限是全场最大的，你选择一项：1.失去一点体力。2.失去一点体力上限。"
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
                    Name = "毒爪",
                    Description = "你对其他角色造成伤害后，你可以减少两点体力上限，令其获得技能【崩坏】直到其下一个回合结束。"
                };
                return info;
            }
        }
        public Yoshika()
        {
            MaxHP = 10;
            HP = 10;
            Country = Miko.CountryNameOfLeader;
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
        }

        public override Charactor Clone()
        {
            return new Yoshika();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "宫古芳香";
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            info.AbilityRadar = new AbilityRadar() { Attack = 3, Defence = 5, Control = 3, Auxiliary = 4, LastStages = 3, Difficulty = 4 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Yoshika");
            return info;
        }
    }
}
