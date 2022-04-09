using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core
{
    public class ShaObject : INotifyPropertyChanging, INotifyPropertyChanged
    {
        private string keyname;
        public string KeyName
        {
            get { return this.keyname; }
            set { IvProp0("KeyName"); this.keyname = value; IvProp1("KeyName"); }
        }

        private Dictionary<string, int> properties;

        public int GetValue(string name)
        {
            int value;
            if (properties == null) return 0;
            if (!properties.TryGetValue(name, out value)) return 0;
            return value;
        }

        public void SetValue(string name, int value)
        {
            SetValue(name, value, null);
        }

        public void SetValue(string name, int value, Event ev)
        {
            int oldvalue = GetValue(name);
            if (value == oldvalue && ev == null) return;
            if (properties == null)
                properties = new Dictionary<string, int>();
            if (properties.ContainsKey(name))
                properties[name] = value;
            else
                properties.Add(name, value);
            ShaPropertyChanged?.Invoke(this, new ShaPropertyChangedEventArgs(this, name, oldvalue, value, ev));
        }

        public void ClearAllValues(Event ev)
        {
            if (properties == null) return;
            List<KeyValuePair<string, int>> oldvalues = properties.ToList();
            properties.Clear();
            foreach (KeyValuePair<string, int> kvp in oldvalues)
                ShaPropertyChanged?.Invoke(this, new ShaPropertyChangedEventArgs(this, kvp.Key, kvp.Value, 0, ev));
        }

        public void LoadValues(ShaObject that)
        {
            if (that.properties == null) return;
            if (properties == null)
                properties = new Dictionary<string, int>();
            foreach (KeyValuePair<string, int> kvp in that.properties)
                SetValue(kvp.Key, kvp.Value);
        }

        public event PropertyChangingEventHandler PropertyChanging;

        public event PropertyChangedEventHandler PropertyChanged;

        public event ShaPropertyChangedEventHandler ShaPropertyChanged;

        protected void IvProp0(string name) { PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(name)); }

        protected void IvProp1(string name) { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)); }
    }

    public class ShaPriorityObject : ShaObject
    {
        private int priority;
        public int Priority
        {
            get { return this.priority; }
            set { IvProp0("Priority"); this.priority = value; IvProp1("Priority"); }
        }
    }

    public class ShaPropertyChangedEventArgs : EventArgs
    {
        public ShaPropertyChangedEventArgs(ShaObject _source, string _propertyname, int _oldvalue, int _newvalue, Event _ev)
        {
            this.source = _source;
            this.propertyname = _propertyname;
            this.oldvalue = _oldvalue;
            this.newvalue = _newvalue;
            this.ev = _ev;
        }

        private ShaObject source;
        public ShaObject Source { get { return this.source; } }

        private string propertyname;
        public string PropertyName { get { return this.propertyname; } }

        private int oldvalue;
        public int OldValue { get { return this.oldvalue; } }

        private int newvalue;
        public int NewValue { get { return this.newvalue; } }

        private Event ev;
        public Event Ev { get { return this.ev; } }
    }

    public delegate void ShaPropertyChangedEventHandler(object sender, ShaPropertyChangedEventArgs e);
}
