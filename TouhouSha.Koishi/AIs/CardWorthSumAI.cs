using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core;
using TouhouSha.Core.AIs;

namespace TouhouSha.Koishi.AIs
{
    public abstract class CardWorthSumAI : CardTargetFilterWorth
    {
        public CardWorthSumAI(Card _thiscard)
        {
            this.thiscard = _thiscard;
        }

        private Card thiscard;
        public Card ThisCard { get { return this.thiscard; } }
    }
}
