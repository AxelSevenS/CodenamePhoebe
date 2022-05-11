using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {

    public class SnowLayer : MonoBehaviour {

        [SerializeField] private RenderTexture defaultThickness;
        public RenderTexture snowThickness;

        private void Reset(){
            snowThickness = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
        }

        private void OnColliderStay(Collision other){
            // if (other.gameObject.layer == 6)
            //     snowThickness.
        }
    }
}
