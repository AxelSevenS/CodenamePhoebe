using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SevenGame.Utility {
    
    public static class Mathfs{

        public static OrientedPoint TransformPoint(this Transform transform, OrientedPoint orientedPoint){
            return new OrientedPoint(transform.TransformPoint(orientedPoint.position), transform.rotation * orientedPoint.rotation);
        }
        public static OrientedPoint InverseTransformPoint(this Transform transform, OrientedPoint orientedPoint){
            return new OrientedPoint(transform.InverseTransformPoint(orientedPoint.position), Quaternion.Inverse(transform.rotation) * orientedPoint.rotation);
        }

        public static Vector3 NullifyInDirection( this Vector3 vector, Vector3 direction) => Vector3.Dot(vector, direction) >= 0f ? Vector3.ProjectOnPlane(vector, direction) : vector;

        public static float CalculateWave(float waveStrength, float time, Vector3 coords, float frequency){
            return waveStrength * Mathf.Sin(time + (coords.x + coords.z) * frequency);
        }

    }
}