using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using SeleneGame.Utility;

namespace SeleneGame.Core {

    public static class EntityUtil {

        
        public static async Task WalkTo( this Entity entity, Vector3 pos, Entity.WalkSpeed speed = Entity.WalkSpeed.walk) {
            entity.walkingTo = true;
            while ((pos - entity.transform.position).magnitude > 0.3f){

                Vector3 dir = Vector3.ProjectOnPlane( pos - entity.transform.position, -entity.gravityDown );
                if (dir.magnitude > 0.4f && ((pos - entity.transform.position).magnitude - dir.magnitude) < 0.3f){
                    entity.SetWalkSpeed(speed);
                    entity.moveDirection.SetVal(dir.normalized);
                }else{
                    entity.moveDirection.SetVal(Vector3.zero);
                    entity.transform.position = Vector3.Lerp(entity.transform.position, pos, GameUtility.timeDelta);
                }
                await Task.Yield();
            }
            entity.walkingTo = false;
            entity.moveDirection.SetVal(Vector3.zero);
        }

        public static async Task TurnTo( this Entity entity, Vector3 dir ) {
            // entity.turningTo = true;
            // Vector3 direction = Vector3.ProjectOnPlane( dir, -entity.gravityDown ).normalized;
            // while (Vector3.Dot(entity.relativeForward.normalized, direction) < 0.99f){
            //     entity.relativeForward = Vector3.Lerp(entity.relativeForward, Quaternion.Inverse(entity.rotation) * direction, 0.1f);
            //     entity.evadeDirection = Vector3.Lerp(entity.evadeDirection, entity.absoluteForward, 0.1f);
            //     // entity.rotationForward = Vector3.Lerp(entity.rotationForward, entity.relativeForward, 0.7f).normalized;
            //     await Task.Yield();
            // }
            // entity.turningTo = false;
        }
    }
}
