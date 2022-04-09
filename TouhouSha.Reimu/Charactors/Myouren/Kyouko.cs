using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Myouren
{
    public class Kyouko : Charactor
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
                    Name = "回音",
                    Description = "你使用的牌到结算完毕后，如果有牌响应或者使用了【无懈可击】，你可以摸那个数量+1的牌。"
                        + "你打出牌或者使用【无懈可击】时，你可以摸一张牌。"
                };
                return info;
            }
        }
        public Kyouko()
        {
            MaxHP = 4;
            HP = 4;
            Country = Hiziri.CountryNameOfLeader;
            Skills.Add(new Skill_0());
        }

        public override Charactor Clone()
        {
            return new Kyouko();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "幽谷响子";
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            info.AbilityRadar = new AbilityRadar() { Attack = 2, Defence = 4, Control = 2, Auxiliary = 2, LastStages = 2, Difficulty = 5 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Kyouko");
            return info;
        }
    }
}
