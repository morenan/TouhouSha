using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;
using TouhouSha.Reimu.Charactors.SelfCrafts;

namespace TouhouSha.Reimu.Charactors.Myouren
{
    public class Hiziri : Charactor
    {
        public const string CountryNameOfLeader = "命莲";

        public Hiziri()
        {
            MaxHP = 4;
            HP = 4;
            Country = CountryNameOfLeader;
            OtherCountries.Add(Marisa.CountryNameOfLeader);
        }

        public override Charactor Clone()
        {
            return new Hiziri();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore core = new CharactorInfoCore(this);
            core.Name = "圣白莲";
            core.Skills.Add(new SkillInfo()
            {
                Name = "僧戒",
                Description = "锁定技，你的【酒】只能用于重铸（丢弃此牌并摸一张牌）。"
            }); 
            core.Skills.Add(new SkillInfo()
            {
                Name = "经卷",
                Description = "当你没有发动过该技能，或者发动该技能之后，连续使用或者打出三张不同花色的牌时，结算完毕你可以选择：1.摸两张牌。2.弃置任意张牌并摸同数量+1的牌。"
            });
            core.Skills.Add(new SkillInfo()
            {
                Name = "超人",
                Description = "当你没有发动过该技能，或者发动该技能之后，连续使用或者打出三张不同点数的牌时，结算完毕你可以选择：1.回复一点体力，2.对一名角色造成一点伤害。"
            });
            core.Skills.Add(new SkillInfo()
            {
                Name = "复诵",
                Description = "当你没有发动过该技能，或者发动该技能之后，连续使用或者打出三张不同类型的牌时，结算完毕你可以选择：1.视作你使用了上一张使用或者打出的牌。2.你使用下一张牌可以额外指定一个目标。"
            });
            core.Skills.Add(new SkillInfo()
            {
                Name = "授业",
                Description = "主公技，每回合限一次，和你同势力的其他角色摸牌时，可以选择交给你，然后你交给其X张牌（X为其摸牌的数量减一且至少为1）。",
            });
            core.Image = ImageHelper.LoadCardImage("Charactors", "Hiziri");
            core.AbilityRadar = new AbilityRadar() { Attack = 4, Defence = 3, Auxiliary = 2, Control = 3, LastStages = 3, Difficulty = 1 };
            return core;

        }
    }
}
