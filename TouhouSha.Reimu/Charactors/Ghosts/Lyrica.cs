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
    /// 角色【莉莉卡·普利兹姆利巴】
    /// </summary>
    /// <remarks>
    ///  幽灵 3勾玉
    /// 【键灵】：限定技，你的回合结束时，若你本回合使用或打出的牌的花色超过两张且均相同时，你摸X张牌
    ///     （X为你本回合使用或打出的牌的数量）。
    ///     此时若你的【键冥】已经发动过，恢复到未发动的状态。
    /// 【键冥】：限定技，你的回合结束时，若你本回合使用或打出的牌的花色超过两张且均不同时，你弃置场上任意名角色至多X张牌
    ///     （X为你本回合使用或打出的牌的数量）。
    ///     此时若你的【键灵】已经发动过，恢复到未发动的状态。
    /// 【独奏】：限定技，出牌阶段可以发动，直到回合结束前，将【键灵】【键冥】视为非限定技并进行修改：
    ///     【键灵】若你使用或打出的牌的花色和上一张使用或打出的牌相同时，你摸两张牌。
    ///     【键冥】若你使用或打出的牌的花色和上一张使用或打出的牌不同时，你弃置场上一名角色一张牌。
    /// </remarks>
    public class Lyrica : Charactor
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
                    Name = "键灵",
                    Description = "限定技，你的回合结束时，若你本回合使用或打出的牌的花色超过两张且均相同时，你摸X张牌" +
                        "（X为你本回合使用或打出的牌的数量）。" +
                        "此时若你的【键冥】已经发动过，将【键冥】恢复到未发动的状态。"
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
                    Name = "键冥",
                    Description = "限定技，你的回合结束时，若你本回合使用或打出的牌的花色超过两张且均不同时，你弃置场上任意名角色至多X张牌" +
                        "（X为你本回合使用或打出的牌的数量）。" +
                        "此时若你的【键灵】已经发动过，将【键灵】恢复到未发动的状态。"
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
                    Name = "独奏",
                    Description = "限定技，出牌阶段可以发动，直到回合结束前，将【键灵】【键冥】视为非限定技并进行修改："
                };
                info.AttachedSkills.Add(new SkillInfo()
                {
                    Name = "键灵",
                    Description = "若你使用或打出的牌的花色和上一张使用或打出的牌相同时，你摸两张牌。"
                });
                info.AttachedSkills.Add(new SkillInfo()
                {
                    Name = "键冥",
                    Description = "若你使用或打出的牌的花色和上一张使用或打出的牌不同时，你弃置场上一名角色一张牌。"
                });
                return info;
            }
        }

        public Lyrica()
        {
            MaxHP = 3;
            HP = 3;
            Country = Yoyoko.CountryNameOfLeader;
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
            Skills.Add(new Skill_2());
        }

        public override Charactor Clone()
        {
            return new Lyrica();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "莉莉卡";
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            info.AbilityRadar = new AbilityRadar() { Attack = 3, Defence = 2, Control = 4, Auxiliary = 3, LastStages = 1, Difficulty = 1 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Lyrica");
            return info;
        }
    }
}
