
using System;
using UnityEngine;
using UnityEngine.InputSystem;

using TMPro;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.Core.UI {
    
    [DefaultExecutionOrder(100)]
    public class HUDController : Singleton<HUDController> {

        private const float SCREENMARGIN = 25f;

        [Header("Interaction HUD")]
        public RectTransform interactCursor;
        public RectTransform interactGUI;
        [SerializeField] private TextMeshProUGUI interactDescription;
        [SerializeField] private TextMeshProUGUI interactBind;


        private static InputAction _interactAction;
        private static InputAction interactAction {
            get {
                if (_interactAction == null) {
                    _interactAction = Keybinds.playerMap.FindAction("Interact");
                }
                return _interactAction;
            }
        }
        // private static System.Guid interactBindID;



        public static Vector3 ClampHUDToScreen(Vector3 position, float width, float height, float margin){
            float horizontalMargin = margin + width/4f;
            float verticalMargin = margin + height/4f;

            float clampedX = Mathf.Clamp(position.x, horizontalMargin, Screen.width - horizontalMargin);
            float clampedY = Mathf.Clamp(position.y, verticalMargin, Screen.height - verticalMargin);
            return new Vector3( clampedX, clampedY, 0 );
        }
        public static Vector3 ClampHUDToScreen(RectTransform rect, float margin){
            float horizontalMargin = margin + rect.rect.width/4f;
            float verticalMargin = margin + rect.rect.height/4f;

            float clampedX = Mathf.Clamp(rect.position.x, horizontalMargin, Screen.width - horizontalMargin);
            float clampedY = Mathf.Clamp(rect.position.y, verticalMargin, Screen.height - verticalMargin);
            return new Vector3( clampedX, clampedY, 0 );
        }

        private void UpdateKeybind(Guid keybindId){
            if (keybindId == Keybinds.GetInputBinding(interactAction).id){
                UpdateInteractionBindDisplay();
            }
        }
        private void UpdateControllerType( Keybinds.ControllerType controllerType ){
            UpdateInteractionBindDisplay();
        }

        private void UpdateInteractionBindDisplay() {
            interactBind.text = Keybinds.GetInputBinding(interactAction).ToDisplayString();
        }




        private void Awake(){
            UpdateInteractionBindDisplay();
        }
        
        protected void OnEnable(){
            SetCurrent();
            
            Keybinds.onUpdateKeybind += UpdateKeybind;
            Keybinds.onControllerTypeChange += UpdateControllerType;
        }
        private void OnDisable(){
            Keybinds.onUpdateKeybind -= UpdateKeybind;
            Keybinds.onControllerTypeChange -= UpdateControllerType;
        }

        private void LateUpdate() {

            if ( Player.current == null ) return;
            
            IInteractable interactCandidate = Player.current.interactionCandidate;
            bool playerCanInteract = Player.current.canInteract && interactCandidate != null && interactCandidate.InteractDescription != String.Empty;

            interactCursor.gameObject.SetActive(playerCanInteract);

            // Move the Interaction Prompt out of the screen if the player is not able to interact with the object
            float interactGUIXPos = playerCanInteract ? interactGUI.rect.width/2f + SCREENMARGIN : -interactGUI.rect.width; 
            float XPosLerp = Mathf.Lerp(interactGUI.anchoredPosition.x, interactGUIXPos, 15f * GameUtility.timeDelta);
            interactGUI.anchoredPosition = new Vector2(XPosLerp, interactGUI.anchoredPosition.y); 

            if ( !playerCanInteract ) return;

            interactDescription.text = interactCandidate.InteractDescription;

            MonoBehaviour interactionCandidate = interactCandidate as MonoBehaviour;

            if (interactionCandidate == null) return;

            Vector3 screenPos = CameraController.current.camera.WorldToScreenPoint(interactionCandidate.transform.position); 
            interactCursor.position = ClampHUDToScreen(screenPos, interactCursor.rect.width, interactCursor.rect.height, 0);

        }

    }
}
