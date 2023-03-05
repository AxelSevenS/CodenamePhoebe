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

        public override EidolonMaskModel LoadModel(MaskedEntity entity, EidolonMask mask) {
            return new SimpleEidolonMaskModel(entity, mask, this);
        }
    }

    public sealed class SimpleEidolonMaskModel : EidolonMaskModel {

        [SerializeField] [ReadOnly] private GameObject _model;
        [SerializeField] [ReadOnly] private Animator _animator;


        private Transform headTransform => mask.maskedEntity["head"]?.transform ?? null;
        public override Transform mainTransform => _model.transform;


        public SimpleEidolonMaskModel(MaskedEntity entity, EidolonMask mask, SimpleEidolonMaskCostume costume) : base(entity, mask, costume) {
            if (mask?.maskedEntity != null && costume?.model != null) {

                _model = GameObject.Instantiate(costume.model, mask.maskedEntity.transform.parent);

                _costumeData = _model.GetComponent<CostumeData>();

                _animator = _model.GetComponent<Animator>();
                _animator ??= _model.AddComponent<Animator>();
            }
        }

        public override void Unload() {
            _model = GameUtility.SafeDestroy(_model);
        }

        public override void Display() {
            _model?.SetActive(true);
        }

        public override void Hide() {
            _model?.SetActive(false);
        }

        public override void Update() {

            base.Update();


            if (_model == null || headTransform == null)
                return;


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

            if (_animator != null) {
                _animator?.SetBool("OnFace", mask.onFace);
                _animator?.SetFloat("OnRight", mask.onRight ? 1f : 0f);
            }
        }
    } 
}