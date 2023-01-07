using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

using SevenGame.Utility;

namespace SeleneGame.Core{

    // [RequireComponent(typeof(Animator))]
    public class CostumeData : MonoBehaviour {
        
        public SerializableDictionary<string, GameObject> bones;
        public List<Collider> hurtColliders;
        public List<Collider> hitColliders;

        public RuntimeAnimatorController animatorController;
        public Avatar animatorAvatar;



        public Bounds bounds {
            get {
                Bounds newBounds = new Bounds(transform.position, Vector3.zero);
                foreach( Collider collider in hurtColliders ){
                    newBounds.Encapsulate(collider.bounds);
                }
                return newBounds;
            }
        }

        public Bounds localSpaceBounds {
            get {
                Bounds newBounds = new Bounds(Vector3.zero, Vector3.zero);
                foreach( Collider collider in hurtColliders ){
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
