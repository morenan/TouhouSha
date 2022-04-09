using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Desires
{
    public class Seiga : Charactor
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
                    Name = "隧壁",
                    Description = "出牌阶段限一次，你选择你的上家或者下家，你对其造成一点伤害，并交换你与其的座次。"
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
                    Name = "尸化",
                    Description = "限定技，一名角色的濒死阶段才能发动，你将其体力上限调整为10，体力恢复至体力上限，并获得技能【崩坏】。"
                };
                info.AttachedSkills.Add(new SkillInfo()
                {
                    Name = "崩坏",
                    Description = "锁定技，你不能恢复体力。你的回合结束，若你体力上限是全场最大的，你选择一项：1.失去一点体力。2.失去一点体力上限。"
                });
                return info;
            }
        }

        public Seiga()
        {
            MaxHP = 4;
            HP = 4;
            Country = Miko.CountryNameOfLeader;
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
        }

        public override Charactor Clone()
        {
            return new Seiga();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "霍青娥";
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            info.AbilityRadar = new AbilityRadar() { Attack = 3, Defence = 5, Control = 4, Auxiliary = 5, LastStages = 3, Difficulty = 5 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Seiga");
            return info;
        }


    }
}
