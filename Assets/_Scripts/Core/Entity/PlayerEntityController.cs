using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    public class PlayerEntityController : EntityController {

        public static PlayerEntityController current;


        public bool talking;
        public bool nearInteractable;
        public bool canInteract => interactionCandidate != null;

        public bool menu;
        public bool canLook => (!menu);


        public static Vector3 defaultCameraPosition = new Vector3(1f, 0f, -3.5f);


        Collider[] _colliderBuffer = new Collider[5];
    
        public IInteractable interactionCandidate;
        public IManipulable manipulationCandidate;

        private BoolData interactInput;
        private BoolData switchStyle1Input;
        private BoolData switchStyle2Input;
        private BoolData switchStyle3Input;
        
        #if UNITY_EDITOR
            private BoolData debugInput;
        #endif

        private Vector2 mousePos;

        public Quaternion softEntityRotation;
        public QuaternionData localCameraRotation;
        public Quaternion worldCameraRotation => softEntityRotation * localCameraRotation;

        protected override void SetController() {
            base.SetController();
            if (Application.isEditor) {
                foreach (PlayerEntityController playerController in FindObjectsOfType<PlayerEntityController>() ) {
                    if (playerController != this)
                        playerController.gameObject.AddComponent<EntityController>();
                }
            } else if (current != null && current != this) {
                current.gameObject.AddComponent<EntityController>();
            }

            current = this;
        }

        // private IManipulable GetManipulationCandidate(Collider[] buffer){
        //     // if ( !(entity.state is FocusState) ) return null;

        //     Physics.OverlapSphereNonAlloc(entity.transform.position, 15f, buffer, Global.EntityObjectMask);
        //     foreach ( Collider hit in buffer ){

        //         Rigidbody rb = hit?.attachedRigidbody ?? null;
        //         if (rb == null || rb.transform == entity.transform) continue;

        //         if ( rb.TryGetComponent<IManipulable>(out var manipulationComponent) ){
        //             Ray cameraRay = new Ray(camera.transform.position, camera.transform.forward);

        //             if ( hit.Raycast(cameraRay, out RaycastHit ManipulationHit, 15f) )
        //                 return manipulationComponent;
        //         }
        //     }

        //     return null;
        // }

        private IInteractable GetInteractionCandidate(Collider[] buffer){

            if ( menu || talking /* || entity.walkingTo */ ) return null;

            const float interactionDistance = 5f;
            const float interactionAngle = 0.75f;

            Physics.OverlapSphereNonAlloc(entity.transform.position, 5f, buffer, Global.EntityObjectMask);

            IInteractable candidate = null;
            float closestDistance = float.MaxValue;
            float closestAngle = float.MinValue;

            foreach ( Collider hit in buffer ) {

                if (hit == null) continue;

                Transform collisionTransform = hit.attachedRigidbody?.transform ?? hit.transform;
                float distance = Vector3.Distance(collisionTransform.position, entity.transform.position);
                float angle = Vector3.Dot( (collisionTransform.position - entity.transform.position).normalized, entity.transform.forward );
                if (
                    distance < interactionDistance && 
                    angle > interactionAngle &&
                    distance <= closestDistance &&
                    angle >= closestAngle &&
                    collisionTransform != entity.transform && 
                    collisionTransform.TryGetComponent<IInteractable>(out var interactionComponent) &&
                    interactionComponent.IsInteractable()
                ) {
                    candidate = interactionComponent;
                    closestDistance = distance;
                    closestAngle = angle;
                }
                
            }

            return candidate;
        }



        private void PlayerInput() {
            interactInput.SetVal(ControlsManager.current.playerMap.IsBindPressed("Interact"));
            switchStyle1Input.SetVal(ControlsManager.current.playerMap.IsBindPressed("PrimaryWeapon"));
            switchStyle2Input.SetVal(ControlsManager.current.playerMap.IsBindPressed("SecondaryWeapon"));
            switchStyle3Input.SetVal(ControlsManager.current.playerMap.IsBindPressed("TertiaryWeapon"));

            #if UNITY_EDITOR
                debugInput.SetVal( ControlsManager.current.debugMap.IsBindPressed("Debug1") );
            #endif



            if (interactInput.started && canInteract)
                interactionCandidate.Interact(entity);

            if (switchStyle1Input.started)
                entity.SetStyle(0);
            if (switchStyle2Input.started)
                entity.SetStyle(1);
            if (switchStyle3Input.started)
                entity.SetStyle(2);

            #if UNITY_EDITOR
                if (debugInput.started)
                    entity.Damage(50);
            #endif
        }
        private Quaternion UpdateCameraRotation(Quaternion currentRotation){
            softEntityRotation = Quaternion.Slerp(softEntityRotation, entity.rotation, GameUtility.timeDelta * 6f);

            if (!canLook) return currentRotation;

            Vector2 mouseInput = lookInput * ControlsManager.cameraSpeed;

            float additionalCameraSpeed = ControlsManager.current.controllerType == ControlsManager.ControllerType.Controller ? ControlsManager.current.stickSpeed : ControlsManager.current.mouseSpeed;
            mouseInput *= additionalCameraSpeed;

            mousePos = new Vector2( Mathf.Clamp(mousePos.x-mouseInput.y, -90, 90), mousePos.y+mouseInput.x );

            return Quaternion.AngleAxis(mousePos.y, Vector3.up) * Quaternion.AngleAxis(mousePos.x, Vector3.right);
        }

        private void EntityControl(){

            lightAttackInput.SetVal( ControlsManager.current.playerBindings["LightAttack"].IsActuated() );
            heavyAttackInput.SetVal( ControlsManager.current.playerBindings["HeavyAttack"].IsActuated() );
            jumpInput.SetVal( ControlsManager.current.playerBindings["Jump"].IsActuated() );
            evadeInput.SetVal( ControlsManager.current.playerBindings["Evade"].IsActuated() );
            walkInput.SetVal( ControlsManager.current.playerBindings["Walk"].IsActuated() );
            crouchInput.SetVal( ControlsManager.current.playerBindings["Crouch"].IsActuated() );
            focusInput.SetVal( ControlsManager.current.playerBindings["Focus"].IsActuated() );
            shiftInput.SetVal( ControlsManager.current.playerBindings["Shift"].IsActuated() );
            moveInput.SetVal( ControlsManager.current.playerBindings["Move"].ReadValue<Vector2>() );
            lookInput.SetVal( ControlsManager.current.playerBindings["Look"].ReadValue<Vector2>() );

            localCameraRotation.SetVal( UpdateCameraRotation( localCameraRotation ) );

            interactionCandidate = GetInteractionCandidate(_colliderBuffer);

        }

        public void LookAt( Vector3 direction) {
            localCameraRotation.SetVal( Quaternion.LookRotation( Quaternion.Inverse(entity.rotation) * direction, entity.rotation * Vector3.up ) );
        }

        public override void RawInputToGroundedMovement(out Quaternion camRotation, out Vector3 groundedMovement){
            Vector3 camRight = worldCameraRotation * Vector3.right;
            Vector3 camUp = entity.rotation * Vector3.up;
            Vector3 camForward = Vector3.Cross(camRight, camUp).normalized;
            camRotation = Quaternion.LookRotation(camForward, camUp);
            groundedMovement = camRotation * new Vector3(moveInput.x, 0, moveInput.y);
        }
        public override void RawInputToCameraRelativeMovement(out Quaternion camRotation, out Vector3 cameraRelativeMovement){
            camRotation = worldCameraRotation;
            cameraRelativeMovement = camRotation * new Vector3(moveInput.x, 0, moveInput.y);
        }



        

        private void Update() {

            if (entity == null || !entity.enabled) return;

            EntityControl();
            PlayerInput();
        }


    }
}
