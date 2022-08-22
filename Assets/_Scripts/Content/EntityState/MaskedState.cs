using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Entities;
using SevenGame.Utility;

namespace SeleneGame.States {
    
    public class MaskedState : State{

        public override StateType stateType => StateType.flyingState;
        // protected override float GetSpeedMultiplier() => entity.GravityMultiplier();
        protected override Vector3 GetCameraPosition(){
            if (shiftFalling){
                return new Vector3(0f, 1f, -4f) - new Vector3(0,0,additionalCameraDistance);
            }else{
                return new Vector3(0.7f, 0.8f, -2.5f) - new Vector3(0,0,additionalCameraDistance);
            }
        }

        private MaskedEntity maskedEntity;

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

            if ( !(entity is MaskedEntity masked) ) {
                Debug.Log($"Entity {entity.name} cannot switch to Masked State because it is not Masked");
                entity.SetState(entity.defaultState);
                return;
            }
            maskedEntity = masked;

            // landCursor = GameObject.Instantiate(Resources.Load("Prefabs/UI/LandCursor"), HUDController.current.transform) as GameObject;
        }

        public override void OnExit(){
            landCursor = GameUtility.SafeDestroy(landCursor);
        }

        public override void StateUpdate(){

            shiftFalling.SetVal( maskedEntity.controller.evadeInput.trueTimer > 0.125f );

            // Gravity Shifting Movement
            if ( maskedEntity.inWater ){
                maskedEntity.StopShifting(Vector3.down);
            }

            if ( maskedEntity.onGround && shiftFalling ) {
                maskedEntity.StopShifting(fallDirection);
            }

            if (shiftFalling.started){
                maskedEntity.controller.RawInputToCameraRelativeMovement(out Quaternion cameraRotation, out _);
                fallDirection = cameraRotation * Vector3.forward;
                randomRotation = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

                Debug.DrawRay(maskedEntity.transform.position, 10f * fallDirection, Color.red, 3f);
            }

            if ( shiftFalling && maskedEntity.controller.evadeInput.stopped && maskedEntity.controller.evadeInput.trueTimer < 0.125f)
                maskedEntity.Evade( maskedEntity.moveDirection );

            // additionalCameraDistance = (maskedEntity.focusing ? -0.7f : 0f) + (maskedEntity.walkSpeed == Entity.WalkSpeed.sprint ? 0.4f : 0f);

            UpdateLandCursorPos();
        }

        public override void StateFixedUpdate(){


            Vector3 finalRotation;
            Vector3 finalUp;
            if (shiftFalling){

                // maskedEntity.rigidbody.velocity += maskedEntity.evadeDirection*maskedEntity.character.baseSpeed*maskedEntity.inertiaMultiplier*GameUtility.timeDelta;
                maskedEntity.Gravity(maskedEntity.moveSpeed, fallDirection);

                finalRotation = new Vector3(randomRotation.x, 0f, randomRotation.y);
                finalUp = -fallDirection;

            }else{

                if ( maskedEntity.EvadeUpdate(out _, out float evadeCurve) )
                    maskedEntity.Move( GameUtility.timeDelta * evadeCurve * maskedEntity.character.evadeSpeed * maskedEntity.evadeDirection );

                finalRotation = maskedEntity.relativeForward;
                finalUp = maskedEntity.rotation * Vector3.up;
            }

            maskedEntity.moveDirection.SetVal(maskedEntity.absoluteForward);
            maskedEntity.RotateTowardsRelative(finalRotation, finalUp);
            maskedEntity.gravityDown = -finalUp;
            

        }

        public override void HandleInput(){

            const float turningStrength = 0.65f;


            if (shiftFalling){

                fallInput = Vector2.Lerp(fallInput, (maskedEntity.controller.moveInput.normalized * turningStrength), 2.5f * GameUtility.timeDelta);
                maskedEntity.controller.RawInputToCameraRelativeMovement(out Quaternion cameraRotation, out _);

                Quaternion currentRotation = Quaternion.LookRotation(fallDirection, cameraRotation * Vector3.up);

                Quaternion rotationDelta = Quaternion.AngleAxis(-fallInput.y, currentRotation * Vector3.right) * Quaternion.AngleAxis(fallInput.x, currentRotation * Vector3.up);
                Vector3 newDirection = (rotationDelta * (fallDirection)).normalized;

                maskedEntity.rotation.SetVal( Quaternion.FromToRotation(fallDirection, newDirection) * maskedEntity.rotation );

                fallDirection = newDirection;
                maskedEntity.moveDirection.SetVal(fallDirection);

            }else {

                maskedEntity.controller.RawInputToGroundedMovement(out _, out Vector3 groundedMovement);
                float verticalInput = (maskedEntity.controller.jumpInput ? 1f: 0f) - (maskedEntity.controller.crouchInput ? 1f: 0f);
                Vector3 verticalGroundedMovement = groundedMovement + (maskedEntity.rotation * Vector3.up) * verticalInput;
                
                if (verticalGroundedMovement.sqrMagnitude != 0f){

                    maskedEntity.moveDirection.SetVal(verticalGroundedMovement);
                    maskedEntity.rigidbody.velocity += maskedEntity.character.baseSpeed * GameUtility.timeDelta * verticalGroundedMovement;
                }

            }
            maskedEntity.absoluteForward = maskedEntity.moveDirection;

            
            float newSpeed = maskedEntity.character.baseSpeed * 0.5f;
            newSpeed = shiftFalling ? newSpeed * 0.75f : newSpeed;

            float speedDelta = newSpeed > maskedEntity.moveSpeed ? 1f : 0.65f;

            maskedEntity.moveSpeed = Mathf.MoveTowards(maskedEntity.moveSpeed, newSpeed, speedDelta * maskedEntity.character.acceleration * GameUtility.timeDelta);
        }

        protected void UpdateLandCursorPos(){
            if (landCursor == null) return;

            if ( !shiftFalling ) {
                landCursor.SetActive(false);
                return;
            }
            
            bool hitGround = Physics.Raycast(maskedEntity.transform.position, fallDirection.normalized, out RaycastHit fallCursorHit, 200f, Global.GroundMask);
            bool inLineOfSight = Vector3.Dot(maskedEntity.transform.forward, fallDirection) > 0;

            if ( !(hitGround && inLineOfSight) ) {
                landCursor.SetActive(false);
                return;
            }

            landCursor.SetActive(true);

            landCursor.transform.position = CameraController.current.camera.WorldToScreenPoint(fallCursorHit.point);
        }

    }
    
}
