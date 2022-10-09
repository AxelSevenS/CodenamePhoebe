using System;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public abstract class State {


        [ReadOnly] public string name;
        public Entity entity { get; private set; }



        public virtual float gravityMultiplier => 1f;

        public virtual Vector3 jumpDirection => -entity.gravityDown;
        public virtual bool canJump => entity.onGround.falseTimer < 0.2f;

        public virtual Vector3 evadeDirection => entity.absoluteForward;
        public virtual bool canEvade => true;


        public virtual Vector3 cameraPosition => Global.cameraDefaultPosition;



        public virtual void OnEnter(Entity entity){
            name = GetType().Name.Replace("State","");
            this.entity = entity;
        }
        public virtual void OnExit(){;}
        
        public virtual void StateAnimation(){;}
        public abstract void HandleInput(EntityController controller);
        public virtual void StateUpdate(){;}
        public virtual void StateFixedUpdate(){;}
    }
}