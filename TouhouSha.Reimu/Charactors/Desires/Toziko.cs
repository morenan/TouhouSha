using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;
using TouhouSha.Reimu.Charactors.Ghosts;

namespace TouhouSha.Reimu.Charactors.Desires
{
    public class Toziko : Charactor
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
                    Name = "龙卷",
                    Description = "游戏开始时，你获得一个【旋风】标记。你的回合开始阶段，你可以令场上的【旋风】标记移动到上家或者下家，并对拥有【旋风】标记的角色和其距离为1以内的你以外的角色结算：" + 
                        "若该角色没有被横置，横置之，否则你弃置其一张牌。"
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
                    Name = "鹿雷",
                    Description = "你可以将一张黑色牌作为【雷攻】使用。你的【雷攻】可以额外指定两个目标。"
                };
                info.AttachedSkills.Add(new SkillInfo()
                {
                    Name = "雷攻",
                    Description = "出牌阶段对一名有手牌的角色为对象发动。目标展示一张手牌，可以选择丢弃一张和其点数差不超过1的手牌，对目标造成一点雷电伤害。" +
                    "若丢弃的牌的点数是A或者K，此伤害+1。"
                });
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
                    Name = "释怀",
                    Description = "限定技，当你进入濒死状态时，你可以选择增加一点体力上限，回复一点体力并摸两张牌，将【龙卷】修改为不移动也能发动后续效果。"
                };
                return info;
            }
        }

        public class Skill_3 : Skill
        {
            public override Skill Clone()
            {
                return new Skill_3();
            }

            public override Skill Clone(Card newcard)
            {
                return new Skill_3();
            }

            public override SkillInfo GetInfo()
            {
                SkillInfo info = new SkillInfo()
                {
                    Name = "神裔",
                    Description = "限定技，当你进入濒死状态时，你可以选择增加一点体力上限，回复一点体力并摸两张牌，将【龙卷】中“你弃置其一张牌”改为“你弃置其至多两张牌”。"
                };
                return info;
            }
        }

        public class Skill_4 : Skill
        {
            public override Skill Clone()
            {
                return new Skill_4();
            }

            public override Skill Clone(Card newcard)
            {
                return new Skill_4();
            }

            public override SkillInfo GetInfo()
            {
                SkillInfo info = new SkillInfo()
                {
                    Name = "崇佛",
                    Description = "限定技，当你进入濒死状态时，你可以选择增加一点体力上限，回复一点体力并摸两张牌，本局游戏【龙卷】每结算完一个目标你摸一张牌。"
                };
                return info;
            }
        }

        public Toziko()
        {
            MaxHP = 1;
            HP = 1;
            Country = Miko.CountryNameOfLeader;
            OtherCountries.Add(Yoyoko.CountryNameOfLeader);
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
            Skills.Add(new Skill_2());
            Skills.Add(new Skill_3());
            Skills.Add(new Skill_4());
        }

        public override Charactor Clone()
        {
            return new Futo();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "苏我屠自古";
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            info.AbilityRadar = new AbilityRadar() { Attack = 4, Defence = 4, Control = 4, Auxiliary = 3, LastStages = 4, Difficulty = 2 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Toziko");
            return info;
        }

    }
}
