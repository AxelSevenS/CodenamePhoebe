using UnityEngine;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public abstract class ValuePair{
    }

    [System.Serializable]
    public class ValuePair<T1, T2> : ValuePair{
        public T1 valueOne;
        public T2 valueTwo;
    }
}