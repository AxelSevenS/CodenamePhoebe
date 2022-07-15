using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SevenGame.Utility {

    [System.Serializable]
    public abstract class Map {
    }

    [System.Serializable]
    public class Map<TKey, TVal> : Map, IEnumerable{

        [SerializeField] private List< ValuePair<TKey, TVal> > pairs = new List< ValuePair<TKey, TVal> >();

        public TVal this[TKey key]{
            get {
                int index = GetIndex(key);
                if (index != -1) return pairs[index].Value;
                throw new KeyNotFoundException();
            }
            set {
                Set(key, value);
            }
        }

        public bool Exists(TKey key){
            return GetIndex(key) != -1;
        }

        public void Clear() => pairs = new List< ValuePair<TKey, TVal> >();

        public void Remove(TKey key){
            int index = GetIndex(key);
            if (index != -1) pairs.RemoveAt(index);
        }

        public void Set(TKey key, TVal value){
            int index = GetIndex(key);
            if (index != -1){ 
                pairs[index].Value = value;
            }else{
                ValuePair<TKey, TVal> newPair = new ValuePair<TKey, TVal>(key, value);
                pairs.Add(newPair);
            }
        }
        private int GetIndex(TKey key){
            for(int i = 0; i < pairs.Count; i++)
                if ( EqualityComparer<TKey>.Default.Equals(key, pairs[i].Key) ) return i;
            return -1;
        }


        public IEnumerator GetEnumerator() => pairs.GetEnumerator();

    }


}
