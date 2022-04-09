using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Chirendens
{
    public class Yamame : Charactor
    {
        public Yamame()
        {
            MaxHP = 4;
            HP = 4;
            Country = Satori.CountryNameOfLeader;
            Skills.Add(new Yamame_Skill_0());
            Skills.Add(new Yamame_Skill_1());
        }

        public override Charactor Clone()
        {
            return new Yamame();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore core = new CharactorInfoCore(this);
            core.Name = "黑谷山女";
            core.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            core.AbilityRadar = new AbilityRadar() { Attack = 2, Defence = 3, Control = 5, Auxiliary = 2, LastStages = 4, Difficulty = 3 };
            core.Image = ImageHelper.LoadCardImage("Charactors", "Yamame");
            return core;
        }

    }

    public class Yamame_Skill_0 : Skill
    {
        public override Skill Clone()
        {
            return new Yamame_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yamame_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "疫病",
                Description = "出牌阶段，你可以选择一名其他角色，将一张手牌放置到该角色牌上，作为【疫病】。" 
                + "同一角色牌上不能存在两张相同花色的【疫病】。"
                + "角色牌上每有一张【疫病】，该角色的手牌上限-1。"
            };
        }
    }

    public class Yamame_Skill_1 : Skill
    {
        public override Skill Clone()
        {
            return new Yamame_Skill_1();
        }

        public override Skill Clone(Card newcard)
        {
            return new Yamame_Skill_1();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "传播",
                Description = "当场上一名角色使用牌指定目标时，如果使用者的角色牌上存在相同花色的【疫病】，并且目标的角色牌不存在相同花色的【疫病】，"
                + "你将本次使用的牌作为【疫病】放置到目标的角色牌上。如果目标是你，你将这张【疫病】去除，并选择一项：1. 回复一点体力。2. 摸两张牌。"
            };
        }
    }
}
