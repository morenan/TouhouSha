using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.UIs
{
    public class ParallelSelectCardBoardCore : ShaObject
    {
        private string message;
        public string Message
        {
            get { return this.message; }
            set { IvProp0("Message"); this.message = value; IvProp1("Message"); }
        }

        private World world;
        public World World
        {
            get { return this.world; }
            set { IvProp0("World"); this.world = value; IvProp1("World"); }
        }


        private List<Player> controllers = new List<Player>();
        public List<Player> Controllers
        {
            get { return this.controllers; }
        }

        private CardFilter cardfilter;
        public CardFilter CardFilter
        {
            get { return this.cardfilter; }
            set { IvProp0("CardFilter"); this.cardfilter = value; IvProp1("CardFilter"); }
        }

        private bool cancancel;
        public bool CanCancel
        {
            get { return this.cancancel; }
            set { IvProp0("CanCancel"); this.cancancel = value; IvProp1("CanCancel"); }
        }

        private int timeout;
        public int Timeout
        {
            get { return this.timeout; }
            set { IvProp0("Timeout"); this.timeout = value; IvProp1("Timeout"); }
        }

        private bool isyes;
        public bool IsYes
        {
            get { return this.isyes; }
            set { IvProp0("IsYes"); this.isyes = value; IvProp1("IsYes"); }
        }

        private List<Card> selectedcards = new List<Card>();
        public List<Card> SelectedCards
        {
            get { return this.selectedcards; }
        }
    }

    public class ParallelSelectCardBoardOpenEventArgs : EventArgs
    {
        public ParallelSelectCardBoardOpenEventArgs(ParallelSelectCardBoardCore _core) { this.core = _core; }

        private ParallelSelectCardBoardCore core;
        public ParallelSelectCardBoardCore Core { get { return this.core; } }
    }

    public delegate void ParallelSelectCardBoardOpenEventHandler(object sender, ParallelSelectCardBoardOpenEventArgs e);
}
