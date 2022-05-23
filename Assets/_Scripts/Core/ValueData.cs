using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public class ValueData<T> {
        public T currentValue;
        public T lastValue;
        
        public static implicit operator T(ValueData<T> data) => data.currentValue;

        public ValueData(){
            GameController.onUpdate += Update;
        }
        ~ValueData(){
            GameController.onUpdate -= Update;
        }
        
        public virtual void SetVal(T updatedValue){
            currentValue = updatedValue;
        }

        protected virtual void Update(){;}
    }

    [System.Serializable]
    public class VectorData : ValueData<Vector3> {

        public float zeroTimer;
        public float nonZeroTimer;
        
        protected override void Update(){
            zeroTimer = currentValue == Vector3.zero ? zeroTimer + Global.timeDelta : 0f;
            nonZeroTimer = currentValue != Vector3.zero ? nonZeroTimer + Global.timeDelta : 0f;
        }
    }

    [System.Serializable]
    public class QuaternionData : ValueData<Quaternion> {
        
    }
    
    [System.Serializable]
    public class BoolData : ValueData<bool> {

        public float trueTimer;
        public float falseTimer;
        public bool started => currentValue && !lastValue;
        public bool stopped => !currentValue && lastValue;
        
        protected override void Update(){
            lastValue = currentValue;
            UpdateTimer();
        }

        private void UpdateTimer(){
            trueTimer = currentValue ? trueTimer + Global.timeDelta : 0f;
            falseTimer = !currentValue ? falseTimer + Global.timeDelta : 0f;
        }

    }
}