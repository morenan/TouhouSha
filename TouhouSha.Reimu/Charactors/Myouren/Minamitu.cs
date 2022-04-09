using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;
using TouhouSha.Reimu.Charactors.Ghosts;

namespace TouhouSha.Reimu.Charactors.Myouren
{
    public class Minamitu : Charactor
    {
        public Minamitu()
        {
            MaxHP = 3;
            HP = 3;
            Country = "命莲";
            OtherCountries.Add(Yoyoko.CountryNameOfLeader);
        }

        public override Charactor Clone()
        {
            return new Minamitu();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore core = new CharactorInfoCore(this);
            core.Name = "村纱水蜜";
            SkillInfo skill_0 = new SkillInfo()
            {
                Name = "水难",
                Description = "限定技，出牌阶段，你选择你以外的一名角色，其获得技能【舀满】。"
            };
            skill_0.AttachedSkills.Add(new SkillInfo()
            {
                Name = "舀满",
                Description = "锁定技，回合开始以及结束阶段，当你的手牌少于X时，你将手牌摸至X张（X为体力上限且最多为5），并选择一项：1.失去一点体力，2.失去一点体力上限并摸两张牌。"
            });
            SkillInfo skill_1 = new SkillInfo()
            {
                Name = "倾覆",
                Description = "限定技，出牌阶段，你选择你以外的一名角色，其获得技能【同路】。"
            };
            skill_1.AttachedSkills.Add(new SkillInfo()
            {
                Name = "同路",
                Description = "锁定技，当你获得手牌时，场上的村纱水密也摸相同数量的牌。当你的手牌以使用或者打出以外的方式置于弃牌堆时，场上的村纱水密也弃置相同数量的牌（不足则全部弃置）。"
            });
            SkillInfo skill_2 = new SkillInfo()
            {
                Name = "沉溺",
                Description = "限定技，出牌阶段，你选择你以外的一名角色，其获得技能【漩涡】。"
            };
            skill_2.AttachedSkills.Add(new SkillInfo()
            {
                Name = "漩涡",
                Description = "锁定技，当你横置或者重置时，你距离范围1以内的角色也横置或者重置，此法横置的角色弃一张手牌，重置的角色摸一张牌。"
            });
            core.Skills.Add(skill_0);
            core.Skills.Add(skill_1);
            core.Skills.Add(skill_2);
            core.Image = ImageHelper.LoadCardImage("Charactors", "Minamitu");
            core.AbilityRadar = new AbilityRadar() { Attack = 3, Defence = 3, Auxiliary = 5, Control = 4, LastStages = 0, Difficulty = 3 };
            return core;
        }

    }
}
