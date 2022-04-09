using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.UIs
{
    public class CharactorInfoCore
    {
        public CharactorInfoCore(Charactor _charactor)
        {
            this.charactor = _charactor;
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }
        
        private Charactor charactor;
        public Charactor Charactor
        {
            get { return this.charactor; }
        }

        private CardImage image;
        public CardImage Image
        {
            get { return this.image; }
            set { this.image = value; }
        }

        private List<SkillInfo> skills = new List<SkillInfo>();
        public List<SkillInfo> Skills
        {
            get { return this.skills; }
        }

        private AbilityRadar abilityradar;
        public AbilityRadar AbilityRadar
        {
            get { return this.abilityradar; }
            set { this.abilityradar = value; }
        }
    }

    public class SkillInfo
    {
        private string name = "技能";
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        private string description = "技能描述";
        public string Description 
        {
            get { return this.description; }
            set { this.description = value; }
        }

        private List<SkillInfo> attachedskills = new List<SkillInfo>();
        public List<SkillInfo> AttachedSkills
        {
            get { return this.attachedskills; }
        }
    }

    public class AbilityRadar
    {
        public double Attack { get; set; }

        public double Defence { get; set; }

        public double Control { get; set; }

        public double Auxiliary { get; set; }
        
        public double LastStages { get; set; }

        public double Difficulty { get; set; }
    }
}
