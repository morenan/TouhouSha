using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TouhouSha.Core
{
    public interface IPackage
    {
        string PackageName { get; }

        bool IsNecessary { get; }

        string GetCountryName(string country);
        
        Color? GetCountryColor(string country);
        
        string GetZoneName(string zonekeyname);
        
        IEnumerable<Charactor> GetCharactors();

        IEnumerable<Card> GetCards();

        IEnumerable<Card> GetCardInstances();

        IEnumerable<Trigger> GetGlobalTriggersBeforePlayer();

        IEnumerable<Trigger> GetGlobalTriggersAfterPlayer();
    }
    
}
