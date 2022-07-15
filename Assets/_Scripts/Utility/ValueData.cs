using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SevenGame.Utility {
    
    [System.Serializable]
    public class ValueData<T> {
        public T currentValue;
        public T lastValue;
        
        public static implicit operator T(ValueData<T> data) => data.currentValue;

        public ValueData(){
            UpdateCaller.onUpdate += Update;
        }
        ~ValueData(){
            UpdateCaller.onUpdate -= Update;
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

        public float x => currentValue.x;
        public float y => currentValue.y;
        public float z => currentValue.z;
        public float magnitude => currentValue.magnitude;
        public Vector3 normalized => currentValue.normalized;

        public static Vector3 operator *(VectorData a, float b) => a.currentValue * b;
        public static Vector3 operator *(float a, VectorData b) => a * b.currentValue;
        
        protected override void Update(){
            lastValue = currentValue;
            zeroTimer = currentValue == Vector3.zero ? zeroTimer + GameUtility.timeDelta : 0f;
            nonZeroTimer = currentValue != Vector3.zero ? nonZeroTimer + GameUtility.timeDelta : 0f;
            // currentValue = default(Vector3);
        }
    }

    [System.Serializable]
    public class QuaternionData : ValueData<Quaternion> {

        public static Vector3 operator *(QuaternionData a, Vector3 b) => a.currentValue * b;
        public static Vector3 operator *(QuaternionData a, VectorData b) => a.currentValue * b.currentValue;
        public static Quaternion operator *(QuaternionData a, QuaternionData b) => a.currentValue * b.currentValue;
        
        protected override void Update(){
            lastValue = currentValue;
            // currentValue = default(Quaternion);
        }
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
            // currentValue = default(bool);
        }

        private void UpdateTimer(){
            trueTimer = currentValue ? trueTimer + GameUtility.timeDelta : 0f;
            falseTimer = !currentValue ? falseTimer + GameUtility.timeDelta : 0f;
        }

    }
}