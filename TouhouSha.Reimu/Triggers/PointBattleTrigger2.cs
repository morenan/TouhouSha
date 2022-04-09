using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Koishi.Triggers;
using TouhouSha.Reimu.Charactors.Moriya;

namespace TouhouSha.Reimu.Triggers
{
    public class PointBattleTrigger2 : OverrideTrigger, ITriggerInEvent
    {
        public PointBattleTrigger2()
        {
            Condition = new PointBattleCondition();
        }
        
        string ITriggerInEvent.EventKeyName { get => PointBattleBeginEvent.DefaultKeyName; }

        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is PointBattleBeginEvent)) return;
            PointBattleBeginEvent ev = (PointBattleBeginEvent)(ctx.Ev);
            Card card0 = null;
            Card card1 = null;
            Task task0 = Task.Factory.StartNew(() =>
            {
                if (ev.Source.Skills.FirstOrDefault(_skill => _skill is Suwako_Skill_1) != null)
                    card0 = ctx.World.GetDrawTops(1).FirstOrDefault();
                else if (ev.Source.Skills.FirstOrDefault(_skill => _skill is Tensoku_Skill_0) != null)
                    ctx.World.RequireCard("拼点使用的拼点牌", "请使用一张牌来拼点。", ev.Source,
                        new FulfillNumberCardFilter(1, 1)
                        {
                            Allow_Hand = true,
                            Allow_Judging = false,
                            Allow_Equiped = false
                        },
                        false, 15,
                        (cards) => { card0 = cards.FirstOrDefault(); }, null);
                else
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
                if (ev.Target.Skills.FirstOrDefault(_skill => _skill is Suwako_Skill_1) != null)
                    card1 = ctx.World.GetDrawTops(1).FirstOrDefault();
                else
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

}
