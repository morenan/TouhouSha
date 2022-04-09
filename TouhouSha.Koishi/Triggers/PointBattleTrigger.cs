using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;

namespace TouhouSha.Koishi.Triggers
{
    public class PointBattleTrigger : Trigger, ITriggerInEvent
    {
        public PointBattleTrigger()
        {
            Condition = new PointBattleCondition();
        }  
        
        string ITriggerInEvent.EventKeyName { get => PointBattleBeginEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is PointBattleBeginEvent)) return;
            PointBattleBeginEvent ev = (PointBattleBeginEvent)(ctx.Ev);
            Card card0 = ev.SourceCard;
            Card card1 = ev.TargetCard;
            Task task0 = Task.Factory.StartNew(() =>
            {
                if (card0 != null) return;
                ctx.World.RequireCard("拼点使用的拼点牌", "请使用一张牌来拼点。", ev.Source,
                    new FulfillNumberCardFilter(1, 1)
                    {
                        Allow_Hand = true,
                        Allow_Judging = false,
                        Allow_Equiped = false
                    },
                    false, 15,
                    (cards) => { card0 = cards.FirstOrDefault(); }, null);
            });
            Task task1 = Task.Factory.StartNew(() =>
            {
                if (card1 != null) return;
                ctx.World.RequireCard("拼点使用的拼点牌", "请使用一张牌来拼点。", ev.Target,
                    new FulfillNumberCardFilter(1, 1)
                    {
                        Allow_Hand = true,
                        Allow_Judging = false,
                        Allow_Equiped = false
                    },
                    false, 15,
                    (cards) => { card1 = cards.FirstOrDefault(); }, null);
            });
            task0.Wait();
            task1.Wait();
            if (card0 == null) return;
            if (card1 == null) return;
            Zone desktopzone = ctx.World.CommonZones.FirstOrDefault(_zone => _zone.KeyName?.Equals(Zone.Desktop) == true);
            if (desktopzone == null) return;
            ctx.World.MoveCards(ev.Source, new List<Card>() { card0, card1 }, desktopzone, ev);
            PointBattlePreviewEvent ev1 = new PointBattlePreviewEvent();
            ev1.Reason = ev.Reason;
            ev1.Source = ev.Source;
            ev1.Target = ev.Target;
            ev1.SourceCard = card0;
            ev1.TargetCard = card1;
            ctx.World.InvokeEventAfterEvent(ev1, ev);
        }
    } 
    
    public class PointBattleCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is PointBattleBeginEvent)) return false;
            PointBattleBeginEvent ev = (PointBattleBeginEvent)(ctx.Ev);
            if (ev.Cancel) return false;
            return true;
        }
    }

    public class PointBattlePreviewTriggger : Trigger, ITriggerInEvent
    {
        public PointBattlePreviewTriggger()
        {
            Condition = new PointBattlePreviewCondition();
        }

        string ITriggerInEvent.EventKeyName { get => PointBattlePreviewEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is PointBattlePreviewEvent)) return;
            PointBattlePreviewEvent ev = (PointBattlePreviewEvent)(ctx.Ev);
            PointBattleDoneEvent ev1 = new PointBattleDoneEvent();
            ev1.Reason = ev.Reason;
            ev1.Source = ev.Source;
            ev1.Target = ev.Target;
            ev1.SourceCard = ev.SourceCard;
            ev1.TargetCard = ev.TargetCard;
            ctx.World.InvokeEventAfterEvent(ev1, ev);
        }
    }
    
    public class PointBattlePreviewCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            if (!(ctx.Ev is PointBattlePreviewEvent)) return false;
            PointBattlePreviewEvent ev = (PointBattlePreviewEvent)(ctx.Ev);
            if (ev.Cancel) return false;
            return true;
        }
    }


}
