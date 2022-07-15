using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core{

    [RequireComponent(typeof(Animator))]
    public class CostumeData : MonoBehaviour {
        public Animator animator;
        public Map<string, GameObject> bones;
        public Map<string, Collider> hurtColliders;
        public Map<string, Collider> hitColliders;

        private void Reset(){
            animator = GetComponent<Animator>();
        }

    }
}
