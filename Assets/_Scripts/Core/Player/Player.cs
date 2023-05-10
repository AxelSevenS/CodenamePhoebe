using UnityEngine;

using SevenGame.Utility;
using SeleneGame.Core.UI;

namespace SeleneGame.Core {
    
    [DefaultExecutionOrder(-100)]
    public class Player : EntityController {

        private static Player _current;
        public static Player current {
            get => _current;
            private set {
                if (Application.isEditor) {
                    foreach (Player playerController in FindObjectsOfType<Player>() ) {
                        if (playerController != value) {
                            GameObject go = playerController.gameObject;
                            DestroyImmediate(playerController);
                            go.AddComponent<EntityController>();
                        }
                    }
                }
                
                if (_current != null && _current != value) {
                    GameObject go = _current.gameObject;
                    GameUtility.SafeDestroy(_current);
                    go.AddComponent<EntityController>();
                }

                _current = value;
            }
        }


        public static Vector3 defaultCameraPosition = new Vector3(1f, 0f, -3.5f);



        [Header("Input")]

        public KeyInputData lightAttackInput;
        public KeyInputData heavyAttackInput;
        public KeyInputData interactInput;
        public KeyInputData jumpInput;
        public KeyInputData evadeInput;
        public KeyInputData walkInput;
        public KeyInputData crouchInput;
        public KeyInputData focusInput;
        // public KeyInputData shiftInput;
        public Vector2Data moveInput;
        public Vector2Data lookInput;

        public KeyInputData switchStyle1Input;
        public KeyInputData switchStyle2Input;
        public KeyInputData switchStyle3Input;

        private KeyInputData cancelInput;
        
        #if UNITY_EDITOR
            public KeyInputData debugInput;
        #endif
        

        private Vector2 mousePos;



        Collider[] _colliderBuffer = new Collider[15];
    
        public IInteractable interactionCandidate;

        public Quaternion softEntityRotation;
        public QuaternionData localCameraRotation;



        public bool canInteract => interactionCandidate != null;
        public bool canLook => true;
        public Quaternion worldCameraRotation => softEntityRotation * localCameraRotation;




        public void RawInputToGroundedMovement(out Quaternion camRotation, out Vector3 groundedMovement){
            Vector3 camRight = worldCameraRotation * Vector3.right;
            Vector3 camUp = entity.transform.rotation * Vector3.up;
            Vector3 camForward = Vector3.Cross(camRight, camUp).normalized;
            camRotation = Quaternion.LookRotation(camForward, camUp);
            groundedMovement = camRotation * new Vector3(moveInput.x, 0, moveInput.y).normalized;
        }
        public void RawInputToCameraRelativeMovement(out Quaternion camRotation, out Vector3 cameraRelativeMovement){
            camRotation = worldCameraRotation;
            cameraRelativeMovement = camRotation * new Vector3(moveInput.x, 0, moveInput.y);
        }

        private IInteractable GetInteractionCandidate(Collider[] buffer){

            // if the entity is sitting, it can only interact with the seat
            if ( entity.behaviour is SittingBehaviour sittingBehaviour && sittingBehaviour.seat.IsInteractable ) {
                return sittingBehaviour.seat;
            }


            const float interactionDistance = 7.5f;
            const float sqrInteractionDistance = interactionDistance * interactionDistance;
            const float interactionAngle = 0.5f;

            Physics.OverlapSphereNonAlloc(entity.transform.position, interactionDistance, buffer, CollisionUtils.EntityObjectMask);

            IInteractable candidate = null;
            float closestDistance = float.PositiveInfinity;
            float closestAngle = float.NegativeInfinity;

            foreach ( Collider hit in buffer ) {

                if (hit == null) continue;

                Transform collisionTransform = hit.attachedRigidbody?.transform ?? hit.transform;

                if ( collisionTransform == entity.transform ) continue;

                float sqrDistance = (collisionTransform.position - entity.transform.position).sqrMagnitude;
                float angle = Vector3.Dot( (collisionTransform.position - entity.transform.position).normalized, entity.absoluteForward );

                if ( sqrDistance > sqrInteractionDistance || angle < interactionAngle ) continue;
                if ( sqrDistance > closestDistance || angle < closestAngle ) continue;
                

                if ( collisionTransform.TryGetComponent<IInteractable>(out IInteractable interactionComponent) && interactionComponent.IsInteractable ) {
                    candidate = interactionComponent;
                    closestDistance = sqrDistance;
                    closestAngle = angle;
                }
                
            }

            return candidate;
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

            // if ( Time.timeScale == 0f ) return;

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

        private void PlayerInput() {
            interactInput.SetVal(Keybinds.playerMap.IsBindPressed("Interact"));
            cancelInput.SetVal(Keybinds.uiMap.IsBindPressed("Cancel"));
            switchStyle1Input.SetVal(Keybinds.playerMap.IsBindPressed("PrimaryWeapon"));
            switchStyle2Input.SetVal(Keybinds.playerMap.IsBindPressed("SecondaryWeapon"));
            switchStyle3Input.SetVal(Keybinds.playerMap.IsBindPressed("TertiaryWeapon"));

            #if UNITY_EDITOR
                debugInput.SetVal( Keybinds.debugMap.IsBindPressed("Debug1") );
            #endif

            if ( cancelInput.started ) {
                UIController.current.Cancel();
            }

            if (interactInput.started && canInteract)
                interactionCandidate.Interact(entity);

            entity.HandleInput(this);

            Entity targetEntity = entity;
            if (targetEntity.behaviour is SittingBehaviour sittingState && sittingState.seat.seatEntity != null) {
                targetEntity = sittingState.seat.seatEntity;
            }

            if (switchStyle1Input.started)
                targetEntity.SetStyle(0);
            if (switchStyle2Input.started)
                targetEntity.SetStyle(1);
            if (switchStyle3Input.started)
                targetEntity.SetStyle(2);

        }


        public void LookAt( Vector3 direction) {
            localCameraRotation.SetVal( Quaternion.LookRotation( Quaternion.Inverse(entity.transform.rotation) * direction, entity.transform.rotation * Vector3.up ) );
        }

        private void OnDrawGizmos() {
            // Gizmos.DrawIcon(transform.position, "bruh.jpg", true);
        }

        private void OnEnable() {
            Reset();
        }

        private void Update() {

            if (entity == null || !entity.enabled) return;

            EntityControl();
            PlayerInput();
        }
        protected override void Reset() {
            base.Reset();

            current = this;
        }


    }
}
