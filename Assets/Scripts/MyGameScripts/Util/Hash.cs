using System;
using System.Collections.Generic;

namespace Assets.Scripts.MyGameScripts.Utils {
    public class Hash<K, V> {
        private Dictionary<K, V> _dic;
        private List<V> _list;

        public Hash() {
            _dic = new Dictionary<K, V>();
            _list = new List<V>();
        }

        public int Length {
            get {
                return _list.Count;
            }
        }

        public bool Has(K key) {
            if (key == null) {
                return false;
            }
            return _dic.ContainsKey(key);
        }

        public void Add(K key, V value) {
            if (Has(key) == false) {
                _dic.Add(key, value);
                _list.Add(value);
            }
        }

        public void Remove(K key) {
            if (Has(key)) {
                V obj = _dic[key];
                _dic.Remove(key);
                _list.Remove(obj);
            }
        }

        public void Replace(K key, V value) {
            Remove(key);
            Add(key, value);
        }

        public void Replace2(K key, V value) {
            int index = _list.IndexOf(value);
            _dic[key] = value;
            if (index != -1) {
                _list.RemoveAt(index);
                index = Math.Min(_list.Count, index);
                _list.Insert(index, value);
            } else {
                _list.Add(value);
            }
        }

        public void Unload() {
            _dic = null;
            _list = null;
        }

        public void Clear() {
            _dic.Clear();
            _list.Clear();
        }

        public V Take(K key) {
            if (Has(key)) {
                return _dic[key];
            }
            return default(V);
        }

        public V this[K key] {
            get { return Take(key); }
            set {
                Replace(key, value);
            }
        }

        public K GetKey(V value) {
            foreach (K key in _dic.Keys) {
                V item = _dic[key];
                if (object.Equals(item, value)) {
                    return key;
                }
            }
            return default(K);
        }

        public V Shift() {
            V obj = _list[0];
            _list.RemoveAt(0);
            K key = (K)GetKey(obj);
            _dic.Remove(key);
            return obj;
        }


        public Dictionary<K, V> Dic {
            get {
                return _dic;
            }
            set {
                _dic = value;
            }
        }


        public List<V> List {
            get {
                return _list;
            }
            set {
                _list = value;
            }
        }

    }
}
