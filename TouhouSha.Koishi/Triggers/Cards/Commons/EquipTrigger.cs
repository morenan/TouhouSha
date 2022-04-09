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
    public class EquipTrigger : Trigger, ITriggerInEvent
    {
        public EquipTrigger()
        {
            KeyName = "卡片被装备";
        }
        
        string ITriggerInEvent.EventKeyName { get => CardEvent.DefaultKeyName; }
        
        public override void Action(Context ctx)
        {
            if (!(ctx.Ev is CardEvent)) return;
            CardEvent ev = (CardEvent)(ctx.Ev);
            if (ev.Cancel) return;
            if (ev.Card?.CardType?.E != Enum_CardType.Equip) return;
            Player target = ev.Targets.FirstOrDefault();
            if (target == null) return;
            EquipZone equipzone = target.Zones.FirstOrDefault(_zone => _zone is EquipZone) as EquipZone;
            if (equipzone == null) return;
            ctx.World.MoveCard(ev.Source, ev.Card, equipzone, ev);
        }
    }

    public class EquipTriggerCondition : ConditionFilter
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
