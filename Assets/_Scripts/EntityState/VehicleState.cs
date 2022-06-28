using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;

namespace SeleneGame.States {
    
    [System.Serializable]
    public class VehicleState : State{

        public override int id => 7;
        protected override Vector3 GetCameraPosition() => new Vector3(0.3f, 0.5f, -6.5f);

        // public override bool masked => false;

        private float accelerationLinger = 0f;
        private Vector3 finalDirection = Vector3.zero;
        private Vector3 inputDirection;
        

        public override void OnEnter(Entity entity){
            base.OnEnter(entity);

            entity.onJump += OnEntityJump;
        }
        public override void OnExit(){

            entity.onJump -= OnEntityJump;
        }

        public override void StateUpdate(){

            entity.jumpCooldown = Mathf.MoveTowards( entity.jumpCooldown, 0f, Global.timeDelta );

            if (entity.onGround.started) 
                entity.StartWalkAnim();

        }

        public override void StateFixedUpdate(){

            entity.JumpGravity(entity.GravityMultiplier(), entity.gravityDown, entity.jumpInput);
            
            if ( entity.onGround ){
                if( entity.jumpCooldown == 0f )
                    entity.jumpCount = 1;
            }

            entity.absoluteForward = Vector3.Slerp(entity.absoluteForward, inputDirection, Global.timeDelta * 3f).normalized;
            entity.moveDirection.SetVal(entity.absoluteForward);

            entity.GroundedMove( entity.moveSpeed * Global.timeDelta * entity.moveDirection, false );

            bool terrainFlatEnough = Vector3.Dot(entity.groundOrientation * -entity.gravityDown, -entity.gravityDown) > 0.75f;

            Vector3 groundUp = terrainFlatEnough ? entity.groundOrientation * -entity.gravityDown : -entity.gravityDown;
            Vector3 rightDir = Vector3.Cross(entity.absoluteForward, groundUp);
            Vector3 finalUp = (groundUp*4f + (Vector3.Dot( inputDirection, rightDir ) * rightDir)).normalized;
            // finalUp = entity.onGround ? finalUp : (finalUp + finalUp + finalUp - entity._transform.forward).normalized;

            entity.RotateTowardsAbsolute(entity.absoluteForward, finalUp);



            // // When the Entity is sliding
            // if (entity.sliding)
            //     entity._rb.velocity += entity.groundOrientation * entity.evadeDirection *entity.data.baseSpeed * entity.inertiaMultiplier * Global.timeDelta;


        }


        public override void HandleInput(){

            entity.RawInputToGroundedMovement(out Vector3 camRight, out Vector3 camForward, out Vector3 groundDirection, out Vector3 groundDirection3D);

            if (groundDirection.magnitude != 0f)
                inputDirection = groundDirection.normalized;

            float newLinger = groundDirection.magnitude;
            accelerationLinger = Mathf.Lerp(accelerationLinger, newLinger, Global.timeDelta * (newLinger > accelerationLinger ? 3f : 2f) );

            entity.sliding.SetVal(entity.evadeInput && entity.onGround);
            
            // Jump if the Jump key is pressed.
            if ( entity.jumpInput && entity.jumpCount != 0 && entity.onGround.falseTimer <= 0.4f )
                entity.Jump( -entity.gravityDown );
            
            float newSpeed = Vector3.Dot(entity.moveDirection, inputDirection) * accelerationLinger * entity.data.baseSpeed;
            if (entity.walkSpeed != Entity.WalkSpeed.run) 
                newSpeed *= entity.walkSpeed == Entity.WalkSpeed.sprint ? /* entity.data.sprintSpeed */1f : entity.data.slowSpeed;

            float speedDelta = newSpeed > entity.moveSpeed ? 1f : 0.65f;
            entity.moveSpeed = Mathf.MoveTowards(entity.moveSpeed, newSpeed, speedDelta * entity.data.moveIncrement * Global.timeDelta);
        }

        private void OnEntityJump(Vector3 jumpDirection){
            entity.jumpCount--;
            entity.jumpCooldown = 0.4f;
        }
    }
}
