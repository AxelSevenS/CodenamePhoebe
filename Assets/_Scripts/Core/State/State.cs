using System;
using UnityEngine;
using UnityEditor;

namespace SeleneGame.Core {
    
    public abstract class State : MonoBehaviour{

        public virtual int id => 0;
        public new string name;
        public Entity entity;

        public Vector3 cameraPosition => GetCameraPosition();
        public Vector3 jumpDirection => GetJumpDirection();

        protected virtual Vector3 GetCameraPosition() => new Vector3(1f, 1f, -3.5f);
        protected virtual Vector3 GetJumpDirection() => -entity.gravityDown;


        protected virtual void StateAwake(){;}
        protected virtual void StateStart(){;}
        protected virtual void StateDestroy(){;}
        protected virtual void StateUpdate(){;}
        protected virtual void StateLateUpdate(){;}
        protected virtual void StateFixedUpdate(){;}

        public virtual void StateAnimation(){;}

        private void Awake(){
            Reset();
            StateAwake();
        }
        private void Start(){
            StateStart();
        }

        private void OnDestroy(){
            StateDestroy();
        }

        private void Update(){
            StateUpdate();
        }
        private void LateUpdate(){
            StateLateUpdate();
        }

        private void FixedUpdate(){
            StateFixedUpdate();
        }

        private void Reset(){
            name = GetType().Name.Replace("State","");
            entity = GetComponent<Entity>();
        }

        public static Type GetStateTypeByName(string stateName){
            return Type.GetType($"SeleneGame.States.{stateName}State");
        }
        

        public virtual float UpdateMoveSpeed(){
            return entity.moveSpeed;
        }

        public abstract void HandleInput();

    }
}