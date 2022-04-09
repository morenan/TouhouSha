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
    public class Star : Charactor
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
                    Name = "感知",
                    Description = "出牌阶段，你可以将一张牌当作【斯塔的恶作剧】使用。"
                };
                info.AttachedSkills.Add(new SkillInfo()
                {
                    Name = "斯塔的恶作剧",
                    Description = "延时锦囊牌。放置于一名角色的判定区（不能有同名卡存在）。该角色发动技能（非锁定，限定和觉醒技）时进行判定，若不为♠，该技能发动无效。结算完毕弃置此牌。"
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
                    Name = "协同",
                    Description = "场上角色使用转化的延时锦囊牌时，你可令其摸一张牌。"
                };
                return info;
            }
        }
        public Star()
        {
            MaxHP = 3;
            HP = 3;
            Country = Crino.CountryNameOfLeader;
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
        }

        public override Charactor Clone()
        {
            return new Luna();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "斯塔";
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            info.AbilityRadar = new AbilityRadar() { Attack = 1, Defence = 2, Control = 5, Auxiliary = 5, LastStages = 1, Difficulty = 4 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Star");
            return info;
        }
    }
}
