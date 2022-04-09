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
    public class Luna : Charactor
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
                    Name = "消音",
                    Description = "出牌阶段，你可以将一张牌当作【露娜的恶作剧】使用。"
                };
                info.AttachedSkills.Add(new SkillInfo()
                {
                    Name = "露娜的恶作剧",
                    Description = "延时锦囊牌。放置于一名角色的判定区（不能有同名卡存在）。判定阶段进行判定，若不为♦，本回合你的牌均视为无花色，无种类和无类型。结算完毕弃置此牌。"
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
                    Name = "迟钝",
                    Description = "锁定技，当你于回合外受到伤害后，本回合【杀】和普通锦囊牌对你无效。"
                };
                return info;
            }
        }
        public Luna()
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
            info.Name = "露娜";
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            info.AbilityRadar = new AbilityRadar() { Attack = 1, Defence = 3, Control = 4, Auxiliary = 4, LastStages = 1, Difficulty = 2 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Luna");
            return info;
        }

    }
}
