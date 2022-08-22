using System;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public abstract class State {

        public enum StateType {groundState, waterState, flyingState, immobileState};

        [Tooltip("The type of State this state belongs to. This is mainly used for Animation State Machines. ( e.g. when switching to the walking state from the sitting state, the animation changes state, too. )")]
        [SerializeField] 
        public abstract StateType stateType { get; }
        [ReadOnly] public new string name;
        public Entity entity { get; private set; }

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
        public virtual void StateFixedUpdate(){;}

        public virtual void StateAnimation(){;}

        public abstract void HandleInput();

    }
}