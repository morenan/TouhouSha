using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Cards;

namespace TouhouSha.Koishi.Triggers
{
    /// <summary>
    /// 触发器【（会进入目标的响应阶段）的卡片事件的处理】
    /// </summary>
    public class CardEnterHandleTrigger : Trigger, ITriggerInEvent
    {
        public const string DefaultKeyName = "卡片打出放置到桌面并进响应";
        
        public CardEnterHandleTrigger()
        {
            KeyName = DefaultKeyName;
        }

        string ITriggerInEvent.EventKeyName { get => CardEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            // 必须是未经取消的卡片事件(CardEvent)才能通过。
            if (!(ctx.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(ctx.Ev);
            if (ev.Cancel) return;
            // 确认桌面堆存在。打出的牌首先加入到桌面堆中。
            Zone desktopzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Desktop) == true);
            if (desktopzone == null) return;
            // 确认弃牌堆存在。打出的牌最后可能从桌面堆加入到弃牌堆中。
            Zone discardzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Discard) == true);
            if (discardzone == null) return;
            // 杀的相关属性：
            // 1. 【杀】的默认伤害为1，如果出【杀】的对象喝过酒，则伤害+1。喝过多少酒就加多少伤害（各种鬼王）。
            #region 杀的属性设置
            switch (ev.Card?.KeyName)
            {
                case KillCard.Normal:
                case KillCard.Fire:
                case KillCard.Thunder:
                    int damagevalue = 1;
                    damagevalue += ev.Source.GetValue(LiqureCard.BullUp);
                    ev.Source.SetValue(LiqureCard.BullUp, 0);
                    ev.SetValue(CardEvent.DamageValue, damagevalue);
                    break;
            }
            #endregion 
            // 触发卡片预先事件(CardPreviewEvent)。
            // 1. 可以将这个事件的Cancel属性设为true，取消这个卡片的结算。
            // 2. 可以在这个事件的目标列表(Targets)中添加或者删除目标。
            #region 卡片预先事件(CardPreviewEvent)
            switch (ev.Card?.KeyName)
            {
                case KillCard.Normal:
                case KillCard.Fire:
                case KillCard.Thunder:
                case MissCard.Normal:
                case PeachCard.Normal:
                case LiqureCard.Normal:
                case AttackAllCard.Normal:
                case ArrowAllCard.Normal:
                case HarvestCard.Normal:
                case PeachAllCard.Normal:
                case BirthCard.Normal:
                case NegateCard.Normal:
                case DuelCard.Normal:
                case FireCard.Normal:
                case BridgeBoomCard.Normal:
                case SheepCard.Normal:
                case ChainCard.Normal:
                case BorrowKnifeCard.Normal:
                    if (!ev.Card.IsVirtual)
                        ctx.World.MoveCard(ev.Source, ev.Card, desktopzone, ev);
                    CardPreviewEvent ev_preview = new CardPreviewEvent();
                    ev_preview.LoadValues(ev);
                    ev_preview.Reason = ev.Reason;
                    ev_preview.Card = ev.Card;
                    ev_preview.Source = ev.Source;
                    ev_preview.Targets.Clear();
                    ev_preview.Targets.AddRange(ev.Targets);
                    ctx.World.InvokeEvent(ev_preview);
                    ev.LoadValues(ev_preview);
                    ev.Cancel = ev_preview.Cancel;
                    ev.Card = ev_preview.Card;
                    ev.Source = ev_preview.Source;
                    ev.Targets.Clear();
                    ev.Targets.AddRange(ev_preview.Targets);
                    if (ev.Cancel) return;
                    if (ev.Targets.Count() == 0) return;
                    break;
            }
            #endregion
            // 【五谷丰登】要将牌堆顶和存活玩家相同数目的牌，从牌堆顶拿出并组成一个异步选择表。
            // 后续每个玩家去轮流控制这个表，并拿自己想要的牌。
            #region 五谷丰登选择表
            switch (ev.Card?.KeyName)
            {
                case HarvestCard.Normal:
                    {
                        DesktopCardBoardCore core = new DesktopCardBoardCore();
                        core.KeyName = HarvestCard.DesktopBoard;
                        core.IsAsync = true;
                        DesktopCardBoardZone zone = new DesktopCardBoardZone(core);
                        zone.KeyName = HarvestCard.DesktopBoard;
                        core.Zones.Add(zone);
                        List<Card> cards = ctx.World.GetDrawTops(ev.Targets.Count());
                        if (cards.Count() < ev.Targets.Count()) break;
                        HarvestCard.DesktopStack.Push(core);
                        ctx.World.ShowDesktop(ev.Source, core, new List<IList<Card>>() { cards }, true, ev);
                    }
                    break;
            }
            #endregion
            // 进入每个目标的响应阶段。
            // 由处理响应阶段的触发器来结算卡的具体效果。
            // 所有目标都响应完毕，进行以下工作：
            // 1. 关闭【五谷丰登】的异步选择表，表里剩余的牌进入弃牌堆。
            // 2. 触发卡片完成事件(CardDoneEvent)。
            // 3. 当前卡片如果不是虚拟牌（例如文文的风神），并且在桌面堆，将其转入弃牌堆，
            #region 进入响应阶段，以及卡片完成事件  
            switch (ev.Card?.KeyName)
            {
                case KillCard.Normal:
                case KillCard.Fire:
                case KillCard.Thunder:
                case MissCard.Normal:
                case PeachCard.Normal:
                case LiqureCard.Normal:
                case AttackAllCard.Normal:
                case ArrowAllCard.Normal:
                case HarvestCard.Normal:
                case PeachAllCard.Normal:
                case BirthCard.Normal:
                case NegateCard.Normal:
                case DuelCard.Normal:
                case FireCard.Normal:
                case BridgeBoomCard.Normal:
                case SheepCard.Normal:
                case ChainCard.Normal:
                case BorrowKnifeCard.Normal:
                    for (int i = 0; i < ev.Targets.Count(); i++)
                    {
                        // 借刀杀人的第二个目标是被杀的目标，不进入响应阶段。
                        if (i > 1 && ev.Card.KeyName.Equals(BorrowKnifeCard.Normal)) break;
                        // 这个目标进入响应阶段。
                        // 互动型的卡（例如决斗），互动方用TargetIndex来标记。
                        Player target = ev.Targets[i];
                        ev.SetValue("TargetIndex", i);
                        State newstate = new State();
                        newstate.Ev = ev;
                        newstate.KeyName = State.Handle;
                        newstate.Owner = target;
                        newstate.Step = 0;
                        ctx.World.EnterState(newstate);
                    }
                    // 触发卡片完成事件(CardDoneEvent)
                    CardDoneEvent ev_done = new CardDoneEvent();
                    ev_done.LoadValues(ev);
                    ev_done.Reason = ev.Reason;
                    ev_done.Card = ev.Card;
                    ev_done.Source = ev.Source;
                    ev_done.Targets.Clear();
                    ev_done.Targets.AddRange(ev.Targets);
                    ctx.World.InvokeEvent(ev_done);
                    // 非虚拟卡并且没有被曹老板拿走，则进入弃牌堆。
                    if (!ev.Card.IsVirtual
                     && ev.Card.Zone?.KeyName?.Equals(Zone.Desktop) == true)
                        ctx.World.MoveCard(ev.Source, ev.Card, discardzone, ev);
                    // 关闭五谷丰登异步表
                    if (ev.Card.KeyName.Equals(HarvestCard.Normal))
                    {
                        DesktopCardBoardCore core = HarvestCard.DesktopStack.Pop();
                        ctx.World.CloseDesktop(core);
                    }
                    break;
            }
            #endregion
        }
    }

    public class CardEnterHandleCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is CardEvent)) return false;
            CardEvent ev = (CardEvent)(ctx.Ev);
            if (ev.Card == null) return false;
            return true;
        }
    }
}
