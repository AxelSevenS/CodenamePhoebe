using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core{

    // [RequireComponent(typeof(Animator))]
    public class CostumeData : MonoBehaviour {
        public Map<string, GameObject> bones;
        public Map<string, Collider> hurtColliders;
        public Map<string, Collider> hitColliders;

        public RuntimeAnimatorController animatorController;
        public Avatar animatorAvatar;

        public Bounds bounds {
            get {
                Bounds newBounds = new Bounds(transform.position, Vector3.zero);
                foreach( ValuePair<string, Collider> pair in hurtColliders ){
                    Collider collider = pair.Value;
                    newBounds.Encapsulate(collider.bounds);
                }
                return newBounds;
            }
        }

        public Bounds localSpaceBounds {
            get {
                Bounds newBounds = new Bounds(Vector3.zero, Vector3.zero);
                foreach( ValuePair<string, Collider> pair in hurtColliders ){
                    Collider collider = pair.Value;
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
