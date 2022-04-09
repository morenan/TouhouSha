using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.UIs.Events
{
    public class UIMoveCardEvent : UIEvent
    {
        public UIMoveCardEvent(IEnumerable<Card> _movedcards, Zone _oldzone, Zone _newzone)
        {
            this.movedcards = _movedcards.ToList();
            this.oldzone = _oldzone;
            this.newzone = _newzone;
            this.iscardvisibled = false;
            if (oldzone != null && (oldzone.Flag & Enum_ZoneFlag.CardUnvisibled) == Enum_ZoneFlag.None)
                this.iscardvisibled = true;
            if (newzone != null && (newzone.Flag & Enum_ZoneFlag.CardUnvisibled) == Enum_ZoneFlag.None)
                this.iscardvisibled = true;
        }

        private List<Card> movedcards;
        public List<Card> MovedCards
        {
            get { return this.movedcards; }
        }

        private Zone oldzone;
        public Zone OldZone
        {
            get { return this.oldzone; }
        }

        private Zone newzone;
        public Zone NewZone
        {
            get { return this.newzone; }
        }

        private bool iscardvisibled;
        public bool IsCardVisibled
        {
            get { return this.iscardvisibled; }
            set { this.iscardvisibled = value; }
        }
    }
}
