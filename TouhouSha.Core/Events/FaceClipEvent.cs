using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public class FaceClipEvent : Event
    {
        public const string DefaultKeyName = "角色翻面时";

        public FaceClipEvent()
        {
            KeyName = DefaultKeyName;
        }

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

        public override Player GetHandleStarter()
        {
            return Source;
        }
        public override bool IsStopHandle()
        {
            if (Source != null && !Source.IsAlive) return true;
            if (Target == null || !Target.IsAlive) return true;
            return false;
        }
    }
}
