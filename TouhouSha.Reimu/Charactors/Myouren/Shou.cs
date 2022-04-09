using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Myouren
{
    public class Shou : Charactor
    {
        public Shou()
        {
            MaxHP = 4;
            HP = 4;
            Country = Hiziri.CountryNameOfLeader;
        }

        public override Charactor Clone()
        {
            return new Shou();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore core = new CharactorInfoCore(this);
            core.Name = "寅丸星";
            core.Skills.Add(new SkillInfo()
            {
                Name = "财运",
                Description = "一名角色的回合结束阶段，你可以令其丢弃所有手牌，并摸等量的牌。",
            });
            core.Skills.Add(new SkillInfo()
            {
                Name = "宝塔",
                Description = "出牌阶段每种类型限一次，你从牌堆中随机挑选三张此类型的牌，你获得一张其中的牌，令场上一名其他角色获得一张其中的牌，剩下的牌弃置。",
            });
            core.Image = ImageHelper.LoadCardImage("Charactors", "Shou");
            core.AbilityRadar = new AbilityRadar() { Attack = 3, Defence = 2, Auxiliary = 5, Control = 2, LastStages = 1, Difficulty = 4 };
            return core;
        }

    }
}
