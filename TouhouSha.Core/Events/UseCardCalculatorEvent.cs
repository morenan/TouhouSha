using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public class UseCardCalculatorEvent : Event
    {
        public const string DefaultKeyName = "使用卡片转换器时";

        public UseCardCalculatorEvent()
        {
            KeyName = DefaultKeyName;
        }

        private Player source;
        public Player Source
        {
            get { return this.source; }
            set { this.source = value; }
        }
        
        private CardCalculator oldcalculator;
        public CardCalculator OldCalculator
        {
            get { return this.oldcalculator; }
            set { this.oldcalculator = value; }
        }

        private CardCalculator newcalculator;
        public CardCalculator NewCalculator
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
