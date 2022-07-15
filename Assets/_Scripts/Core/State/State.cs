using System;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public abstract class State {

        public enum StateType {groundState, waterState, flyingState, immobileState};

        public abstract StateType stateType { get; }
        [ReadOnly] public new string name;
        public Entity entity;

        public Vector3 cameraPosition => GetCameraPosition();
        public Vector3 jumpDirection => GetJumpDirection();

        protected virtual Vector3 GetCameraPosition() => Global.cameraDefaultPosition;
        protected virtual Vector3 GetJumpDirection() => -entity.gravityDown;




        public virtual void OnEnter(Entity entity){
            name = GetType().Name.Replace("State","");
            this.entity = entity;
        }
        public virtual void OnExit(){;}
        public virtual void StateUpdate(){;}
        public virtual void StateLateUpdate(){;}
        public virtual void StateFixedUpdate(){;}

        public virtual void StateAnimation(){;}

        public abstract void HandleInput();

    }
}