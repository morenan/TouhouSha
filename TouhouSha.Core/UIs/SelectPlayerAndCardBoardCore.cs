using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.UIs
{
    public class SelectPlayerAndCardBoardCore : ShaObject
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

        private Player controller;
        public Player Controller
        {
            get { return this.controller; }
            set { IvProp0("Controller"); this.controller = value; IvProp1("Controller"); }
        }

        private CardFilter cardfilter;
        public CardFilter CardFilter
        {
            get { return this.cardfilter; }
            set { IvProp0("CardFilter"); this.cardfilter = value; IvProp1("CardFilter"); }
        }

        private PlayerFilter playerfilter;
        public PlayerFilter PlayerFilter
        {
            get { return this.playerfilter; }
            set { IvProp0("PlayerFilter"); this.playerfilter = value; IvProp1("PlayerFilter"); }
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

        private List<Player> selectedplayers = new List<Player>();
        public List<Player> SelectedPlayers
        {
            get { return this.selectedplayers; }
        }

    }
}
