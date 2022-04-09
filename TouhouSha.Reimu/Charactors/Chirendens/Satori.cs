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
    /// 角色【古明地觉】
    /// </summary>
    /// <remarks>
    /// HP:4/4 地灵
    /// 【心瞳】出牌阶段限一次，你可以观看一名其他角色的手牌，令其持续展示一张牌。展示的牌不能被使用或打出，当展示的牌置入弃牌堆时，你选择一项：1. 你获得此牌，2. 令目标失去1点体力。
    /// 【想起】转换技，出牌阶段限一次：
    ///     阳：你失去一点体力上限，选择一名场上其他角色，你声明一个花色并展示其手牌，如果存在你声明的花色，则你获得其一个技能（非限定技，觉醒技）。
    ///     阴：你增加一点体力上限并失去此法获得的技能（若存在），选择一名场上其他角色，你声明一个花色并展示其手牌，其丢弃所有和你声明花色相同的手牌。
    /// 【殿主】主公技，场上的地灵角色受到伤害时可以发动，伤害每有一点，你摸一张牌，受到伤害的角色摸一张牌。
    /// </remarks>
    public class Satori : Charactor
    {
        public const string CountryNameOfLeader = "地灵";

        public Satori()
        {
            MaxHP = 4;
            HP = 4;
            Country = CountryNameOfLeader;
        }

        public override Charactor Clone()
        {
            return new Satori();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "古明地觉";
            info.Skills.Add(new SkillInfo()
            {
                Name = "心瞳",
                Description = "出牌阶段限一次，你可以观看一名其他角色的手牌，从中选择一张牌令其持续展示。展示的牌不能被使用或打出，当展示的牌置入弃牌堆时，你选择一项：1. 你获得此牌，2. 令目标失去1点体力。"
            });
            info.Skills.Add(new SkillInfo()
            {
                Name = "想起",
                Description = "转换技，出牌阶段限一次：\n" +
                    "\t阳：你失去一点体力上限，选择一名场上其他有手牌的角色，你声明一个花色并展示其手牌，如果存在你声明的花色，则你复制其一个技能到你的技能栏（非限定技，觉醒技）。\n" +
                    "\t阴：你增加一点体力上限并失去此法获得的技能（若存在），选择一名场上其他有手牌的角色，你声明一个花色并展示其手牌，其丢弃所有和你声明花色相同的手牌。\n"
            });
            info.Skills.Add(new SkillInfo()
            {
                Name = "殿主",
                Description = "主公技，场上的地灵角色受到伤害时可以发动，伤害每有一点，你摸一张牌，受到伤害的角色摸一张牌。"
            });
            info.AbilityRadar = new AbilityRadar() { Attack = 3, Defence = 3, Control = 4, Auxiliary = 3, LastStages = 3, Difficulty = 1 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Satori");
            return info;
        }
    }
}
