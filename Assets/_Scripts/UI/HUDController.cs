
using System;
using UnityEngine;
using UnityEngine.InputSystem;

using TMPro;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.UI {
    public class HUDController : Singleton<HUDController> {

        private const float SCREENMARGIN = 25f;

        [Header("Interaction HUD")]
        public RectTransform interactCursor;
        public RectTransform interactGUI;
        [SerializeField] private TextMeshProUGUI interactDescription;
        [SerializeField] private TextMeshProUGUI interactBind;

        private static InputBinding interactBinding;


        
        private void Awake(){
            interactBinding = ControlsManager.current.playerMap["Interact"].bindings[0];
            interactBind.text = interactBinding.ToDisplayString();
        }
        
        protected void OnEnable(){
            SetCurrent();
            
            GameEvents.onUpdateKeybind += UpdateKeybind;
        }
        private void OnDisable(){
            GameEvents.onUpdateKeybind -= UpdateKeybind;
        }

        private void Update() {
            UpdateInteractionDisplay();
        }

        private void UpdateInteractionDisplay() {

            if ( PlayerEntityController.current == null ) return;
            
            IInteractable interactCandidate = PlayerEntityController.current.interactionCandidate;
            bool playerCanInteract = PlayerEntityController.current.canInteract && interactCandidate != null && interactCandidate.InteractDescription() != String.Empty;

            interactCursor.gameObject.SetActive(playerCanInteract);

            // Move the Interaction Prompt out of the screen if the player is not able to interact with the object
            float interactGUIXPos = playerCanInteract ? interactGUI.rect.width/2f + SCREENMARGIN : -interactGUI.rect.width; 
            float XPosLerp = Mathf.Lerp(interactGUI.anchoredPosition.x, interactGUIXPos, 15f * GameUtility.timeDelta);
            interactGUI.anchoredPosition = new Vector2(XPosLerp, interactGUI.anchoredPosition.y); 

            if ( !playerCanInteract ) return;

            interactDescription.text = interactCandidate.InteractDescription();
                
            MonoBehaviour interactionCandidate = interactCandidate as MonoBehaviour;

            if (interactionCandidate == null) return;

            Vector3 screenPos = CameraController.current.camera.WorldToScreenPoint(interactionCandidate.transform.position); 
            interactCursor.position = ClampHUDToScreen(screenPos, interactCursor.rect.width, interactCursor.rect.height, 0);
        }


        private void UpdateKeybind(Guid keybindId){
            if (keybindId == interactBinding.id){
                interactBind.text = interactBinding.ToDisplayString();
            }
        }



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
    }
}
