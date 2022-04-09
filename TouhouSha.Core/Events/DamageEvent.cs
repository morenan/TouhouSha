using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Events
{
    public class DamageEvent : Event
    {
        public const string DefaultKeyName = "造成伤害时";
        public const string Normal = "无属性";
        public const string Fire = "火属性";
        public const string Thunder = "雷属性";
        public const string Lost = "体力流失";

        public DamageEvent()
        {
            KeyName = DefaultKeyName;
        }
        
        static public string GetDamage(int id)
        {
            switch (id)
            {
                case 0: return Normal;
                case 1: return Fire;
                case 2: return Thunder;
                case 3: return Lost;
            }
            return String.Empty;
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

        private string damagetype;
        public string DamageType
        {
            get { return this.damagetype; }
            set { this.damagetype = value; }
        }

        private int damagevalue;
        public int DamageValue
        {
            get { return this.damagevalue; }
            set { this.damagevalue = value; }
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
