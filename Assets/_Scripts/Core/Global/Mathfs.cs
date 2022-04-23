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


            Transform capsuleTransform = capsule.transform;
            skinThickness = Mathf.Max(skinThickness, 0.01f);

            // depending on the collider's "direction" we set the direction of the capsule length; 
            // if capsule.direction is 0, the capsule will extend in the collider's right and left directions;
            Vector3 capsuleDirection;
            float heightScale;
            float radiusScale;
            switch (capsule.direction){
                default:
                    capsuleDirection = capsuleTransform.right;
                    heightScale = capsuleTransform.localScale.x;
                    radiusScale = Mathf.Max(capsuleTransform.localScale.y, capsuleTransform.localScale.z);
                    break;
                case 1:
                    capsuleDirection = capsuleTransform.up;
                    heightScale = capsuleTransform.localScale.y;
                    radiusScale = Mathf.Max(capsuleTransform.localScale.x, capsuleTransform.localScale.z);
                    break;
                case 2:
                    capsuleDirection = capsuleTransform.forward;
                    heightScale = capsuleTransform.localScale.z;
                    radiusScale = Mathf.Max(capsuleTransform.localScale.x, capsuleTransform.localScale.y);
                    break;
            }

            Vector3 capsulePosition = position + capsuleTransform.rotation * Vector3.Scale(capsule.center, capsuleTransform.localScale);
            float scaledRadius = (capsule.radius * radiusScale);

            float scaledHalfHeight = capsule.height/2f * heightScale;
            Vector3 capsuleHalf = (scaledHalfHeight - scaledRadius) * capsuleDirection;

            Vector3 startPos = capsulePosition + capsuleHalf; 
            Vector3 endPos = capsulePosition - capsuleHalf;
            float skinnedRadius = scaledRadius - skinThickness;

            // Debug.DrawLine(startPos, endPos, Color.red);
            // Debug.DrawLine(startPos, startPos + capsuleDirection * skinnedRadius, Color.blue);
            // Debug.DrawLine(endPos, endPos - capsuleDirection * skinnedRadius, Color.blue);

            var result = Physics.CapsuleCast(startPos, endPos, skinnedRadius, direction.normalized, out hit, direction.magnitude + skinThickness, Global.GroundMask);

            return result;
        }

        // public static bool SkinnedCapsuleCast( this CapsuleCollider capsule, Vector3 position, Vector3 direction, float skinThickness, out RaycastHit hit){
        //     Transform capsuleTransform = capsule.transform;
        //     skinThickness = Mathf.Max(skinThickness, 0.01f);
        //     float totalScale = capsuleTransform.localScale.x * capsuleTransform.localScale.y * capsuleTransform.localScale.z;

        //     // depending on the collider's "direction" we set the direction of the capsule length; 
        //     // if capsule.direction is 0, the capsule will extend in the collider's right and left directions;
        //     Vector3 capsuleDirection = (capsule.direction == 0 ? capsuleTransform.right : capsule.direction == 1 ? capsuleTransform.up : capsuleTransform.forward).normalized;
        //     float heightScale = capsule.direction == 0 ? capsuleTransform.localScale.x : capsule.direction == 1 ? capsuleTransform.localScale.y : capsuleTransform.localScale.z;
        //     float radiusScale = totalScale / heightScale;

        //     Vector3 capsulePosition = position + capsuleTransform.rotation * Vector3.Scale(capsule.center, capsuleTransform.localScale);
        //     float scaledRadius = (capsule.radius * radiusScale);

        //     float scaledHalfHeight = capsule.height/2f * heightScale;
        //     Vector3 capsuleHalf = (scaledHalfHeight - scaledRadius) * capsuleDirection;

        //     Vector3 startPos = capsulePosition + capsuleHalf; 
        //     Vector3 endPos = capsulePosition - capsuleHalf;
        //     float skinnedRadius = scaledRadius - skinThickness;

        //     // Debug.DrawLine(startPos, endPos, Color.red);
        //     // Debug.DrawLine(startPos, startPos + capsuleDirection * skinnedRadius, Color.blue);
        //     // Debug.DrawLine(endPos, endPos - capsuleDirection * skinnedRadius, Color.blue);

        //     return Physics.CapsuleCast(startPos, endPos, skinnedRadius, direction.normalized, out hit, direction.magnitude + skinThickness, Global.GroundMask);
        // }

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