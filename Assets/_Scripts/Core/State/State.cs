using System;
using UnityEngine;
using UnityEditor;

namespace SeleneGame.Core {
    
    public abstract class State : MonoBehaviour{

        public virtual int id => 0;
        public new string name;
        public Entity entity;

        public float speedMultiplier => GetSpeedMultiplier();
        public Vector3 cameraPosition => GetCameraPosition();
        public Vector3 jumpDirection => GetJumpDirection();

        protected virtual float GetSpeedMultiplier() => 1f;
        protected virtual Vector3 GetCameraPosition() => new Vector3(1f, 1f, -3.5f);
        protected virtual Vector3 GetJumpDirection() => -entity.gravityDown;

        public virtual bool masked => false;


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
            UpdateMoveSpeed();
            ParryCheck();
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
        

        protected void ParryCheck(){
            bool lightActuated = entity.lightAttackInput.trueTimer < 0.15f && entity.lightAttackInput && entity.heavyAttackInput.started;
            bool heavyActuated = entity.heavyAttackInput.trueTimer < 0.15f && entity.heavyAttackInput && entity.lightAttackInput.started;
            if (lightActuated || heavyActuated)
                entity.Parry();
        }

        protected abstract void UpdateMoveSpeed();

        public abstract void HandleInput();

    }
}