using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core
{
    public class Trigger : ShaPriorityObject
    {
        public override string ToString()
        {
            return String.Format("Trigger:{0}", KeyName);
        }

        private ConditionFilter condition;
        public ConditionFilter Condition
        {
            get { return this.condition; }
            set { this.condition = value; }
        }

        public virtual void Cost(Context ctx)
        {
            
        }

        public virtual void Action(Context ctx)
        {
            
        }
    }
    
    public class OverrideTrigger : Trigger
    {
        private Trigger oldtrigger;
        public Trigger OldTrigger
        {
            get { return this.oldtrigger; }
            set { this.oldtrigger = value; }
        }
    }

    public class SkillTrigger : Trigger, ISkillTrigger
    {
        public SkillTrigger(Skill _skill)
        {
            this.skill = _skill;
        }

        protected Skill skill;
        public Skill Skill { get { return this.skill; } }
    }
    
    public class CardTrigger : Trigger, ICardTrigger
    {
        public CardTrigger(Card _card)
        {
            this.card = _card;
        }

        protected Card card;
        public Card Card { get { return this.card; } }
        
    }

    public class TriggerCollection
    {
        private Dictionary<KeyValuePair<string, int>, SortedDictionary<int, List<Trigger>>> triggerofstates 
            = new Dictionary<KeyValuePair<string, int>, SortedDictionary<int, List<Trigger>>>();
        private Dictionary<string, SortedDictionary<int, List<Trigger>>> triggerofevents 
            = new Dictionary<string, SortedDictionary<int, List<Trigger>>>();
        private SortedDictionary<int, List<Trigger>> triggerofanystates
            = new SortedDictionary<int, List<Trigger>>();
        private SortedDictionary<int, List<Trigger>> triggerofanyevents
            = new SortedDictionary<int, List<Trigger>>();
        private SortedDictionary<int, List<Trigger>> othertriggers
            = new SortedDictionary<int, List<Trigger>>();

        public void Clear()
        {
            triggerofstates.Clear();
            triggerofevents.Clear();
            triggerofanystates.Clear();
            triggerofanyevents.Clear();
            othertriggers.Clear();
        }

        public void Add(Trigger trigger)
        {
            bool isspecial = false;
            if (trigger is ITriggerInState)
            {
                ITriggerInState instate = (ITriggerInState)trigger;
                string statekey = instate.StateKeyName;
                int statestep = instate.StateStep;
                if (String.IsNullOrEmpty(statekey)) statekey = String.Empty;
                KeyValuePair<string, int> key = new KeyValuePair<string, int>(statekey, statestep);
                SortedDictionary<int, List<Trigger>> priorities = null;
                List<Trigger> list = null;
                if (!triggerofstates.TryGetValue(key, out priorities))
                {
                    priorities = new SortedDictionary<int, List<Trigger>>();
                    triggerofstates.Add(key, priorities);
                }
                if (!priorities.TryGetValue(trigger.Priority, out list))
                {
                    list = new List<Trigger>();
                    priorities.Add(trigger.Priority, list);
                }
                list.Add(trigger);
                isspecial = true;
            }
            else if (trigger is ITriggerInAnyState)
            {
                ITriggerInAnyState anystate = (ITriggerInAnyState)trigger;
                List<Trigger> list = null;
                if (!triggerofanystates.TryGetValue(trigger.Priority, out list))
                {
                    list = new List<Trigger>();
                    triggerofanystates.Add(trigger.Priority, list);
                }
                list.Add(trigger);
                isspecial = true;
            }
            if (trigger is ITriggerInEvent)
            {
                ITriggerInEvent inevent = (ITriggerInEvent)trigger;
                string eventkey = inevent.EventKeyName;
                if (String.IsNullOrEmpty(eventkey)) eventkey = String.Empty;
                SortedDictionary<int, List<Trigger>> priorities = null;
                List<Trigger> list = null;
                if (!triggerofevents.TryGetValue(eventkey, out priorities))
                {
                    priorities = new SortedDictionary<int, List<Trigger>>();
                    triggerofevents.Add(eventkey, priorities);
                }
                if (!priorities.TryGetValue(trigger.Priority, out list))
                {
                    list = new List<Trigger>();
                    priorities.Add(trigger.Priority, list);
                }
                list.Add(trigger);
                isspecial = true;
            }
            else if (trigger is ITriggerInAnyEvent)
            {
                ITriggerInAnyEvent anyevent = (ITriggerInAnyEvent)trigger;
                List<Trigger> list = null;
                if (!triggerofanyevents.TryGetValue(trigger.Priority, out list))
                {
                    list = new List<Trigger>();
                    triggerofanyevents.Add(trigger.Priority, list);
                }
                list.Add(trigger);
                isspecial = true;
            }
            if (!isspecial)
            {
                List<Trigger> list = null;
                if (!othertriggers.TryGetValue(trigger.Priority, out list))
                {
                    list = new List<Trigger>();
                    othertriggers.Add(trigger.Priority, list);
                }
                list.Add(trigger);
            }
        }
        
        public bool Remove(Trigger trigger)
        {
            bool isspecial = false;
            bool hasremoved = false;
            if (trigger is ITriggerInState)
            {
                ITriggerInState instate = (ITriggerInState)trigger;
                string statekey = instate.StateKeyName;
                int statestep = instate.StateStep;
                if (String.IsNullOrEmpty(statekey)) statekey = String.Empty;
                KeyValuePair<string, int> key = new KeyValuePair<string, int>(statekey, statestep);
                SortedDictionary<int, List<Trigger>> priorities = null;
                List<Trigger> list = null;
                if (triggerofstates.TryGetValue(key, out priorities)
                 && priorities.TryGetValue(trigger.Priority, out list))
                    hasremoved |= list.Remove(trigger);
                isspecial = true;
            }
            else if (trigger is ITriggerInAnyState)
            {
                ITriggerInAnyState anystate = (ITriggerInAnyState)trigger;
                List<Trigger> list = null;
                if (triggerofanystates.TryGetValue(trigger.Priority, out list))
                    hasremoved |= list.Remove(trigger);
                isspecial = true;
            }
            if (trigger is ITriggerInEvent)
            {
                ITriggerInEvent inevent = (ITriggerInEvent)trigger;
                string eventkey = inevent.EventKeyName;
                if (String.IsNullOrEmpty(eventkey)) eventkey = String.Empty;
                SortedDictionary<int, List<Trigger>> priorities = null;
                List<Trigger> list = null;
                if (triggerofevents.TryGetValue(eventkey, out priorities)
                 && priorities.TryGetValue(trigger.Priority, out list))
                    hasremoved |= list.Remove(trigger);
                isspecial = true;
            }
            else if (trigger is ITriggerInAnyEvent)
            {
                ITriggerInAnyEvent anyevent = (ITriggerInAnyEvent)trigger;
                List<Trigger> list = null;
                if (triggerofanyevents.TryGetValue(trigger.Priority, out list))
                    hasremoved |= list.Remove(trigger);
                isspecial = true;
            }
            if (!isspecial)
            {
                List<Trigger> list = null;
                if (othertriggers.TryGetValue(trigger.Priority, out list))
                    hasremoved |= list.Remove(trigger);
            }
            return hasremoved;
        }

        public IEnumerable<Trigger> GetAcceptedTriggerFollowPriority(Context ctx,
            SortedDictionary<int, List<Trigger>> sort0)
        {
            foreach (List<Trigger> list in sort0.Values.Reverse())
                foreach (Trigger trigger in list)
                {
                    if (trigger.Condition == null) continue;
                    if (!trigger.Condition.Accept(ctx)) continue;
                    yield return trigger;
                }
        }

        public IEnumerable<Trigger> GetAcceptedTriggerFollowPriority(Context ctx,
            SortedDictionary<int, List<Trigger>> sort0,
            SortedDictionary<int, List<Trigger>> sort1)
        {
            List<Trigger> accepteds = new List<Trigger>();
            SortedDictionary<int, List<Trigger>>.Enumerator enum0 = sort0.GetEnumerator();
            SortedDictionary<int, List<Trigger>>.Enumerator enum1 = sort1.GetEnumerator();
            while (enum0.Current.Value != null && enum1.Current.Value != null)
            {
                if (enum1.Current.Value == null || enum1.Current.Key > enum0.Current.Key)
                {
                    foreach (Trigger trigger in enum0.Current.Value)
                    {
                        if (trigger.Condition == null) continue;
                        if (!trigger.Condition.Accept(ctx)) continue;
                        accepteds.Add(trigger);
                    }
                }
                else
                {
                    foreach (Trigger trigger in enum1.Current.Value)
                    {
                        if (trigger.Condition == null) continue;
                        if (!trigger.Condition.Accept(ctx)) continue;
                        accepteds.Add(trigger);
                    }
                }
            }
            accepteds.Reverse();
            return accepteds;
        }

        public IEnumerable<Trigger> GetAcceptedTriggerOfEvent(Context ctx)
        {
            Event ev = ctx.Ev;
            if (ev == null) return GetAcceptedTriggerOfOthers(ctx);
            string eventkey = ev.KeyName;
            if (String.IsNullOrEmpty(eventkey)) eventkey = String.Empty;
            SortedDictionary<int, List<Trigger>> sort0 = null;
            SortedDictionary<int, List<Trigger>> sort1 = triggerofanyevents;
            if (!triggerofevents.TryGetValue(String.Empty, out sort0))
                return GetAcceptedTriggerFollowPriority(ctx, sort1);
            return GetAcceptedTriggerFollowPriority(ctx, sort0, sort1);
        }

        public IEnumerable<Trigger> GetAcceptedTriggerOfState(Context ctx)
        {
            State state = ctx.World.GetCurrentState();
            if (state == null) return GetAcceptedTriggerOfOthers(ctx);
            string statekey = state.KeyName;
            int statestep = state.Step;
            if (String.IsNullOrEmpty(statekey)) statekey = String.Empty;
            SortedDictionary<int, List<Trigger>> sort0 = null;
            SortedDictionary<int, List<Trigger>> sort1 = triggerofanystates;
            KeyValuePair<string, int> key = new KeyValuePair<string, int>(statekey, statestep);
            if (!triggerofstates.TryGetValue(key, out sort0))
                return GetAcceptedTriggerFollowPriority(ctx, sort1);
            return GetAcceptedTriggerFollowPriority(ctx, sort0, sort1);
        }
        
        public IEnumerable<Trigger> GetAcceptedTriggerOfOthers(Context ctx)
        {
            return GetAcceptedTriggerFollowPriority(ctx, othertriggers);
        }
    }

    public interface ISkillTrigger
    {
        Skill Skill { get; }
    }
  
    public interface ICardTrigger
    {
        Card Card { get; }
    }

    public interface ITriggerInState
    {
        string StateKeyName { get; }
        int StateStep { get; }
    }

    public interface ITriggerInAnyState
    {
        
    }
    
    public interface ITriggerInEvent
    {
        string EventKeyName { get; }
    }

    public interface ITriggerInAnyEvent
    {

    }

    public interface ITriggerAsk
    {
        string Message { get; }

        //Player Asked { get; }

        Player GetAsked(Context ctx);
    }
}
