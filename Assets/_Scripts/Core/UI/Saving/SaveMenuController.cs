using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using SeleneGame.Core;

using SevenGame.SavingSystem;
using System;

namespace SeleneGame.Core.UI {


    public abstract class SaveMenuController<TData> : UIMenu<SaveMenuController<TData>>, IPausedMenu where TData : SaveData, new() {
        
        [SerializeField] private GameObject saveMenu;
        
        [SerializeField] private SaveSlot<TData> saveSlot1;
        [SerializeField] private SaveSlot<TData> saveSlot2;
        [SerializeField] private SaveSlot<TData> saveSlot3;


        public Action<uint> onSaveSlotSelected { get; protected set; }


        public void OpenSaveDataMenu() {

            current.onSaveSlotSelected = SaveData;

            void SaveData(uint slotNumber){
                SavingSystem<TData>.SaveData(slotNumber);
            }

            Enable();
        }

        public void OpenLoadDataMenu() {

            current.onSaveSlotSelected = LoadData;

            void LoadData(uint slotNumber){
                SavingSystem<TData>.LoadData(slotNumber);
            }

            Enable();
        }

        public override void Enable(){
            base.Enable();

            saveMenu.SetActive( true );

            UIController.current.UpdateMenuState();

            ResetGamePadSelection();
        }

        public override void Disable(){
            base.Disable();

            saveMenu.SetActive( false );
            
            UIController.current.UpdateMenuState();
        }

        public override void ResetGamePadSelection(){
            SetSelected(saveSlot1.gameObject);
        }

    }
}
