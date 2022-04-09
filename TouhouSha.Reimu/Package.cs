using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TouhouSha.Core;
using TouhouSha.Reimu.Charactors.SelfCrafts;
using TouhouSha.Reimu.Charactors.Homos;
using TouhouSha.Reimu.Charactors.YouKais;
using TouhouSha.Reimu.Charactors.Wizards;
using TouhouSha.Reimu.Charactors.Forevers;
using TouhouSha.Reimu.Charactors.Chirendens;
using TouhouSha.Reimu.Charactors.Moriya;
using TouhouSha.Reimu.Charactors.Ghosts;
using TouhouSha.Reimu.Charactors.AnotherLands;
using TouhouSha.Reimu.Charactors.Onis;
using TouhouSha.Reimu.Charactors.Fairy;
using TouhouSha.Reimu.Charactors.Myouren;
using TouhouSha.Reimu.Charactors.Tsukumo;
using TouhouSha.Reimu.Charactors.Desires;

namespace TouhouSha.Reimu
{
    public class Package : IPackage
    {
        public string PackageName => "标准角色";

        public bool IsNecessary => false;
        
        public IEnumerable<Card> GetCards()
        {
            yield break;
        }

        public IEnumerable<Card> GetCardInstances()
        {
            yield break;
        }
        
        public IEnumerable<Charactor> GetCharactors()
        {
            yield return new TouhouSha.Reimu.Charactors.SelfCrafts.Reimu();
            yield return new Marisa();
            yield return new Sakuya();
            yield return new Sanae();
            yield return new Yoyoko();
            yield return new YouMon();
            yield return new Reisen();
            yield return new Crino();
            yield return new Aya();
            yield return new Remilia();
            yield return new Flandre();
            yield return new Pachuli();
            yield return new HongMeiLing();
            yield return new DevilLily();
            yield return new Rumiya();
            yield return new Daiyosi();
            yield return new Yucari();
            yield return new Ran();
            yield return new Chen();
            yield return new Lunasa();
            yield return new Merlin();
            yield return new Lyrica();
            yield return new Yuka();
            yield return new Alice();
            yield return new Erin();
            yield return new Kaguya();
            yield return new Keine();
            yield return new Mokou();
            yield return new Tewi();
            yield return new Satori();
            yield return new TouhouSha.Reimu.Charactors.Chirendens.Koishi();
            yield return new NikoRin();
            yield return new Utsuho();
            yield return new Parsee();
            yield return new Yugi();
            yield return new Kisume();
            yield return new Yamame();
            yield return new Kanako();
            yield return new Suwako();
            yield return new Komachi();
            yield return new Shiki();
            yield return new Suika();
            yield return new Kasen();
            yield return new Nazrin();
            yield return new Kogasa();
            yield return new Minamitu();
            yield return new Ichirin();
            yield return new Shou();
            yield return new Hiziri();
            yield return new Nue();
            yield return new Kokoro();
            yield return new Miko();
            yield return new Futo();
            yield return new Toziko();
            yield return new Kyouko();
            yield return new Seiga();
            yield return new Yoshika();
            yield return new Sunny();
            yield return new Luna();
            yield return new Star();
        }

        public Color? GetCountryColor(string country)
        {
            return Colors.White;
        }

        public string GetCountryName(string country)
        {
            return country;
        }

        public IEnumerable<Trigger> GetGlobalTriggersAfterPlayer()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Trigger> GetGlobalTriggersBeforePlayer()
        {
            throw new NotImplementedException();
        }

        public string GetZoneName(string zonekeyname)
        {
            throw new NotImplementedException();
        }
        
    }
}
