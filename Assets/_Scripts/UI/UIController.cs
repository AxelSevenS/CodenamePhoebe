using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SeleneGame.Core;

namespace SeleneGame {

    public class UIController : MonoBehaviour{
        
        private float screenUIMargin = 50f;
        public GameObject interactGUI;
        private RectTransform interactGUIRect;
        private TextMeshProUGUI interactDescription;
        private TextMeshProUGUI interactBind;
        public GameObject landCursor;
        public GameObject aimCursor;
        // Vector3 deltaRotation = new Vector3(0,0,0);

        void Awake(){
            interactGUIRect = interactGUI.GetComponent<RectTransform>();
            interactDescription = interactGUI.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
            interactBind = interactGUI.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();
        }

        void Update(){
            // Interaction
            if (interactGUI != null && Player.current.interactionCandidate != null){
                var interactionCandidate = Player.current.interactionCandidate as MonoBehaviour;

                interactDescription.text = Player.current.interactionCandidate.interactionDescription;
                interactBind.text = Player.current.playerControls["Interact"].bindings[0].ToDisplayString();
                interactGUI.transform.position = ScreenBoundItem(interactionCandidate.transform, interactGUIRect.rect.width, interactGUIRect.rect.height, screenUIMargin);
            }
            interactGUI.SetActive(Player.current.canInteract);

            // Focus
            aimCursor.SetActive(Player.current.entity.focusing);
            
            
        }

        private Vector3 ScreenBoundItem(Transform item, float width, float height, float margin){
            float totalWidth = margin+width;
            float totalHeight = margin+height;
            Vector3 screenPos = Player.current.camera.WorldToScreenPoint(item.position); 
            return new Vector3(Mathf.Clamp( screenPos.x, totalWidth, Screen.width-totalWidth), Mathf.Clamp(screenPos.y, totalHeight, Screen.height-totalHeight), 0);
        }
    }
}