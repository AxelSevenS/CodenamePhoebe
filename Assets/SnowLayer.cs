using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {

    public class SnowLayer : MonoBehaviour {

        [SerializeField] private RenderTexture thickness;
        private MaterialPropertyBlock mpb;
        public RenderTexture snowThickness;

        private Renderer renderer;

        private void Awake() {
            renderer = GetComponent<Renderer>();
            mpb = new MaterialPropertyBlock();
        }

        private void Reset(){
            snowThickness = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
        }

        private void Update(){
            mpb.SetTexture("_SnowThickness", thickness);
            renderer.SetPropertyBlock(mpb);
        }

        private void OnColliderStay(Collision other){
            // if (other.gameObject.layer == 6)
            //     snowThickness.
        }
    }
}
