using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.AIs
{
    /// <summary>
    /// 价值评估器的实现接口。
    /// </summary>
    public interface IWorthAccessment
    {
        /// <summary>
        /// 获取这名玩家（对于控制者来说的）的仇恨值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        double GetHatred(Context ctx, Player controller, Player target);

        /// <summary>
        /// 获取这名玩家到自己的回合能创造的价值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        double GetWorthInPhase(Context ctx, Player controller, Player target);

        /// <summary>
        /// 获取这名玩家所拥有的总价值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        double GetWorth(Context ctx, Player controller, Player target);
        /// <summary>
        /// 获取这张卡的价值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="card"></param>
        /// <returns></returns>
        double GetWorth(Context ctx, Player controller, Card card);
        /// <summary>
        /// 获取这张卡和相关行为的价值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="card"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        double GetWorth(Context ctx, Player controller, Card card, Enum_CardBehavior behavior);
        /// <summary>
        /// 获取这张卡放到区域的相应位置时的价值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="card"></param>
        /// <param name="indexofzone"></param>
        /// <returns></returns>
        double GetWorth(Context ctx, Player controller, Card card, int indexofzone);
        /// <summary>
        /// 获取这个区域的卡的平均价值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="zone"></param>
        /// <returns></returns>
        double GetWorthExpected(Context ctx, Player controller, Zone zone);
        /// <summary>
        /// 获取这名玩家每一点HP所包含的价值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        double GetWorthPerHp(Context ctx, Player controller, Player target);
        /// <summary>
        /// 获取这名玩家每一点HP和对其行为所包含的价值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="target"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        double GetWorthPerHp(Context ctx, Player controller, Player target, Enum_HpBehavior behavior);
        /// <summary>
        /// 获取这名玩家每一点HP上限所包含的价值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        double GetWorthPerMaxHp(Context ctx, Player controller, Player target);
        /// <summary>
        /// 获取应答是的价值与应答否的价值之间的差。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="keyname"></param>
        /// <returns></returns>
        double GetWorthAsk(Context ctx, Player controller, string keyname);
        /// <summary>
        /// 获取选择这张卡时所获得的价值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="keyname"></param>
        /// <param name="card"></param>
        /// <param name="selecteds"></param>
        /// <returns></returns>
        double GetWorthSelect(Context ctx, Player controller, string keyname, Card card, IEnumerable<Card> selecteds);
        /// <summary>
        /// 获取选择这名玩家时所获得的价值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="keyname"></param>
        /// <param name="target"></param>
        /// <param name="selecteds"></param>
        /// <returns></returns>
        double GetWorthSelect(Context ctx, Player controller, string keyname, Player target, IEnumerable<Player> selecteds);
        /// <summary>
        /// 获取技能选择这张卡时所获得的价值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="skill"></param>
        /// <param name="card"></param>
        /// <param name="selecteds"></param>
        /// <returns></returns>
        double GetWorthSelect(Context ctx, Player controller, Skill skill, Card card, IEnumerable<Card> selecteds);
        /// <summary>
        /// 获取技能选择这名玩家时所获得的价值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="skill"></param>
        /// <param name="target"></param>
        /// <param name="selecteds"></param>
        /// <returns></returns>
        double GetWorthSelect(Context ctx, Player controller, Skill skill, Player target, IEnumerable<Player> selecteds);
        /// <summary>
        /// 获取对一个目标使用卡时所获得的价值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="usecard"></param>
        /// <param name="target"></param>
        /// <param name="selecteds"></param>
        /// <returns></returns>
        double GetWorthUse(Context ctx, Player controller, Card usecard, Player target, IEnumerable<Player> selecteds);
        /// <summary>
        /// 获取响应时打出这张卡所获得的价值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="handledcard"></param>
        /// <returns></returns>
        double GetWorthHandle(Context ctx, Player controller, Card handledcard);
        /// <summary>
        /// 获取目标拼点赢的剩余价值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="target"></param>
        /// <param name="ev"></param>
        /// <returns></returns>
        double GetWorthPointBattleWin(Context ctx, Player controller, Player target, Event ev);
        /// <summary>
        /// 获取目标拼点输的剩余价值。
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="controller"></param>
        /// <param name="target"></param>
        /// <param name="ev"></param>
        /// <returns></returns>
        /// <remarks>
        /// 我在想，有些武将手牌非常吃紧（例如火诸葛这类），无论拼点输赢都有较大手牌损失代价。
        /// 并且，我还设计了一些拼点输会触发技能的武将，也算在剩余价值上。
        /// </remarks>
        double GetWorthPointBattleLose(Context ctx, Player controller, Player target, Event ev);
    }

    /// <summary>
    /// 关于卡片的行为。
    /// </summary>
    public enum Enum_CardBehavior
    {
        /// <summary>
        /// 使用这张卡。
        /// </summary>
        Use,
        /// <summary>
        /// 打出这张卡。
        /// </summary>
        Handle,
        /// <summary>
        /// 失去这张卡。
        /// </summary>
        Lost,
    }

    public enum Enum_HpBehavior
    {
        /// <summary>
        /// 受到伤害时失去体力。
        /// </summary>
        Damage,
        /// <summary>
        /// 回复体力。
        /// </summary>
        Heal,
        /// <summary>
        /// 仅变化体力，体力流失属于这类。
        /// </summary>
        Change,
    }
}
