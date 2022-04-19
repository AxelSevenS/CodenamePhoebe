using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace SeleneGame.Core {

    public static class EntityUtil {
        
        public static bool GroundCheck( this Entity entity, out RaycastHit groundHitOut) {
            float minRadius = Mathf.Min(entity._colliderBounds.extents.x, Mathf.Min(entity._colliderBounds.extents.y, entity._colliderBounds.extents.z));
            float maxRadius = Mathf.Max(entity._colliderBounds.extents.x, Mathf.Max(entity._colliderBounds.extents.y, entity._colliderBounds.extents.z));
            Debug.DrawLine(entity._transform.position, entity._transform.position + entity.gravityDown*maxRadius, Color.red);
            return Physics.SphereCast( entity._transform.position, (minRadius - 0.05f), entity.gravityDown, out groundHitOut, maxRadius + 0.1f, Global.GroundMask, QueryTriggerInteraction.Ignore );
        }

        public static bool DirectionCheck( this Entity entity, Vector3 direction, out RaycastHit checkHit ) {

            foreach (Collider col in entity._colliders){
                bool hasHitWall = col.ColliderCast( col.transform.position, direction, out RaycastHit tempHit );
                if (!hasHitWall) continue;

                checkHit = tempHit;
                return true;
            }
            checkHit = new RaycastHit();
            return false;
        }

        public static bool WallCheck( this Entity entity, out RaycastHit wallHitOut ) {
            for (int i = 0; i < 11; i++){
                float angle = (i%2 == 0 && i != 0) ? (i-1 * -30f) : i * 30f;
                Quaternion angleTurn = Quaternion.AngleAxis( angle, entity.rotation * Vector3.down);
                bool hasHitWall = entity.DirectionCheck( angleTurn * entity.absoluteForward * 0.3f, out RaycastHit tempHit );
                if (hasHitWall){
                    wallHitOut = tempHit;
                    return true;
                }
            }
            wallHitOut = new RaycastHit();
            return false;
        }

        public static void LookAt( this Entity entity, Vector3 direction) {
            entity.lookRotationData.SetVal( Quaternion.LookRotation( Quaternion.Inverse(entity.rotation) * direction, entity.rotation * Vector3.up ) );
        }
        
        public static async Task WalkTo( this Entity entity, Vector3 pos, Entity.WalkSpeed speed = Entity.WalkSpeed.walk) {
            entity.walkingTo = true;
            while ((pos - entity._transform.position).magnitude > 0.3f){

                Vector3 dir = Vector3.ProjectOnPlane( pos - entity._transform.position, -entity.gravityDown );
                if (dir.magnitude > 0.4f && ((pos - entity._transform.position).magnitude - dir.magnitude) < 0.3f){
                    entity.SetWalkSpeed(speed);
                    entity.moveDirection = dir.normalized;
                }else{
                    entity.moveDirection = Vector3.zero;
                    entity._transform.position = Vector3.Lerp(entity._transform.position, pos, Time.deltaTime);
                }
                await Task.Yield();
            }
            entity.walkingTo = false;
            entity.moveDirection = Vector3.zero;
        }

        public static async Task TurnTo( this Entity entity, Vector3 dir ) {
            entity.turningTo = true;
            Vector3 direction = Vector3.ProjectOnPlane( dir, -entity.gravityDown ).normalized;
            while (Vector3.Dot(entity.relativeForward.normalized, direction) < 0.99f){
                entity.relativeForward = Vector3.Lerp(entity.relativeForward, Quaternion.Inverse(entity.rotation) * direction, 0.1f);
                entity.evadeDirection = Vector3.Lerp(entity.evadeDirection, entity.absoluteForward, 0.1f);
                entity.rotationForward = Vector3.Lerp(entity.rotationForward, entity.relativeForward, 0.7f).normalized;
                await Task.Yield();
            }
            entity.turningTo = false;
        }
    }
}
