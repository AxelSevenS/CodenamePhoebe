using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public sealed class SwimmingBehaviour : EntityBehaviour {

        public const float swimTreshold = 1f;
        public const float surfaceTreshold = 2f;



        private Vector3 moveDirection;
        [SerializeField] private float moveSpeed;

        [SerializeField] private float distanceToWaterSurface;




        public sealed override float gravityMultiplier => 0f; 

        private bool isOnWaterSurface => Mathf.Abs(distanceToWaterSurface) < surfaceTreshold;

        public override Vector3 direction => moveDirection;
        public override float speed => moveSpeed;

        protected override Vector3 jumpDirection => base.jumpDirection;
        protected override Vector3 evadeDirection => base.evadeDirection;
        protected override bool canParry => base.canParry;



        public override void Initialize(Entity entity, EntityBehaviour previousBehaviour = null) {
            _entity = entity;
            entity.gravityDown = Vector3.down;

            _evadeBehaviour = gameObject.AddComponent<EvadeBehaviour>();
            _evadeBehaviour.Initialize(entity);

            if (previousBehaviour == null) return;

            moveDirection = previousBehaviour.direction;
            moveSpeed = previousBehaviour.speed;
        }

        protected internal override void HandleInput(Player controller){

            base.HandleInput(controller);
            
            if ( controller.evadeInput.started )
                Evade(evadeDirection);

            if (isOnWaterSurface && controller.jumpInput.started) {
                JumpOutOfWater();
                // Dispose();
                return;
            }
            
            controller.RawInputToCameraRelativeMovement(out Quaternion cameraRotation, out Vector3 cameraRelativeMovement);
            float verticalInput = (controller.jumpInput ? 1f: 0f) - (controller.crouchInput ? 1f: 0f);

            Move( cameraRelativeMovement + (Vector3.up * verticalInput) );

        }


        protected internal override void Move(Vector3 direction) {
            moveDirection = direction;
        }
        protected internal override void Jump() {
        }

        private void JumpOutOfWater() {
            if (entity.behaviour.GetType() != typeof(SwimmingBehaviour))
                return;

            entity.transform.position = entity.transform.position + (Vector3.up * (Mathf.Max(distanceToWaterSurface + swimTreshold/2f, 0f))); 

            entity.ResetBehaviour();
            
            entity.behaviour.jumpBehaviour.Jump(jumpDirection * 1.1f);
        }

        protected internal override void Evade(Vector3 direction) {
            base.Evade(direction);
        }
        protected internal override void LightAttack() {
            base.LightAttack();
        }
        protected internal override void HeavyAttack() {
            base.HeavyAttack();
        }
        protected internal override void SetSpeed(Entity.MovementSpeed speed) {
            
        }


        protected override void LightAttackAction() {
            base.LightAttackAction();
        }
        protected override void HeavyAttackAction() {
            base.HeavyAttackAction();
        }


        private void OnEnable() {
            entity.gravityDown = Vector3.down;
        }

        private void Update() {

            distanceToWaterSurface = entity.physicsComponent.totalWaterHeight - entity.centerOfMass.y;

            bool canSwim = entity.weightCategory != Entity.WeightCategory.Heavy;
            if ( !entity.inWater || !canSwim )
                entity.ResetBehaviour();

            if (moveDirection.sqrMagnitude != 0f){
                entity.absoluteForward = Vector3.Lerp( entity.absoluteForward, moveDirection, 20f * GameUtility.timeDelta);
            }


            float newSpeed = moveDirection.sqrMagnitude == 0f ? 0f : entity.character.data.baseSpeed;
            float speedDelta = newSpeed > moveSpeed ? 0.5f : 0.25f;
            moveSpeed = Mathf.MoveTowards(moveSpeed, newSpeed, speedDelta * entity.character.data.acceleration * GameUtility.timeDelta);




            Vector3 newUp;
            Vector3 modelForward;
            if (isOnWaterSurface) {                
                newUp = -entity.gravityDown;
                modelForward = Vector3.Cross(newUp, Vector3.Cross(entity.absoluteForward, newUp));
            } else {
                newUp = Vector3.Cross(entity.absoluteForward, Vector3.Cross(entity.absoluteForward, entity.gravityDown));
                modelForward = entity.absoluteForward;
            }

            entity.transform.rotation = Quaternion.FromToRotation(entity.transform.up, -entity.gravityDown) * entity.transform.rotation;

            entity.character.model.RotateTowards(modelForward, newUp, 5f);
            


            float floatingDisplacement = isOnWaterSurface ? distanceToWaterSurface + swimTreshold/3f : 0f;
            Vector3 floatingDisplacementVector = Vector3.up * floatingDisplacement * 3f;

            Vector3 movement = entity.absoluteForward * moveSpeed;
            if (isOnWaterSurface && !entity.onGround) {
                _evadeBehaviour.currentDirection = _evadeBehaviour.currentDirection.NullifyInDirection(Vector3.up);
                entity.inertia = entity.inertia.NullifyInDirection(Vector3.up);
                movement = movement.NullifyInDirection(Vector3.up);
            }

            Vector3 displacement = (floatingDisplacementVector + movement) * GameUtility.timeDelta;

            entity.Displace( displacement, deltaTime: 1f );



            entity.inertia *= 0.95f;

        }
    }
}