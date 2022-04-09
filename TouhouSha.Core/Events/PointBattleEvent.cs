using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public abstract class PointBattleEventBase : Event
    {
        private Player source;
        public Player Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        private Player target;
        public Player Target
        {
            get { return this.target; }
            set { this.target = value; }
        }

        private Card sourcecard;
        public Card SourceCard
        {
            get { return this.sourcecard; }
            set { this.sourcecard = value; }
        }

        private Card targetcard;
        public Card TargetCard
        {
            get { return this.targetcard; }
            set { this.targetcard = value; }
        }

        public override Player GetHandleStarter()
        {
            return Source;
        }

        public override bool IsStopHandle()
        {
            if (source == null || !source.IsAlive) return true;
            if (target == null || !target.IsAlive) return true;
            return false;
        }
        public bool IsWin(Player player)
        {
            if (SourceCard == null)
                return false;
            if (TargetCard == null)
                return false;
            if (player == Source)
                return SourceCard.CardPoint > TargetCard.CardPoint;
            if (player == Target)
                return !IsWin(Source);
            return false;
        }
    }

    public class PointBattleBeginEvent : PointBattleEventBase
    {
        public const string DefaultKeyName = "拼点发起";
        
        public PointBattleBeginEvent()
        {
            KeyName = DefaultKeyName;
        }
    }

    public class PointBattlePreviewEvent : PointBattleEventBase
    {
        public const string DefaultKeyName = "拼点生效前";

        public PointBattlePreviewEvent()
        {
            KeyName = DefaultKeyName;
        }
    }
    
    public class PointBattleDoneEvent : PointBattleEventBase
    {
        public const string DefaultKeyName = "拼点生效后";

        public PointBattleDoneEvent()
        {
            KeyName = DefaultKeyName;
        }

    }

    public class PointMultiBattleEventBase : Event, IMultiTargetsEvent
    {
        private Player source;
        public Player Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        private List<Player> targets = new List<Player>();
        public List<Player> Targets
        {
            get { return targets; }
        }

        private Card sourcecard;
        public Card SourceCard
        {
            get { return this.sourcecard; }
            set { this.sourcecard = value; }
        }

        private List<Card> targetcards = new List<Card>();
        public List<Card> TargetCards
        {
            get { return this.targetcards; }
        }

        public override Player GetHandleStarter()
        {
            return Source;
        }

        public override bool IsStopHandle()
        {
            if (source == null || !source.IsAlive) return true;
            if (targets.FirstOrDefault(_target => _target.IsAlive) == null) return true;
            return false;
        }
    }

    public class PointMultiBattleBeginEvent : PointMultiBattleEventBase
    {
        public const string DefaultKeyName = "多人拼点发起";

        public PointMultiBattleBeginEvent()
        {
            KeyName = DefaultKeyName;
        }

    }

    public class PointMultiBattlePreviewEvent : PointMultiBattleEventBase
    {
        public const string DefaultKeyName = "多人拼点生效前";

        public PointMultiBattlePreviewEvent()
        {
            KeyName = DefaultKeyName;
        }
    }

    public class PointMultiBattleDoneEvent : PointMultiBattleEventBase
    {
        public const string DefaultKeyName = "多人拼点生效后";

        public PointMultiBattleDoneEvent()
        {
            KeyName = DefaultKeyName;
        }
    }

}
