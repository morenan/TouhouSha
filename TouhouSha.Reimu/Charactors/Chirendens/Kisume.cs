using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Chirendens
{
    public class Kisume : Charactor
    {
        public Kisume()
        {
            MaxHP = 5;
            HP = 5;
            Country = Satori.CountryNameOfLeader;
            Skills.Add(new Kisume_Skill_0());
        }

        public override Charactor Clone()
        {
            return new Kisume();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore core = new CharactorInfoCore(this);
            core.Name = "琪斯美";
            core.Skills.AddRange(Skills.Select(_skill => _skill.GetInfo()));
            core.AbilityRadar = new AbilityRadar() { Attack = 1, Defence = 5, Control = 3, Auxiliary = 5, LastStages = 5, Difficulty = 5 };
            core.Image = ImageHelper.LoadCardImage("Charactors", "Kisume");
            return core;
        }
    }

    public class Kisume_Skill_0 : Skill
    {
        public override Skill Clone()
        {
            return new Kisume_Skill_0();
        }

        public override Skill Clone(Card newcard)
        {
            return new Kisume_Skill_0();
        }

        public override SkillInfo GetInfo()
        {
            return new SkillInfo()
            {
                Name = "降桶",
                Description = "回合开始阶段，你获得一个【鬼火】标记。"
                + "出牌阶段开始时，你可以将场上的【鬼火】标记转移给另一名角色。"
                + "当你受到伤害时，拥有【鬼火】标记的角色也受到同内容的伤害。"
                + "拥有【鬼火】标记的角色死亡时，你获得一个【鬼火】标记。",
            };
        }
    }
}
