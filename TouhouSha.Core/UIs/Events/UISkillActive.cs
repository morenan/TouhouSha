using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.UIs.Events
{
    public class UISkillActive : UIEvent
    {
        private Skill skill;
        public Skill Skill
        {
            get { return this.skill; }
            set { this.skill = value; }
        }

        private Player skillactiver;
        public Player SkillActiver
        {
            get { return this.skillactiver; }
            set { this.skillactiver = value; }
        }

        private List<Player> skilltargets = new List<Player>();
        public List<Player> SkillTargets
        {
            get { return this.skilltargets; }
        }

        private bool showtargets = true;
        public bool ShowTargets
        {
            get { return this.showtargets; }
            set { this.showtargets = value; }
        }
    }
}
