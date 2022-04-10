using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {
    
    [System.Serializable]
    public class SafeDictionary<TKey, TVal>{
        private Dictionary<TKey, TVal> dict = new Dictionary<TKey, TVal>();
        public TVal this[TKey key]{
            get { try{ return dict[key]; } catch{ return default(TVal); } }
            set { dict[key] = value; }
        }
    }
}