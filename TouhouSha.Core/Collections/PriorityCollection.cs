using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TouhouSha.Core.Collections
{
    public class PriorityCollection<TValue> : INotifyCollectionChanged, ICollection<TValue> where TValue : ShaPriorityObject
    {
        private SortedDictionary<int, Dictionary<string, TValue>> dict = new SortedDictionary<int, Dictionary<string, TValue>>(new InverseCompareInt());
        private SortedDictionary<int, List<TValue>> emptykeys = new SortedDictionary<int, List<TValue>>(new InverseCompareInt());
        private SortedDictionary<int, List<TValue>> multikeys = new SortedDictionary<int, List<TValue>>(new InverseCompareInt());

        public int Count => dict.Sum(_sub_dict => _sub_dict.Value?.Count() ?? 0);

        public bool IsReadOnly => false;

        protected void _Add(TValue item)
        {
            if (String.IsNullOrEmpty(item.KeyName))
            {
                List<TValue> list = null;
                if (!emptykeys.TryGetValue(item.Priority, out list))
                {
                    list = new List<TValue>();
                    emptykeys.Add(item.Priority, list);
                }
                list.Add(item);
                return;
            }
            Dictionary<string, TValue> subdict = null;
            if (!dict.TryGetValue(item.Priority, out subdict))
            {
                subdict = new Dictionary<string, TValue>();
                dict.Add(item.Priority, subdict);
            }
            if (subdict.ContainsKey(item.KeyName))
            {
                List<TValue> list = null;
                if (!multikeys.TryGetValue(item.Priority, out list))
                {
                    list = new List<TValue>();
                    multikeys.Add(item.Priority, list);
                }
                list.Add(item);
                return;
            }
            subdict.Add(item.KeyName, item);
            
        }

        public void Add(TValue item)
        {
            if (item == null) return;
            _Add(item);
            item.PropertyChanging += OnValuePropertyChanging;
            item.PropertyChanged += OnValuePropertyChanged;
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            List<TValue> removeds = new List<TValue>();
            foreach (TValue value in this)
            {
                value.PropertyChanging -= OnValuePropertyChanging;
                value.PropertyChanged -= OnValuePropertyChanged;
                removeds.Add(value);
            }
            dict.Clear();
            emptykeys.Clear();
            multikeys.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, (IList)removeds));
        }
        
        public bool Contains(TValue item)
        {
            if (item == null) return false;
            if (String.IsNullOrEmpty(item.KeyName))
            {
                List<TValue> list = null;
                if (!emptykeys.TryGetValue(item.Priority, out list)) return false;
                return list.Contains(item);
            }

            Dictionary<string, TValue> subdict = null;
            if (!dict.TryGetValue(item.Priority, out subdict)) return false;
            if (subdict.ContainsKey(item.KeyName)) return true;
            List<TValue> list1 = null;
            if (!multikeys.TryGetValue(item.Priority, out list1)) return false;
            return list1.Contains(item);
        }

        public void CopyTo(TValue[] array, int arrayIndex)
        {
            foreach (TValue item in this)
            {
                if (arrayIndex >= array.Length) return;
                array[arrayIndex++] = item;
            }
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            SortedDictionary<int, Dictionary<string, TValue>>.Enumerator e0 = dict.GetEnumerator();
            SortedDictionary<int, List<TValue>>.Enumerator e1 = emptykeys.GetEnumerator();
            SortedDictionary<int, List<TValue>>.Enumerator e2 = multikeys.GetEnumerator();
            
            while (e0.Current.Value != null 
                && e1.Current.Value != null
                && e2.Current.Value != null)
            {
                if (e0.Current.Value != null
                 && (e1.Current.Value == null || e1.Current.Key <= e0.Current.Key)
                 && (e2.Current.Value == null || e2.Current.Key <= e0.Current.Key))
                {
                    foreach (TValue value in e0.Current.Value.Values)
                        yield return value;
                    e0.MoveNext();
                }
                if (e1.Current.Value != null
                 && (e0.Current.Value == null || e0.Current.Key <= e1.Current.Key)
                 && (e2.Current.Value == null || e2.Current.Key <= e1.Current.Key))
                {
                    foreach (TValue value in e1.Current.Value)
                        yield return value;
                    e1.MoveNext();
                }
                if (e2.Current.Value != null
                 && (e0.Current.Value == null || e0.Current.Key <= e2.Current.Key)
                 && (e1.Current.Value == null || e1.Current.Key <= e2.Current.Key))
                {
                    foreach (TValue value in e2.Current.Value)
                        yield return value;
                    e2.MoveNext();
                }
            }
        }

        protected bool _Remove(TValue item)
        {
            if (String.IsNullOrEmpty(item.KeyName))
            {
                List<TValue> list1 = null;
                if (!emptykeys.TryGetValue(item.Priority, out list1)) return false;
                return list1.Remove(item);
            }
            Dictionary<string, TValue> subdict = null;
            if (!dict.TryGetValue(item.Priority, out subdict)) return false;
            TValue value = null;
            if (subdict.TryGetValue(item.KeyName, out value)
             && value == item)
            {
                subdict.Remove(item.KeyName);
                return true;
            }
            List<TValue> list = null;
            if (!multikeys.TryGetValue(item.Priority, out list)) return false;
            return list.Remove(item);
        }

        public bool Remove(TValue item)
        {
            if (item == null) return false;
            if (!_Remove(item)) return false;
            item.PropertyChanging += OnValuePropertyChanging;
            item.PropertyChanged += OnValuePropertyChanged;
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnValuePropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (!(sender is TValue)) return;
            TValue value = sender as TValue;
            switch (e.PropertyName)
            {
                case "Priority":
                case "KeyName":
                    _Remove(value);
                    break;
            }
        }

        private void OnValuePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is TValue)) return;
            TValue value = sender as TValue;
            switch (e.PropertyName)
            {
                case "Priority":
                case "KeyName":
                    _Add(value);
                    break;
            }
        }
    }

    public class InverseCompareInt : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            return -x.CompareTo(y);
        }
    }

}
