using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SeleneGame.Core;

namespace SeleneGame {

    public class UIController : MonoBehaviour{
        
        private float screenUIMargin = 50f;
        public GameObject interactCursor;
        public GameObject interactGUI;
        [SerializeField] private RectTransform interactGUIRect;
        [SerializeField] private TextMeshProUGUI interactDescription;
        [SerializeField] private TextMeshProUGUI interactBind;
        public GameObject aimCursor;
        // Vector3 deltaRotation = new Vector3(0,0,0);

        void Update(){
            // Focus
            aimCursor.SetActive(false/* Player.current.entity.focusing */);

            // Interact
            var interactCandidate = Player.current.interactionCandidate;

            interactCursor.SetActive(Player.current.canInteract);
            interactGUI.SetActive(Player.current.canInteract && interactCandidate.interactionDescription != "");

            if ( interactCandidate == null ) return;

            interactDescription.text = interactCandidate.interactionDescription;
            interactBind.text = Player.current.playerControls["Interact"].bindings[0].ToDisplayString();
                
            var interactionCandidate = interactCandidate as MonoBehaviour;

            interactCursor.transform.position = ScreenBoundItem(interactionCandidate.transform, interactGUIRect.rect.width, interactGUIRect.rect.height, screenUIMargin);

            
            
        }

        private Vector3 ScreenBoundItem(Transform item, float width, float height, float margin){
            float totalWidth = margin+width;
            float totalHeight = margin+height;
            Vector3 screenPos = Player.current.camera.WorldToScreenPoint(item.position); 
            return new Vector3(Mathf.Clamp( screenPos.x, totalWidth, Screen.width-totalWidth), Mathf.Clamp(screenPos.y, totalHeight, Screen.height-totalHeight), 0);
        }
    }
}