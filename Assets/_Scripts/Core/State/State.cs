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

        public float jumpCount;
        public float evadeCount = 1f;

        protected virtual void StateAwake(){;}
        protected virtual void StateStart(){;}
        protected virtual void StateEnable(){;}
        protected virtual void StateDisable(){;}
        protected virtual void StateDestroy(){;}
        protected virtual void StateUpdate(){;}
        protected virtual void StateFixedUpdate(){;}

        public virtual void StateAnimation(){;}

        private void Awake(){
            StateAwake();
            Reset();
        }
        private void Start(){
            StateStart();
        }

        private void OnEnable(){
            StateEnable();
            entity.lightAttackInputData.started += ParryCheck;
            entity.heavyAttackInputData.started += ParryCheck;
        }

        private void OnDisable(){
            StateDisable();
            entity.lightAttackInputData.started -= ParryCheck;
            entity.heavyAttackInputData.started -= ParryCheck;
        }

        private void OnDestroy(){
            StateDestroy();
        }

        private void Update(){
            UpdateMoveSpeed();
            StateUpdate();
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

        protected abstract void UpdateMoveSpeed();
        

        protected void ParryCheck(float timer){
            bool lightActuated = entity.lightAttackInputData.trueTimer < 0.15f && entity.lightAttackInputData.currentValue;
            bool heavyActuated = entity.heavyAttackInputData.trueTimer < 0.15f && entity.heavyAttackInputData.currentValue;
            if (lightActuated && heavyActuated)
                entity.Parry();
        }

        public abstract void HandleInput();

    }
}