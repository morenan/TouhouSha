using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TouhouSha.Core.Events;

namespace TouhouSha.Core
{
    public class StackHandler
    {
        public StackHandler(World _world)
        {
            this.world = _world;
        }

        public virtual void Dispose()
        {
            this.world = null;
        }

        private World world;
        public World World
        {
            get { return this.world; }
        }

        public virtual void Handle()
        {

        }
    }

    public class StateHandler : StackHandler
    {
        public StateHandler(World _world, State _state)
            : base(_world)
        {
            this.state = _state;
        }
        
        private State state;
        public State State
        {
            get { return this.state; }
        }

        public override void Handle()
        {
            base.Handle();
            Player player = state.Owner;
            Context ctx = new Context(World, null);
            List<Player> players = new List<Player>();
            List<Trigger> triggers = new List<Trigger>();
            players.AddRange(World.GetAlivePlayersStartHere(player));
            triggers.AddRange(World.GlobalTriggerBeforePlayers.GetAcceptedTriggerOfState(ctx));
            foreach (Player other in players)
                triggers.AddRange(other.Triggers.GetAcceptedTriggerOfState(ctx));
            triggers.AddRange(World.GlobalTriggerAfterPlayers.GetAcceptedTriggerOfState(ctx));
            foreach (Trigger trigger in triggers)
            {
                if (!player.IsAlive) return;
                Trigger newtrigger = World.TryReplaceNewTrigger(trigger, state.Ev);
                if (newtrigger == null) continue;
                ConditionFilter newcondition = World.TryReplaceNewCondition(newtrigger.Condition, state.Ev);
                if (newcondition == null) continue;
                if (!newcondition.Accept(ctx)) continue;
                newtrigger.Cost(ctx);
                if (!player.IsAlive) return;
                newtrigger.Action(ctx);
                if (!player.IsAlive) return;
            }
        }

    }
    public class EventHandler : StackHandler
    {
        public EventHandler(World _world, Event _event)
            : base(_world)
        {
            this.ev = _event;
        }

        private Event ev;
        public Event Event
        {
            get { return this.ev; }
        }

        public override void Handle()
        {
            base.Handle();
            Context ctx = new Context(World, ev);
            Player starter = ev.GetHandleStarter();
            List<Player> players = new List<Player>();
            List<Trigger> triggers = new List<Trigger>();
            if (starter != null)
                players.AddRange(World.GetAlivePlayersStartHere(starter));
            else
                players.AddRange(World.GetAlivePlayers());
            triggers.AddRange(World.GlobalTriggerBeforePlayers.GetAcceptedTriggerOfEvent(ctx));
            foreach (Player other in players)
                triggers.AddRange(other.Triggers.GetAcceptedTriggerOfEvent(ctx));
            triggers.AddRange(World.GlobalTriggerAfterPlayers.GetAcceptedTriggerOfEvent(ctx));
            foreach (Trigger trigger in triggers)
            {
                if (ev.IsStopHandle()) return;
                Trigger newtrigger = trigger;
                if (!(ev is UseTriggerEvent) && !(ev is UseFilterEvent) && !(ev is UseCalculatorEvent) && !(ev is UseCardCalculatorEvent))
                    newtrigger = World.TryReplaceNewTrigger(newtrigger, ev);
                if (newtrigger == null) continue;
                ConditionFilter newcondition = newtrigger.Condition;
                if (!(ev is UseTriggerEvent) && !(ev is UseFilterEvent) && !(ev is UseCalculatorEvent) && !(ev is UseCardCalculatorEvent))
                    newcondition = World.TryReplaceNewCondition(newtrigger.Condition, ev);
                if (newcondition == null) continue;
                if (!newcondition.Accept(ctx)) continue;
                newtrigger.Cost(ctx);
                if (ev.IsStopHandle()) return;
                newtrigger.Action(ctx);
                if (ev.IsStopHandle()) return;
            }
        }



    }
}
