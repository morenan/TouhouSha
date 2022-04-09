using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core.UIs;

namespace TouhouSha.Core.AIs
{
    public abstract class PlayerFilterAuto
    {
        private AITools aitools;
        public AITools AITools
        {
            get { return this.aitools; }
            set { this.aitools = value; }
        }

        public abstract bool GetNo(Context ctx, SelectPlayerBoardCore core);

        public abstract List<Player> GetSelection(Context ctx, SelectPlayerBoardCore core);
    }
}
