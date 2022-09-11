using System;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public abstract class State {


        [ReadOnly] public string name;
        public Entity entity { get; private set; }


        public abstract StateType stateType { get; }
        public virtual Vector3 cameraPosition => Global.cameraDefaultPosition;
        public virtual Vector3 jumpDirection => -entity.gravityDown;




        public virtual void OnEnter(Entity entity){
            name = GetType().Name.Replace("State","");
            this.entity = entity;
        }
        public virtual void OnExit(){;}
        
        public virtual void StateAnimation(){;}
        public abstract void HandleInput();
        public virtual void StateUpdate(){;}
        public virtual void StateFixedUpdate(){;}



        public enum StateType { groundState, waterState, flyingState, immobileState };
    }
}