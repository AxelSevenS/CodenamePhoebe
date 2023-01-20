using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

using SevenGame.Utility;
using EasySplines;

namespace SeleneGame.Content {

    public abstract class EidolonMask : Costumable<EidolonMask, EidolonMaskCostume> {


        [Header("Mask Data")]
        
        private float maskPosT = 1f;
        private bool faceState = false;
        private bool onRight = true;
        private Vector3 hoveringPosition;
        private Vector3 relativePos;



        private Vector3 rightPosition => _entity.modelTransform.rotation * new Vector3(1.2f, 1.3f, -0.8f);
        private Vector3 leftPosition => _entity.modelTransform.rotation * new Vector3(-1.2f, 1.3f, -0.8f);

        public GameObject model => costume.modelInstance;



        protected internal void HandleInput(PlayerEntityController controller) {
            if ( !(_entity is MaskedEntity masked) ) return;

            if (controller.shiftInput.tapped) {

                if (masked.state is MaskedState) {
                    masked.gravityDown = Vector3.down;
                    masked.ResetState();
                } else {
                    masked.shiftCooldown = 0.3f;
                    if (masked.onGround) masked.rigidbody.velocity += -masked.gravityDown*3f;
                    
                    masked.SetState<MaskedState>();
                }
            }
        }

        

        public void SetState(bool onFace) {
            faceState = onFace;
        }


        protected internal virtual void MaskUpdate() {
            Transform headTransform = _entity["head"].transform;
            
            BezierQuadratic currentCurve = new BezierQuadratic(
                hoveringPosition,
                headTransform.position,
                headTransform.position + headTransform.forward
            );

            costume.modelInstance.transform.position = currentCurve.GetPoint(maskPosT).position;
            costume.modelInstance.transform.rotation = Quaternion.Slerp(_entity.modelTransform.rotation, headTransform.rotation, maskPosT);
        }

        protected internal virtual void MaskFixedUpdate(){
            bool onFace = faceState || (positionBlocked(leftPosition) && positionBlocked(rightPosition));

            relativePos = Vector3.Lerp(relativePos, onRight ? rightPosition : leftPosition, 3f * GameUtility.timeDelta);

            if (!onFace && (positionBlocked(relativePos)))
                onRight = !onRight;

            hoveringPosition = Vector3.Lerp(hoveringPosition, _entity.modelTransform.position + relativePos, 15f * GameUtility.timeDelta);
            maskPosT = Mathf.MoveTowards(maskPosT, System.Convert.ToSingle(onFace), 4f * GameUtility.timeDelta);

            costume.animator.SetBool("OnFace", onFace);
            costume.animator.SetFloat("OnRight", onRight ? 1f : 0f);

            bool positionBlocked(Vector3 position) {
                return Physics.SphereCast(_entity.modelTransform.position, 0.35f, position, out _, position.magnitude, Global.GroundMask);
            }
        }
    }

}