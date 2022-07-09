using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace SeleneGame.UI {
    public class RebindButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

        public InputAction action;
        public int bindingIndex;

        [SerializeField] private Text bindLabel;
        [SerializeField] private Text bindText;

        public void OnPointerClick(PointerEventData eventData) {
            // if (eventData.button == PointerEventData.InputButton.Left) {
            //     GameEvents.PlayerInput();
            // }
            // Debug.Log("Pointer Click");
            StartAssignment();
        }

        public void OnPointerEnter(PointerEventData eventData) {
            // if (eventData.button == PointerEventData.InputButton.Left) {
            //     GameEvents.PlayerInput();
            // }
            Debug.Log("Pointer Enter");
        }
        public void OnPointerExit(PointerEventData eventData) {
            // if (eventData.button == PointerEventData.InputButton.Left) {
            //     GameEvents.PlayerInput();
            // }
            Debug.Log("Pointer Exit");
        }

        public void StartAssignment(){

            var rebindOperation = action.PerformInteractiveRebinding()
                .WithTargetBinding(bindingIndex)
                // .WithControlsExcluding("Pointer")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => CompleteRebind(operation))
                .OnCancel(operation => CompleteRebind(operation));

            if (action.bindings[bindingIndex].isPartOfComposite){
                rebindOperation.WithExpectedControlType("Button");
            }

            rebindOperation.Start();

            void CompleteRebind(InputActionRebindingExtensions.RebindingOperation operation){
                UpdateKeyBinding();
                operation.Dispose();
                
                // FindObjectOfType<EventSystem>().SetSelectedGameObject(null);
            }
        }


        public void UpdateKeyBinding(){
            bindText.text = action.GetBindingDisplayString(bindingIndex);
        }

        public void SetBindingText(string text){
            bindLabel.text = text;
        }
        
    }
}
