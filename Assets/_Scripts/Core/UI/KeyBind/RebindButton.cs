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

        public void StartAssignment() {

            var rebindOperation = action.PerformInteractiveRebinding()
                .WithTargetBinding(bindingIndex)
                // .WithControlsExcluding("Pointer")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => CompleteRebind(operation))
                .OnCancel(operation => CompleteRebind(operation));

            Debug.Log(action.bindings);

            if (action.bindings[bindingIndex].isPartOfComposite){
                rebindOperation.WithExpectedControlType("Button");
            }

            rebindOperation.Start();

            void CompleteRebind(InputActionRebindingExtensions.RebindingOperation operation){
                Keybinds.UpdateKeybind(action.bindings[bindingIndex].id);
                UpdateKeybind();
                operation.Dispose();
            }
        }


        public void SetBindingText(string text){
            bindLabel.text = text;
        }


        public void UpdateKeybind(){
            bindText.text = action.GetBindingDisplayString(bindingIndex);
        }

        private void UpdateKeybind(System.Guid guid) {
            if (action.bindings[bindingIndex].id == guid){
                UpdateKeybind();
            }
        }

        
        
        private void OnEnable() {
            Keybinds.onUpdateKeybind += UpdateKeybind;
        }
        private void OnDisable() {
            Keybinds.onUpdateKeybind -= UpdateKeybind;
        }

        
    }
}
