using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using SeleneGame.Entities;
using SeleneGame.States;

namespace SeleneGame.Core {
    
    public class Player : MonoBehaviour{
        
        public static Player current;

        public Entity entity;
        public new Camera camera;
        public InputActionAsset controls;
        public InputActionMap playerControls;
        public InputActionMap debugControls;
    
        public IInteractable interactionCandidate;
        public IManipulable manipulationCandidate;

        private string[] inputKeys = new string[]{ "LightAttack", "HeavyAttack", "Jump", "Interact", "Evade", "Walk", "Crouch", "Focus", "Shift" };

        private const float cameraSpeed = 0.1f;
        private Vector2 mousePos;

        public Vector3 defaultCameraPosition = new Vector3(1f, 0f, -3.5f);

        public bool talking;
        public bool menu;
        public bool nearInteractable;
        private bool canPlay => (!menu && !entity.walkingTo && !entity.turningTo);
        public bool canLook => (!menu);
        public bool canInteract => interactionCandidate != null;

        private RaycastHit landingHit;
        
        
        public float holdDuration = 0.2f;
        public float mouseSpeed = 1f;
        public float stickSpeed = 5f;
    
        private void Awake(){
            GameEvents.Reset();
            camera = Camera.main;
        }

        private void OnEnable(){
            if (current != null)
                Destroy(current);
            current = this;

            playerControls = controls.FindActionMap("Player");
            debugControls = controls.FindActionMap("Debug");

            playerControls["Interact"].performed += ctx => OnInteract();
            playerControls["PrimaryWeapon"].performed += ctx => OnSwitchWeapon(0);
            playerControls["SecondaryWeapon"].performed += ctx => OnSwitchWeapon(1);
            playerControls["TertiaryWeapon"].performed += ctx => OnSwitchWeapon(2);

            playerControls["Interact"].performed += ctx => OnPlayerValidate();

            debugControls["DebugKeyBindMenu"].performed += ctx => GameEvents.ToggleMenu();
            debugControls["SaveSlot1"].performed += ctx => SavingSystem.SavePlayerData(1);
            debugControls["SaveSlot2"].performed += ctx => SavingSystem.SavePlayerData(2);
            debugControls["SaveSlot3"].performed += ctx => SavingSystem.SavePlayerData(3);
            debugControls["LoadSlot1"].performed += ctx => SavingSystem.LoadPlayerData(1);
            debugControls["LoadSlot2"].performed += ctx => SavingSystem.LoadPlayerData(2);
            debugControls["LoadSlot3"].performed += ctx => SavingSystem.LoadPlayerData(3);
            debugControls["Debug1"].performed += ctx => entity.Damage(50);
            // debugControls["Debug2"].performed += ctx => null;

            playerControls.actionTriggered += ctx => ControllerAction(ctx);

            controls.Enable();
        }
        private void OnDisable(){
            playerControls["Interact"].performed -= ctx => OnInteract();
            playerControls["PrimaryWeapon"].performed -= ctx => OnSwitchWeapon(0);
            playerControls["SecondaryWeapon"].performed -= ctx => OnSwitchWeapon(1);
            playerControls["TertiaryWeapon"].performed -= ctx => OnSwitchWeapon(2);

            playerControls["Interact"].performed -= ctx => OnPlayerValidate();

            debugControls["DebugKeyBindMenu"].performed -= ctx => GameEvents.ToggleMenu();
            debugControls["SaveSlot1"].performed -= ctx => SavingSystem.SavePlayerData(1);
            debugControls["SaveSlot2"].performed -= ctx => SavingSystem.SavePlayerData(2);
            debugControls["SaveSlot3"].performed -= ctx => SavingSystem.SavePlayerData(3);
            debugControls["LoadSlot1"].performed -= ctx => SavingSystem.LoadPlayerData(1);
            debugControls["LoadSlot2"].performed -= ctx => SavingSystem.LoadPlayerData(2);
            debugControls["LoadSlot3"].performed -= ctx => SavingSystem.LoadPlayerData(3); 
            debugControls["Debug1"].performed -= ctx => entity.Damage(50);
            // debugControls["Debug2"].performed -= ctx => null;

            playerControls.actionTriggered -= ctx => ControllerAction(ctx);

            controls.Disable();
        }
        
