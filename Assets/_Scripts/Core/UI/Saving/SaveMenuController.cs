using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using SeleneGame.Core;

using SevenGame.SavingSystem;

namespace SeleneGame.Core.UI {

    public abstract class SaveMenuController : UIMenu<SaveMenuController>, IUIPausedMenu {

    }

    public abstract class SaveMenuController<TData> : SaveMenuController where TData : SaveData, new() {
        
        [SerializeField] private GameObject saveMenu;
        
        [SerializeField] private SaveSlot<TData> saveSlot1;
        [SerializeField] private SaveSlot<TData> saveSlot2;
        [SerializeField] private SaveSlot<TData> saveSlot3;



        public override void Enable(){
            base.Enable();

            saveSlot1.LoadPreviewData();
            saveSlot2.LoadPreviewData();
            saveSlot3.LoadPreviewData();

            saveMenu.SetActive( true );

            ResetGamePadSelection();

            UIController.current.UpdateMenuState();
        }

        public override void Disable(){
            base.Disable();

            saveMenu.SetActive( false );
            
            UIController.current.UpdateMenuState();
        }

        public override void ResetGamePadSelection(){
            EventSystem.current.SetSelectedGameObject(saveSlot1.gameObject);
        }

    }
}
