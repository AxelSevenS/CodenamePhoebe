using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public class ValueData<T> {
        public T currentValue;
        public T lastValue;
        
        public virtual void SetVal(T updatedValue){
            currentValue = updatedValue;
        }

        public virtual void Update(){;}
    }

    [System.Serializable]
    public class VectorData : ValueData<Vector3> {
        
    }

    [System.Serializable]
    public class QuaternionData : ValueData<Quaternion> {
        
    }
    
    [System.Serializable]
    public class BoolData : ValueData<bool> {

        public float trueTimer;
        public float falseTimer;
        public event Action<float> started;
        public event Action<float> stopped;

        public BoolData(){
        }
        public BoolData(Action<float> newStartAction, Action<float> newStopAction){
            started = timer => newStartAction.Invoke(timer);
            stopped = timer => newStopAction.Invoke(timer);
        }
        public BoolData(Action<float> newStartAction, Action newStopAction){
            started = timer => newStartAction.Invoke(timer);
            stopped = _ => newStopAction.Invoke();
        }
        public BoolData(Action newStartAction, Action<float> newStopAction){
            started = _ => newStartAction.Invoke();
            stopped = timer => newStopAction.Invoke(timer);
        }
        public BoolData(Action newStartAction, Action newStopAction){
            started = _ => newStartAction.Invoke();
            stopped = _ => newStopAction.Invoke();
        }
        public BoolData(Action newAction, bool isStarted = true){
            if (isStarted){
                started = _ => newAction.Invoke();
            }else{
                stopped = _ => newAction.Invoke();
            }
        }
        public BoolData(Action<float> newAction, bool isStarted = true){
            if (isStarted){
                started = timer => newAction.Invoke(timer);
            }else{
                stopped = timer => newAction.Invoke(timer);
            }
        }
        // public BoolData(Action<float>[] newStartActions, Action<float>[] newStopActions){
        //     foreach(Action<float> action in newStartActions)
        //         started += action;
        //     foreach(Action<float> action in newStopActions)
        //         stopped += action;
        // }

        
        public override void Update(){
            if (currentValue && !lastValue && started != null) started(falseTimer);
            if (!currentValue && lastValue && stopped != null) stopped(trueTimer);
            lastValue = currentValue;
            UpdateTimer();
        }

        private void UpdateTimer(){
            trueTimer = currentValue ? trueTimer + Global.timeDelta : 0f;
            falseTimer = !currentValue ? falseTimer + Global.timeDelta : 0f;
        }

    }
}