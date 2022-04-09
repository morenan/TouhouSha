﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core.UIs;

namespace TouhouSha.Core.AIs
{
    public abstract class CardFilterAuto
    {
        private AITools aitools;
        public AITools AITools
        {
            get { return this.aitools; }
            set { this.aitools = value; }
        }

        public abstract bool GetNo(Context ctx, SelectCardBoardCore core);
        public abstract List<Card> GetSelection(Context ctx, SelectCardBoardCore core);
    }
}
