using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {

    public class SnowLayer : MonoBehaviour { 

        [SerializeField] private RenderTexture thickness;
        public RenderTexture snowThickness;

        private new Renderer _renderer;
        private MaterialPropertyBlock mpb;

        private void Awake() {
            _renderer = GetComponent<Renderer>();
            _renderer.GetPropertyBlock(mpb);
        }

        private void Reset(){
            snowThickness = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGB32);
        }

        private void Update(){
            mpb.SetTexture("_SnowThickness", thickness);
            _renderer.SetPropertyBlock(mpb);
        }

    }
}
