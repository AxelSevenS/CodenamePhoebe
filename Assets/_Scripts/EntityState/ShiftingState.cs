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
        

        public override bool masked => true;

        private Vector2 randomRotation = Vector3.zero;
        private Vector2 fallInput = Vector3.zero;
        private Quaternion fallRotation = Quaternion.identity;
        private Vector3 fallDirection = Vector3.zero;
        private Vector3 floatDirection = Vector3.zero;
        private float additionalCameraDistance;


        private GameObject landCursor;

        protected override void StateAwake(){

            landCursor = GameObject.Instantiate(Resources.Load("Prefabs/UI/LandCursor"), Global.ui.transform.GetChild(0)) as GameObject;
        }

        protected override void StateEnable(){

            entity.slidingData.started += OnSlideStart;
            entity.evadeInputData.stopped += OnEvadeInputStop;
        }
        protected override void StateDisable(){

            entity.slidingData.started -= OnSlideStart;
            entity.evadeInputData.stopped -= OnEvadeInputStop;
        }

        protected override void StateDestroy(){
            landCursor = Global.SafeDestroy(landCursor);
        }

        protected override void StateUpdate(){

            // Gravity Shifting Movement
            if ( entity.inWater || entity.shiftInputData.trueTimer > Player.current.holdDuration ){
                StopShifting(Vector3.down);
            }

            if ( entity.onGround && entity.sliding ) {
                StopShifting(fallDirection);
            }

            entity.inertiaMultiplier = Mathf.Max(entity.inertiaMultiplier, 4f);

            additionalCameraDistance = (entity.focusing ? -0.7f : 0f) + (entity.walkSpeed == Entity.WalkSpeed.sprint ? 0.4f : 0f);

            UpdateLandCursorPos();
        }

        protected override void StateFixedUpdate(){
            if ( entity.EvadeUpdate(out float evadeSpeed) )
                entity.Move( Global.timeDelta * evadeSpeed * entity.evadeDirection );



            Vector3 finalRotation;
            Vector3 finalUp;
            if (entity.sliding){

                Vector3 oldFallDirection = fallDirection;
                Quaternion rotationDelta = Quaternion.AngleAxis(-fallInput.y, fallRotation * Vector3.right) * Quaternion.AngleAxis(fallInput.x, fallRotation * Vector3.up);
                fallDirection = (rotationDelta * fallDirection).normalized;

                entity.rotation = Quaternion.FromToRotation(oldFallDirection, fallDirection) * entity.rotation;
                fallRotation = Quaternion.LookRotation(fallDirection, entity.rotation * Vector3.up);

                entity.absoluteForward = fallDirection;

                // entity._rb.velocity += entity.evadeDirection*entity.data.baseSpeed*entity.inertiaMultiplier*Global.timeDelta;
                entity.Gravity(entity.inertiaMultiplier, fallDirection);

                finalRotation = new Vector3(randomRotation.x, 0f, randomRotation.y);
                finalUp = -fallDirection;

            }else{

                if (floatDirection.magnitude == 0f){
                    entity.absoluteForward = floatDirection;
                    entity.evadeDirection = Vector3.Lerp(entity.evadeDirection, entity.absoluteForward, 0.1f * Global.timeDelta);

                    entity._rb.velocity += entity.data.baseSpeed * Global.timeDelta * floatDirection;
                }


                finalRotation = entity.relativeForward;
                finalUp = entity.rotation * Vector3.up;
            }

            entity.moveDirection = entity.sliding ? fallDirection : floatDirection;
            entity.RotateTowardsRelative(finalRotation, finalUp);
            entity.gravityDown = -finalUp;
            

        }

        protected override void UpdateMoveSpeed(){ 
            float newSpeed = entity.data.baseSpeed * entity.currentWeapon?.speedMultiplier ?? 1f;
            
            entity.moveSpeed = Mathf.MoveTowards(entity.moveSpeed, newSpeed, entity.data.moveIncrement * Global.timeDelta);
        }

        public override void HandleInput(){
            
            entity.RawInputToGroundedMovement(out Vector3 camRight, out Vector3 camForward, out Vector3 groundDirection, out Vector3 groundDirection3D);

            Vector2 newfallInput = new Vector3(entity.moveInputData.currentValue.x, entity.moveInputData.currentValue.z);
            fallInput = Vector2.Lerp(fallInput, (newfallInput.normalized * 0.85f), 10f * Global.timeDelta);
            floatDirection = groundDirection3D;

            entity.slidingData.SetVal( entity.evadeInputData.trueTimer > 0.125f );

            if (entity.shiftInputData.trueTimer > Player.current.holdDuration)
                StopShifting(Vector3.down);
        }

        public void StopShifting(Vector3 newDown){
            entity.gravityDown = newDown;
            entity.SetState("Walking");
        }

        protected void UpdateLandCursorPos(){
            if (landCursor == null) return;

            if ( !(entity.isPlayer && entity.sliding) ) {
                landCursor.SetActive(false);
                return;
            }
            
            bool hitGround = Physics.Raycast(entity.bottom, fallDirection.normalized, out RaycastHit fallCursorHit, 200f, Global.GroundMask);
            bool inLineOfSight = Vector3.Dot(entity.transform.forward, fallDirection) > 0;

            if ( !(hitGround && inLineOfSight) ) {
                landCursor.SetActive(false);
                return;
            }

            landCursor.SetActive(true);

            landCursor.transform.position = Player.current.camera.WorldToScreenPoint(fallCursorHit.point);
        }

        private void OnSlideStart(float timer){
            Vector3 dir = entity.lookDirection;
            fallDirection = dir;
            randomRotation = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

            Debug.DrawRay(entity._transform.position, 10f * dir, Color.red, 3f);
        }

        private void OnEvadeInputStop(float timer){
            if (timer < 0.125f)
                entity.Evade(floatDirection);
        }

    }
    
}
