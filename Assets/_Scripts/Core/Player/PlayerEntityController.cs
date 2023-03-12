using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class PlayerEntityController : EntityController {

        public static PlayerEntityController current;


        public bool talking;
        public bool nearInteractable;


        public static Vector3 defaultCameraPosition = new Vector3(1f, 0f, -3.5f);


        Collider[] _colliderBuffer = new Collider[5];
    
        public IInteractable interactionCandidate;
        // public IManipulable manipulationCandidate;

        private Vector2 mousePos;

        public Quaternion softEntityRotation;
        public QuaternionData localCameraRotation;



        public bool canInteract => interactionCandidate != null;
        public bool canLook => true;
        public Quaternion worldCameraRotation => softEntityRotation * localCameraRotation;



        protected override void SetController() {
            base.SetController();

            if (Application.isEditor) {
                foreach (PlayerEntityController playerController in FindObjectsOfType<PlayerEntityController>() ) {
                    if (playerController != this) {
                        GameObject go = playerController.gameObject;
                        DestroyImmediate(playerController);
                        go.AddComponent<EntityController>();
                    }
                }
            }
            
            if (current != null && current != this) {
                GameObject go = current.gameObject;
                GameUtility.SafeDestroy(current);
                go.AddComponent<EntityController>();
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

            if ( talking ) return null;

            const float interactionDistance = 7.5f;
            const float interactionAngle = 0.5f;

            Physics.OverlapSphereNonAlloc(entity.transform.position, interactionDistance, buffer, Global.EntityObjectMask);

            IInteractable candidate = null;
            float closestDistance = float.MaxValue;
            float closestAngle = float.MinValue;

            foreach ( Collider hit in buffer ) {

                if (hit == null) continue;

                Transform collisionTransform = hit.attachedRigidbody?.transform ?? hit.transform;
                float distance = Vector3.Distance(collisionTransform.position, entity.transform.position);
                float angle = Vector3.Dot( (collisionTransform.position - entity.transform.position).normalized, entity.absoluteForward );
                if (
                    distance < interactionDistance && 
                    angle > interactionAngle &&
                    distance <= closestDistance &&
                    angle >= closestAngle &&
                    collisionTransform != entity.transform && 
                    collisionTransform.TryGetComponent<IInteractable>(out var interactionComponent) &&
                    interactionComponent.IsInteractable
                ) {
                    candidate = interactionComponent;
                    closestDistance = distance;
                    closestAngle = angle;
                }
                
            }

            return candidate;
        }



        private void PlayerInput() {
            interactInput.SetVal(Keybinds.playerMap.IsBindPressed("Interact"));
            switchStyle1Input.SetVal(Keybinds.playerMap.IsBindPressed("PrimaryWeapon"));
            switchStyle2Input.SetVal(Keybinds.playerMap.IsBindPressed("SecondaryWeapon"));
            switchStyle3Input.SetVal(Keybinds.playerMap.IsBindPressed("TertiaryWeapon"));

            #if UNITY_EDITOR
                debugInput.SetVal( Keybinds.debugMap.IsBindPressed("Debug1") );
            #endif

            if (interactInput.started && canInteract)
                interactionCandidate.Interact(entity);

            entity.HandleInput(this);

            Entity targetEntity = entity;
            if (targetEntity.state is Sitting sittingState && sittingState.seat.seatEntity != null) {
                targetEntity = sittingState.seat.seatEntity;
            }

            if (switchStyle1Input.started)
                targetEntity.SetStyle(0);
            if (switchStyle2Input.started)
                targetEntity.SetStyle(1);
            if (switchStyle3Input.started)
                targetEntity.SetStyle(2);

        }
        
        private Quaternion UpdateCameraRotation(Quaternion currentRotation){
            softEntityRotation = Quaternion.Slerp(softEntityRotation, entity.transform.rotation, GameUtility.timeDelta * 6f);

            if (!canLook) return currentRotation;

            Vector2 mouseInput = lookInput * Keybinds.cameraSpeed;

            float additionalCameraSpeed = Keybinds.controllerType == Keybinds.ControllerType.MouseKeyboard ? Keybinds.mouseSpeed : Keybinds.stickSpeed;
            mouseInput *= additionalCameraSpeed;

            mousePos = new Vector2( Mathf.Clamp(mousePos.x-mouseInput.y, -90, 90), mousePos.y+mouseInput.x );

            return Quaternion.AngleAxis(mousePos.y, Vector3.up) * Quaternion.AngleAxis(mousePos.x, Vector3.right);
        }

        private void EntityControl(){

            if ( Time.timeScale == 0f ) return;

            lightAttackInput.SetVal( Keybinds.playerMap.IsBindPressed("LightAttack") );
            heavyAttackInput.SetVal( Keybinds.playerMap.IsBindPressed("HeavyAttack") );
            jumpInput.SetVal( Keybinds.playerMap.IsBindPressed("Jump") );
            evadeInput.SetVal( Keybinds.playerMap.IsBindPressed("Evade") );
            walkInput.SetVal( Keybinds.playerMap.IsBindPressed("Walk") );
            crouchInput.SetVal( Keybinds.playerMap.IsBindPressed("Crouch") );
            focusInput.SetVal( Keybinds.playerMap.IsBindPressed("Focus") );
            // shiftInput.SetVal( Keybinds.playerMap.IsBindPressed("Shift") );
            moveInput.SetVal( Keybinds.playerMap.FindAction("Move").ReadValue<Vector2>() );
            lookInput.SetVal( Keybinds.playerMap.FindAction("Look").ReadValue<Vector2>() );

            localCameraRotation.SetVal( UpdateCameraRotation( localCameraRotation ) );

            interactionCandidate = GetInteractionCandidate(_colliderBuffer);

        }

        public void LookAt( Vector3 direction) {
            localCameraRotation.SetVal( Quaternion.LookRotation( Quaternion.Inverse(entity.transform.rotation) * direction, entity.transform.rotation * Vector3.up ) );
        }

        public override void RawInputToGroundedMovement(out Quaternion camRotation, out Vector3 groundedMovement){
            Vector3 camRight = worldCameraRotation * Vector3.right;
            Vector3 camUp = entity.transform.rotation * Vector3.up;
            Vector3 camForward = Vector3.Cross(camRight, camUp).normalized;
            camRotation = Quaternion.LookRotation(camForward, camUp);
            groundedMovement = camRotation * new Vector3(moveInput.x, 0, moveInput.y).normalized;
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
