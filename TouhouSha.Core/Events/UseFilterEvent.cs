using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public class UseFilterEvent : Event
    {
        public const string DefaultKeyName = "使用过滤器时";

        public UseFilterEvent()
        {
            KeyName = DefaultKeyName;
        }

        private Player source;
        public Player Source
        {
            get { return this.source; }
            set { this.source = value; }
        }
        
        private Filter oldfilter;
        public Filter OldFilter
        {
            get { return this.oldfilter; }
            set { this.oldfilter = value; }
        }

        private Filter newfilter;
        public Filter NewFilter
        {
            get { return this.newfilter; }
            set { this.newfilter = value; }
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
