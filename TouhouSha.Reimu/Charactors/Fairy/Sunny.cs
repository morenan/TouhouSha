using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;
using TouhouSha.Reimu.Charactors.SelfCrafts;

namespace TouhouSha.Reimu.Charactors.Fairy
{
    public class Sunny : Charactor
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
                    Name = "折光",
                    Description = "出牌阶段，你可以将一张牌当作【桑尼的恶作剧】使用。"
                };
                info.AttachedSkills.Add(new SkillInfo()
                {
                    Name = "桑尼的恶作剧",
                    Description = "延时锦囊牌。放置于一名角色的判定区（不能有同名卡存在）。当其成为一张卡的目标时进行判定，若不为♥，转移给其选择的一个距离1以内的角色（不能是使用者）。结算完毕弃置此牌。"
                });
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
                    Name = "光愈",
                    Description = "场上的【桑尼的恶作剧】生效后，若你已受伤，你回复一点体力。"
                };
                return info;
            }
        }
        public Sunny()
        {
            MaxHP = 3;
            HP = 3;
            Country = Crino.CountryNameOfLeader;
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
        }

        public override Charactor Clone()
        {
            return new Sunny();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "桑尼";
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            info.AbilityRadar = new AbilityRadar() { Attack = 1, Defence = 5, Control = 4, Auxiliary = 5, LastStages = 2, Difficulty = 5 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Sunny");
            return info;
        }
    }
}
