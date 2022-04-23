using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.Entities;
using SeleneGame.States;

namespace SeleneGame.Weapons {
    
    public class HypnosWeapon : Weapon{

        private Vector3 wallRunDir;
        private Vector3 wallRunNormal;
        [SerializeField] private float wallRunTimer = 4f;

        public override bool shifting => wallRun;

        [SerializeField] private BoolData wallStandData;
        [SerializeField] private BoolData wallRunData;


        private bool wallStand => wallStandData.currentValue;
        private bool wallRun => wallRunData.currentValue;


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


        protected override void WeaponAwake(){
            wallStandData = new BoolData(OnStartWallStanding);
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
            
            wallStandData.SetVal(entity.onWall && wallRunTimer > 0f && entity.groundData.falseTimer > 0.4f );
            wallRunData.SetVal(entity.sliding && wallStand && entity.jumpCooldown == 0f);

        }

        private void LateUpdate() {
            wallRunData.Update();
        }

        protected override void FixedUpdateEquipped(){

            // Wall-stand when standing against a wall. (Feather Grip)
            if ( wallStand ){
                entity._rb.velocity = entity._rb.velocity.NullifyInDirection(entity.gravityDown);
                entity.Move( -entity.wallHit.normal * Global.timeDelta * (entity.wallHit.distance - 0.2f) );
            }

            // Wall-run when running against a wall. (Feather Grip)
            if ( wallRun ){
                
                entity.state.evadeCount = 1;
                entity.state.jumpCount = 1;

                Quaternion dirChange = Quaternion.FromToRotation( wallRunNormal, entity.wallHit.normal );
                wallRunDir = dirChange * wallRunDir;
                wallRunNormal = entity.wallHit.normal;

                entity.inertiaMultiplier = Mathf.Max( entity.inertiaMultiplier, 10f );
                // entity.evadeDirection = wallRunDir;


                wallRunTimer = Mathf.MoveTowards(wallRunTimer, 0f, Global.timeDelta);

                entity.Move(wallRunDir * Global.timeDelta * entity.data.baseSpeed * 0.45f);

                if ( entity.jumpInputData.currentValue )
                    entity.Jump( (wallRunDir + entity.wallHit.normal*1.2f - entity.gravityDown*2f).normalized * 1.4f );
            }
        }

        private void OnStartWallStanding(){

            entity._rb.velocity = entity._rb.velocity.NullifyInDirection(entity.gravityDown);
            entity.evadeTimer = 0;
            entity.inertiaMultiplier = Mathf.Max( entity.inertiaMultiplier, 12f );
        }

        private void OnStartWallRunning(){
            
            wallRunNormal = entity.wallHit.normal;
            wallRunDir = Vector3.ProjectOnPlane(entity.moveDirection, entity.wallHit.normal).normalized;

            entity.inertia = wallRunDir * Mathf.Min(entity.inertiaMultiplier + 3.5f, 10f);
        }

        private void OnEntityLand(){
            wallRunTimer = 7f;
        }
    }
}