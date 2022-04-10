using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {
    
    [System.Serializable]
    public class ObjectAccessor : ISerializationCallbackReceiver {
        private Dictionary<string, GameObject> _dictionary = new Dictionary<string, GameObject>();
        private List<ValuePairStringGameObject> objects = new List<ValuePairStringGameObject>();

        private bool edited;

        public GameObject this[string key]{
            get { try{ return _dictionary[key]; } catch{ return null; } }
            set { _dictionary[key] = value; }
        }

        public void OnBeforeSerialize(){
            if (edited) return;


        }
        public void OnAfterDeserialize(){
            
        }
    }
}