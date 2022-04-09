using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.AIs
{
    public abstract class AskWorth
    {

        private AITools aitools;
        public AITools AITools
        {
            get { return this.aitools; }
            set { this.aitools = value; }
        }

        public abstract double GetWorthYes(Context ctx, Player controller, string keyname);
        public abstract double GetWorthNo(Context ctx, Player controller, string keyname);

    }
}
