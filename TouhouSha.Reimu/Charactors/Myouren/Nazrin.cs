using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Myouren
{
    public class Nazrin : Charactor
    {
        public Nazrin()
        {
            MaxHP = 3;
            HP = 3;
            Country = "命莲";
        }

        public override Charactor Clone()
        {
            return new Nazrin();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore core = new CharactorInfoCore(this);
            core.Name = "纳兹琳";
            core.Skills.Add(new SkillInfo()
            {
                Name = "寻宝",
                Description = "直到你的下一个回合开始阶段，每项限选一次：\n"
                    + "\t1.当有基本牌置入弃牌堆时，你可以从弃牌堆找到一张和该牌相同点数的基本牌并加入手卡。\n"
                    + "\t2.当有锦囊牌置入弃牌堆时，你可以从摸牌堆随机获得一张和该牌相同花色的锦囊牌。\n"
                    + "\t3.当有装备牌置入弃牌堆时，你可以从场上一名角色的装备区将相同功能（武器，防具，UFO）的牌加入手卡或者置于你的装备区。"
            });
            core.Skills.Add(new SkillInfo()
            {
                Name = "售罄",
                Description = "当你失去手牌中一种类型的牌的最后一张牌时，你可以摸一张牌。"
            });
            core.Image = ImageHelper.LoadCardImage("Charactors", "Nazrin");
            core.AbilityRadar = new AbilityRadar() { Attack = 4, Defence = 3, Auxiliary = 3, Control = 4, LastStages = 4, Difficulty = 1 };
            return core;
        }

    }
}
