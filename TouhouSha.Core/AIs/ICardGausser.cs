using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.AIs
{
    public interface ICardGausser
    {
        Player Owner { get; }
        double GetProbablyOfCardKey(Context ctx, Player player, string cardkey);
        double GetProbablyOfCardColor(Context ctx, Player player, Enum_CardColor cardcolor);
        double GetProbablyOfCardPointNotLess(Context ctx, Player player, int cardpoint);
        double[] GetProbablyArrayOfCardKey(Context ctx, Player player, string cardkey);
        double[] GetProbablyArrayOfCardColor(Context ctx, Player player, Enum_CardColor cardcolor);
        double[] GetProbablyArrayOfCardPointNotLess(Context ctx, Player player, int cardpoint);
        void LetKnow(Context ctx, Event ev);
    }

}
