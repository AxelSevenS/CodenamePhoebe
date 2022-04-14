using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;

namespace SeleneGame.States {
    
    public class ShiftingState : State{

        public override int id => 2;
        protected override float GetSpeedMultiplier() => entity.currentWeapon.speedMultiplier;
        protected override Vector3 GetCameraPosition(){
            if (entity.sliding){
                return new Vector3(0f, 1f, -4f) - new Vector3(0,0,additionalCameraDistance);
            }else{
                return new Vector3(0.7f, 0.8f, -2.5f) - new Vector3(0,0,additionalCameraDistance);
            }
        }
        protected override Vector3 GetEntityUp() => entity.sliding ? -entity.inertiaDirection : entity.rotation * Vector3.up;
        

        protected override bool canJump => false;
        protected override bool canEvade => entity.evadeTimer == 0f || entity.currentWeapon.canEvade;
        
        protected override bool canTurn => true;
        protected override bool useGravity => false;

        public override bool masked => true;

        public override bool sliding => entity.evadeInputData.trueTimer > 0.125f;

        private float forwardMovement = 0f;
        private Vector3 startFallDirection = Vector3.forward;
        private Vector3 fallTurn = Vector3.zero;
        private Quaternion fallRotation = Quaternion.identity;
        private Vector3 floatDirection = Vector3.zero;
        private float additionalCameraDistance;


        private GameObject landCursor;

        public void StopShifting(Vector3 newDown){
            entity.gravityDown = newDown;
            entity.SetState("Walking");
        }

        public override void StateEnable(){
            entity.evadeInputData.startAction += OnEvadeInputStart;
            entity.evadeInputData.stopAction += OnEvadeInputStop;
        }
        public override void StateDisable(){
            entity.evadeInputData.startAction -= OnEvadeInputStart;
            entity.evadeInputData.stopAction -= OnEvadeInputStop;
        }

        public override void StateAwake(){
            landCursor = GameObject.Instantiate(Resources.Load("Prefabs/UI/LandCursor"), Global.ui.transform.GetChild(0)) as GameObject;
        }

        public override void StateDestroy(){
            landCursor = Global.SafeDestroy(landCursor);
        }

        public override void StateUpdate(){

            // Gravity Shifting Movement
            if ( entity.inWater || entity.shiftInputData.trueTimer > Player.current.holdDuration ){
                StopShifting(Vector3.down);
            }

            if ( entity.onGround && entity.sliding ) {
                StopShifting(entity.inertiaDirection);
            }

            entity.inertiaMultiplier = Mathf.Max(entity.inertiaMultiplier, 4f);

            additionalCameraDistance = (entity.focusing ? -0.7f : 0f) + (entity.walkSpeed == Entity.WalkSpeed.sprint ? 0.4f : 0f);

            if (entity.shiftInputData.trueTimer > Player.current.holdDuration){
                StopShifting(Vector3.down);
            }

            UpdateLandCursorPos();
        }

