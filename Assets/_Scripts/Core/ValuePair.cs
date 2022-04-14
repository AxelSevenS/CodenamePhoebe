using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public abstract class ValuePair{
    }

    [System.Serializable]
    public class ValuePair<T1, T2> : ValuePair{
        public T1 valueOne;
        public T2 valueTwo;
    }

    // [System.Serializable]
    // public class ValuePairStringGameObject : ValuePair<string, GameObject>{

    //     public ValuePairStringGameObject(string one, GameObject two){
    //         valueOne = one;
    //         valueTwo = two;
    //     }
    // }

    // [System.Serializable]
    // public class StringPair : ValuePair<string, string>{

    //     public StringPair(string one, string two){
    //         valueOne = one;
    //         valueTwo = two;
    //     }
    // }
}