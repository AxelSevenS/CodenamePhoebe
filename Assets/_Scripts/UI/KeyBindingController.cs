using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class KeyBindingController : Singleton<KeyBindingController>{

        [SerializeField] private GameObject keybindingButtonTemplate;
        [SerializeField] private GameObject keyBindingMenu;

        // private List< ValueTrio< GameObject, InputAction, int > > keybinds = new List<ValueTrio< GameObject, InputAction, int > >();
        private List<RebindButton> rebinds = new List<RebindButton>();

        private void Awake(){
            InitializeKeybindings();
        }
        private void OnEnable() {
            SetCurrent();
        }

        private void InitializeKeybindings(){

            foreach (var action in ControlsManager.current.playerMap.actions){
                if (action.name == "Move"){
                    CreateRebindButton(action, 1, "Forward");
                    CreateRebindButton(action, 2, "Left");
                    CreateRebindButton(action, 3, "Backwards");
                    CreateRebindButton(action, 4, "Right");
                }else if (action.name != "Look"){
                    CreateRebindButton(action, 0);
                }
            }

        }

        public void UpdateKeyBindings(){
            foreach (RebindButton rebind in rebinds) {
                rebind.UpdateKeyBinding();
            }
        }

        // public void UpdateKeyBinding(GameObject buttonObject, InputAction action, int bindingIndex){
        //     buttonObject.transform.GetChild(1).GetComponentInChildren<Text>().text = action.GetBindingDisplayString(bindingIndex);
        // }

        private void CreateRebindButton(InputAction action, int bindingIndex, string buttonText = ""){
            var buttonObject = Instantiate(keybindingButtonTemplate, Vector3.zero, Quaternion.identity, keyBindingMenu.transform);
            var button = buttonObject.GetComponentInChildren<RebindButton>();
            button.action = action;
            button.bindingIndex = bindingIndex;
            button.UpdateKeyBinding();
            button.SetBindingText( $"{action.name}{buttonText}".Nicify() );
            
            rebinds.Add( button );
        }
    }
}