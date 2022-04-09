using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.AIs;

namespace TouhouSha.Core.Filters
{
    public class FulfillNumberPlayerFilter : PlayerFilter
    {
        public FulfillNumberPlayerFilter(int _minvalue, int _maxvalue, Player _selfcannot)
        {
            this.minvalue = _minvalue;
            this.maxvalue = _maxvalue;
            this.selfcannot = _selfcannot;
        }

        private int minvalue;
        public int MinValue { get { return this.minvalue; } }

        private int maxvalue;
        public int MaxValue { get { return this.maxvalue; } }

        private Player selfcannot;
        public Player SelfCannot { get { return this.selfcannot; } }

        private PlayerFilterWorth worthai;
        public PlayerFilterWorth WorthAI
        {
            get { return this.worthai; }
            set { this.worthai = value; }
        }

        private PlayerFilterAuto autoai; 
        public PlayerFilterAuto AutoAI
        {
            get { return this.autoai; }
            set { this.autoai = value; }
        }

        public override bool CanSelect(Context ctx, IEnumerable<Player> selecteds, Player want)
        {
            if (selecteds.Count() >= maxvalue) return false;
            if (want == selfcannot) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Player> selecteds)
        {
            return selecteds.Count() >= minvalue;
        }

        public override PlayerFilterWorth GetWorthAI()
        {
            return WorthAI;
        }

        public override PlayerFilterAuto GetAutoAI()
        {
            return AutoAI;
        }
    }
}
