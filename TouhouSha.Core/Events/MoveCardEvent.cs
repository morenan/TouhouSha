using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public abstract class MoveCardEventBase : Event
    {
        public MoveCardEventBase(IEnumerable<Card> _movedcards, Player _controller, Zone _oldzone, Zone _newzone)
        {
            this.movedcards = _movedcards.ToList();
            this.controller = _controller;
            this.oldzone = _oldzone;
            this.newzone = _newzone;
        }

        private List<Card> movedcards;
        public List<Card> MovedCards
        {
            get { return this.movedcards; }
        }

        private Player controller;
        public Player Controller
        {
            get { return this.controller; }
        }

        private Zone oldzone;
        public Zone OldZone
        {
            get { return this.oldzone; }
            set { this.oldzone = value; }
        }

        private Zone newzone;
        public Zone NewZone
        {
            get { return this.newzone; }
            set { this.newzone = value; }
        }

        private Enum_MoveCardFlag flag;
        public Enum_MoveCardFlag Flag
        {
            get { return this.flag; }
            set { this.flag = value; }
        }

        public override Player GetHandleStarter()
        {
            return oldzone?.Owner ?? newzone?.Owner;
        }

        public override bool IsStopHandle()
        {
            if (newzone?.Owner != null && !newzone.Owner.IsAlive) return true;
            return false;
        }
    }

    public class MoveCardEvent : MoveCardEventBase
    {
        public const string DefaultKeyName = "卡片移动前";

        public MoveCardEvent(IEnumerable<Card> _movedcards, Player _controller, Zone _oldzone, Zone _newzone)
            : base(_movedcards, _controller, _oldzone, _newzone)
        {
            KeyName = DefaultKeyName;
        }
    }

    public class MoveCardDoneEvent : MoveCardEventBase
    {
        public const string DefaultKeyName = "卡片移动后";

        public MoveCardDoneEvent(IEnumerable<Card> _movedcards, Player _controller, Zone _oldzone, Zone _newzone)
            : base(_movedcards, _controller, _oldzone, _newzone)
        {
            KeyName = DefaultKeyName;
        }
    }

    public enum Enum_MoveCardFlag
    {
        None = 0,
        MoveToFirst = 1,
        FaceUp = 2,
        Randomly = 4,
    }
}
