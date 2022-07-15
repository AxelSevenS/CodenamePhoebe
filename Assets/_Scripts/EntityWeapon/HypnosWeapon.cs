using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;
using SeleneGame.Core;
using SeleneGame.Entities;
using SeleneGame.States;

namespace SeleneGame.Weapons {
    
    [System.Serializable]
    public class HypnosWeapon : Weapon{
        public override WeaponType weaponType => WeaponType.spear;

        // private Vector3 wallRunDir;
        // private Vector3 wallRunNormal;
        // [SerializeField] private float wallRunTimer = 4f;


        

        // private BoolData wallData = new BoolData();
        // private BoolData wallRun = new BoolData();

        // public RaycastHit wallHit;


        protected override float GetWeightModifier() => 0.75f;

        public override void WeaponUpdate(){
            base.WeaponUpdate();

            if (!isEquipped) return;
            
            // wallData.SetVal( entity.WallCheck( out wallHit, Global.GroundMask ) );
            // wallRun.SetVal(entity.sliding && wallData);

            // if (entity.onGround.stopped)
            //     wallRunTimer = 7f;

        }

        public override void WeaponFixedUpdate(){
            base.WeaponFixedUpdate();

            if (!isEquipped) return;

            // // Wall-stand when standing against a wall. (Feather Grip)
            // if ( wallStand ){
            //     entity.rb.velocity = entity.rb.velocity.NullifyInDirection(entity.gravityDown);
            //     entity.Move( -wallHit.normal * GameUtility.timeDelta * (wallHit.distance - 0.2f) );
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


            //     wallRunTimer = Mathf.MoveTowards(wallRunTimer, 0f, GameUtility.timeDelta);

            //     entity.Move(wallRunDir * GameUtility.timeDelta * entity.data.baseSpeed * 0.45f);

            //     if ( entity.jumpInput )
            //         entity.Jump( (wallRunDir + wallHit.normal*1.2f - entity.gravityDown*2f).normalized * 1.4f );
            // }
        }

        // private void OnStartWallStanding(){

        //     entity.rb.velocity = entity.rb.velocity.NullifyInDirection(entity.gravityDown);
        //     entity.evadeTimer = 0;
        //     entity.inertiaMultiplier = Mathf.Max( entity.inertiaMultiplier, 12f );
        // }

        private void OnStartWallRunning(){
            
            // wallRunNormal = wallHit.normal;
            // wallRunDir = Vector3.ProjectOnPlane(entity.moveDirection, wallHit.normal).normalized;

            // entity.inertia = wallRunDir * Mathf.Min(entity.inertiaMultiplier + 3.5f, 10f);
        }
    }
}