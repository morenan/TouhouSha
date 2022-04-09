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
    /// 角色【灵乌路空】
    /// </summary>
    /// <remarks>
    /// HP:4/4 地灵
    /// 【核爆】出牌阶段限一次，你可以丢弃两张相同花色的牌，对一名角色造成2点火焰伤害，其距离为1以内的其他角色造成1点火焰伤害。
    /// </remarks>
    public class Utsuho : Charactor
    {
        public Utsuho()
        {
            MaxHP = 4;
            HP = 4;
            Country = Satori.CountryNameOfLeader;
        }

        public override Charactor Clone()
        {
            return new Utsuho();
        }

        public override CharactorInfoCore GetInfo()
        {
            CharactorInfoCore info = new CharactorInfoCore(this);
            info.Name = "灵乌路空";
            info.Skills.Add(new SkillInfo()
            {
                Name = "核爆",
                Description = "出牌阶段限一次，你可以丢弃两张相同花色的牌，对一名角色造成2点火焰伤害，其距离为1以内的其他角色造成1点火焰伤害。"
            });
            info.AbilityRadar = new AbilityRadar() { Attack = 4, Defence = 1, Control = 1, Auxiliary = 1, LastStages = 2, Difficulty = 5 };
            info.Image = ImageHelper.LoadCardImage("Charactors", "Utsuho");
            return info;
        }
    }
}
