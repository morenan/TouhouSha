using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Koishi.Cards;
using TouhouSha.Reimu.Charactors.SelfCrafts;

namespace TouhouSha.Reimu.AIs
{
    public class ChangeJudgeCardWorth : TouhouSha.Koishi.AIs.ChangeJudgeCardWorth
    {
        protected override double GetWorthJudge(Context ctx, Player controller, JudgeEvent ev_judge, Card judgecard)
        {
            if (ev_judge.Reason is SkillEvent)
            {
                SkillEvent ev_skill = (SkillEvent)(ev_judge.Reason);
                switch (ev_skill.Skill?.KeyName)
                {
                    #region 灵梦【退治】
                    case TouhouSha.Reimu.Charactors.SelfCrafts.Reimu.Skill_1.DefaultKeyName:
                        {
                            // 判定红则复原翻面，赚了一个回合的收益。
                            if (judgecard.CardColor?.SeemAs(Enum_CardColor.Red) == true)
                            {
                                double optworth = -10000;
                                foreach (Player player in ctx.World.GetAlivePlayers())
                                {
                                    if (!player.IsFacedDown) continue;
                                    optworth = Math.Max(optworth, AITools.WorthAcc.GetWorthInPhase(ctx, controller, player));
                                }
                                return optworth;
                            }
                            // 判定黑则贴乐，损失一个回合的收益。
                            if (judgecard.CardColor?.SeemAs(Enum_CardColor.Black) == true)
                            {
                                double optworth = -10000;
                                foreach (Player player in ctx.World.GetAlivePlayers())
                                {
                                    Zone judgezone = player.Zones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Judge) == true);
                                    if (judgezone == null) continue;
                                    if (judgezone.Cards.FirstOrDefault(_card => _card.KeyName?.Equals(HappyCard.Normal) == true) != null) continue;
                                    optworth = Math.Max(optworth, -AITools.WorthAcc.GetWorthInPhase(ctx, controller, player));
                                }
                                return optworth;
                            }
                            break;
                        }
                        #endregion 
                }
            }
            return base.GetWorthJudge(ctx, controller, ev_judge, judgecard);
        }
    }
}
