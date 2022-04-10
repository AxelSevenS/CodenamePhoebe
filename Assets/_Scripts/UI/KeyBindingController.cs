using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SeleneGame {
    
    public class KeyBindingController : MonoBehaviour{

        [SerializeField] private GameObject keyBindingMenu;

        private void OnDisable(){
            GameEvents.onToggleMenu -= ToggleMenu;
        }
        private void OnEnable(){
            GameEvents.onToggleMenu += ToggleMenu;
        }

        private void Awake(){
            keyBindingMenu.SetActive(true);
            SavingSystem.LoadControls();
            InitializeKeybindings();
            keyBindingMenu.SetActive(Player.current.menu);
        }

        // private void Start(){
        //     SavingSystem.LoadControls();
        // }

        private void InitializeKeybindings(){
            float btnY = 240f;
            foreach (var action in Player.current.playerControls.actions){
                if (action.name == "Move"){
                    CreateRebindButton("MoveForward", action, ref btnY);
                    CreateRebindButton("MoveLeft", action, ref btnY);
                    CreateRebindButton("MoveBack", action, ref btnY);
                    CreateRebindButton("MoveRight", action, ref btnY);
                }else if (action.name != "Look"){
                    CreateRebindButton(action.name, action, ref btnY);
                }
            }
        }

        private void CreateRebindButton(string actionName, InputAction action, ref float height){
            var newbuttonObject = Instantiate(Resources.Load<GameObject>($"Templates/forward"), Vector3.zero, Quaternion.identity, keyBindingMenu.transform);
            var newButtonComponent = newbuttonObject.GetComponent<Button>();
            newbuttonObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(85f, height, 0f);
            newButtonComponent.onClick.AddListener(() => StartAssignment(newButtonComponent));

            // Give The Button a name which is used as an identifier for the Key Assignment later.
            // e.g. a button named Move1 will change the key that corresponds to the up/forward component of the "Move" keybind.
            if (actionName.Contains("Forward") || actionName.Contains("Up"))
                newbuttonObject.name = $"{action.name}1";
            else if (actionName.Contains("Left"))
                newbuttonObject.name = $"{action.name}2";
            else if (actionName.Contains("Back") || actionName.Contains("Down"))
                newbuttonObject.name = $"{action.name}3";
            else if (actionName.Contains("Right"))
                newbuttonObject.name = $"{action.name}4";
            else
                newbuttonObject.name = action.name;
            
            RenameRebindButton(action, newButtonComponent);
            newbuttonObject.transform.GetChild(1).GetComponentInChildren<Text>().text = actionName.Nicify();
            height -= 30f;
        }

        void ToggleMenu(){
            Player.current.menu = !Player.current.menu;
            keyBindingMenu.SetActive(Player.current.menu);
            if (Player.current.menu){
                Player.current.playerControls.Disable();
            }else{
                Player.current.playerControls.Enable();
            }
        }

        public void StartAssignment(Button button){
            string actionName = button.name;
            int index = 0;
            if ( System.Char.IsNumber(actionName[actionName.Length - 1]) ){
                index = (int)char.GetNumericValue( actionName[actionName.Length - 1] );
                actionName = actionName.Substring(0, actionName.Length - 1);
            }

            var actionToRebind = Player.current.playerControls[actionName];
            var rebindOperation = actionToRebind.PerformInteractiveRebinding()
                .WithTargetBinding(index)
                // .WithControlsExcluding("Pointer")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => CompleteRebind(operation, button))
                .OnCancel(operation => CompleteRebind(operation, button));

            if (actionToRebind.bindings[index].isPartOfComposite){
                rebindOperation.WithExpectedControlType("Button");
            }

            rebindOperation.Start();
        }

        private void CompleteRebind(InputActionRebindingExtensions.RebindingOperation operation, Button button){
            RenameRebindButton(operation.action, button);
            operation.Dispose();
            FindObjectOfType<EventSystem>().SetSelectedGameObject(null);

            SavingSystem.SaveControls();
        }

        private void RenameRebindButton(InputAction action, Button button){
            int index = 0;
            if (System.Char.IsNumber( button.name[button.name.Length - 1]) ){
                index = (int)char.GetNumericValue( button.name[button.name.Length - 1] );
            }
            button.transform.GetChild(0).GetComponentInChildren<Text>().text = action.GetBindingDisplayString(index);
        }
    }
}