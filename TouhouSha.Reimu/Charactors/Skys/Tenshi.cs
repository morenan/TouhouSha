using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Skys
{
    public class Tenshi : Charactor
    {
        public Tenshi()
        {
            MaxHP = 3;
            HP = 3;
            Country = "天人";
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
        }

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
                    Name = "洞察",
                    Description = "游戏开始时，你将牌堆顶一张牌盖于你的角色牌上，称作【气质】。" + 
                    "你成为其他角色使用的卡的目标时，你可以将【气质】加入手卡并展示，" +
                    "如果展示的牌与使用的牌的类型一致，则取消对你的结算。" +
                    "之后你洗切手卡，将一张手卡作为【气质】盖于你的角色牌上。"
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
                    Name = "地震",
                    Description = "你的回合结束时或者当你受到伤害时，你对场上其他所有角色发动。" +
                        "若目标判定区有牌，其将判定区随机一张牌交给你。" +
                        "若目标装备区有牌，其将装备区随机一张牌交给距离最近的随机一名角色，并受到你的一点伤害。" +
                        "若目标手牌区有牌，其将随机一张手牌交给距离最近的随机一名角色。" + 
                        "如果你以此法获得的牌首次到达两张，你回复一点体力。"
                };
            }
        }

        public override Charactor Clone()
        {
            return new Tenshi();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "比那名居天子";
            info.Image = ImageHelper.LoadCardImage("Charactors", "Tenshi");
            info.AbilityRadar = new AbilityRadar() { Attack = 1, Defence = 5, Auxiliary = 3, Control = 4, LastStages = 5, Difficulty = 2 };
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            return info;
        }

    }
}
