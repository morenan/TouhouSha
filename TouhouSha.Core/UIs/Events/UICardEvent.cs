using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.UIs.Events
{
    public class UICardEvent
    {
        private Card card;
        public Card Card
        {
            get { return this.card; }
            set { this.card = value; }
        }

        private Player carduser;
        public Player CardUser
        {
            get { return this.carduser; }
            set { this.carduser = value; }
        }

        private List<Player> cardtargets = new List<Player>();
        public List<Player> CardTargets
        {
            get { return this.cardtargets; }
        }

        private bool showtargets = true;
        public bool ShowTargets
        {
            get { return this.showtargets; }
            set { this.showtargets = value; }
        }
    }
}