        public override void StateFixedUpdate(){

            if (entity.evading){

                if (entity.evadeTimer > entity.data.evadeCooldown + entity.data.evadeDuration - 0.2f){
                    entity.evadeDirection = entity.absoluteForward;
                }

                entity.Move(entity.evadeDirection * Time.deltaTime * 24f * entity.data.evadeCurve.Evaluate( 1 - ( (entity.evadeTimer - entity.data.evadeCooldown) / entity.data.evadeDuration ) ));
            }

            entity.gravityDown = -entityUp;



            if (entity.sliding){
            
                Vector3 oldFallDirection = entity.inertiaDirection;
                entity.inertiaDirection = (Quaternion.AngleAxis(-fallTurn.z, fallRotation * Vector3.right) * Quaternion.AngleAxis(fallTurn.x, fallRotation * Vector3.up) * entity.inertiaDirection).normalized;
                entity.rotation = Quaternion.FromToRotation(oldFallDirection, entity.inertiaDirection)* entity.rotation;

                entity.moveDirection = entity.inertiaDirection;
                entity.absoluteForward = entity.inertiaDirection;
                entity.rotationForward = Vector3.forward;
                entity.evadeDirection = Vector3.Lerp(entity.evadeDirection, entity.absoluteForward, 0.1f);

                // entity._rb.velocity += entity.evadeDirection*entity.data.baseSpeed*entity.inertiaMultiplier*Time.deltaTime;
                Gravity(entity.inertiaMultiplier, entity.inertiaDirection);

            }else{

                entity.moveDirection = floatDirection;
                if (entity.moveDirection.magnitude > 0f){
                    entity.relativeForward = Quaternion.Inverse(entity.rotation) * entity.moveDirection;
                    entity.evadeDirection = Vector3.Lerp(entity.evadeDirection, entity.absoluteForward, 0.1f);
                }
                entity.rotationForward = Vector3.Lerp(entity.rotationForward, Quaternion.Inverse(entity.rotation) * entity.absoluteForward, 0.7f).normalized;

                entity._rb.velocity += entity.moveDirection*entity.data.baseSpeed*Time.deltaTime;
                
            }



            fallRotation = Quaternion.LookRotation(entity.inertiaDirection, entity.rotation * Vector3.up);
            
            RotateEntity(entity.rotationForward);

        }

        public override void UpdateMoveSpeed(){ 
            // float newSpeed = 0f;
            // switch ( entity.walkSpeed ){
            //     case Entity.WalkSpeed.sprint:
            //         newSpeed = entity.data.baseSpeed*entity.data.sprintSpeed;
            //         break;
            //     default:
            //         newSpeed = entity.data.baseSpeed;
            //         break;
            // }
            float newSpeed = entity.data.baseSpeed * entity.currentWeapon?.speedMultiplier ?? 1f;
            
            entity.moveSpeed = Mathf.MoveTowards(entity.moveSpeed, newSpeed, entity.data.moveIncrement * Time.deltaTime);
        }

        public override void HandleInput(){
            
            RawInputToGroundedMovement(entity, out Vector3 camRight, out Vector3 camForward, out Vector3 groundDirection, out Vector3 groundDirection3D);
            if (entity.isPlayer){
                Vector3 newfallTurn = new Vector3(entity.moveInputData.currentValue.x, 0, entity.moveInputData.currentValue.z);
                fallTurn = Vector3.Lerp(fallTurn, (newfallTurn.normalized * 0.85f), 10f * Time.deltaTime);
                floatDirection = groundDirection3D;
                forwardMovement = Mathf.Min(floatDirection.magnitude, 1f);
            }else{
                entity.inertiaDirection = groundDirection;
                forwardMovement = Mathf.Min(floatDirection.magnitude, 1f);
            }
        }

        protected void UpdateLandCursorPos(){
            if (landCursor == null) return;

            if ( !(entity.isPlayer && entity.sliding) ) {
                landCursor.SetActive(false);
                return;
            }
            
            bool hitGround = Physics.Raycast(entity.bottom, entity.moveDirection.normalized, out RaycastHit fallCursorHit, 200f, Global.GroundMask);
            bool inLineOfSight = Vector3.Dot(Player.current.camera.transform.forward, entity.moveDirection) > 0;

            if ( !(hitGround && inLineOfSight) ) {
                landCursor.SetActive(false);
                return;
            }

            landCursor.SetActive(true);

            landCursor.transform.position = Player.current.camera.WorldToScreenPoint(fallCursorHit.point);
        }

        private void OnEvadeInputStart(float timer){
            entity.inertiaDirection = entity.lookRotationData.currentValue * Vector3.forward;
            // entity.Evade();
        }

        private void OnEvadeInputStop(float timer){
            if (timer < 0.125f)
                Evade();
        }

    }
    
}
