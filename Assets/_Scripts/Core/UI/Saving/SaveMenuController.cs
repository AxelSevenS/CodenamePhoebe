using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using SeleneGame.Core;

using SevenGame.SavingSystem;
using System;
using TMPro;

namespace SeleneGame.Core.UI {


    public abstract class SaveMenuController<TData> : UIModal<SaveMenuController<TData>> where TData : SaveData, new() {

        [SerializeField] private TextMeshProUGUI titleText;
        
        [SerializeField] private GameObject saveMenu;
        
        [SerializeField] private SaveSlot<TData> saveSlot1;
        [SerializeField] private SaveSlot<TData> saveSlot2;
        [SerializeField] private SaveSlot<TData> saveSlot3;


        public Action<uint> onSaveSlotSelected { get; protected set; }


        public void OpenSaveDataMenu() {

            titleText.text = "Save Game";

            current.onSaveSlotSelected = SaveData;

            void SaveData(uint slotNumber){
                Debug.Log($"Saving to slot {slotNumber}");
                SavingSystem<TData>.SaveData(slotNumber);
            }

            Enable();
        }

        public void OpenLoadDataMenu() {

            titleText.text = "Load Game";

            current.onSaveSlotSelected = LoadData;

            void LoadData(uint slotNumber){
                Debug.Log($"Loading from slot {slotNumber}");
                SavingSystem<TData>.LoadData(slotNumber);
            }

            Enable();
        }

        public override void Enable(){
            base.Enable();

            saveMenu.SetActive( true );

            ResetGamePadSelection();
        }

        public override void Disable(){
            base.Disable();

            saveMenu.SetActive( false );
        }

        public override void Refresh(){
            ResetGamePadSelection();
        }

        public override void EnableInteraction() {
            saveSlot1.EnableInteraction();
            saveSlot2.EnableInteraction();
            saveSlot3.EnableInteraction();
        }

        public override void DisableInteraction() {
            saveSlot1.DisableInteraction();
            saveSlot2.DisableInteraction();
            saveSlot3.DisableInteraction();
        }

        public override void ResetGamePadSelection(){
            EventSystem.current.SetSelectedGameObject(saveSlot1.gameObject);
        }

    }
}
