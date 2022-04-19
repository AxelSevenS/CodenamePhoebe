using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    [System.Serializable]
    public class OrientedPoint{

        public Vector3 position;
        public Quaternion rotation;

        public OrientedPoint(){
            this.position = Vector3.zero;
            this.rotation = Quaternion.identity;
        }
        public OrientedPoint(OrientedPoint op){
            this.position = op.position;
            this.rotation = op.rotation;
        }
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

        public void Set(Vector3 pos){
            this.position = pos;
        }
        public void Set(Quaternion rot){
            this.rotation = rot;
        }
        public void Set(Vector3 pos, Quaternion rot){
            this.position = pos;
            this.rotation = rot;
        }
        public void Set(OrientedPoint op){
            this.position = op.position;
            this.rotation = op.rotation;
        }
    }
}
