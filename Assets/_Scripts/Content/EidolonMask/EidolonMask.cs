using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;

using SevenGame.Utility;
using EasySplines;
using System;

namespace SeleneGame.Content {

    public class EidolonMask : Costumable<EidolonMaskData, EidolonMaskCostume, EidolonMaskModel> {

        private const int MAX_GRABBED_OBJECTS = 4;
        private static readonly Vector3[] grabbedObjectPositions = new Vector3[MAX_GRABBED_OBJECTS]{
            new Vector3(3.5f, 1.5f, 3f), 
            new Vector3(-3.5f, 1.5f, 3f), 
            new Vector3(2.5f, 2.5f, 3f), 
            new Vector3(-2.5f, 2.5f, 3f)
        };


        [SerializeField] [ReadOnly] private MaskedEntity _maskedEntity;

        public SerializableStack<GrabbableObject> _grabbables = null;

        private Quaternion _grabbedObjectRotation = Quaternion.identity;

        public MaskedEntity maskedEntity => _maskedEntity;



        public EidolonMask(MaskedEntity maskedEntity, EidolonMaskData data, EidolonMaskCostume costume = null) : base(data){
            _maskedEntity = maskedEntity;
            displayed = true;
            SetCostume(costume);
            
            _grabbables = new SerializableStack<GrabbableObject>(MAX_GRABBED_OBJECTS);
        }


        
        public override void SetCostume(EidolonMaskCostume costume) {
            _model?.Dispose();
            
            costume ??= Data.baseCostume ?? AddressablesUtils.GetDefaultAsset<EidolonMaskCostume>();
            _model = costume?.LoadModel(maskedEntity, this);

            if (displayed)
                _model?.Display();
            else
                _model?.Hide();
        }


        protected internal virtual void HandleInput(Player controller) {
            if ( controller.Entity != _maskedEntity ) return;

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

            _grabbedObjectRotation = controller.WorldCameraRotation;

            if (controller.focusInput.Tapped()) {
                Throw();
            }

            if ( _grabbables.Count < MAX_GRABBED_OBJECTS && controller.focusInput.trueTimer > 0.25f ) {

                Collider[] colliders = Physics.OverlapSphere(_maskedEntity.transform.position, 10f, CollisionUtils.EntityObjectMask);
                foreach (Collider collider in colliders) {
                    if ( _grabbables.Count >= MAX_GRABBED_OBJECTS ) break;

                    if ( collider.TryGetComponent(out IGrabbable grabbable) ) {
                        Grab( new GrabbableObject(grabbable) );
                    }
                }

            }
        }
        
        
        public void Grab(GrabbableObject grabbableObject){
            if (_grabbables.Count >= MAX_GRABBED_OBJECTS) return;
            
            grabbableObject.grabbable.Grab();
            _grabbables.Push(grabbableObject);
            
        }

        public void Throw(){

            if ( !_grabbables.TryPeek(out GrabbableObject grabbableObject) ) return;

            Vector3 direction = _grabbedObjectRotation * Vector3.forward;
            grabbableObject.grabbable.Throw( direction * 75f );
            _grabbables.Pop();
        }



        public override void Update() {
            base.Update();

            Data?.MaskUpdate(this);

            Model?.Update();
        }

        public override void LateUpdate() {
            base.LateUpdate();

            int index = 0;
            foreach (GrabbableObject grabbableObject in _grabbables) {

                Quaternion newRotation = grabbableObject.grabbable.grabTransform.rotation * grabbableObject.randomSpin;
                grabbableObject.grabbable.grabTransform.rotation = Quaternion.Slerp(grabbableObject.grabbable.grabTransform.rotation, newRotation, 3f * GameUtility.timeDelta) ;

                Vector3 targetPosition = _maskedEntity.transform.position + _grabbedObjectRotation * grabbedObjectPositions[index++];
                grabbableObject.grabbable.grabTransform.position = Vector3.Lerp(grabbableObject.grabbable.grabTransform.position, targetPosition, 10f* GameUtility.timeDelta);

            }
                
            Data?.MaskUpdate(this);

            Model?.LateUpdate();
        }

        public override void FixedUpdate() {
            base.FixedUpdate();

            Data?.MaskFixedUpdate(this);

            Model?.FixedUpdate();
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