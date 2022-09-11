using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.UI {
    public class SaveMenuController : MenuController<SaveMenuController> {
        [SerializeField] private GameObject saveMenu;
        
        [SerializeField] private SaveSlot saveSlot1;
        [SerializeField] private SaveSlot saveSlot2;
        [SerializeField] private SaveSlot saveSlot3;



        public override void Enable(){
            if (!Enabled)
                UIController.current.DisableAllMenus();

            base.Enable();

            saveSlot1.LoadPreviewData();
            saveSlot2.LoadPreviewData();
            saveSlot3.LoadPreviewData();

            saveMenu.SetActive( true );

            SetSelectedObject();

            UIController.current.UpdateMenuState();
        }

        public override void Disable(){
            base.Disable();

            saveMenu.SetActive( false );
            
            UIController.current.UpdateMenuState();
        }

        public override void SetSelectedObject(){
            if ( Enabled && ControlsManager.controllerType != ControlsManager.ControllerType.MouseKeyboard )
                EventSystem.current.SetSelectedGameObject(saveSlot1.gameObject);
        }

    }
}
