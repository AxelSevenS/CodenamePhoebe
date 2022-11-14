using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Entities;

using SevenGame.Utility;
using System;

namespace SeleneGame.Content {

    public abstract class EidolonMask : Costumable<EidolonMask, EidolonMaskCostume> {


        [Header("Mask Data")]
        [SerializeField] [ReadOnly] private MaskedEntity entity;
        

        private float maskPosT = 1f;
        private bool faceState = false;
        private bool onRight = true;
        private Vector3 hoveringPosition;
        private Vector3 relativePos;



        private Vector3 rightPosition => entity.transform.rotation * new Vector3(1.2f, 1.3f, -0.8f);
        private Vector3 leftPosition => entity.transform.rotation * new Vector3(-1.2f, 1.3f, -0.8f);

        public GameObject model => costume.modelInstance;



        public void Initialize(MaskedEntity entity, EidolonMaskCostume costume = null) {
            if (this.entity != null) {
                throw new InvalidOperationException("Mask already initialized");
            }

            this.entity = entity;
            SetCostume( EidolonMaskCostume.GetInstanceOf(costume ?? baseCostume) );

            relativePos = rightPosition;
            hoveringPosition = entity.transform.position + relativePos;
        }

        public override void SetCostume(EidolonMaskCostume costume) {
            if (costume == null) return;

            _costume?.UnloadModel();

            _costume = costume;
            _costume.Initialize(entity);
            _costume.LoadModel();
        }

        public void SetState(bool onFace) {
            faceState = onFace;
        }



        protected internal virtual void MaskUpdate() {


        }

        protected internal virtual void MaskFixedUpdate(){
            bool onFace = faceState || (positionBlocked(leftPosition) && positionBlocked(rightPosition));

            relativePos = Vector3.Lerp(relativePos, onRight ? rightPosition : leftPosition, 3f * GameUtility.timeDelta);

            if (!onFace && (positionBlocked(relativePos)))
                onRight = !onRight;

            hoveringPosition = Vector3.Lerp(hoveringPosition, entity.transform.position + relativePos, 15f * GameUtility.timeDelta);
            maskPosT = Mathf.MoveTowards(maskPosT, System.Convert.ToSingle(onFace), 4f * GameUtility.timeDelta);

            costume.animator.SetBool("OnFace", onFace);
            costume.animator.SetFloat("OnRight", onRight ? 1f : 0f);

            bool positionBlocked(Vector3 position) {
                return Physics.SphereCast(entity.transform.position, 0.35f, position, out _, position.magnitude, Global.GroundMask);
            }
            
            Transform headTransform = entity["head"].transform;
            
            BezierQuadratic currentCurve = new BezierQuadratic(
                hoveringPosition,
                headTransform.position,
                headTransform.position + headTransform.forward
            );

            costume.modelInstance.transform.position = currentCurve.GetPoint(maskPosT).position;
            costume.modelInstance.transform.rotation = Quaternion.Slerp(entity.transform.rotation, headTransform.rotation, maskPosT);
        }
    }

}