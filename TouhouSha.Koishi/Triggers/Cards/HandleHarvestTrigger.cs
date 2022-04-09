using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.Events;
using TouhouSha.Core.Filters;
using TouhouSha.Core.UIs;
using TouhouSha.Koishi.Cards;

namespace TouhouSha.Koishi.Triggers
{
    public class HandleHarvestTrigger : Trigger, ITriggerInState
    {
        public const string DefaultKeyName = "响应桃园结义";

        public HandleHarvestTrigger()
        {
            KeyName = DefaultKeyName;
            Condition = new HandleHarvestCondition();
        }

        string ITriggerInState.StateKeyName { get => State.Handle; }

        int ITriggerInState.StateStep { get => StateChangeEvent.Step_Start; }

        public override void Action(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            CardEvent ev = (CardEvent)(state.Ev);
            DesktopCardBoardCore desktop = HarvestCard.DesktopStack.Peek();
            if (desktop == null) return;
            desktop.SelectedCards.Clear();
            ctx.World.ControlDesktop(state.Owner, desktop);
        }
    }


    public class HandleHarvestCondition : ConditionFilter
    {
        public override bool Accept(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (!(state?.Ev is CardEvent)) return false;
            if (state.GetValue(NegateCard.HasBeenNegated) == 1) return false;
            CardEvent ev = (CardEvent)(state.Ev);
            if (ev.Card.KeyName?.Equals(HarvestCard.Normal) != true) return false;
            return true;
        }
    }
}
