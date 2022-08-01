using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

using SeleneGame.Saving;

namespace SeleneGame.UI {
    public class SaveSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {

        [SerializeField] uint slotNumber = 1;

        [SerializeField] private GameObject saveInfo;
        [SerializeField] private TextMeshProUGUI slotNumberText;
        [SerializeField] private TextMeshProUGUI playTime;
        [SerializeField] private TextMeshProUGUI saveDateTime;

        [SerializeField] private Image slotBackground;

        [SerializeField] private Sprite slotEmpty;
        [SerializeField] private Sprite slotFull;
        [SerializeField] private Sprite slotSelected;

        private SaveData saveData;

        public void LoadPreviewData(){
            slotNumberText.text = $"0{slotNumber}";
            saveData = SavingSystem.LoadDataFromFile(slotNumber);

            if (saveData != null){
                saveInfo.SetActive(true);
                playTime.text = saveData.GetTotalPlaytime().ToString();
                saveDateTime.text = saveData.GetTimeOfLastSave().ToString();
                slotBackground.sprite = slotFull;
            }else {
                saveInfo.SetActive(false);
                playTime.text = "";
                saveDateTime.text = "";
                slotBackground.sprite = slotEmpty;
            }
        }

        public void OnPointerEnter(PointerEventData eventData) {
            if (saveData != null){
                slotBackground.sprite = slotSelected;
            }
        }

        public void OnPointerExit(PointerEventData eventData) {
            if (saveData != null){
                slotBackground.sprite = slotFull;
            }else {
                slotBackground.sprite = slotEmpty;
            }
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Left) {
                SavingSystem.SavePlayerData(slotNumber);
                LoadPreviewData();
            } else if (eventData.button == PointerEventData.InputButton.Right) {
                SavingSystem.LoadPlayerData(slotNumber);
            }

            UIController.current.ToggleSaveMenu();
        }
    }
}

