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


        

        private BoolData wallData = new BoolData();
        private BoolData wallRunData = new BoolData();

        public RaycastHit wallHit;

        private bool wallRun => wallRunData.currentValue;


        protected override float GetSpeedMultiplier(){
            return entity.state is ShiftingState ? 1.5f : 1f;
        }

        protected override float GetWeightModifier() => 0.75f;

        // protected override Vector3 GetJumpDirection(){
        //     if (wallStand && !wallRun)
        //         return (wallHit.normal + -entity.gravityDown + Vector3.ProjectOnPlane(entity.absoluteForward, wallHit.normal).normalized) / 2f;
        //     else if (wallRun)
        //         return (wallRunDir + wallHit.normal - entity.gravityDown * 2f)/4f;
        //     return  -entity.gravityDown;
        // }

        // protected override Vector3 GetCameraPosition(){
        //     if (wallRun && Vector3.Dot( -wallHit.normal, entity._transform.right ) > 0.75f)
        //         return new Vector3(-1f, 0f, -3.5f);
        //     else
        //         return base.GetCameraPosition();
        // }

        // protected override Vector3 GetOverrideRotation(){
        //     if (wallRun)
        //         return wallRunDir;
        //     else if (wallStand)
        //         return -wallHit.normal;
        //     return entity.absoluteForward;
        // }


        // protected override void WeaponAwake(){
        // }

        protected override void WeaponEnable(){
        }
        protected override void WeaponDisable(){
        }

        protected override void UpdateAlways(){;}
        
        protected override void UpdateEquipped(){
            
            wallData.SetVal( entity.WallCheck( out wallHit, Global.GroundMask ) );
            wallRunData.SetVal(entity.sliding && wallData.currentValue);

            if (entity.groundData.stopped)
                wallRunTimer = 7f;

        }

        private void LateUpdate() {
            wallData.Update();
            wallRunData.Update();
        }

        protected override void FixedUpdateEquipped(){

            // // Wall-stand when standing against a wall. (Feather Grip)
            // if ( wallStand ){
            //     entity._rb.velocity = entity._rb.velocity.NullifyInDirection(entity.gravityDown);
            //     entity.Move( -wallHit.normal * Global.timeDelta * (wallHit.distance - 0.2f) );
            // }

            // // Wall-run when running against a wall. (Feather Grip)
            // if ( wallRun ){

            //     Debug.DrawRay(transform.position, wallRunDir, Color.red);
                
            //     entity.state.evadeCount = 1;
            //     entity.state.jumpCount = 1;

            //     Quaternion dirChange = Quaternion.FromToRotation( wallRunNormal, wallHit.normal );
            //     wallRunDir = dirChange * wallRunDir;
            //     wallRunNormal = wallHit.normal;

            //     entity.inertiaMultiplier = Mathf.Max( entity.inertiaMultiplier, 10f );
            //     // entity.evadeDirection = wallRunDir;


            //     wallRunTimer = Mathf.MoveTowards(wallRunTimer, 0f, Global.timeDelta);

            //     entity.Move(wallRunDir * Global.timeDelta * entity.data.baseSpeed * 0.45f);

            //     if ( entity.jumpInputData.currentValue )
            //         entity.Jump( (wallRunDir + wallHit.normal*1.2f - entity.gravityDown*2f).normalized * 1.4f );
            // }
        }

        // private void OnStartWallStanding(){

        //     entity._rb.velocity = entity._rb.velocity.NullifyInDirection(entity.gravityDown);
        //     entity.evadeTimer = 0;
        //     entity.inertiaMultiplier = Mathf.Max( entity.inertiaMultiplier, 12f );
        // }

        private void OnStartWallRunning(){
            
            wallRunNormal = wallHit.normal;
            wallRunDir = Vector3.ProjectOnPlane(entity.moveDirection, wallHit.normal).normalized;

            // entity.inertia = wallRunDir * Mathf.Min(entity.inertiaMultiplier + 3.5f, 10f);
        }
    }
}