using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public static class Mathfs{

        public static Vector3 NullifyInDirection( this Vector3 vector, Vector3 direction) => Vector3.Dot(vector, direction) >= 0f ? Vector3.ProjectOnPlane(vector, direction) : vector;

        public static float CalculateWave(float waveStrength, float time, Vector3 coords, float frequency){
            return waveStrength * Mathf.Sin(time + (coords.x + coords.z) * frequency);
        }

        public static bool ColliderCast( this Collider collider, Vector3 position, Vector3 direction, float stepHeight, out RaycastHit hit, float skinThickness = 0.15f ){
            if ( collider is CapsuleCollider){
                CapsuleCollider capsule = (CapsuleCollider) collider;
                Vector3 colliderPosition = position + collider.transform.rotation * capsule.center;
                float skinnedRadius = capsule.radius - Mathf.Min(skinThickness, capsule.radius - 0.01f);
                float skinnedHalfHeight = capsule.height/2f - skinnedRadius;

                Vector3 startPosUp = colliderPosition + collider.transform.up * skinnedHalfHeight;
                Vector3 startPosDown = colliderPosition - collider.transform.up * (skinnedHalfHeight - stepHeight);

                return Physics.CapsuleCast(startPosUp, startPosDown, skinnedRadius, direction, out hit, direction.magnitude + skinnedRadius, Global.GroundMask);
            }else{
                BoxCollider box = (BoxCollider) collider;
                return Physics.BoxCast(box.transform.position, box.size - new Vector3(skinThickness, skinThickness, skinThickness), direction, out hit, box.transform.rotation, 1f, Global.GroundMask);
            }
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




        public struct BezierCurve{
            public OrientedPoint controlPoint1;
            public OrientedPoint controlPoint2;
            public OrientedPoint handle1;
            public OrientedPoint handle2;

            public BezierCurve(Vector3 cp1Pos, Vector3 cp2Pos, Vector3 h1Pos, Vector3 h2Pos){
                this.controlPoint1 = new OrientedPoint(cp1Pos);
                this.controlPoint2 = new OrientedPoint(cp2Pos);
                this.handle1 = new OrientedPoint(h1Pos);
                this.handle2 = new OrientedPoint(h2Pos);
            }
            public BezierCurve(OrientedPoint cp1Pos, OrientedPoint cp2Pos, OrientedPoint h1Pos, OrientedPoint h2Pos){
                this.controlPoint1 = cp1Pos;
                this.controlPoint2 = cp2Pos;
                this.handle1 = h1Pos;
                this.handle2 = h2Pos;
            }
            public BezierCurve(Transform cp1Pos, Transform cp2Pos, Transform h1Pos, Transform h2Pos){
                this.controlPoint1 = new OrientedPoint(cp1Pos);
                this.controlPoint2 = new OrientedPoint(cp2Pos);
                this.handle1 = new OrientedPoint(h1Pos);
                this.handle2 = new OrientedPoint(h2Pos);
            }

            public OrientedPoint GetPoint(float t){
                Vector3 a = Vector3.Lerp(controlPoint1.position, handle1.position, t);
                Vector3 b = Vector3.Lerp(handle1.position, handle2.position, t);
                Vector3 c = Vector3.Lerp(handle2.position, controlPoint2.position, t);

                Vector3 d = Vector3.Lerp(a, b, t);
                Vector3 e = Vector3.Lerp(b, c, t);

                Vector3 tUp = Quaternion.Lerp(controlPoint1.rotation, controlPoint2.rotation, t) * Vector3.up;
                Quaternion rotation = Quaternion.LookRotation( (e - d).normalized, tUp);

                return new OrientedPoint( Vector3.Lerp( d, e, t ), rotation);
            }

            public Vector3 GetVelocity(float t){
                float tPow2 = Mathf.Pow(t, 2f);
                Vector3 p0 = controlPoint1.position * ( (-3f * tPow2) + (6f * t) - 3f );
                Vector3 p1 = handle1.position * ( (9f * tPow2) - (12f * t) + 3f );
                Vector3 p2 = handle2.position * ( (-9f * tPow2) + (6f * t) );
                Vector3 p3 = controlPoint2.position * ( 3f * tPow2 );

                return p0 + p1 + p2 + p3;
            }

            public Vector3 GetAcceleration(float t){
                Vector3 p0 = controlPoint1.position * ( -6f * t + 6f );
                Vector3 p1 = handle1.position * ( 18f * t - 12f );
                Vector3 p2 = handle2.position * ( -18f * t + 6f );
                Vector3 p3 = controlPoint2.position * ( 6f * t );

                return p0 + p1 + p2 + p3;
            }

            // public Vector3 GetCurvature(float t){
            //     Vector3 velo = GetVelocity(t);
            //     Vector3 accel = GetAcceleration(t);

            //     return ( Vector3.Dot() );
            // }
        }




        public struct OrientedPoint{
            public Vector3 position;
            public Quaternion rotation;

            public OrientedPoint(Vector3 pos, Quaternion rot){
                this.position = pos;
                this.rotation = rot;
            }
            public OrientedPoint(Vector3 pos){
                this.position = pos;
                this.rotation = Quaternion.identity;
            }
            public OrientedPoint(Transform obj){
                this.position = obj.position;
                this.rotation = obj.rotation;
            }
        }
        
    }
}