using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public class BezierCurve{

        public OrientedPoint controlPoint1;
        public OrientedPoint controlPoint2;
        public OrientedPoint handle1;
        public OrientedPoint handle2;

        public BezierCurve(){
            controlPoint1 = new OrientedPoint();
            controlPoint2 = new OrientedPoint();
            handle1 = new OrientedPoint();
            handle2 = new OrientedPoint();
        }
        public BezierCurve(Vector3 cp1Pos, Vector3 cp2Pos, Vector3 h1Pos, Vector3 h2Pos){
            controlPoint1 = new OrientedPoint(cp1Pos);
            controlPoint2 = new OrientedPoint(cp2Pos);
            handle1 = new OrientedPoint(h1Pos);
            handle2 = new OrientedPoint(h2Pos);
        }
        public BezierCurve(OrientedPoint cp1Pos, OrientedPoint cp2Pos, OrientedPoint h1Pos, OrientedPoint h2Pos){
            controlPoint1 = cp1Pos;
            controlPoint2 = cp2Pos;
            handle1 = h1Pos;
            handle2 = h2Pos;
        }
        public BezierCurve(Transform cp1Pos, Transform cp2Pos, Transform h1Pos, Transform h2Pos){
            controlPoint1 = new OrientedPoint(cp1Pos);
            controlPoint2 = new OrientedPoint(cp2Pos);
            handle1 = new OrientedPoint(h1Pos);
            handle2 = new OrientedPoint(h2Pos);
        }

        public void Move(Vector3 direction){
            controlPoint1.position += direction;
            controlPoint2.position += direction;
            handle1.position += direction;
            handle2.position += direction;
        } 

        public OrientedPoint GetPoint(float t){
            Vector3 a = Vector3.Lerp(controlPoint1.position, handle1.position, t);
            Vector3 b = Vector3.Lerp(handle1.position, handle2.position, t);
            Vector3 c = Vector3.Lerp(handle2.position, controlPoint2.position, t);

            Vector3 d = Vector3.Lerp(a, b, t);
            Vector3 e = Vector3.Lerp(b, c, t);

            Vector3 tForward = (e - d).normalized;
            Vector3 tUp = Quaternion.Lerp(controlPoint1.rotation, controlPoint2.rotation, t) * Vector3.up;
            Quaternion rotation = tForward == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation( tForward, tUp);

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

}
