using UnityEngine;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public abstract class ValuePair{
    }

    [System.Serializable]
    public class ValuePair<T1, T2> : ValuePair{
        [SerializeReference] public T1 valueOne;
        [SerializeReference] public T2 valueTwo;
    }
}