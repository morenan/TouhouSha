using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Tsukumo
{
    public class Kokoro : Charactor
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
                    Name = "面具",
                    Description = "锁定技，你防具栏的卡视作以下防具。" +
                        "你装备过防具后，你下一个装备的防具不能和该防具同花色。" +
                        "当你防具栏没有防具并且满足上述条件时，必须用手牌当作以下防具来装备。"
                };
                info.AttachedSkills.Add(new SkillInfo()
                {
                    Name = "福神",
                    Description = "花色♥的牌视为此防具。当你回复体力时，你选择场上另一名已受伤角色，其回复等量的体力。结算完毕，你将此牌加入手卡。",
                });
                info.AttachedSkills.Add(new SkillInfo()
                {
                    Name = "火男",
                    Description = "花色♦的牌视为此防具。当你摸牌时，你选择场上另一名角色，其摸等量的牌。结算完毕，你将此牌加入手卡。",
                });
                info.AttachedSkills.Add(new SkillInfo()
                {
                    Name = "般若",
                    Description = "花色♠的牌视为此防具。当你受到伤害时，你选择场上另一名角色，其受到同内容的伤害。结算完毕，你将此牌加入手卡。",
                });
                info.AttachedSkills.Add(new SkillInfo()
                {
                    Name = "大飞出",
                    Description = "花色♣的牌视为此防具。当你以使用或者打出以外的方式弃置牌时，你选择场上另一名角色，你弃置其等量的牌。结算完毕，你将此牌加入手卡。",
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
                    Name = "亡失",
                    Description = "出牌阶段限一次，你选择另一名角色，你弃置你与其装备栏任意不同花色的装备。根据弃置的你的装备的数量发动以下效果：" +
                        "0: 你受到其造成的一点伤害，并结束出牌阶段。" +
                        "1: 你摸一张牌。" +
                        "2: 你回复一点体力。" +
                        "3或者以上: 你摸四张牌。"
                };
                return info;
            }
        }

        public Kokoro()
        {
            MaxHP = 3;
            HP = 3;
            Country = "付丧";
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
        }

        public override Charactor Clone()
        {
            return new Kokoro();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "秦心";
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            info.AbilityRadar = new AbilityRadar() { Attack = 3, Defence = 4, Control = 3, Auxiliary = 5, LastStages = 3, Difficulty = 1 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Kokoro");
            return info;
        }
    }
}
