using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Desires
{
    public class Futo : Charactor
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
                return new SkillInfo()
                {
                    Name = "风水",
                    Description = "出牌阶段，你可以弃置一张手牌。" +
                        "每发动2次该技能，你计算与其他角色的距离-1。" + 
                        "每发动4次该技能，你的手牌上限+1。" + 
                        "每发动8次该技能，你摸牌阶段多摸一张牌。" +
                        "回合结束阶段，你摸X张牌，并视作使用了一张可指定至多X个与你距离不超过1的目标的【铁索连环】（X为此回合该技能的发动次数）。"
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
                    Name = "纵火",
                    Description = "你可以将一张红色牌当作【火攻】使用。你的【火攻】可以额外指定两个目标。",
                };
            }
        }

        public Futo()
        {
            MaxHP = 3;
            HP = 3;
            Country = Miko.CountryNameOfLeader;
            Skills.Add(new Skill_0());
            Skills.Add(new Skill_1());
        }

        public override Charactor Clone()
        {
            return new Futo();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "物部布都";
            info.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            info.AbilityRadar = new AbilityRadar() { Attack = 4, Defence = 3, Control = 3, Auxiliary = 4, LastStages = 3, Difficulty = 4 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Futo");
            return info;
        }

    }
}
