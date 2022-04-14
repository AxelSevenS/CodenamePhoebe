using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
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
        public IManipulable ManipulationCandidate;

        private string[] inputKeys = new string[]{ "LightAttack", "HeavyAttack", "Jump", "Interact", "Evade", "Walk", "Crouch", "Focus", "Shift" };

        private const float cameraSpeed = 0.1f;
        private Vector2 mousePos;

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

            debugControls["DebugKeyBindMenu"].performed += ctx => GameEvents.ToggleMenu();
            debugControls["SaveSlot1"].performed += ctx => SavingSystem.SavePlayerData(1);
            debugControls["SaveSlot2"].performed += ctx => SavingSystem.SavePlayerData(2);
            debugControls["SaveSlot3"].performed += ctx => SavingSystem.SavePlayerData(3);
            debugControls["LoadSlot1"].performed += ctx => SavingSystem.LoadPlayerData(1);
            debugControls["LoadSlot2"].performed += ctx => SavingSystem.LoadPlayerData(2);
            debugControls["LoadSlot3"].performed += ctx => SavingSystem.LoadPlayerData(3);

            playerControls.actionTriggered += ctx => ControllerAction(ctx);

            controls.Enable();
        }
        private void OnDisable(){
            playerControls["Interact"].performed -= ctx => OnInteract();
            playerControls["PrimaryWeapon"].performed -= ctx => OnSwitchWeapon(0);
            playerControls["SecondaryWeapon"].performed -= ctx => OnSwitchWeapon(1);
            playerControls["TertiaryWeapon"].performed -= ctx => OnSwitchWeapon(2);

            debugControls["DebugKeyBindMenu"].performed -= ctx => GameEvents.ToggleMenu();
            debugControls["SaveSlot1"].performed -= ctx => SavingSystem.SavePlayerData(1);
            debugControls["SaveSlot2"].performed -= ctx => SavingSystem.SavePlayerData(2);
            debugControls["SaveSlot3"].performed -= ctx => SavingSystem.SavePlayerData(3);
            debugControls["LoadSlot1"].performed -= ctx => SavingSystem.LoadPlayerData(1);
            debugControls["LoadSlot2"].performed -= ctx => SavingSystem.LoadPlayerData(2);
            debugControls["LoadSlot3"].performed -= ctx => SavingSystem.LoadPlayerData(3); 

            playerControls.actionTriggered -= ctx => ControllerAction(ctx);

            controls.Disable();
        }
        
        private void Update(){
            Physics.Raycast(entity.bottom, entity.gravityDown, out landingHit, Global.GroundMask);

            // Player Input        
            if (canPlay){

                SafeDictionary<string, bool> inputDict = new SafeDictionary<string, bool>();
                foreach(string key in inputKeys){
                    inputDict[key] = playerControls[key].IsActuated();
                }
                // inputDict["LightAttack"] = playerControls["LightAttack"].IsActuated();
                // inputDict["HeavyAttack"] = playerControls["HeavyAttack"].IsActuated();
                // inputDict["Jump"] = playerControls["Jump"].IsActuated();
                // inputDict["Interact"] = playerControls["Interact"].IsActuated();
                // inputDict["Evade"] = playerControls["Evade"].IsActuated();
                // inputDict["Walk"] = playerControls["Walk"].IsActuated();
                // inputDict["Crouch"] = playerControls["Crouch"].IsActuated();
                // inputDict["Focus"] = playerControls["Focus"].IsActuated();
                // inputDict["Shift"] = playerControls["Shift"].IsActuated();

                float jump = System.Convert.ToSingle(inputDict["Jump"]) - System.Convert.ToSingle(inputDict["Crouch"]);
                Quaternion lookRotation = UpdateCameraRotation( entity.lookRotationData.currentValue );

                Vector2 dir = playerControls["Move"].ReadValue<Vector2>();
                Vector3 moveDirection = new Vector3(dir.x, jump, dir.y);
                

                entity.EntityInput(moveDirection, lookRotation, inputDict);
            }

            ObjectManipulation();

            ObjectInteraction();
        }

        private Quaternion UpdateCameraRotation(Quaternion currentRotation){
            if (!canLook) return currentRotation;
            Vector2 mouseInput = playerControls["Look"].ReadValue<Vector2>() * cameraSpeed;

            float additionalCameraSpeed = controllerType == Player.ControllerType.Controller ? stickSpeed : mouseSpeed;
            mouseInput *= additionalCameraSpeed;

            mousePos = new Vector2( Mathf.Clamp(mousePos.x-mouseInput.y, -90, 90), mousePos.y+mouseInput.x );

            return Quaternion.AngleAxis(mousePos.y, Vector3.up) * Quaternion.AngleAxis(mousePos.x, Vector3.right);
        }

        private void ObjectManipulation(){
            Ray cameraRay = camera.ScreenPointToRay(new Vector3(Screen.width/2f,Screen.height/2f,0f));
            Physics.Raycast(cameraRay, out RaycastHit cameraHit, 15f, Global.ObjectEntityMask);

            if ( entity.focusing ){
                
                foreach ( Collider hit in Physics.OverlapSphere(entity._transform.position, 15f, Global.ObjectEntityMask) ){
                    if (hit.TryGetComponent<IManipulable>(out var manipulationComponent)){
                        Vector3 vectorToObject = (hit.transform.position - Camera.main.transform.position);
                        Physics.Raycast(Camera.main.transform.position, vectorToObject, out RaycastHit ManipulationHit, 15f, Global.ObjectEntityMask);
                        if (cameraHit.collider == hit || (Vector3.Dot(vectorToObject.normalized, Camera.main.transform.forward.normalized) > 87.5f/90f && ManipulationHit.collider == hit)){

                            ManipulationCandidate = manipulationComponent;
                        }
                    }
                }
            }
        }

        private void ObjectInteraction(){
            if ( !menu && !talking && !entity.walkingTo ){

                if (entity.currentState is SittingState){
                    interactionCandidate = ((SittingState)entity.currentState).seat;

                }else{
                    interactionCandidate = null;

                    foreach ( Collider hit in Physics.OverlapSphere(entity._transform.position, 5f, Global.ObjectEntityMask) ){

                        Vector3 vectorToObject = (hit.transform.position - entity._transform.position);
                        bool inFrontOfPlayer = (Vector3.Dot(vectorToObject.normalized, entity.absoluteForward) > 60f/90f);

                        if ( hit.enabled && inFrontOfPlayer && hit.TryGetComponent<IInteractable>(out var interactionComponent) ){
                            interactionCandidate = interactionComponent;
                        }
                    }
                }
            }else{
                interactionCandidate = null;
            }
        }


        private void OnInteract(){
            OnPlayerValidate();

            if (canInteract){
                interactionCandidate.Interact(entity);
                Debug.Log("Interact");
            }
        }

        private void OnSwitchWeapon(int value){
            if (canPlay)
                entity.SwitchWeapon(value);
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