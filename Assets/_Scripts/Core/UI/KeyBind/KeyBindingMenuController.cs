using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.Core.UI {
    
    public class KeyBindingMenuController : UIModal<KeyBindingMenuController>, IPausedMenu {

        [SerializeField] private GameObject keyBindingMenu;
        [SerializeField] private GameObject keyBindingContainer;
        [SerializeField] private GameObject keyBindingButtonTemplate;
        
        [SerializeField] private List<RebindButton> rebinds = new List<RebindButton>();



        private void InitializeKeybindings() {
            rebinds = new List<RebindButton>();
            int childCount = keyBindingContainer.transform.childCount;
            for (int i = 0; i < childCount; i++) {
                Transform child = keyBindingContainer.transform.GetChild(0);
                GameUtility.SafeDestroy(child.gameObject);
            }

            for (int i = 0; i < Keybinds.playerMap.actions.Count; i++){
                InputAction action = Keybinds.playerMap.actions[i];
                if (action.name == "Move"){
                    CreateRebindButton(action, 1, "Forward");
                    CreateRebindButton(action, 2, "Left");
                    CreateRebindButton(action, 3, "Backwards");
                    CreateRebindButton(action, 4, "Right");
                }else if (action.name != "Look"){
                    CreateRebindButton(action, 0);
                }

                if (i == 0)
                    ResetGamePadSelection();

            }

            // RebindButton lastButton = rebinds[rebinds.Count - 1];
            // lastButton.elementDown = returnButton;
            // returnButton.elementUp = lastButton;

        }

        public void UpdateKeybindings(){
            foreach (RebindButton rebind in rebinds) {
                rebind.UpdateKeybind();
            }
        }

        private void CreateRebindButton(InputAction action, int bindingIndex, string buttonText = ""){
            var buttonObject = Instantiate(keyBindingButtonTemplate, Vector3.zero, Quaternion.identity, keyBindingContainer.transform);
            var button = buttonObject.GetComponentInChildren<RebindButton>();
            button.action = action;
            button.bindingIndex = bindingIndex;
            button.UpdateKeybind();
            button.SetBindingText( $"{action.name}{buttonText}".Nicify() );
            
            rebinds.Add( button );

        }



        public override void Enable() {

            base.Enable();

            InitializeKeybindings();

            keyBindingMenu.SetActive( true );
        }

        public override void Disable() {
            base.Disable();

            keyBindingMenu.SetActive( false );
        }

        public override void Refresh(){
            ResetGamePadSelection();
        }

        public override void EnableInteraction() {
            foreach (RebindButton rebind in rebinds) {
                rebind.EnableInteraction();
            }
        }

        public override void DisableInteraction() {
            foreach (RebindButton rebind in rebinds) {
                rebind.DisableInteraction();
            }
        }

        public override void ResetGamePadSelection() {
            EventSystem.current.SetSelectedGameObject(rebinds[0].gameObject);
        }
    }
}