using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SeleneGame.Core;
using SeleneGame.Utility;

namespace SeleneGame.UI {

    public class UIController : MonoBehaviour{

        public static UIController current;
        
        private const float SCREENMARGIN = 25f;
        public RectTransform interactCursor;
        public RectTransform interactGUI;
        [SerializeField] private TextMeshProUGUI interactDescription;
        [SerializeField] private TextMeshProUGUI interactBind;
        public GameObject aimCursor;
        // Vector3 deltaRotation = new Vector3(0,0,0);

        private void Awake(){
            current = this;
        }

        void Update(){
            // Focus
            aimCursor.SetActive(false/* Player.current.entity.focusing */);

            // Interact
            IInteractable interactCandidate = Player.current.interactionCandidate;

            interactCursor.gameObject.SetActive(Player.current.canInteract);


            bool playerCanInteract = Player.current.canInteract && interactCandidate.interactionDescription != "";
            float interactGUIXPos = playerCanInteract ? interactGUI.rect.width/2f + SCREENMARGIN : -interactGUI.rect.width; 
            float XPosLerp = Mathf.Lerp(interactGUI.anchoredPosition.x, interactGUIXPos, 15f * GameUtility.timeDelta);
            interactGUI.anchoredPosition = new Vector2(XPosLerp, interactGUI.anchoredPosition.y);

            if ( interactCandidate == null ) return;

            interactDescription.text = interactCandidate.interactionDescription;
            interactBind.text = Player.current.playerControls["Interact"].bindings[0].ToDisplayString();
                
            MonoBehaviour interactionCandidate = interactCandidate as MonoBehaviour;

            Vector3 screenPos = Player.current.camera.WorldToScreenPoint(interactionCandidate.transform.position); 
            interactCursor.position = ClampUIToScreen(screenPos, interactCursor.rect.width, interactCursor.rect.height, 0);

            
        }

        public static Vector3 ClampUIToScreen(Vector3 position, float width, float height, float margin){
            float horizontalMargin = margin + width/4f;
            float verticalMargin = margin + height/4f;

            float clampedX = Mathf.Clamp(position.x, horizontalMargin, Screen.width - horizontalMargin);
            float clampedY = Mathf.Clamp(position.y, verticalMargin, Screen.height - verticalMargin);
            return new Vector3( clampedX, clampedY, 0 );
        }
        public static Vector3 ClampUIToScreen(RectTransform rect, float margin){
            float horizontalMargin = margin + rect.rect.width/4f;
            float verticalMargin = margin + rect.rect.height/4f;

            float clampedX = Mathf.Clamp(rect.position.x, horizontalMargin, Screen.width - horizontalMargin);
            float clampedY = Mathf.Clamp(rect.position.y, verticalMargin, Screen.height - verticalMargin);
            return new Vector3( clampedX, clampedY, 0 );
        }
    }
}