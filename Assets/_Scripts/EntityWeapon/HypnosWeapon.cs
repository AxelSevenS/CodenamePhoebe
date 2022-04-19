using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.Entities;
using SeleneGame.States;

namespace SeleneGame.Weapons {
    
    public class HypnosWeapon : Weapon{

        [SerializeField] private bool wallStand;

        private bool wallRun => wallRunData.currentValue;
        private Vector3 wallRunDir;
        private Vector3 wallRunNormal;
        [SerializeField] private float wallRunTimer = 4f;


        protected override float GetSpeedMultiplier(){
            if (wallStand && !wallRun){
                return 0f;
            }else{
                return entity.state is ShiftingState ? 1.5f : 1f;
            }
        }

        protected override float GetWeightModifier() => 0.75f;

        protected override Vector3 GetJumpDirection(){
            if (wallStand && !wallRun)
                return (entity.wallHit.normal + -entity.gravityDown + Vector3.ProjectOnPlane(entity.absoluteForward, entity.wallHit.normal).normalized) / 2f;
            else if (wallRun)
                return (wallRunDir + entity.wallHit.normal - entity.gravityDown * 2f)/4f;
            return  -entity.gravityDown;
        }

        protected override Vector3 GetCameraPosition(){
            if (wallRun && Vector3.Dot( -entity.wallHit.normal, entity._transform.right ) > 0.75f)
                return new Vector3(-1f, 0f, -3.5f);
            else
                return base.GetCameraPosition();
        }

        protected override Vector3 GetOverrideRotation(){
            if (wallRun)
                return Quaternion.Inverse(entity.rotation) * wallRunDir;
            else if (wallStand)
                return Quaternion.Inverse(entity.rotation) * -entity.wallHit.normal;
            return entity.relativeForward;
        }

        public override bool canJump => wallStand;
        public override bool canEvade => wallStand;
        public override bool cannotTurn => wallRun;
        public override bool noGravity => wallStand;

        public override bool shifting => wallRun;

        private BoolData wallRunData;

        protected override void WeaponAwake(){
            wallRunData = new BoolData(OnStartWallRunning);
        }

        protected override void WeaponEnable(){
            entity.groundData.stopped += _ => OnEntityLand();
        }
        protected override void WeaponDisable(){
            entity.groundData.stopped -= _ => OnEntityLand();
        }

        protected override void UpdateAlways(){;}
        
        protected override void UpdateEquipped(){

            bool wallStandingThisFrame = entity.onWall && wallRunTimer > 0f && !entity.onGround;
            // When Starting to WallStand
            if (wallStandingThisFrame && !wallStand){

                entity._rb.velocity = Vector3.ProjectOnPlane(entity._rb.velocity, -entity.gravityDown);
                entity.evadeTimer = 0;
                entity.state.evadeCount = 1;
                entity.inertiaMultiplier = 3.5f;
            }
            wallStand = wallStandingThisFrame;

            
            wallRunData.SetVal(entity.sliding && wallStand);



            // Wall-stand when standing against a wall. (Feather Grip)
            if ( wallStand ){

                entity.Move( -entity.wallHit.normal * Time.deltaTime * (entity.wallHit.distance - 0.2f) );
            }

            // Wall-run when running against a wall. (Feather Grip)
            if ( wallRun ){
                Quaternion dirChange = Quaternion.FromToRotation( wallRunNormal, entity.wallHit.normal );
                wallRunDir = dirChange * wallRunDir;
                wallRunNormal = entity.wallHit.normal;

                entity.inertiaMultiplier = Mathf.Max( entity.inertiaMultiplier, 10f );
                // entity.evadeDirection = wallRunDir;

                entity.GroundedMove(wallRunDir * Time.deltaTime * entity.data.baseSpeed * 0.45f);

                wallRunTimer = Mathf.MoveTowards(wallRunTimer, 0f, Time.deltaTime);
            }
        }

        private void LateUpdate() {
            wallRunData.Update();
        }

        private void OnStartWallRunning(){
            if (entity.state is WalkingState){
                ((WalkingState)entity.state).jumpCount = 1f;
            }
            
            wallRunNormal = entity.wallHit.normal;
            wallRunDir = Vector3.ProjectOnPlane(entity.moveDirection, entity.wallHit.normal).normalized;

            entity.inertia = wallRunDir * Mathf.Min(entity.inertiaMultiplier + 3.5f, 10f);
        }

        private void OnEntityLand(){
            wallRunTimer = 7f;
        }
    }
}