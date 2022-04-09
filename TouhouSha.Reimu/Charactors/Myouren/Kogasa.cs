using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Myouren
{
    public class Kogasa : Charactor
    {
        public Kogasa()
        {
            MaxHP = 3;
            HP = 3;
            Country = "命莲";
            OtherCountries.Add("付丧");
        }

        public override Charactor Clone()
        {
            return new Kogasa();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore core = new CharactorInfoCore(this);
            core.Name = "多多良小伞";
            core.Skills.Add(new SkillInfo()
            {
                Name = "惊吓",
                Description = "其他角色的回合开始阶段，你将一张手牌背面朝上放置到桌面上，令其猜测类型并翻开。" 
                    + "猜错的场合，本回合该角色不能将相同类型的牌使用或打出。猜对的场合，其对你造成一点伤害。"
                    + "若你放置的卡为装备卡，翻开时你摸两张牌。"
            });
            SkillInfo skill_1 =new SkillInfo()
            {
                Name = "唐伞",
                Description = "出牌阶段限一次，你可以将一张手牌作为【保护伞】放置到场上任意一名角色的装备区。"
            };
            skill_1.AttachedSkills.Add(new SkillInfo()
            {
                Name = "保护伞",
                Description = "你受到和该防具花色相同的卡牌造成的伤害+1，花色不同的卡牌或者非卡牌造成的伤害-1。当你受到伤害时，你弃置这张卡。"
            });
            core.Skills.Add(skill_1);
            core.Image = ImageHelper.LoadCardImage("Charactors", "Kogasa");
            core.AbilityRadar = new AbilityRadar() { Attack = 1, Defence = 4, Control = 5, Auxiliary = 5, LastStages = 4, Difficulty = 2 };
            return core;
        }
    }
}
