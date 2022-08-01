using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.UI {

    public class UIController : Singleton<UIController>{

        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject saveMenu;
        [SerializeField] private SaveSlot saveSlot1;
        [SerializeField] private SaveSlot saveSlot2;
        [SerializeField] private SaveSlot saveSlot3;

        private BoolData menuInput;
        private bool pauseMenuActive;
        private bool saveMenuActive;


        private void OnEnable() { 
            SetCurrent();
        }

        public void TogglePauseMenu(){
            pauseMenuActive = !pauseMenuActive;
            UpdateMenuState();

            pauseMenu.SetActive( pauseMenuActive );
        }

        public void ToggleSaveMenu(){
            saveMenuActive = !saveMenuActive;
            UpdateMenuState();

            saveSlot1.LoadPreviewData();
            saveSlot2.LoadPreviewData();
            saveSlot3.LoadPreviewData();
            saveMenu.SetActive( saveMenuActive );
        }

        private void UpdateMenuState(){

            bool gameInterrupted = pauseMenuActive || saveMenuActive;

            if ( gameInterrupted ){
                Time.timeScale = 0;
                ControlsManager.current.playerMap.Disable();
                PlayerEntityController.current.menu = true;
            }else{
                Time.timeScale = 1;
                ControlsManager.current.playerMap.Enable();
                PlayerEntityController.current.menu = false;
            }
        }

        private void Update() {
            
            menuInput.SetVal( ControlsManager.current.debugMap.IsBindPressed("DebugKeyBindMenu") );

            if (menuInput.started /* && !saveMenuActive */)
                ToggleSaveMenu();
        }

    }
}