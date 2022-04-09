using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.UIs
{
    public class DesktopCardBoardCore : ShaObject
    {
        private World world;
        public World World
        {
            get { return this.world; }
            set { this.world = value; }
        }

        private string message = String.Empty;
        public string Message
        {
            get { return this.message; }
            set { this.message = value; }
        }

        private List<DesktopCardBoardZone> zones = new List<DesktopCardBoardZone>();
        public List<DesktopCardBoardZone> Zones { get { return this.zones; } }

        private Player controller;
        public Player Controller
        {
            get { return this.controller; }
            set { IvProp0("Controller"); this.controller = value; IvProp1("Controller"); }
        }

        private bool isasync = false;
        public bool IsAsync
        {
            get { return this.isasync; }
            set { IvProp0("IsAsync"); this.isasync = value; IvProp1("IsAsync"); }
        }

        private Enum_DesktopCardBoardFlag flag = Enum_DesktopCardBoardFlag.None;
        public Enum_DesktopCardBoardFlag Flag
        {
            get { return this.flag; }
            set { IvProp0("Flag"); this.flag = value; IvProp1("Flag"); }
        }

        private CardFilter cardfilter;
        public CardFilter CardFilter
        {
            get { return this.cardfilter; }
            set { IvProp0("CardFilter"); this.cardfilter = value; IvProp1("CardFilter"); }
        }

        private bool isyes = false;
        public bool IsYes
        {
            get { return this.isyes; }
            set { IvProp0("IsYes"); this.isyes = value; IvProp1("IsYes"); }
        }

        private List<Card> seletedcards = new List<Card>();
        public List<Card> SelectedCards
        {
            get { return this.seletedcards; }
            set { IvProp0("SelectedCards"); this.seletedcards = value; IvProp1("SelectedCards"); }
        }
    }
    
    public class DesktopCardBoardZone : Zone
    {
        public DesktopCardBoardZone(DesktopCardBoardCore _parent) { this.parent = _parent; }

        private DesktopCardBoardCore parent;
        public DesktopCardBoardCore Parent { get { return this.parent; } }

        private string message;
        public string Message
        {
            get { return this.message; }
            set { IvProp0("Message"); this.message = value; IvProp1("Message"); }
        }

        private Enum_DesktopZoneFlag flag;
        public new Enum_DesktopZoneFlag Flag
        {
            get { return this.flag; }
            set { IvProp0("Flag"); this.flag = value; IvProp1("Flag"); }
        }

        private DesktopCardBoardZoneGetter cardgetter = new DesktopCardBoardZoneGetter();
        public DesktopCardBoardZoneGetter CardGetter
        {
            get { return this.cardgetter; }
            set { IvProp0("CardGetter"); this.cardgetter = value; IvProp1("CardGetter"); }
        }

        private DesktopCardBoardZoneLoster cardloster = new DesktopCardBoardZoneLoster();
        public DesktopCardBoardZoneLoster CardLoster
        {
            get { return this.cardloster; }
            set { IvProp0("CardLoster"); this.cardloster = value; IvProp1("CardLoster"); }
        }
    }

    public class DesktopCardBoardZoneLoster
    {
        public virtual bool CanLost(DesktopCardBoardZone zone, Card want)
        {
            return true;
        }
    }

    public class DesktopCardBoardZoneGetter
    {
        public virtual bool CanGet(DesktopCardBoardZone zone, Card want, int insertindex)
        {
            return true;
        }

        public virtual bool Fulfill(DesktopCardBoardZone zone)
        {
            return true;
        }
    }

    public enum Enum_DesktopCardBoardFlag
    {
        None = 0,
        SelectCardAndYes = 1,
        CannotNo = 2,
        CardInBoard = 4,
        CanDrag = 8,
    }

    public enum Enum_DesktopZoneFlag
    {
        None = 0,
        FaceDown = 1,
        AsSelected = 2,
        CanSort = 4,
        CanZip = 8,
    }

    
}
