using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

using SeleneGame.Saving;

namespace SeleneGame.UI {
    public class SaveSlot : CustomButton {

        [SerializeField] private Sprite bgEmptyUnselected;
        [SerializeField] private Sprite bgEmptySelected;
        [SerializeField] private Sprite bgEmptyClicked;

        [SerializeField] uint slotNumber = 1;

        [SerializeField] private GameObject saveInfo;
        [SerializeField] private TextMeshProUGUI slotNumberText;
        [SerializeField] private TextMeshProUGUI playTime;
        [SerializeField] private TextMeshProUGUI saveDateTime;

        private SaveData saveData;
        

        public void LoadPreviewData(){
            slotNumberText.text = $"0{slotNumber}";
            saveData = SavingSystem.LoadDataFromFile(slotNumber);

            if (saveData != null){
                saveInfo.SetActive(true);
                playTime.text = saveData.GetTotalPlaytime().ToString();
                saveDateTime.text = saveData.GetTimeOfLastSave().ToString();

                SetBackground( bgUnselected );
            }else {
                saveInfo.SetActive(false);
                playTime.text = "";
                saveDateTime.text = "";

                SetBackground( bgEmptyUnselected );
            }
        }

        public override void OnSelect(BaseEventData eventData) {
            if (saveData == null)
                SetBackground( bgEmptySelected );
            else
                SetBackground( bgSelected );
        }

        public override void OnDeselect(BaseEventData eventData) {
            if (saveData == null)
                SetBackground( bgEmptyUnselected );
            else
                SetBackground( bgUnselected );
        }

        public override void OnPointerDown(PointerEventData eventData) {
            if (saveData == null)
                SetBackground( bgEmptyClicked );
            else
                SetBackground( bgClicked );
        }

        public override void OnPointerUp(PointerEventData eventData) {
            if (saveData == null)
                SetBackground( bgEmptySelected );
            else
                SetBackground( bgSelected );
        }

        public override void OnPointerClick(PointerEventData eventData) {

            if (eventData.button == PointerEventData.InputButton.Left) {
                SavingSystem.SavePlayerData(slotNumber);
                LoadPreviewData();
            } else if (eventData.button == PointerEventData.InputButton.Right && saveData != null) {
                SavingSystem.LoadPlayerData(slotNumber);
            }

            SaveMenuController.current.Disable();
        }
    }
}

