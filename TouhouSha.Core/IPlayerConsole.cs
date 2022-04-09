using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouhouSha.Core.UIs;

namespace TouhouSha.Core
{
    public interface IPlayerConsole
    {
        Player Owner { get; }
        void MarkCharactors(IList<Charactor> charactors);
        void MarkPlayers(IList<Player> players);
        void MarkSkills(IList<Skill> skills);
        void MarkCards(IList<Card> cards);
        void MarkZones(IList<Zone> zones);
        void MarkFilters(IList<Filter> filters);

        void SelectCharactor(SelectCharactorBoardCore core);
        void SelectCards(SelectCardBoardCore core);
        void SelectPlayers(SelectPlayerBoardCore core);
        void SelectDesktop(DesktopCardBoardCore core);
        void ControlDesktop(DesktopCardBoardCore core);
        void CloseDesktop(DesktopCardBoardCore core);
        void SelectList(ListBoardCore core);
        bool Ask(Context ctx, string keyname, string message, int timeout);
        Event QuestEventInUseCardState(Context ctx);

    }

    public class RemoteUseCardEventArgs : EventArgs
    {
        private Player source;
        public Player Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        private List<Player> targets = new List<Player>();
        public List<Player> Targets
        {
            get { return this.targets; }
        }

        private Card card;
        public Card Card
        {
            get { return this.card; }
            set { this.card = value; }
        }
    }

    public class RemoteUseSkillEventArgs : EventArgs
    {
        private Player source;
        public Player Source
        {
            get { return this.source; }
            set { this.source = value; }
        }

        private Skill skill;
        public Skill Skill
        {
            get { return this.skill; }
            set { this.skill = value; }
        }

        private List<Player> targets = new List<Player>();
        public List<Player> Targets
        {
            get { return this.targets; }
        }

        private List<Card> costs = new List<Card>();
        public List<Card> Costs
        {
            get { return this.costs; }
        }
    }

    public class RemoteLeaveUseCardStateEventArgs : EventArgs
    {

    }

    public delegate void RemoteUseCardEventHandler(object sender, RemoteUseCardEventArgs e);

    public delegate void RemoteUseSkillEventHandler(object sender, RemoteUseSkillEventArgs e);

    public delegate void RemoteLeaveUseCardStateEventHandler(object sender, RemoteLeaveUseCardStateEventArgs e);
}
