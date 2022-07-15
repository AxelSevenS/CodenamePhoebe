using UnityEngine;

namespace SevenGame.Utility {
    
    [System.Serializable]
    public abstract class ValuePair{
    }

    [System.Serializable]
    public class ValuePair<T1, T2> : ValuePair{
        [SerializeReference] public T1 Key;
        [SerializeReference] public T2 Value;

        public ValuePair(T1 Key, T2 Value){
            this.Key = Key;
            this.Value = Value;
        }
    }

    [System.Serializable]
    public abstract class ValueTrio{
    }

    [System.Serializable]
    public class ValueTrio<T1, T2, T3> : ValueTrio{
        [SerializeReference] public T1 Key;
        [SerializeReference] public T2 Value;
        [SerializeReference] public T3 valueThree;

        public ValueTrio(T1 Key, T2 Value, T3 valueThree){
            this.Key = Key;
            this.Value = Value;
            this.valueThree = valueThree;
        }
    }
}