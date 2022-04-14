using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public abstract class ObjectFollower : MonoBehaviour{

        [SerializeField] protected GameObject followedObject;
        [SerializeField] protected Vector3 relativePlacement = new Vector3(0,0,0);
        [SerializeField] protected Vector3 directionalPlacement = new Vector3(0,0,0);
        public virtual void SetFollowedObject(GameObject newTarget) => followedObject = newTarget;
        protected void FollowObject(){
            if (followedObject == null) return;

            transform.position = followedObject.transform.position + relativePlacement + followedObject.transform.rotation * directionalPlacement;
            transform.rotation = followedObject.transform.rotation;
        }
    }
}