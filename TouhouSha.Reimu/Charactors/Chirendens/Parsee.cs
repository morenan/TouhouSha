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
    /// 角色【水桥帕露西】
    /// </summary>
    /// <remarks>
    /// HP:3/3 地灵
    /// 【嫉妒】出牌阶段限一次，你指定一名角色，以下效果依次计算。
    ///     1. 若其手牌数量大于你，其需交一张手牌给你。
    ///     2. 若其装备数量大于你，其需交一张装备给你。
    ///     3. 若其体力大于你，其流失一点体力，你回复一点体力。
    /// 【绿眼】你每受到一点伤害，你可以指定两名手牌数量不同的角色，手牌少的角色获取手牌多的角色的一张手牌，然后你摸一张牌。
    /// </remarks>
    public class Parsee : Charactor
    {
        public Parsee()
        {
            MaxHP = 3;
            HP = 3;
            Country = Satori.CountryNameOfLeader;
        }

        public override Charactor Clone()
        {
            return new Parsee();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "水桥帕露西";
            info.Skills.Add(new SkillInfo()
            {
                Name = "嫉妒",
                Description = "出牌阶段限一次，你指定一名角色，以下效果依次计算。\n" +
                        "\t1. 若其手牌数量大于你，其需交一张手牌给你。\n" +
                        "\t2. 若其装备数量大于你，其需交一张装备给你。\n" +
                        "\t3. 若其体力大于你，其流失一点体力，你回复一点体力。"
            });
            info.Skills.Add(new SkillInfo()
            {
                Name = "绿眼",
                Description = "你每受到一点伤害，你可以指定两名手牌数量不同的角色，手牌少的角色获取手牌多的角色的一张手牌，然后你摸一张牌。"
            });
            info.AbilityRadar = new AbilityRadar() { Attack = 3, Defence = 3, Control = 3, Auxiliary = 3, LastStages = 3, Difficulty = 5 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Parsee");
            return info;
        }
    }
}
