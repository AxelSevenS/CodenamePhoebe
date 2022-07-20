using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Entities;
using SevenGame.Utility;

namespace SeleneGame.States {
    
    public class ShiftingState : State{

        public override StateType stateType => StateType.flyingState;
        // protected override float GetSpeedMultiplier() => entity.GravityMultiplier();
        protected override Vector3 GetCameraPosition(){
            if (shiftFalling){
                return new Vector3(0f, 1f, -4f) - new Vector3(0,0,additionalCameraDistance);
            }else{
                return new Vector3(0.7f, 0.8f, -2.5f) - new Vector3(0,0,additionalCameraDistance);
            }
        }

        private GravityShifterEntity gravityShifter;

        public BoolData shiftFalling = new BoolData();

        private Vector2 randomRotation = Vector3.zero;
        
        private Vector2 fallInput = Vector3.zero;

        private Vector3 fallDirection = Vector3.forward;
        // private Quaternion fallRotation => Quaternion.LookRotation(fallDirection, entity.finalRotation * Vector3.up);

        private float additionalCameraDistance;

        private GameObject landCursor;

        public override void OnEnter(Entity entity){
            base.OnEnter(entity);

            shiftFalling = new BoolData();

            if ( !(entity is GravityShifterEntity shifter) ) {
                Debug.Log($"Entity {entity.name} cannot switch to Shifting State because it is not a Gravity Shifter");
                entity.SetState(entity.defaultState);
                return;
            }
            gravityShifter = shifter;

            landCursor = GameObject.Instantiate(Resources.Load("Prefabs/UI/LandCursor"), HUDController.current.transform) as GameObject;
        }

        public override void OnExit(){
            landCursor = GameUtility.SafeDestroy(landCursor);
        }

        public override void StateUpdate(){

            shiftFalling.SetVal( gravityShifter.evadeInput.trueTimer > 0.125f );

            // Gravity Shifting Movement
            if ( gravityShifter.inWater || gravityShifter.shiftInput.trueTimer > Global.HOLDTIME ){
                gravityShifter.StopShifting(Vector3.down);
            }

            if ( gravityShifter.onGround && shiftFalling ) {
                gravityShifter.StopShifting(fallDirection);
            }

            if (shiftFalling.started){
                fallDirection = gravityShifter.finalRotation * Vector3.forward;
                randomRotation = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

                Debug.DrawRay(gravityShifter.transform.position, 10f * fallDirection, Color.red, 3f);
            }

            if ( shiftFalling && gravityShifter.evadeInput.stopped && gravityShifter.evadeInput.trueTimer < 0.125f)
                gravityShifter.Evade( gravityShifter.moveDirection );

            additionalCameraDistance = (gravityShifter.focusing ? -0.7f : 0f) + (gravityShifter.walkSpeed == Entity.WalkSpeed.sprint ? 0.4f : 0f);

            UpdateLandCursorPos();
        }

        public override void StateFixedUpdate(){


            Vector3 finalRotation;
            Vector3 finalUp;
            if (shiftFalling){

                // gravityShifter.rb.velocity += gravityShifter.evadeDirection*gravityShifter.data.baseSpeed*gravityShifter.inertiaMultiplier*GameUtility.timeDelta;
                gravityShifter.Gravity(gravityShifter.moveSpeed, fallDirection);

                finalRotation = new Vector3(randomRotation.x, 0f, randomRotation.y);
                finalUp = -fallDirection;

            }else{

                if ( gravityShifter.EvadeUpdate(out _, out float evadeCurve) )
                    gravityShifter.Move( GameUtility.timeDelta * evadeCurve * gravityShifter.data.evadeSpeed * gravityShifter.evadeDirection );

                finalRotation = gravityShifter.relativeForward;
                finalUp = gravityShifter.rotation * Vector3.up;
            }

            gravityShifter.moveDirection.SetVal(gravityShifter.absoluteForward);
            gravityShifter.RotateTowardsRelative(finalRotation, finalUp);
            gravityShifter.gravityDown = -finalUp;
            

        }

        public override void HandleInput(){
            
            gravityShifter.RawInputToGroundedMovement(out Vector3 camRight, out Vector3 camForward, out Vector3 groundDirection, out Vector3 groundDirection3D);

            const float turningStrength = 0.65f;

            Vector2 newfallInput = new Vector3(gravityShifter.moveInput.x, gravityShifter.moveInput.z);
            fallInput = Vector2.Lerp(fallInput, (newfallInput.normalized * turningStrength), 2.5f * GameUtility.timeDelta);


            if (shiftFalling){

                Quaternion currentRotation = Quaternion.LookRotation(fallDirection, entity.finalRotation * Vector3.up);

                Quaternion rotationDelta = Quaternion.AngleAxis(-fallInput.y, currentRotation * Vector3.right) * Quaternion.AngleAxis(fallInput.x, currentRotation * Vector3.up);
                Vector3 newDirection = (rotationDelta * (fallDirection)).normalized;

                gravityShifter.rotation.SetVal( Quaternion.FromToRotation(fallDirection, newDirection) * gravityShifter.rotation );

                fallDirection = newDirection;
                gravityShifter.moveDirection.SetVal(fallDirection);

            }else {
                
                if (groundDirection3D.magnitude != 0f){

                    gravityShifter.moveDirection.SetVal(groundDirection3D);
                    gravityShifter.rb.velocity += gravityShifter.data.baseSpeed * GameUtility.timeDelta * groundDirection3D;
                }

            }
            gravityShifter.absoluteForward = gravityShifter.moveDirection;

            if (gravityShifter.shiftInput.trueTimer > Global.HOLDTIME)
                gravityShifter.StopShifting(Vector3.down);
            

            
            float newSpeed = gravityShifter.data.baseSpeed * 0.5f;
            newSpeed = shiftFalling ? newSpeed * 0.75f : newSpeed;

            float speedDelta = newSpeed > gravityShifter.moveSpeed ? 1f : 0.65f;

            gravityShifter.moveSpeed = Mathf.MoveTowards(gravityShifter.moveSpeed, newSpeed, speedDelta * gravityShifter.data.moveIncrement * GameUtility.timeDelta);
        }

        protected void UpdateLandCursorPos(){
            if (landCursor == null) return;

            if ( !shiftFalling ) {
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
