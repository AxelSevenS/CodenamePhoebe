using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

using SevenGame.SavingSystem;
using SevenGame.UI;

namespace SeleneGame.Core.UI {

    public abstract class SaveSlot<TData> : CustomButton where TData : SaveData, new() {

        [SerializeField] uint slotNumber = 1;

        [SerializeField] private GameObject saveInfo;
        [SerializeField] private TextMeshProUGUI slotNumberText;
        [SerializeField] private TextMeshProUGUI playTime;
        [SerializeField] private TextMeshProUGUI saveDateTime;

        private TData saveData;
        

        public void LoadPreviewData(){
            slotNumberText.text = $"0{slotNumber}";
            saveData = SavingSystem<TData>.LoadDataFromFile(slotNumber);

            if (saveData != null) {
                saveInfo.SetActive(true);
                playTime.text = saveData.GetTotalPlaytime().ToString();
                saveDateTime.text = saveData.GetTimeOfLastSave().ToString();
            } else {
                saveInfo.SetActive(false);
                playTime.text = "";
                saveDateTime.text = "";
            }

            EnableInteraction();
        }

        protected override void OnEnable() {
            base.OnEnable();
            LoadPreviewData();
        }

        public override void OnSubmit(BaseEventData eventData) {
            if (!interactable) return;

            base.OnSubmit(eventData);

            SaveMenuController<TData>.current.onSaveSlotSelected?.Invoke(slotNumber);

            // if (eventData.button == PointerEventData.InputButton.Left) {
            //     SavingSystem<TData>.SaveData(slotNumber);
            //     LoadPreviewData();
            // } else if (eventData.button == PointerEventData.InputButton.Right && saveData != null) {
            //     SavingSystem<TData>.LoadData(slotNumber);
            // }
            LoadPreviewData();

            SaveMenuController<TData>.current.Disable();
        }

        public override void EnableInteraction() {
            interactable = saveData != null;
        }

        public override void DisableInteraction() {
            interactable = false;
        }
    }
}

