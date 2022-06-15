using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame.States {
    
    [System.Serializable]
    public class ShiftingState : State{

        public override int id => 2;
        // protected override float GetSpeedMultiplier() => entity.GravityMultiplier();
        protected override Vector3 GetCameraPosition(){
            if (shiftFalling){
                return new Vector3(0f, 1f, -4f) - new Vector3(0,0,additionalCameraDistance);
            }else{
                return new Vector3(0.7f, 0.8f, -2.5f) - new Vector3(0,0,additionalCameraDistance);
            }
        }

        private GravityShifterEntity gravityShifter;
        

        // public override bool masked => true;

        public BoolData shiftFallingData = new BoolData();
        public bool shiftFalling => shiftFallingData;

        private Vector2 randomRotation = Vector3.zero;
        
        private Vector2 fallInput = Vector3.zero;
        private Vector3 floatDirection = Vector3.zero;

        private Vector3 fallDirection = Vector3.forward;
        private Quaternion fallRotation => Quaternion.LookRotation(fallDirection, entity.cameraRotation * Vector3.up);

        private float additionalCameraDistance;

        private GameObject landCursor;

        public override void OnEnter(Entity entity){
            base.OnEnter(entity);

            if ( !(entity is GravityShifterEntity shifter) ) {
                Debug.Log($"Entity {entity.name} cannot switch to Shifting State because it is not a Gravity Shifter");
                entity.SetState(entity.defaultState);
                return;
            }
            gravityShifter = shifter;

            landCursor = GameObject.Instantiate(Resources.Load("Prefabs/UI/LandCursor"), Global.ui.transform.GetChild(0)) as GameObject;
        }

        public override void OnExit(){
            landCursor = Global.SafeDestroy(landCursor);
        }

        public override void StateUpdate(){

            shiftFallingData.SetVal( gravityShifter.evadeInput.trueTimer > 0.125f );

            // Gravity Shifting Movement
            if ( gravityShifter.inWater || gravityShifter.shiftInput.trueTimer > Player.current.holdDuration ){
                StopShifting(Vector3.down);
            }

            if ( gravityShifter.onGround && shiftFalling ) {
                StopShifting(fallDirection);
            }

            if (shiftFallingData.started){
                fallDirection = gravityShifter.cameraRotation * Vector3.forward;
                randomRotation = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

                Debug.DrawRay(gravityShifter._transform.position, 10f * fallDirection, Color.red, 3f);
            }

            if (gravityShifter.evadeInput.stopped && gravityShifter.evadeInput.trueTimer < 0.125f)
                gravityShifter.Evade(floatDirection);

            additionalCameraDistance = (gravityShifter.focusing ? -0.7f : 0f) + (gravityShifter.walkSpeed == Entity.WalkSpeed.sprint ? 0.4f : 0f);

            UpdateLandCursorPos();
        }

        public override void StateFixedUpdate(){


            Vector3 finalRotation;
            Vector3 finalUp;
            if (shiftFalling){

                Quaternion currentRotation = fallRotation;
                Quaternion rotationDelta = Quaternion.AngleAxis(-fallInput.y, currentRotation * Vector3.right) * Quaternion.AngleAxis(fallInput.x, currentRotation * Vector3.up);
                Vector3 newDirection = (rotationDelta * fallDirection).normalized;

                gravityShifter.rotation.SetVal( Quaternion.FromToRotation(fallDirection, newDirection) * gravityShifter.rotation );

                fallDirection = newDirection;
                gravityShifter.absoluteForward = fallDirection;

                // gravityShifter._rb.velocity += gravityShifter.evadeDirection*gravityShifter.data.baseSpeed*gravityShifter.inertiaMultiplier*Global.timeDelta;
                gravityShifter.Gravity(gravityShifter.moveSpeed, fallDirection);

                finalRotation = new Vector3(randomRotation.x, 0f, randomRotation.y);
                finalUp = -fallDirection;

            }else{

                if ( gravityShifter.EvadeUpdate(out _, out float evadeCurve) )
                    gravityShifter.Move( Global.timeDelta * evadeCurve * gravityShifter.data.evadeSpeed * gravityShifter.evadeDirection );

                if (floatDirection.magnitude != 0f){
                    gravityShifter.absoluteForward = floatDirection;

                    gravityShifter._rb.velocity += gravityShifter.data.baseSpeed * Global.timeDelta * floatDirection;
                }


                finalRotation = gravityShifter.relativeForward;
                finalUp = gravityShifter.rotation * Vector3.up;
            }

            gravityShifter.moveDirection.SetVal(gravityShifter.absoluteForward);
            gravityShifter.RotateTowardsRelative(finalRotation, finalUp);
            gravityShifter.gravityDown = -finalUp;
            

        }

        public override float UpdateMoveSpeed(){ 
            
            float newSpeed = gravityShifter.data.baseSpeed;
            newSpeed = shiftFalling ? newSpeed * 0.75f : newSpeed;

            return newSpeed;
        }

        public override void HandleInput(){
            
            gravityShifter.RawInputToGroundedMovement(out Vector3 camRight, out Vector3 camForward, out Vector3 groundDirection, out Vector3 groundDirection3D);

            Vector2 newfallInput = new Vector3(gravityShifter.moveInput.x, gravityShifter.moveInput.z);
            fallInput = Vector2.Lerp(fallInput, (newfallInput.normalized * 0.85f), 10f * Global.timeDelta);
            floatDirection = groundDirection3D;

            if (gravityShifter.shiftInput.trueTimer > Player.current.holdDuration)
                StopShifting(Vector3.down);
        }

        public void StopShifting(Vector3 newDown){
            gravityShifter.gravityDown = newDown;
            gravityShifter.SetState(gravityShifter.defaultState);
        }

        protected void UpdateLandCursorPos(){
            if (landCursor == null) return;

            if ( !(gravityShifter.isPlayer && shiftFalling) ) {
                landCursor.SetActive(false);
                return;
            }
            
            bool hitGround = Physics.Raycast(gravityShifter.transform.position, fallDirection.normalized, out RaycastHit fallCursorHit, 200f, Global.GroundMask);
            bool inLineOfSight = Vector3.Dot(gravityShifter.transform.forward, fallDirection) > 0;

            if ( !(hitGround && inLineOfSight) ) {
                landCursor.SetActive(false);
                return;
            }

            landCursor.SetActive(true);

            landCursor.transform.position = Player.current.camera.WorldToScreenPoint(fallCursorHit.point);
        }

    }
    
}
