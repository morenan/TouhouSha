using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.UIs.Events
{
    public class UIDamageEvent : UIEvent
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

        private int damagevalue;
        public int DamageValue
        {
            get { return this.damagevalue; }
            set { this.damagevalue = value; }
        }

        private string damagetype;
        public string DamageType
        {
            get { return this.damagetype; }
            set { this.damagetype = value; }
        }
    }
}
