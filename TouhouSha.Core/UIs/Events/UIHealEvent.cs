using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.UIs.Events
{
    public class UIHealEvent : UIEvent
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

        private int healvalue;
        public int HealValue
        {
            get { return this.healvalue; }
            set { this.healvalue = value; }
        }

        private string Healtype;
        public string HealType
        {
            get { return this.Healtype; }
            set { this.Healtype = value; }
        }
    }
}
