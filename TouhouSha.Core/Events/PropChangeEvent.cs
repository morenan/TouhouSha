using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public class PropChangeEvent : Event
    {
        public const string DefaultKeyName = "对象的属性更改时";
        
        public PropChangeEvent()
        {
            KeyName = DefaultKeyName;
        }


        private ShaObject source;
        public ShaObject Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        private string propertyname;
        public string PropertyName
        {
            get { return this.propertyname; }
            set { this.propertyname = value; }
        }

        private int oldvalue;
        public int OldValue
        {
            get { return this.oldvalue; }
            set { this.oldvalue = value; }
        }

        private int newvalue;
        public int NewValue
        {
            get { return this.newvalue; }
            set { this.newvalue = value; }
        }

        public override Player GetHandleStarter()
        {
            return Source as Player;
        }

        public override bool IsStopHandle()
        {
            if (Source is Player && !((Player)Source).IsAlive) return true;
            return false;
        }
    }
}
