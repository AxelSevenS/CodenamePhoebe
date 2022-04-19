using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public static class Mathfs{

        public static Vector3 NullifyInDirection( this Vector3 vector, Vector3 direction) => Vector3.Dot(vector, direction) >= 0f ? Vector3.ProjectOnPlane(vector, direction) : vector;

        public static float CalculateWave(float waveStrength, float time, Vector3 coords, float frequency){
            return waveStrength * Mathf.Sin(time + (coords.x + coords.z) * frequency);
        }

        public static bool ColliderCast( this Collider collider, Vector3 position, Vector3 direction, out RaycastHit hit, float skinThickness = 0.15f ){
            if ( collider is CapsuleCollider ) {
                CapsuleCollider capsule = (CapsuleCollider) collider;

                return capsule.SkinnedCapsuleCast( position, direction, skinThickness, out hit);
            }else if ( collider is SphereCollider) {
                SphereCollider sphere = (SphereCollider) collider;

                float skinnedRadius = sphere.radius - Mathf.Min(skinThickness, sphere.radius - 0.01f);
                return Physics.SphereCast(sphere.transform.position, skinnedRadius, direction, out hit, direction.magnitude + skinnedRadius, Global.GroundMask);
            }else if ( collider is BoxCollider) {
                BoxCollider box = (BoxCollider) collider;
                
                Vector3 skinThicknessVector = new Vector3(skinThickness, skinThickness, skinThickness);
                return Physics.BoxCast(box.transform.position, box.size - skinThicknessVector, direction, out hit, box.transform.rotation, 1f, Global.GroundMask);
            }else{
                hit = new RaycastHit();
                return false;
            }
        }

        public static bool SkinnedCapsuleCast( this CapsuleCollider capsule, Vector3 position, Vector3 direction, float skinThickness, out RaycastHit hit){
            Vector3 capsulePosition = position + capsule.transform.rotation * capsule.center;
            float skinnedRadius = capsule.radius - Mathf.Min(skinThickness, capsule.radius - 0.01f);
            float skinnedHalfHeight = capsule.height/2f - skinnedRadius;

            // depending on the collider's "direction" we set the direction of the capsule length; 
            // if capsule.direction is 0, the capsule will extend in the right and left directions;
            Vector3 capsuleDirection = capsule.direction == 0 ? capsule.transform.right : (capsule.direction == 1 ? capsule.transform.up : capsule.transform.forward);

            Vector3 startPosOne = capsulePosition + capsuleDirection * skinnedHalfHeight; 
            Vector3 startPosTwo = capsulePosition - capsuleDirection * skinnedHalfHeight;

            return Physics.CapsuleCast(startPosOne, startPosTwo, skinnedRadius, direction, out hit, direction.magnitude + skinnedRadius, Global.GroundMask);
        }

        public static Vector3 GetSize( this Collider collider ){
            if ( collider is CapsuleCollider ){
                CapsuleCollider capsule = (CapsuleCollider)collider;
                return new Vector3(capsule.radius, capsule.height, capsule.radius);
            }else if ( collider is BoxCollider ){
                BoxCollider box = (BoxCollider)collider;
                return box.size;
            }else if ( collider is SphereCollider ){
                SphereCollider sphere = (SphereCollider)collider;
                return new Vector3(sphere.radius, sphere.radius, sphere.radius);
            }
            return Vector3.zero;
        }

        public static Vector3 GetCenter( this Collider collider ){
            if ( collider is CapsuleCollider )
                return ( (CapsuleCollider)collider ).center;
            else if ( collider is BoxCollider )
                return ( (BoxCollider)collider ).center;
            else if ( collider is SphereCollider )
                return ( (SphereCollider)collider ).center;
            return Vector3.zero;
        }

        public static bool ColliderGroundCheck( this Collider collider, out RaycastHit hit, float skinThickness = 0.15f ){
            if (collider is CapsuleCollider){
                CapsuleCollider capsule = (CapsuleCollider) collider;
                Vector3 colliderPosition = collider.transform.position + collider.transform.rotation * capsule.center;
                float skinnedRadius = capsule.radius - Mathf.Min(skinThickness, capsule.radius - 0.01f);

                float castLength = capsule.height/2f + 0.1f;

                return Physics.SphereCast( colliderPosition, skinnedRadius, -collider.transform.up * castLength, out hit, castLength, Global.GroundMask );
            }else{
                BoxCollider box = (BoxCollider) collider;
                return Physics.BoxCast(box.transform.position, box.size - new Vector3(skinThickness, skinThickness, skinThickness), -collider.transform.up, out hit, box.transform.rotation, 1f, Global.GroundMask);
            }
        }

        
    }
}