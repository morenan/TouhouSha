using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.AIs
{
    public interface IAssGausser
    {

        double ProbablyOfSlave(Context ctx, Player player);

        double ProbablyOfAvenger(Context ctx, Player player);

        double ProbablyOfSpy(Context ctx, Player player);

        void LetKnow(Context ctx, Event ev);
    }
}
