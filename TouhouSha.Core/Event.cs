using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core
{
    public abstract class Event : ShaObject
    {
        public override string ToString()
        {
            return String.Format("Event:{0}", KeyName);
        }

        private Event reason;
        public Event Reason
        {
            get { return this.reason; }
            set { this.reason = value; }
        }

        private bool cancel = false;
        public bool Cancel
        {
            get { return this.cancel; }
            set { this.cancel = value; }
        }

        private bool noticeui = true;
        public bool NoticeUI
        {
            get { return this.noticeui; }
            set { this.noticeui = value; }
        }

        public abstract Player GetHandleStarter();
        public abstract bool IsStopHandle();
    }
}
