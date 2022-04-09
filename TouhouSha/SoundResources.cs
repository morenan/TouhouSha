using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Reflection;

namespace TouhouSha
{
    public class SoundResources
    {
        static public readonly SoundResources Default = new SoundResources();

        static public string GetApplicationPath()
        {
            return System.IO.Directory.GetParent(System.Windows.Forms.Application.ExecutablePath).FullName;
        }

        public SoundResources()
        {
            StreamReader sr = null;
            try
            {
                sr = new StreamReader(Path.Combine(GetApplicationPath(), "Sounds", "SoundList.txt"));
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    int i = line.IndexOf('=');
                    if (i < 0) continue;
                    string key = line.Substring(0, i);
                    string name = line.Substring(i + 1);
                    string filename = Path.Combine(GetApplicationPath(), "Sounds", name + ".mp3");
                    if (!File.Exists(filename)) filename = Path.Combine(GetApplicationPath(), "Sounds", name + ".wma");
                    if (!File.Exists(filename)) continue;
                    Enum_SoundUsage usage = default(Enum_SoundUsage);
                    if (!Enum.TryParse(key, out usage)) continue;
                    SoundPackage package = this[usage];
                    name = Path.GetFileNameWithoutExtension(filename);
                    package.Sounds.Add(new SoundObject(name, filename));
                }
            }
            catch (Exception exce)
            {
                MessageBox.Show(exce.Message);
            }
        }

        private List<SoundPackage> packagelist = new List<SoundPackage>();
        private List<SEObject> selist = new List<SEObject>();
        
        public SoundPackage this[Enum_SoundUsage usage]
        {
            get
            {
                int i = (int)usage;
                while (packagelist.Count() <= i) packagelist.Add(new SoundPackage((Enum_SoundUsage)i));
                return packagelist[i];
            }
        }

        public SEObject this[Enum_SEUsage usage]
        {
            get
            {
                int i = (int)usage;
                while (selist.Count() <= i) selist.Add(new SEObject((Enum_SEUsage)i));
                return selist[i];
            }
        }
    }
   
    public class SoundPackage
    {
        public SoundPackage(Enum_SoundUsage _usage)
        {
            this.usage = _usage;
        }

        private Enum_SoundUsage usage;
        public Enum_SoundUsage Usage { get { return this.usage; } }

        private List<SoundObject> sounds = new List<SoundObject>();
        public List<SoundObject> Sounds { get { return this.sounds; } }

        private int preferredindex = -1;
        public int PreferredIndex { get { return this.preferredindex; } set { this.preferredindex = value; } }
    }

    public class SoundObject
    {
        public SoundObject(string _name, string _filename)
        {
            this.name = _name;
            this.filename = _filename;
        }


        private string name;
        public string Name { get { return this.name; } }

        private string filename;
        public string Filename { get { return this.filename; } }
    }

    public class SEObject
    {
        public SEObject(Enum_SEUsage _usage)
        {
            this.usage = _usage;
            this.filename = Path.Combine(App.GetApplicationPath(), "Sounds", "SEs", String.Format("{0}.wav", usage));
        }

        private Enum_SEUsage usage;
        public Enum_SEUsage Usage { get { return this.usage; } }

        private string filename;
        public string Filename { get { return this.filename; } }

    }

    public enum Enum_SoundUsage
    {
        Menu,
        Gallery,
        Game,
    }

    public enum Enum_SEUsage
    {
        Drop,
        Broken,
    }

    
}
