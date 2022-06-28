using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public struct OrientedPoint{

        public Vector3 position;
        public Quaternion rotation;

        public OrientedPoint(OrientedPoint op){
            this.position = op.position;
            this.rotation = op.rotation.normalized;
        }
        public OrientedPoint(Vector3 pos, Quaternion rot){
            this.position = pos;
            this.rotation = rot.normalized;
        }
        public OrientedPoint(Vector3 pos){
            this.position = pos;
            this.rotation = Quaternion.identity;
        }
        public OrientedPoint(Transform obj){
            this.position = obj.position;
            this.rotation = obj.rotation.normalized;
        }

        public void Set(Vector3 pos){
            this.position = pos;
        }
        public void Set(Quaternion rot){
            this.rotation = rot.normalized;
        }
        public void Set(Vector3 pos, Quaternion rot){
            this.position = pos;
            this.rotation = rot.normalized;
        }
        public void Set(OrientedPoint op){
            this.position = op.position;
            this.rotation = op.rotation;
        }

        public static OrientedPoint operator +(OrientedPoint a, Vector3 b) => new OrientedPoint(a.position + b, a.rotation);
        public static OrientedPoint operator +(Vector3 a, OrientedPoint b) => new OrientedPoint(b.position + a, b.rotation);
        public static OrientedPoint operator -(OrientedPoint a, Vector3 b) => new OrientedPoint(a.position - b, a.rotation);
        public static OrientedPoint operator -(Vector3 a, OrientedPoint b) => new OrientedPoint(b.position - a, b.rotation);
        public static bool operator ==(OrientedPoint op1, OrientedPoint op2) => op1.position == op2.position && op1.rotation == op2.rotation;
        public static bool operator !=(OrientedPoint op1, OrientedPoint op2) => op1.position != op2.position || op1.rotation != op2.rotation;
    }
}
