using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

using SevenGame.Utility;
using EasySplines;

namespace SeleneGame.Content {

    public class EidolonMask : Costumable<EidolonMaskData, EidolonMaskCostume, EidolonMaskModel> {


        [SerializeField] [ReadOnly] private MaskedEntity _maskedEntity;

        public MaskedEntity maskedEntity => _maskedEntity;

        public float maskPosT { get; protected set;} = 1f;
        public bool faceState { get; protected set;} = false;
        public bool onRight { get; protected set;} = true;
        public Vector3 hoveringPosition { get; protected set;}
        public Vector3 relativePos { get; protected set;}




        public bool onFace => faceState || (positionBlocked(leftPosition) && positionBlocked(rightPosition));
        public Vector3 rightPosition => maskedEntity.modelTransform.rotation * new Vector3(1.2f, 1.3f, -0.8f);
        public Vector3 leftPosition => maskedEntity.modelTransform.rotation * new Vector3(-1.2f, 1.3f, -0.8f);


        public EidolonMask(MaskedEntity maskedEntity, EidolonMaskData data, EidolonMaskCostume costume = null) : base(data){
            _maskedEntity = maskedEntity;
            displayed = true;
            SetCostume(costume);
        }


        
        public override void SetCostume(EidolonMaskCostume costume) {
            _model?.Dispose();
            
            costume ??= data.baseCostume ?? AddressablesUtils.GetDefaultAsset<EidolonMaskCostume>();
            _model = costume?.LoadModel(maskedEntity, this);

            if (displayed)
                _model?.Display();
            else
                _model?.Hide();
        }


        protected internal void HandleInput(PlayerEntityController controller) {
            if ( !(controller.entity is MaskedEntity masked) ) return;

            if (controller.shiftInput.Tapped()) {

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

        private bool positionBlocked(Vector3 position) {
            return Physics.SphereCast(maskedEntity.modelTransform.position, 0.35f, position, out _, position.magnitude, Global.GroundMask);
        }



        public virtual void Update() {

            model?.Update();
        }

        public virtual void FixedUpdate() {

            relativePos = Vector3.Lerp(relativePos, onRight ? rightPosition : leftPosition, 3f * GameUtility.timeDelta);

            if (!onFace && (positionBlocked(relativePos)))
                onRight = !onRight;

            hoveringPosition = Vector3.Lerp(hoveringPosition, maskedEntity.modelTransform.position + relativePos, 15f * GameUtility.timeDelta);
            maskPosT = Mathf.MoveTowards(maskPosT, System.Convert.ToSingle(onFace), 4f * GameUtility.timeDelta);

            model?.FixedUpdate();
        }
    }

}