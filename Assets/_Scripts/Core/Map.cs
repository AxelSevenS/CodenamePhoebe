using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public abstract class Map {
    }

    [System.Serializable]
    public class Map<TKey, TVal> : Map{

        [SerializeField] private List< ValuePair<TKey, TVal> > pairs = new List< ValuePair<TKey, TVal> >();

        public TVal this[TKey key]{
            get {
                int index = GetIndex(key);
                if (index != -1) return pairs[index].valueTwo;
                throw new KeyNotFoundException();
            }
            set {
                int index = GetIndex(key);
                if (index != -1){ 
                    pairs[index].valueTwo = value;
                }else{
                    ValuePair<TKey, TVal> newPair = new ValuePair<TKey, TVal>();
                    newPair.valueOne = key;
                    newPair.valueTwo = value;

                    pairs.Add(newPair);
                }
            }
        }

        public void Clear() => pairs = new List< ValuePair<TKey, TVal> >();

        private int GetIndex(TKey key){
            for(int i = 0; i < pairs.Count; i++)
                if ( EqualityComparer<TKey>.Default.Equals(key, pairs[i].valueOne) ) return i;
            return -1;
        }
    }
}
