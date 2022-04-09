using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TouhouSha.Core.UIs;
using TouhouSha.Core.AIs;

namespace TouhouSha.Core
{
    public abstract class Charactor : ShaObject
    {
        public override string ToString()
        {
            return String.Format("Charactor:{0}", KeyName);
        }
        private ImageSource cardimage;
        public ImageSource CardImage
        {
            get { return this.cardimage; }
            set { this.cardimage = value; }
        }
        
        public int HP
        {
            get { return GetValue("HP"); }
            set { SetValue("HP", value); }
        }

        public int MaxHP
        {
            get { return GetValue("MaxHP"); }
            set { SetValue("MaxHP", value); }
        }

        private string country;
        public string Country
        {
            get { return this.country; }
            set { this.country = value; }
        }

        private List<string> othercountries = new List<string>();
        public List<string> OtherCountries
        {
            get { return this.othercountries; }
        }
        
        private List<Skill> skills = new List<Skill>();
        public List<Skill> Skills
        {
            get { return this.skills; }
        }

        public abstract Charactor Clone();

        public abstract CharactorInfoCore GetInfo();
        
        public virtual AskWorth GetAskWorthAI()
        {
            return null;
        }

        public virtual ListSelectWorth GetListWorthAI()
        {
            return null;
        }

        public virtual CardFilterAuto GetDiscardAuto()
        {
            return null;
        }

    }
}
