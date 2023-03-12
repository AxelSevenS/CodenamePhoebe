using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

using SevenGame.Utility;

namespace SeleneGame.Core{

    public class ModelProperties : MonoBehaviour {
        
        public SerializableDictionary<string, GameObject> bones;
        
        public List<Collider> colliders;

        public Avatar animatorAvatar;



        public Bounds bounds {
            get {
                Bounds newBounds = new Bounds(transform.position, Vector3.zero);
                foreach( Collider collider in colliders ){
                    newBounds.Encapsulate(collider.bounds);
                }
                return newBounds;
            }
        }

        public Bounds localSpaceBounds {
            get {
                Bounds newBounds = new Bounds(Vector3.zero, Vector3.zero);
                foreach( Collider collider in colliders ){
                    Bounds colliderBounds = new Bounds(
                        transform.InverseTransformVector(collider.transform.position - transform.position) + collider.GetCenter(),
                        collider.GetSize()
                    );
                    Debug.Log(colliderBounds.ToString());
                    newBounds.Encapsulate(colliderBounds);
                }
                return newBounds;
            }
        }

    }
}
