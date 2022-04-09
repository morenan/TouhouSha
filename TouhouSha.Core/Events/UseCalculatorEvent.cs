using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public class UseCalculatorEvent : Event
    {
        public const string DefaultKeyName = "使用计算器时";

        public UseCalculatorEvent()
        {
            KeyName = DefaultKeyName;
        }

        private Player source;
        public Player Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        private Calculator oldcalculator;
        public Calculator OldCalculator
        {
            get { return this.oldcalculator; }
            set { this.oldcalculator = value; }
        }

        private Calculator newcalculator;
        public Calculator NewCalculator
        {
            get { return this.newcalculator; }
            set { this.newcalculator = value; }
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
