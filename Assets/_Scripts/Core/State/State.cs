using System;
using UnityEngine;
using UnityEditor;

namespace SeleneGame.Core {
    
    public abstract class State : MonoBehaviour{

        public virtual int id => 0;
        public Entity entity;
        public new string name;

        public float speedMultiplier => GetSpeedMultiplier();
        public Vector3 cameraPosition => GetCameraPosition();
        public Vector3 jumpDirection => GetJumpDirection();
        public Vector3 entityUp => GetEntityUp();

        protected virtual float GetSpeedMultiplier() => 1f;
        protected virtual Vector3 GetCameraPosition() => new Vector3(1f, 1f, -3.5f);
        protected virtual Vector3 GetJumpDirection() => -entity.gravityDown;
        protected virtual Vector3 GetEntityUp() => -entity.gravityDown;
        
        protected virtual bool canJump => false;
        protected virtual bool canEvade => false;
        protected virtual bool canSlide => false;

        protected virtual bool canTurn => false;
        protected virtual bool useGravity => false;

        public virtual bool masked => false;

        public virtual bool sliding => false;

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
            StateUpdate();
        }

        private void FixedUpdate(){
            UpdateMoveSpeed();
            StateFixedUpdate();
        }

        private void Reset(){
            name = GetType().ToString().Replace("State","");
            entity = GetComponent<Entity>();
        }

        public static Type GetStateTypeByName(string stateName){
            return Type.GetType($"SeleneGame.States.{stateName}State");
        }


        public void Gravity(float force, Vector3 direction){        
            entity._rb.AddForce(force * Time.deltaTime * direction);

            // Inertia
            if( entity.onGround ) return;
            
            float floatingMultiplier = 3f;
            float fallingMultiplier = 5f;
            float multiplier = entity.fallVelocity >= 0 ? floatingMultiplier : fallingMultiplier;
            entity._rb.velocity += multiplier * force * Time.deltaTime * direction.normalized;
                
        }

        protected abstract void UpdateMoveSpeed();
        
        public void Jump(){
            if (!canJump) return;
            
            entity.AnimatorTrigger("Jump");
            float gravityJumpMultiplier = (2 - (entity.gravityForce/15f));
            entity._rb.velocity = Vector3.ProjectOnPlane( entity._rb.velocity, -entity.gravityDown );
            entity._rb.velocity += entity.data.jumpHeight * gravityJumpMultiplier * entity.state.jumpDirection;
            entity.jumpCooldown = 0.4f;

            // insert Jump effects here
        }

        public void Evade(){
            if (!canEvade) return;
            
            entity.AnimatorTrigger("Evade");
            entity.evadeTimer = entity.data.evadeCooldown + entity.data.evadeDuration;
            entity._rb.velocity = Vector3.ProjectOnPlane( entity._rb.velocity, -entity.gravityDown ) ;
            entity.inertiaMultiplier = Mathf.Max( 12.5f, entity.inertiaMultiplier);
            if (!entity.onGround && useGravity){
                entity._rb.velocity += -entity.gravityDown.normalized * 5f;
            }
        }

        public virtual void Parry(){
            Debug.Log("Parry");
            entity.parryTimer = 0.15f;
        }

        protected void ParryCheck(float timer){
            bool lightActuated = entity.lightAttackInputData.trueTimer < 0.15f && entity.lightAttackInputData.currentValue;
            bool heavyActuated = entity.heavyAttackInputData.trueTimer < 0.15f && entity.heavyAttackInputData.currentValue;
            if (lightActuated && heavyActuated){
                Parry();
                return;
            }
        }

        public abstract void HandleInput();

        protected void RotateEntity(Vector3 newRotation){
            
            // Rotation of the Entity.
            Quaternion turnDirection = Quaternion.AngleAxis(Mathf.Atan2(newRotation.x, newRotation.z) * Mathf.Rad2Deg, Vector3.up) ;

            entity._transform.rotation = Quaternion.Slerp(entity._transform.rotation, entity.apparentRotation * turnDirection, 12f*Time.deltaTime);
        }

        protected void RawInputToGroundedMovement(Entity entity, out Vector3 camRight, out Vector3 camForward, out Vector3 groundDirection, out Vector3 groundDirection3D){
            camRight = entity.rotation * entity.lookRotationData.currentValue * Vector3.right;
            camForward = Vector3.Cross(camRight, entity._transform.up).normalized;
            groundDirection = (entity.moveInputData.currentValue.x * camRight + entity.moveInputData.currentValue.z * camForward).normalized;
            groundDirection3D = groundDirection + ((entity.lookRotationData.currentValue * Vector3.up) * entity.moveInputData.currentValue.y);
        }
    }
}