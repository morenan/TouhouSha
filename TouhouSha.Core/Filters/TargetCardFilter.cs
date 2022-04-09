using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;

namespace TouhouSha.Core.Filters
{
    public class TargetCardFilter : CardFilter, ICardFilterRequiredCardTypes
    {
        public TargetCardFilter(int _mincount, int _maxcount, params string[] _cardkeynames)
        {
            this.mincount = _maxcount;
            this.maxcount = _maxcount;
            this.cardkeynames = _cardkeynames.ToList();
        }

        private List<string> cardkeynames = new List<string>();
        private int mincount = 1;
        private int maxcount = 1;

        IEnumerable<string> ICardFilterRequiredCardTypes.RequiredCardTypes
        {
            get { return cardkeynames; }
        }

        public override bool CanSelect(Context ctx, IEnumerable<Card> selecteds, Card want)
        {
            if (selecteds.Count() + 1 > maxcount) return false;
            if (!cardkeynames.Contains(want.KeyName)) return false;
            return true;
        }

        public override bool Fulfill(Context ctx, IEnumerable<Card> selecteds)
        {
            return (selecteds.Count() >= mincount);
        }

    }
}
