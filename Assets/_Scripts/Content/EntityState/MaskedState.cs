using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Entities;
using SevenGame.Utility;

namespace SeleneGame.States {
    
    public class MaskedState : NoGravityState {

        private MaskedEntity maskedEntity;

        public BoolData shiftFalling = new BoolData();

        private float moveSpeed;
        private Vector2 randomRotation = Vector3.zero;
        private Vector2 fallInput = Vector3.zero;
        private Vector3 fallDirection = Vector3.forward;
        private Vector3 flyDirection = Vector3.forward;

        private GameObject landCursor;



        public override Vector3 cameraPosition {
            get {
                float additionalCameraDistance = 0f; //(maskedEntity.focusing ? -0.7f : 0f) + (maskedEntity.walkSpeed == Entity.WalkSpeed.sprint ? 0.4f : 0f);
                if (shiftFalling) {
                    return new Vector3(0f, 1f, -4f) - new Vector3(0,0,additionalCameraDistance);
                } else {
                    return new Vector3(0.7f, 0.8f, -2.5f) - new Vector3(0,0,additionalCameraDistance);
                }
            }
        }



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

        public override void HandleInput(EntityController controller){

            shiftFalling.SetVal( controller.evadeInput.trueTimer > 0.125f );



            if (shiftFalling.started){
                controller.RawInputToCameraRelativeMovement(out Quaternion cameraRotation, out _);
                fallDirection = cameraRotation * Vector3.forward;
                randomRotation = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

                Debug.DrawRay(maskedEntity.transform.position, 10f * fallDirection, Color.red, 3f);
            }




            if (shiftFalling){

                const float turningStrength = 0.65f;

                fallInput = Vector2.Lerp(fallInput, (controller.moveInput.normalized * turningStrength), 2.5f * GameUtility.timeDelta);
                controller.RawInputToCameraRelativeMovement(out Quaternion cameraRotation, out _);

                Quaternion currentRotation = Quaternion.LookRotation(fallDirection, cameraRotation * Vector3.up);

                Quaternion rotationDelta = Quaternion.AngleAxis(-fallInput.y, currentRotation * Vector3.right) * Quaternion.AngleAxis(fallInput.x, currentRotation * Vector3.up);
                Vector3 newDirection = (rotationDelta * (fallDirection)).normalized;

                maskedEntity.rotation.SetVal( Quaternion.FromToRotation(fallDirection, newDirection) * maskedEntity.rotation );

                fallDirection = newDirection;
                maskedEntity.absoluteForward = fallDirection;

            }else {

                controller.RawInputToGroundedMovement(out _, out Vector3 groundedMovement);
                float verticalInput = (controller.jumpInput ? 1f: 0f) - (controller.crouchInput ? 1f: 0f);
                Vector3 verticalGroundedMovement = groundedMovement + (maskedEntity.rotation * Vector3.up) * verticalInput;
                
                if (verticalGroundedMovement.sqrMagnitude != 0f){

                    maskedEntity.absoluteForward = verticalGroundedMovement;
                    maskedEntity.rigidbody.velocity += maskedEntity.character.baseSpeed * GameUtility.timeDelta * verticalGroundedMovement;
                }

                flyDirection = Vector3.Lerp(flyDirection, verticalGroundedMovement, 0.5f * GameUtility.timeDelta);

            }

        }

        public override void StateUpdate(){

            // Gravity Shifting Movement
            if ( maskedEntity.inWater ){
                maskedEntity.StopShifting(Vector3.down);
            }

            if ( maskedEntity.onGround && shiftFalling ) {
                maskedEntity.StopShifting(fallDirection);
            }

            
            float newSpeed = maskedEntity.character.baseSpeed * 0.5f;
            newSpeed = shiftFalling ? newSpeed * 0.75f : newSpeed;

            float speedDelta = newSpeed > moveSpeed ? 1f : 0.65f;
            moveSpeed = Mathf.MoveTowards(moveSpeed, newSpeed, speedDelta * maskedEntity.character.acceleration * GameUtility.timeDelta);


            Vector3 finalRotation;
            Vector3 finalUp;
            if (shiftFalling){

                finalRotation = new Vector3(randomRotation.x, 0f, randomRotation.y);
                finalUp = -fallDirection;

            }else{

                finalRotation = maskedEntity.relativeForward;
                finalUp = maskedEntity.rotation * Vector3.up;
            }

            maskedEntity.gravityDown = -finalUp;

            UpdateLandCursorPos();


            maskedEntity.RotateTowardsRelative(finalRotation, finalUp);

        }

        public override void StateFixedUpdate(){

            if (shiftFalling){
            
                maskedEntity.rigidbody.AddForce(fallDirection * 27f);

            } else {
                    
                maskedEntity.Move( moveSpeed * flyDirection );
    
            }
            
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
