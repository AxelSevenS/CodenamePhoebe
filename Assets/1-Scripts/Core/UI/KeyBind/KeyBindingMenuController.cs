using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.Core.UI {
    
    public class KeyBindingMenuController : UIPausedMenu<KeyBindingMenuController> {

        [SerializeField] private GameObject keyBindingMenu;
        [SerializeField] private GameObject keyBindingContainer;
        [SerializeField] private GameObject keyBindingButtonTemplate;
        
        [SerializeField] private List<RebindButton> rebinds = new List<RebindButton>();



        public override void Enable() {

            base.Enable();

            keyBindingMenu.SetActive( true );

            ResetGamePadSelection();

            UIController.current.UpdateMenuState();
        }

        public override void Disable() {
            base.Disable();

            keyBindingMenu.SetActive( false );

            UIController.current.UpdateMenuState();
        }

        public override void ResetGamePadSelection(){
            EventSystem.current.SetSelectedGameObject(rebinds[0].gameObject);
        }


        private void InitializeKeybindings(){
            rebinds = new List<RebindButton>();
            int childCount = keyBindingContainer.transform.childCount;
            for (int i = 0; i < childCount; i++) {
                Transform child = keyBindingContainer.transform.GetChild(0);
                GameUtility.SafeDestroy(child.gameObject);
            }

            foreach (var action in ControlsManager.playerMap.actions){
                if (action.name == "Move"){
                    CreateRebindButton(action, 1, "Forward");
                    CreateRebindButton(action, 2, "Left");
                    CreateRebindButton(action, 3, "Backwards");
                    CreateRebindButton(action, 4, "Right");
                }else if (action.name != "Look"){
                    CreateRebindButton(action, 0);
                }
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
            if (rebinds.Count > 1) {
                RebindButton previousButton = rebinds[rebinds.Count - 2];
                previousButton.elementDown = button;
                button.elementUp = previousButton;
            }

        }


        private void Awake(){
            InitializeKeybindings();
        }
    }
}