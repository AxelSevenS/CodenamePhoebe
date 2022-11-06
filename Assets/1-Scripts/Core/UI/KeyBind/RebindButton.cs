using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

using SeleneGame.Core;

namespace SeleneGame.Core.UI {
    
    public class RebindButton : CustomButton {

        public InputAction action;
        public int bindingIndex;

        [SerializeField] private Text bindLabel;
        [SerializeField] private Text bindText;



        private void UpdateKeybind(Guid keybindId){

            if ( action.bindings[bindingIndex].id == keybindId ) {
                UpdateKeyBinding();
            }
        }

        public override void OnSelect(BaseEventData eventData) {
            base.OnSelect(eventData);
        }
        public override void OnDeselect(BaseEventData eventData) {
            base.OnDeselect(eventData);
        }
        public override void OnPointerClick(PointerEventData eventData) {
            base.OnPointerClick(eventData);
            StartAssignment();
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
            }
        }


        public void UpdateKeyBinding(){
            bindText.text = action.GetBindingDisplayString(bindingIndex);
        }

        public void SetBindingText(string text){
            bindLabel.text = text;
        }


        
        private void Awake() {
            ControlsManager.onUpdateKeybind += UpdateKeybind;
        }
        private void OnDestroy() {
            ControlsManager.onUpdateKeybind -= UpdateKeybind;
        }
        
    }
}
