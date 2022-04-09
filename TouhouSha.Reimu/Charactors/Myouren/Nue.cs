using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.UIs;

namespace TouhouSha.Reimu.Charactors.Myouren
{
    public class Nue : Charactor
    {
        public Nue()
        {
            MaxHP = 3;
            HP = 3;
            Country = Hiziri.CountryNameOfLeader;
        }

        public override Charactor Clone()
        {
            return new Nue();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore core = new CharactorInfoCore(this);
            core.Name = "封兽鵺";
            core.Skills.Add(new SkillInfo()
            {
                Name = "不明",
                Description = "你的回合结束阶段或者你受到伤害时，根据上一张使用或者打出的牌的花色可以发动以下效果：若你之前没有使用或者打出牌，则你可以将以下效果全部发动：\n" +
                "\t♥：你令一名其他角色交给你一张牌并展示，若为♥，你回复一点体力。\n" +
                "\t♦：你翻开牌堆顶四张牌，如果存在♦，你获得其中非♦的牌并将♦的牌置入弃牌。\n" +
                "\t♠：你选择场上一名角色，将其一张牌移到一个合理的位置，如果移动的为♠牌，你对该角色造成一点雷电伤害。\n" +
                "\t♣：你选择弃牌堆至多5张♣牌，你获得其中至多2张牌，将剩余的牌以任意顺序放置于牌堆顶。"
            });
            core.Skills.Add(new SkillInfo()
            {
                Name = "飞船",
                Description = "出牌阶段限一次，你可以从牌堆里随机获得一张UFO，然后可以选择交给场上一名其他角色，根据UFO的种类：\n" +
                    "攻击型UFO：该角色弃X张牌。" +
                    "防御型UFO：该角色摸X张牌。X为场上的UFO的数量的总和且至少为1。"
            });
            core.Image = ImageHelper.LoadCardImage("Charactors", "Nue");
            core.AbilityRadar = new AbilityRadar() { Attack = 4, Defence = 5, Auxiliary = 5, Control = 4, LastStages = 4, Difficulty = 3 };
            return core;
        }

    }
}
