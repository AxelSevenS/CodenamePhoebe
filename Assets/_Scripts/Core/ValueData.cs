using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public abstract class ValueData<T> {
        public T currentValue;
        public T lastValue;
        
        public virtual void SetVal(T updatedValue){
            lastValue = currentValue;
            currentValue = updatedValue;
        }

        public virtual void Update(){;}
    }

    // [System.Serializable]
    // public class VectorData : ValueData<Vector3> {
        
    // }

    // [System.Serializable]
    // public class QuaternionData : ValueData<Quaternion> {
        
    // }
    
    [System.Serializable]
    public class BoolData : ValueData<bool> {

        public float trueTimer;
        public float falseTimer;
        public Action<float> startAction;
        public Action<float> stopAction;

        
        public override void Update(){
            if (currentValue && !lastValue && startAction != null) startAction(falseTimer);
            if (!currentValue && lastValue && stopAction != null) stopAction(trueTimer);
            UpdateTimer();
        }

        private void UpdateTimer(){
            trueTimer = currentValue ? trueTimer + Time.deltaTime : 0f;
            falseTimer = !currentValue ? falseTimer + Time.deltaTime : 0f;
        }

    }
}