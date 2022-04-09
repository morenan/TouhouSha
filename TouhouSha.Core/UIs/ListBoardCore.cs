using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.UIs
{
    public class ListBoardCore : ShaObject
    {
        private Player controller;
        public Player Controller
        {
            get { return this.controller; }
            set { IvProp0("Controller"); this.controller = value; IvProp1("Controller"); }
        }

        private World world;
        public World World
        {
            get { return this.world; }
            set { IvProp0("World"); this.world = value; IvProp1("World"); }
        }

        private string message;
        public string Message
        {
            get { return this.message; }
            set { IvProp0("Message"); this.message = value; IvProp1("Message"); }
        }

        private List<object> items = new List<object>();
        public List<object> Items
        {
            get { return this.items; }
        }

        private List<object> selecteditems = new List<object>();
        public List<object> SelectedItems
        {
            get { return this.selecteditems; }
        }

        private int selectmax = 1;
        public int SelectMax
        {
            get { return this.selectmax; }
            set { IvProp0("SelectMax"); this.selectmax = value; IvProp1("SelectMax"); }
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
    }

    public class ListBoardOpenEventArgs : EventArgs
    {
        public ListBoardOpenEventArgs(ListBoardCore _core) { this.core = _core; }

        private ListBoardCore core;
        public ListBoardCore Core { get { return this.core; } }
    }

    public delegate void ListBoardOpenEventHandler(object sender, ListBoardOpenEventArgs e);
}
