using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.UIs
{
    public class SelectCharactorBoardCore : ShaObject
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

        private List<Charactor> charactors = new List<Charactor>();
        public List<Charactor> Charactors
        {
            get { return this.charactors; }
        }

        private int timeout = 60;
        public int Timeout
        {
            get { return this.timeout; }
            set { IvProp0("Timeout"); this.timeout = value; IvProp1("Timeout"); }
        }

        private Charactor selectedcharactor;
        public Charactor SelectedCharactor
        {
            get { return this.selectedcharactor; }
            set { IvProp0("SelectedCharactor"); this.selectedcharactor = value; IvProp1("SelectedCharactor"); }
        }

    }
    public class SelectCharactorBoardOpenEventArgs : EventArgs
    {
        public SelectCharactorBoardOpenEventArgs(SelectCharactorBoardCore _core) { this.core = _core; }

        private SelectCharactorBoardCore core;
        public SelectCharactorBoardCore Core { get { return this.core; } }
    }

    public delegate void SelectCharactorBoardOpenEventHandler(object sender, SelectCharactorBoardOpenEventArgs e);
}