        private void Update(){

            Physics.Raycast(entity.bottom, entity.gravityDown, out landingHit, Global.GroundMask);

            EntityControl();

        }

        private void EntityControl(){

            Collider[] colliderBuffer = new Collider[5];

            // Player Input
            if (canPlay){

                SafeDictionary<string, bool> inputDict = new SafeDictionary<string, bool>();
                foreach(string key in inputKeys){
                    inputDict[key] = playerControls[key].IsActuated();
                }

                float jump = System.Convert.ToSingle(inputDict["Jump"]) - System.Convert.ToSingle(inputDict["Crouch"]);
                Quaternion lookRotation = UpdateCameraRotation( entity.lookRotation );

                Vector2 dir = playerControls["Move"].ReadValue<Vector2>();
                Vector3 moveDirection = new Vector3(dir.x, jump, dir.y);
                

                entity.EntityInput(moveDirection, lookRotation, inputDict);
            }

            manipulationCandidate = GetManipulationCandidate(colliderBuffer);

            interactionCandidate = GetInteractionCandidate(colliderBuffer);

        }

        private bool IsObjectForwardToEntity(Vector3 entityPosition, Vector3 objectPosition, Vector3 entityForward, float threshold){
            Vector3 entityToObject = (objectPosition - entityPosition);
            return (Vector3.Dot(entityToObject.normalized, entityForward) > threshold/90f);
        }

        private Quaternion UpdateCameraRotation(Quaternion currentRotation){
            if (!canLook) return currentRotation;
            Vector2 mouseInput = playerControls["Look"].ReadValue<Vector2>() * cameraSpeed;

            float additionalCameraSpeed = controllerType == Player.ControllerType.Controller ? stickSpeed : mouseSpeed;
            mouseInput *= additionalCameraSpeed;

            mousePos = new Vector2( Mathf.Clamp(mousePos.x-mouseInput.y, -90, 90), mousePos.y+mouseInput.x );

            return Quaternion.AngleAxis(mousePos.y, Vector3.up) * Quaternion.AngleAxis(mousePos.x, Vector3.right);
        }

        private IManipulable GetManipulationCandidate(Collider[] buffer){
            // if ( !(entity.state is FocusState) ) return null;

            Physics.OverlapSphereNonAlloc(entity._transform.position, 15f, buffer, Global.ObjectEntityMask);
            foreach ( Collider hit in buffer ){

                Rigidbody rb = hit?.attachedRigidbody ?? null;
                if (rb == null || rb.transform == entity._transform) continue;

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

            if (entity.state is SittingState sitting)
                return sitting.seat;

            Physics.OverlapSphereNonAlloc(entity._transform.position, 5f, buffer, Global.ObjectEntityMask);
            foreach ( Collider hit in buffer ) {

                Transform collisionTransform = hit?.transform;
                if ( 
                    collisionTransform != null &&
                    collisionTransform != entity._transform && 
                    IsObjectForwardToEntity(entity._transform.position, collisionTransform.position, entity.absoluteForward, 60f) && 
                    collisionTransform.TryGetComponent<IInteractable>(out var interactionComponent) 
                )
                    return interactionComponent;

                Transform collisionRigidbodyTransform = hit?.attachedRigidbody?.transform ?? null;
                if (
                    collisionRigidbodyTransform != null &&
                    collisionRigidbodyTransform != entity._transform && 
                    IsObjectForwardToEntity(entity._transform.position, collisionRigidbodyTransform.position, entity.absoluteForward, 60f) && 
                    collisionRigidbodyTransform.TryGetComponent<IInteractable>(out interactionComponent) 
                )
                    return interactionComponent;
            }

            return null;
        }


        private void OnInteract(){
            if (canInteract){
                interactionCandidate.Interact(entity);
                Debug.Log("Interact");
            }
        }

        private void OnSwitchWeapon(int value){
            if (canPlay && entity is GravityShifterEntity shifter)
                shifter.weapons.Switch(value);
        }
        private void OnPlayerValidate(){
            GameEvents.PlayerInput();
        }

        public enum ControllerType{
            MouseKeyboard,
            Controller
        };

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