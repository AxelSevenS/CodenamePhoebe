using UnityEngine;

using SevenGame.Utility;
using SevenGame.UI;
using SeleneGame.Core.UI;

namespace SeleneGame.Core {
    
    [DefaultExecutionOrder(-100)]
    public class Player : EntityController {
        const float INTERACTION_DISTANCE = 7.5f;
        const float SQR_INTERACTION_DISTANCE = INTERACTION_DISTANCE * INTERACTION_DISTANCE;
        const float INTERACTION_ANGLE = 0.5f;

        private static Player _current;
        public static Player Current {
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


        public static Vector3 defaultCameraPosition = new(1f, 0f, -3.5f);



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



        readonly Collider[] _colliderBuffer = new Collider[15];
    
        public IInteractable interactionCandidate;

        public Quaternion softEntityRotation;
        public QuaternionData localCameraRotation;



        public bool CanInteract => interactionCandidate != null && !DialogueController.current.Enabled;
        public bool CanLook => true;
        public Quaternion WorldCameraRotation => softEntityRotation * localCameraRotation;




        public void RawInputToGroundedMovement(out Quaternion camRotation, out Vector3 groundedMovement){
            Vector3 camRight = WorldCameraRotation * Vector3.right;
            Vector3 camUp = Entity.transform.rotation * Vector3.up;
            Vector3 camForward = Vector3.Cross(camRight, camUp).normalized;
            camRotation = Quaternion.LookRotation(camForward, camUp);
            groundedMovement = camRotation * new Vector3(moveInput.x, 0, moveInput.y).normalized;
        }
        public void RawInputToCameraRelativeMovement(out Quaternion camRotation, out Vector3 cameraRelativeMovement){
            camRotation = WorldCameraRotation;
            cameraRelativeMovement = camRotation * new Vector3(moveInput.x, 0, moveInput.y);
        }

        private IInteractable GetInteractionCandidate(Collider[] buffer){

            // if the entity is sitting, it can only interact with the seat
            if ( Entity.Behaviour is SittingBehaviour sittingBehaviour && sittingBehaviour.Seat.IsInteractable ) {
                return sittingBehaviour.Seat;
            }

            Physics.OverlapSphereNonAlloc(Entity.transform.position, INTERACTION_DISTANCE, buffer, CollisionUtils.EntityObjectMask);

            IInteractable candidate = null;
            float closestDistance = float.PositiveInfinity;
            float closestAngle = float.NegativeInfinity;

            foreach ( Collider hit in buffer ) {

                if (hit == null) continue;

                Transform collisionTransform = hit.attachedRigidbody?.transform ?? hit.transform;

                if ( collisionTransform == Entity.transform ) continue;

                float sqrDistance = (collisionTransform.position - Entity.transform.position).sqrMagnitude;
                float angle = Vector3.Dot( (collisionTransform.position - Entity.transform.position).normalized, Entity.AbsoluteForward );

                if ( sqrDistance > SQR_INTERACTION_DISTANCE || angle < INTERACTION_ANGLE ) continue;
                if ( sqrDistance > closestDistance || angle < closestAngle ) continue;
                

                if ( collisionTransform.TryGetComponent(out IInteractable interactionComponent) && interactionComponent.IsInteractable ) {
                    candidate = interactionComponent;
                    closestDistance = sqrDistance;
                    closestAngle = angle;
                }
                
            }

            return candidate;
        }

        
        private Quaternion UpdateCameraRotation(Quaternion currentRotation){
            softEntityRotation = Quaternion.Slerp(softEntityRotation, Entity.transform.rotation, GameUtility.timeDelta * 6f);

            if (!CanLook) return currentRotation;

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
            switchStyle1Input.SetVal(Keybinds.playerMap.IsBindPressed("PrimaryWeapon"));
            switchStyle2Input.SetVal(Keybinds.playerMap.IsBindPressed("SecondaryWeapon"));
            switchStyle3Input.SetVal(Keybinds.playerMap.IsBindPressed("TertiaryWeapon"));

            localCameraRotation.SetVal( UpdateCameraRotation( localCameraRotation ) );

            interactionCandidate = GetInteractionCandidate(_colliderBuffer);

        }

        private void PlayerInput() {
            interactInput.SetVal(Keybinds.playerMap.IsBindPressed("Interact"));
            cancelInput.SetVal(Keybinds.uiMap.IsBindPressed("Cancel"));

            #if UNITY_EDITOR
                debugInput.SetVal( Keybinds.debugMap.IsBindPressed("Debug1") );
            #endif

            if ( cancelInput.started ) {
                UIManager.Cancel();
            }

            if (interactInput.started && CanInteract)
                interactionCandidate.Interact(Entity);

            Entity.HandleInput(this);

            // if (switchStyle1Input.started)
            //     targetEntity.SetStyle(0);
            // if (switchStyle2Input.started)
            //     targetEntity.SetStyle(1);
            // if (switchStyle3Input.started)
            //     targetEntity.SetStyle(2);

        }


        public void LookAt( Vector3 direction) {
            localCameraRotation.SetVal( Quaternion.LookRotation( Quaternion.Inverse(Entity.transform.rotation) * direction, Entity.transform.rotation * Vector3.up ) );
        }

        private void OnDrawGizmos() {
            // Gizmos.DrawIcon(transform.position, "bruh.jpg", true);
        }

        private void OnEnable() {
            Reset();
        }

        private void Update() {

            if (Entity == null || !Entity.enabled) return;

            EntityControl();
            PlayerInput();
        }
        protected override void Reset() {
            base.Reset();

            Current = this;
        }


    }
}
