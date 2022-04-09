using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core.UIs;

namespace TouhouSha.Core.AIs
{
    public abstract class ListSelectWorth
    {
        private AITools aitools;
        public AITools AITools
        {
            get { return this.aitools; }
            set { this.aitools = value; }
        }

        public abstract double GetWorthSelect(Context ctx, ListBoardCore core, string want);
         
    }
}
