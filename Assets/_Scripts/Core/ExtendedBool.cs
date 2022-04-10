using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {
    
    [System.Serializable]
    public struct ExtendedBool {

        public bool currentValue;
        public bool lastValue;
        public float trueTimer;
        public float falseTimer;
        public Action<float> startAction;
        public Action<float> stopAction;

        public void UpdateTimer(){
            trueTimer = currentValue ? trueTimer + Time.deltaTime : 0f;
            falseTimer = !currentValue ? falseTimer + Time.deltaTime : 0f;
        }
        
        public void Update(bool updatedValue){
            lastValue = currentValue;
            currentValue = updatedValue;
            if (currentValue && !lastValue && startAction != null) startAction(falseTimer);
            if (!currentValue && lastValue && stopAction != null) stopAction(trueTimer);
            UpdateTimer();
        }
    }
}