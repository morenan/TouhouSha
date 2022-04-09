using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.AIs;

namespace TouhouSha.Core.Filters
{
    public class FulfillNumberCardFilter : CardFilter
    {
        public FulfillNumberCardFilter(int _mincount, int _maxcount)
        {
            this.mincount = _maxcount;
            this.maxcount = _maxcount;
        }

        private int mincount = 1;
        private int maxcount = 1;

        private bool allow_hand = true;
        public bool Allow_Hand
        {
            get { return this.allow_hand; }
            set { this.allow_hand = value; }
        }

        private bool allow_equiped = false;
        public bool Allow_Equiped
        {
            get { return this.allow_equiped; }
            set { this.allow_equiped = value; }
        }

        private bool allow_judging = false;
        public bool Allow_Judging
        {
            get { return this.allow_judging; }
            set { this.allow_judging = value; }
        }

        private List<Zone> allow_otherzones = new List<Zone>();
        public List<Zone> Allow_OtherZones
        {
            get { return this.allow_otherzones; }
        }

        private CardFilterWorth worthai;
        public CardFilterWorth WorthAI
        {
            get { return this.worthai; }
            set { this.worthai = value; }
        }

        private CardFilterAuto autoai;
        public CardFilterAuto AutoAI
        {
            get { return this.autoai; }
            set { this.autoai = value; }
        }

        private DesktopCardFilterWorth desktopworthai;
        public DesktopCardFilterWorth DesktopWorthAI
        {
            get { return this.desktopworthai; }
            set { this.desktopworthai = value; }
        }

        private DesktopCardFilterAuto desktopautoai;
        public DesktopCardFilterAuto DesktopAutoAI
        {
            get { return this.desktopautoai; }
            set { this.desktopautoai = value; }
        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() + 1 > maxcount) return false;
            Zone zone = want.Zone;
            if (zone == null) return false;
            if (allow_hand && zone.KeyName?.Equals(Zone.Hand) == true) return true;
            if (allow_equiped && zone.KeyName?.Equals(Zone.Equips) == true) return true;
            if (allow_judging && zone.KeyName?.Equals(Zone.Judge) == true) return true;
            if (allow_otherzones.Contains(zone)) return true;
            return false;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return (selecteds.Count() >= mincount);
        }

        public override CardFilterWorth GetWorthAI()
        {
            return WorthAI;
        }

        public override CardFilterAuto GetAutoAI()
        {
            return AutoAI;
        }

        public override DesktopCardFilterWorth GetDesktopWorthAI()
        {
            return DesktopWorthAI;
        }

        public override DesktopCardFilterAuto GetDesktopAutoAI()
        {
            return DesktopAutoAI;
        }
    }
}
