using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.AIs;
using TouhouSha.Core.Events;

namespace TouhouSha.Koishi
{
    public static class KoishiTools
    {
        static public State New(this State oldstate, string keyname)
        {
            State newstate = oldstate.Clone();
            newstate.KeyName = keyname;
            newstate.Step = 0;
            return newstate;
        }
    }
}
