using System.Collections;
using System.Collections.Generic;
using EasySplines;
using SeleneGame.Core;
using SevenGame.Utility;
using UnityEngine;


namespace SeleneGame.Content {

    [CreateAssetMenu(fileName = "new Eidolon Mask Costume", menuName = "Costume/Simple Eidolon Mask")]
    public sealed class SimpleEidolonMaskCostume : EidolonMaskCostume {

        [SerializeField] public GameObject model;

        public override CostumeModel<EidolonMaskCostume> LoadModel(EidolonMask mask) {
            return new SimpleEidolonMaskModel(mask, this);
        }
    }

    public sealed class SimpleEidolonMaskModel : EidolonMaskModel {

        [ReadOnly] private GameObject _model;
        [ReadOnly] private Animator _animator;
        protected Transform headTransform => mask.maskedEntity["head"]?.transform ?? null;

        public override Transform mainTransform => _model.transform;

        public SimpleEidolonMaskModel(EidolonMask mask, SimpleEidolonMaskCostume costume) : base(mask, costume) {
            if (mask != null && costume?.model != null) {

                _model = GameObject.Instantiate(costume.model);
                _costumeData = _model.GetComponent<CostumeData>();
                _animator = _model.GetComponent<Animator>();
            }
        }

        public override void Update() {

            base.Update();
            
            _model.transform.position = mask.maskedEntity.transform.position;

            BezierQuadratic currentCurve = new BezierQuadratic(
                mask.hoveringPosition,
                headTransform.position,
                headTransform.position + headTransform.forward
            );

            _model.transform.position = currentCurve.GetPoint(mask.maskPosT).position;
            _model.transform.rotation = Quaternion.Slerp(mask.maskedEntity.modelTransform.rotation, headTransform.rotation, mask.maskPosT);
        }

        public override void FixedUpdate() {

            base.FixedUpdate();

            _animator.SetBool("OnFace", mask.onFace);
            _animator.SetFloat("OnRight", mask.onRight ? 1f : 0f);
        }

        public override void Unload() {
            _model = GameUtility.SafeDestroy(_model);
        }
    } 
}