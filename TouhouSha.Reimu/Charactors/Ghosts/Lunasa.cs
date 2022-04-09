using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Ghosts
{
    /// <summary>
    /// 角色【露娜萨·普利兹姆利巴】
    /// </summary>
    /// <remarks>
    ///  幽灵 4勾玉
    /// 【神弦】：限定技，你的回合结束时，若你本回合使用或打出的牌的点数严格递增时可以发动，
    ///     你摸X张牌并令场上至多X名已受伤角色回复一点体力。
    /// 【伪弦】：限定技，你的回合结束时，若你本回合使用或打出的牌的点数严格递减时可以发动，
    ///     你摸X张牌并令场上至多X名角色失去一点体力。
    /// 【奏弦】：限定技，你的回合结束时，若你本回合使用或打出的牌的点数交替增减时可以发动，
    ///     你摸X张牌并弃置X张牌，然后令场上至多X名角色失去一点体力，其下个回合开始时回复一点体力。
    /// </remarks>
    public class Lunasa : Charactor
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
                    Name = "神弦",
                    Description = "限定技，你的回合结束时，若你本回合使用或打出的牌的点数严格递增时可以发动，" + 
                        "你摸X张牌并令场上至多X名已受伤角色回复一点体力。"
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
                    Name = "伪弦",
                    Description = "限定技，你的回合结束时，若你本回合使用或打出的牌的点数严格递减时可以发动，" +
                        "你摸X张牌并令场上至多X名角色失去一点体力。"
                };
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
                    Name = "奏弦",
                    Description = "限定技，你的回合结束时，若你本回合使用或打出的牌的点数交替增减时可以发动，" +
                        "你摸X张牌并弃置X张牌，然后令场上至多X名角色失去一点体力，其下个回合开始时回复一点体力。"
                };
                return info;
            }
        }

        public Lunasa()
        {
            MaxHP = 4;
            HP = 4;
            Country = Yoyoko.CountryNameOfLeader;
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
            Skills.Add(new Skill_2());
        }

        public override Charactor Clone()
        {
            return new Lunasa();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "露娜萨";
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            info.AbilityRadar = new AbilityRadar() { Attack = 3, Defence = 3, Control = 3, Auxiliary = 4, LastStages = 1, Difficulty = 1 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Lunasa");
            return info;
        }
    }
}
