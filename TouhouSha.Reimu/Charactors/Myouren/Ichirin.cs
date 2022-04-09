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
    public class Ichirin : Charactor
    {
        public Ichirin()
        {
            MaxHP = 4;
            HP = 4;
            Country = Hiziri.CountryNameOfLeader;
            OtherCountries.Add(Marisa.CountryNameOfLeader);
        }

        public override Charactor Clone()
        {
            return new Ichirin();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore core = new CharactorInfoCore(this);
            core.Name = "云居一轮";
            core.Skills.Add(new SkillInfo()
            {
                Name = "嗔怒",
                Description = "当你受到伤害时，你摸一张牌，并将一张手牌作为【怒气】放置于你角色牌上（至多为3）。你每有一个【怒气】，你摸牌阶段多摸一张牌，你的手牌上限+1。",
            }); 
            core.Skills.Add(new SkillInfo()
            {
                Name = "云山",
                Description = "限定技，出牌阶段开始时，你将你角色牌上三张【怒气】加入手牌，本阶段你使用的牌不能被响应，你造成的伤害+1。",
            });
            core.Image = ImageHelper.LoadCardImage("Charactors", "Ichirin");
            core.AbilityRadar = new AbilityRadar() { Attack = 5, Defence = 3, Control = 4, Auxiliary = 1, LastStages = 4, Difficulty = 5 };
            return core;
        }
    }
}
