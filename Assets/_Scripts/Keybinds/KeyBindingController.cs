using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using SeleneGame.Utility;

namespace SeleneGame.Core {
    
    public class KeyBindingController : MonoBehaviour{

        private List<GameObject> keybindObjects = new List<GameObject>();
        private List<InputAction> keybindActions = new List<InputAction>();
        private List<int> keybindIndices = new List<int>();

        [SerializeField] private GameObject keyBindingMenu;

        private void OnDisable(){
            GameEvents.onToggleMenu -= ToggleMenu;
        }
        private void OnEnable(){
            GameEvents.onToggleMenu += ToggleMenu;
        }

        private void Awake(){
            keyBindingMenu.SetActive(true);
            InitializeKeybindings();
            keyBindingMenu.SetActive(Player.current.menu);
        }

        private void ToggleMenu(){
            if (Player.current.menu){
                Player.current.playerControls.Enable();
            }else{
                Player.current.playerControls.Disable();
            }
            Player.current.menu = !Player.current.menu;
            keyBindingMenu.SetActive(Player.current.menu);
        }

        private void InitializeKeybindings(){
            foreach (var action in Player.current.playerControls.actions){
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
            for (int i = 0; i < keybindObjects.Count; i++){
                UpdateKeyBinding(keybindObjects[i], keybindActions[i], keybindIndices[i]);
            }
        }

        public void UpdateKeyBinding(GameObject buttonObject, InputAction action, int bindingIndex){
            buttonObject.transform.GetChild(1).GetComponentInChildren<Text>().text = action.GetBindingDisplayString(bindingIndex);
        }

        private void CreateRebindButton(InputAction action, int bindingIndex, string buttonText = ""){
            var buttonObject = Instantiate(Resources.Load<GameObject>($"Templates/keyBinding"), Vector3.zero, Quaternion.identity, keyBindingMenu.transform);
            var button = buttonObject.GetComponentInChildren<Button>();

            button.onClick.AddListener(() => StartAssignment(buttonObject, action, bindingIndex));
            
            UpdateKeyBinding(buttonObject, action, bindingIndex);
            buttonObject.transform.GetChild(0).GetComponentInChildren<Text>().text = $"{action.name}{buttonText}".Nicify();

            keybindObjects.Add(buttonObject);
            keybindActions.Add(action);
            keybindIndices.Add(bindingIndex);
        }

        public void StartAssignment(GameObject buttonObject, InputAction action, int bindingIndex){

            var rebindOperation = action.PerformInteractiveRebinding()
                .WithTargetBinding(bindingIndex)
                // .WithControlsExcluding("Pointer")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => CompleteRebind(operation, buttonObject, bindingIndex))
                .OnCancel(operation => CompleteRebind(operation, buttonObject, bindingIndex));

            if (action.bindings[bindingIndex].isPartOfComposite){
                rebindOperation.WithExpectedControlType("Button");
            }

            rebindOperation.Start();
        }

        private void CompleteRebind(InputActionRebindingExtensions.RebindingOperation operation, GameObject buttonObject, int bindingIndex){
            UpdateKeyBinding(buttonObject, operation.action, bindingIndex);
            operation.Dispose();
            FindObjectOfType<EventSystem>().SetSelectedGameObject(null);

            // SavingSystem.SaveControlsToFile();
        }
    }
}