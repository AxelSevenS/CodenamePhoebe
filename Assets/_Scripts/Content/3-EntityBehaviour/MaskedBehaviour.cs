using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.Content {
    
    [System.Serializable]
    public class MaskedBehaviour : EntityBehaviour {

        private MaskedEntity maskedEntity;

        public BoolData shiftFalling = new BoolData();

        private Vector2 randomRotation = Vector3.zero;
        private Vector3 fallDirection = Vector3.forward;
        private Vector3 flyDirection = Vector3.forward;

        private Vector2 fallInput = Vector3.zero;

        private GameObject landCursor;



        public override float gravityMultiplier => 0f;
        // public override Vector3 cameraPosition {
        //     get {
        //         float additionalCameraDistance = 0f; //(maskedEntity.focusing ? -0.7f : 0f) + (maskedEntity.walkSpeed == Entity.WalkSpeed.sprint ? 0.4f : 0f);
        //         if (shiftFalling) {
        //             return new Vector3(0f, 1f, -4f) - new Vector3(0,0,additionalCameraDistance);
        //         } else {
        //             return new Vector3(0.7f, 0.8f, -2.5f) - new Vector3(0,0,additionalCameraDistance);
        //         }
        //     }
        // }

        public override Vector3 direction => shiftFalling ? fallDirection : flyDirection;
        public override float speed => entity.rigidbody.velocity.magnitude;

        protected override Vector3 jumpDirection => Vector3.zero;

        protected override Vector3 evadeDirection => base.evadeDirection;

        protected override bool canParry => base.canParry;


        public MaskedBehaviour(Entity entity, EntityBehaviour previousBehaviour) : base(entity, previousBehaviour) {
            if (entity is MaskedEntity maskedEntity) {
                this.maskedEntity = maskedEntity;
            } else {
                Debug.LogError("MaskedBehaviour requires a MaskedEntity");
                entity.ResetBehaviour();
                return;
            }

            _evadeBehaviour = new EvadeBehaviour(maskedEntity);

            if (previousBehaviour == null) return;

            fallDirection = previousBehaviour.direction;
            flyDirection = previousBehaviour.direction;
        }

        protected override void DisposeBehavior() {
            base.DisposeBehavior();
            landCursor = GameUtility.SafeDestroy(landCursor);
        }


        protected override void HandleInput(PlayerEntityController controller){

            base.HandleInput(controller);
            
            if ( controller.evadeInput.started )
                Evade(evadeDirection);

            SetSpeed( controller.jumpInput ? Entity.MovementSpeed.Normal : Entity.MovementSpeed.Slow );

            if (shiftFalling.started){
                controller.RawInputToCameraRelativeMovement(out Quaternion cameraRotation, out _);
                fallDirection = cameraRotation * Vector3.forward;
                randomRotation = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;

                Debug.DrawRay(maskedEntity.transform.position, 10f * fallDirection, Color.red, 3f);
            }


            Vector3 movementDirection;

            if (shiftFalling){

                const float turningStrength = 0.65f;

                fallInput = Vector2.Lerp(fallInput, (controller.moveInput.normalized * turningStrength), 2.5f * GameUtility.timeDelta);
                controller.RawInputToCameraRelativeMovement(out Quaternion cameraRotation, out _);

                Quaternion currentRotation = Quaternion.LookRotation(fallDirection, cameraRotation * Vector3.up);

                Quaternion rotationDelta = Quaternion.AngleAxis(-fallInput.y, currentRotation * Vector3.right) * Quaternion.AngleAxis(fallInput.x, currentRotation * Vector3.up);
                movementDirection = (rotationDelta * (fallDirection)).normalized;

            }else {

                controller.RawInputToGroundedMovement(out _, out Vector3 groundedMovement);
                float verticalInput = (controller.jumpInput ? 1f: 0f) - (controller.crouchInput ? 1f: 0f);
                movementDirection = groundedMovement + (maskedEntity.transform.rotation * Vector3.up) * verticalInput;

            }

            Move(movementDirection);

        }


        protected override void Move(Vector3 direction) {
            if (shiftFalling) {

                maskedEntity.transform.rotation = Quaternion.FromToRotation(fallDirection, direction) * maskedEntity.transform.rotation;
                maskedEntity.absoluteForward = direction;

                fallDirection = direction;
            } else {

                flyDirection = Vector3.Lerp(flyDirection, direction, 0.5f * GameUtility.timeDelta);

                if (direction.sqrMagnitude != 0f){

                    maskedEntity.absoluteForward = direction;
                    maskedEntity.rigidbody.velocity += direction * maskedEntity.character.data.baseSpeed * GameUtility.timeDelta;
                }
            }
        }
        protected override void Evade(Vector3 direction) {
            base.Evade(direction);
        }
        protected override void Jump() {
            
        }
        protected override void LightAttack() {
            base.LightAttack();
        }
        protected override void HeavyAttack() {
            base.HeavyAttack();
        }
        protected override void SetSpeed(Entity.MovementSpeed speed) {
            shiftFalling.SetVal(speed != Entity.MovementSpeed.Slow);
        }


        protected override void LightAttackAction() {
            base.LightAttackAction();
        }
        protected override void HeavyAttackAction() {
            base.HeavyAttackAction();
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

        public override void Update(){

            base.Update();

            // Gravity Shifting Movement
            if ( maskedEntity.inWater ){
                maskedEntity.gravityDown = Vector3.down;
                maskedEntity.ResetBehaviour();
                return;
            }

            if ( maskedEntity.onGround && shiftFalling ) {
                maskedEntity.gravityDown = fallDirection;
                maskedEntity.ResetBehaviour();
                return;
            }


            maskedEntity.gravityDown = shiftFalling ? fallDirection : maskedEntity.transform.rotation * Vector3.down;



            if (shiftFalling){
            
                maskedEntity.rigidbody.AddForce(fallDirection * 27f);

            } else {
                    
                maskedEntity.Displace( (maskedEntity.character.data.baseSpeed * 0.5f) * flyDirection );
    
            }

            Vector3 forwardDirection = shiftFalling ? fallDirection : maskedEntity.absoluteForward;

            maskedEntity.character.model.RotateTowards(forwardDirection, -maskedEntity.gravityDown);

        }

    }
    
}
