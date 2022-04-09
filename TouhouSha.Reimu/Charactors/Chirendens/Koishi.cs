using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Chirendens
{
    /// <summary>
    /// 角色【古明地恋】
    /// </summary>
    /// <remarks>
    /// HP:3/3 地灵
    /// 【闭瞳】锁定技，你无法在回合外获得手牌。你的回合外，当你手牌为0并成为一张卡的唯一目标时，取消之。
    /// 【本我】锁定技，出牌阶段结束时，你丢弃全部手牌，令场上所有其他角色弃置等量的牌，当场上角色无法满足弃置的数量时，其弃置所有的牌并受到你的一点伤害。
    /// 【抑制】觉醒技，当你进入濒死阶段时，你增加一点体力上限，将体力回复至体力上限，并获得技能【超我】。
    /// 【超我】锁定技，当你受到伤害时，你展示牌堆顶的一张牌，可以使用的场合使用之，不可以使用的场合你回复一点体力。
    /// </remarks>
    public class Koishi : Charactor
    {
        public Koishi()
        {
            MaxHP = 3;
            HP = 3;
            Country = Satori.CountryNameOfLeader;
        }

        public override Charactor Clone()
        {
            return new Koishi();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "古明地恋";
            info.Skills.Add(new SkillInfo()
            {
                Name = "闭瞳",
                Description = "锁定技，你在回合外加入手牌的卡立即置于弃牌堆。你的回合外，当你没有手牌并成为一张卡的唯一目标时，取消之。"
            });
            info.Skills.Add(new SkillInfo()
            {
                Name = "表象",
                Description = "锁定技，出牌阶段结束时，你将全部手牌放置到你的角色牌上作为【偏执】，并选择一名其他角色，弃置其至多等量的牌。" 
                    + "其他角色的回合开始阶段，必须选择你角色牌上的一张【偏执】（若存在），除非其展示一张相同花色的牌并获得此牌，否则流失一点体力。"
            });
            SkillInfo skill_2 = new SkillInfo()
            {
                Name = "抑制",
                Description = "觉醒技，当你进入濒死阶段时，你增加一点体力上限，将体力回复至体力上限，并获得技能【超我】。"
            };
            skill_2.AttachedSkills.Add(new SkillInfo()
            {
                Name = "超我",
                Description = "锁定技，每回合限一次，当你受到伤害时，你展示牌堆顶的四张牌，令伤害来源选择一项：1.弃置和展示的牌花色均不同的一张牌。2. 令你回复1点体力。"
            });
            info.Skills.Add(skill_2);
            info.AbilityRadar = new AbilityRadar() { Attack = 3, Defence = 5, Control = 1, Auxiliary = 1, LastStages = 5, Difficulty = 5 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Koishi");
            return info;
        }
    }
}
