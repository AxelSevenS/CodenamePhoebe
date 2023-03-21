using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

using SevenGame.Utility;
using EasySplines;
using System;

namespace SeleneGame.Content {

    public class EidolonMask : Costumable<EidolonMaskData, EidolonMaskCostume, EidolonMaskModel> {


        [SerializeField] [ReadOnly] private MaskedEntity _maskedEntity;

        private const int maxGrabbables = 4;

        public Stack<GrabbableObject> _grabbables = null;
        private static readonly Vector3[] grabbedObjectPositions = new Vector3[maxGrabbables]{
            new Vector3(3.5f, 1.5f, 3f), 
            new Vector3(-3.5f, 1.5f, 3f), 
            new Vector3(2.5f, 2.5f, 3f), 
            new Vector3(-2.5f, 2.5f, 3f)
        };

        private Quaternion _grabbedObjectRotation = Quaternion.identity;



        public Stack<GrabbableObject> grabbables {
            get {
                _grabbables ??= new Stack<GrabbableObject>(maxGrabbables);
                return _grabbables;
            }
        }
        public MaskedEntity maskedEntity => _maskedEntity;



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


        protected internal virtual void HandleInput(PlayerEntityController controller) {
            if ( controller.entity != _maskedEntity ) return;

            // if (controller.focusInput.Tapped()) {

            //     if (_maskedEntity.state is MaskedState) {
            //         _maskedEntity.gravityDown = Vector3.down;
            //         _maskedEntity.ResetState();
            //     } else {
            //         _maskedEntity.shiftCooldown = 0.3f;
            //         if (_maskedEntity.onGround) _maskedEntity.rigidbody.velocity += -_maskedEntity.gravityDown*3f;
                    
            //         _maskedEntity.SetState<MaskedState>();
            //     }
            // }

            _grabbedObjectRotation = controller.worldCameraRotation;

            if (controller.focusInput.Tapped()) {
                Throw();
            }

            if ( grabbables.Count < maxGrabbables && controller.focusInput.trueTimer > 0.25f ) {

                Collider[] colliders = Physics.OverlapSphere(_maskedEntity.transform.position, 10f, LayerMask.GetMask("EntityObject"));
                foreach (Collider collider in colliders) {
                    if ( grabbables.Count >= maxGrabbables ) break;

                    if ( collider.TryGetComponent(out IGrabbable grabbable) ) {
                        Grab( new GrabbableObject(grabbable) );
                    }
                }

            }
        }
        
        
        public void Grab(GrabbableObject grabbableObject){
            if (grabbables.Count >= maxGrabbables) return;
            
            grabbableObject.grabbable.Grab();
            grabbables.Push(grabbableObject);
            
        }

        public void Throw(){

            if ( !grabbables.TryPeek(out GrabbableObject grabbableObject) ) return;

            Vector3 direction = _grabbedObjectRotation * Vector3.forward;
            grabbableObject.grabbable.Throw( direction * 75f );
            grabbables.Pop();
        }



        public override void Update() {

            int index = 0;
            foreach (GrabbableObject grabbableObject in grabbables) {

                Quaternion newRotation = grabbableObject.grabbable.grabTransform.rotation * grabbableObject.randomSpin;
                grabbableObject.grabbable.grabTransform.rotation = Quaternion.Slerp(grabbableObject.grabbable.grabTransform.rotation, newRotation, 3f * GameUtility.timeDelta) ;

                Vector3 targetPosition = _maskedEntity.transform.position + _grabbedObjectRotation * grabbedObjectPositions[index++];
                grabbableObject.grabbable.grabTransform.position = Vector3.Lerp(grabbableObject.grabbable.grabTransform.position, targetPosition, 10f* GameUtility.timeDelta);

            }

            data?.MaskUpdate(this);
            base.Update();
        }

        public override void LateUpdate() {
                
            data?.MaskUpdate(this);
            model?.LateUpdate();
        }

        public override void FixedUpdate() {

            data?.MaskFixedUpdate(this);
            model?.FixedUpdate();
        }


        [Serializable]
        public struct GrabbableObject {
            public IGrabbable grabbable;
            public Quaternion randomSpin;

            public GrabbableObject(IGrabbable grabbable) {
                this.grabbable = grabbable;
                this.randomSpin = UnityEngine.Random.rotationUniform;
            }
        }

    }

}