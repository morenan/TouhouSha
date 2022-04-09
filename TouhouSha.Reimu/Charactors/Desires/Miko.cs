using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Desires
{
    public class Miko : Charactor
    {
        public const string CountryNameOfLeader = "神灵";

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
                    Name = "禅听",
                    Description = "每回合限一次，当一张牌指定目标时，若你手牌没有同类型的牌，你摸X张牌（X为目标的数量）。",
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
                    Name = "道政",
                    Description = "当一张牌指定目标时，你丢弃一张同类型手牌并选择一项：1. 为此牌额外指定一个合理的目标。2. 取消一个目标。",
                };
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
                return new SkillInfo()
                {
                    Name = "圣德",
                    Description = "主公技，场上神灵角色的出牌阶段你可以丢弃一张手牌，令其获得技能【奇才】直到回合结束。",
                };
            }
        }

        public Miko()
        {
            MaxHP = 3;
            HP = 3;
            Country = CountryNameOfLeader;
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
            Skills.Add(new Skill_2());
        }

        public override Charactor Clone()
        {
            return new Miko();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "丰聪耳神子";
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            info.AbilityRadar = new AbilityRadar() { Attack = 3, Defence = 5, Control = 5, Auxiliary = 5, LastStages = 4, Difficulty = 4 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Miko");
            return info;
        }
    }
}
