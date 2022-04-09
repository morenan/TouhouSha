using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public class UseTriggerEvent : Event
    {
        public const string DefaultKeyName = "使用触发器时";

        public UseTriggerEvent()
        {
            KeyName = DefaultKeyName;
        }

        private Player source;
        public Player Source
        {
            get { return this.source; }
            set { this.source = value; }
        }
        
        private Trigger oldtrigger;
        public Trigger OldTrigger
        {
            get { return this.oldtrigger; }
            set { this.oldtrigger = value; }
        }

        private Trigger newtrigger;
        public Trigger NewTrigger
        {
            get { return this.oldtrigger; }
            set { this.oldtrigger = value; }
        }

        public override Player GetHandleStarter()
        {
            return Source;
        }
        public override bool IsStopHandle()
        {
            return false;
        }
    }
}
