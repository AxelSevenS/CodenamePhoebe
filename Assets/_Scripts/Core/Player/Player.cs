using System.Reflection;
using System.Linq;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class Player : Singleton<Player>{

        public Entity entity;
        public new Camera camera;

    
        public IInteractable interactionCandidate;
        public IManipulable manipulationCandidate;

        SafeDictionary<string, bool> inputDictionary = new SafeDictionary<string, bool>();
        Collider[] _colliderBuffer = new Collider[5];

        private const float cameraSpeed = 0.1f;
        private Vector2 mousePos;

        public Vector3 defaultCameraPosition = new Vector3(1f, 0f, -3.5f);

        public bool talking;
        public bool menu;
        public bool nearInteractable;
        private bool canPlay => (!menu && !entity.walkingTo && !entity.turningTo);
        public bool canLook => (!menu);
        public bool canInteract => interactionCandidate != null;

        
        public float mouseSpeed = 1f;
        public float stickSpeed = 5f;


        private BoolData interactInput;
        private BoolData switchStyle1Input;
        private BoolData switchStyle2Input;
        private BoolData switchStyle3Input;
        
        #if UNITY_EDITOR
            private BoolData menuInput;
            private BoolData debugInput;
        #endif
    
        private void Awake(){
            camera = Camera.main;

            interactInput = new BoolData();
            switchStyle1Input = new BoolData(); 
            switchStyle2Input = new BoolData();
            switchStyle3Input = new BoolData();
            
            #if UNITY_EDITOR
                menuInput = new BoolData();
                debugInput = new BoolData(); 
            #endif

            inputDictionary = new SafeDictionary<string, bool>();
        }

        protected void OnEnable(){
            SetCurrent();
            ControlsManager.current.playerMap.actionTriggered += ctx => ControllerAction(ctx);
        }
        private void OnDisable(){

            ControlsManager.current.playerMap.actionTriggered -= ctx => ControllerAction(ctx);
        }
        
        private void Update() {
            PlayerInput();

            if (entity == null || !entity.enabled || !entity.gameObject.activeSelf) return;

            EntityControl();
        }

        private async void PlayerInput() {
            interactInput.SetVal(ControlsManager.current.playerMap.IsBindPressed("Interact"));
            switchStyle1Input.SetVal(ControlsManager.current.playerMap.IsBindPressed("PrimaryWeapon"));
            switchStyle2Input.SetVal(ControlsManager.current.playerMap.IsBindPressed("SecondaryWeapon"));
            switchStyle3Input.SetVal(ControlsManager.current.playerMap.IsBindPressed("TertiaryWeapon"));

            #if UNITY_EDITOR
                menuInput.SetVal( ControlsManager.current.debugMap.IsBindPressed("DebugKeyBindMenu") );
                debugInput.SetVal( ControlsManager.current.debugMap.IsBindPressed("Debug1") );
            #endif



            if (interactInput.started && canInteract)
                interactionCandidate.Interact(entity);

            if (switchStyle1Input.started)
                entity.SetStyle(1);
            if (switchStyle2Input.started)
                entity.SetStyle(2);
            if (switchStyle3Input.started)
                entity.SetStyle(3);

            #if UNITY_EDITOR
                if (menuInput.started)
                    menu = KeyBindingController.current.ToggleMenu(menu);
                if (debugInput.started)
                    entity.Damage(50);
            #endif
        }

        private void EntityControl(){

            // Player Input
            if ( !canPlay ) return;

            foreach(var pair in ControlsManager.current.playerBindings){
                try {
                    inputDictionary[pair.Key] = ControlsManager.current.playerBindings[pair.Key].IsActuated();
                } catch {
                    // Debug.LogError(e.Message);
                }
            }

            float jump = (ControlsManager.current.playerBindings["Jump"].IsActuated() ? 1f : 0f) - (ControlsManager.current.playerBindings["Crouch"].IsActuated() ? 1f : 0f);
            Vector2 dir = ControlsManager.current.playerBindings["Move"].ReadValue<Vector2>();

            Vector3 moveDirection = new Vector3(dir.x, jump, dir.y);
            Quaternion cameraRotation = UpdateCameraRotation( entity.cameraRotation );
            

            entity.EntityInput(moveDirection, cameraRotation, inputDictionary);

            // manipulationCandidate = GetManipulationCandidate(_colliderBuffer);

            interactionCandidate = GetInteractionCandidate(_colliderBuffer);

        }


        private Quaternion UpdateCameraRotation(Quaternion currentRotation){
            if (!canLook) return currentRotation;
            Vector2 mouseInput = ControlsManager.current.playerBindings["Look"].ReadValue<Vector2>() * cameraSpeed;

            float additionalCameraSpeed = controllerType == Player.ControllerType.Controller ? stickSpeed : mouseSpeed;
            mouseInput *= additionalCameraSpeed;

            mousePos = new Vector2( Mathf.Clamp(mousePos.x-mouseInput.y, -90, 90), mousePos.y+mouseInput.x );

            return Quaternion.AngleAxis(mousePos.y, Vector3.up) * Quaternion.AngleAxis(mousePos.x, Vector3.right);
        }

        private IManipulable GetManipulationCandidate(Collider[] buffer){
            // if ( !(entity.state is FocusState) ) return null;

            Physics.OverlapSphereNonAlloc(entity.transform.position, 15f, buffer, Global.EntityObjectMask);
            foreach ( Collider hit in buffer ){

                Rigidbody rb = hit?.attachedRigidbody ?? null;
                if (rb == null || rb.transform == entity.transform) continue;

                if ( rb.TryGetComponent<IManipulable>(out var manipulationComponent) ){
                    Ray cameraRay = new Ray(camera.transform.position, camera.transform.forward);

                    if ( hit.Raycast(cameraRay, out RaycastHit ManipulationHit, 15f) )
                        return manipulationComponent;
                }
            }

            return null;
        }

        private IInteractable GetInteractionCandidate(Collider[] buffer){

            if ( menu || talking || entity.walkingTo ) return null;

            const float interactionDistance = 5f;
            const float interactionAngle = 0.75f;

            Physics.OverlapSphereNonAlloc(entity.transform.position, 5f, buffer, Global.EntityObjectMask);
            // var candidate = buffer
            //     .Where(hit => hit != null && Vector3.Distance(entity.transform.position, hit.transform.position) < 5f)
            //     .OrderBy(hit => Vector3.Distance(hit.transform.position, entity.transform.position))
            //     .OrderBy(hit => - Vector3.Dot( (hit.transform.position - entity.transform.position).normalized, entity.transform.forward ))
            //     .Select(hit => hit.GetComponent<IInteractable>() ?? hit.attachedRigidbody?.GetComponent<IInteractable>())
            //     .Where(candidate => candidate != entity)
            //     .FirstOrDefault();

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




        public enum ControllerType{ MouseKeyboard, Controller };

        public ControllerType controllerType = ControllerType.MouseKeyboard;

        private void ControllerAction(InputAction.CallbackContext context){
            string path = context.control.path;
            // Debug.Log(path);
            if ( path.Contains("Mouse") || path.Contains("Keyboard") ){
                if (controllerType == ControllerType.MouseKeyboard) return;

                controllerType = ControllerType.MouseKeyboard;
                Debug.Log("Switched to Keyboard and Mouse Controls.");

            }else{
                if (controllerType == ControllerType.Controller) return;
                
                controllerType = ControllerType.Controller;
                Debug.Log("Switched to Controller Controls.");
            }
        }

    }
}