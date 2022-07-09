using UnityEngine;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public abstract class ValuePair{
    }

    [System.Serializable]
    public class ValuePair<T1, T2> : ValuePair{
        [SerializeReference] public T1 valueOne;
        [SerializeReference] public T2 valueTwo;

        public ValuePair(T1 valueOne, T2 valueTwo){
            this.valueOne = valueOne;
            this.valueTwo = valueTwo;
        }
    }

    [System.Serializable]
    public abstract class ValueTrio{
    }

    [System.Serializable]
    public class ValueTrio<T1, T2, T3> : ValueTrio{
        [SerializeReference] public T1 valueOne;
        [SerializeReference] public T2 valueTwo;
        [SerializeReference] public T3 valueThree;

        public ValueTrio(T1 valueOne, T2 valueTwo, T3 valueThree){
            this.valueOne = valueOne;
            this.valueTwo = valueTwo;
            this.valueThree = valueThree;
        }
    }
}