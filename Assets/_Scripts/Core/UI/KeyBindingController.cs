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

        public bool ToggleMenu(bool oldState){
            bool newState = !oldState;

            if ( newState ){
                ControlsManager.current.playerMap.Enable();
                keyBindingMenu.SetActive( true );
            }else{
                ControlsManager.current.playerMap.Disable();
                keyBindingMenu.SetActive( false );
            }
            return newState;
        }

        private void InitializeKeybindings(){
            keyBindingMenu.SetActive(true);

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
            
            keyBindingMenu.SetActive(false);
        }

        public void UpdateKeyBindings(){
            // for (int i = 0; i < keybinds.Count; i++){
            //     UpdateKeyBinding(keybinds[i].valueOne, keybinds[i].valueTwo, keybinds[i].valueThree);
            // }
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
            // button.onClick.AddListener(() => StartAssignment(buttonObject, action, bindingIndex));
            
            // UpdateKeyBinding(buttonObject, action, bindingIndex);
            // buttonObject.transform.GetChild(0).GetComponentInChildren<Text>().text = $"{action.name}{buttonText}".Nicify();

            // ValueTrio< GameObject, InputAction, int > keybind = new ValueTrio< GameObject, InputAction, int >( buttonObject, action, bindingIndex );
            rebinds.Add( button );
        }
    }
}