using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using SeleneGame.Utility;
using SeleneGame.Core;

namespace SeleneGame.UI {

    public class DialogueController : MonoBehaviour{


        public static DialogueController current;


        
        [SerializeField] private GameObject dialogueBox;

        [Space(15)]
        
        [SerializeField] private TextMeshProUGUI dialogueName;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Image dialoguePortrait;
        [SerializeField] private Image dialogueIndicator;

        [Space(15)]

        [SerializeField] private Dialogue dialogue;
        [SerializeField] private DialogueLine line;
        [SerializeField] private GameObject dialogueObject;


        [Space(15)]
        [SerializeField] private string displayText;
        public bool isActive; 
        private bool isDone => displayText.Length >= line.text.Length;
        private int dNumber;

        [SerializeField] private float currOpacity;
        private Image[] imageList;
        private TextMeshProUGUI[] textList;

        private void Awake(){
            current = this;
            imageList = dialogueBox.transform.GetComponentsInChildren<Image>();
            textList = dialogueBox.transform.GetComponentsInChildren<TextMeshProUGUI>();
        }
        
        private void Update(){

            if (Player.current.entity.lightAttackInput.started || Player.current.entity.heavyAttackInput.started || Player.current.entity.jumpInput.started)
                SkipDialogue();

            currOpacity = Mathf.MoveTowards(currOpacity, (isActive ? 1f : 0f), 7f * GameUtility.timeDelta);
            dialogueBox.SetActive(isActive);

            foreach (Image i in imageList){
                i.color = new Color(i.color.r, i.color.g, i.color.b, currOpacity);
            }
            foreach (TextMeshProUGUI i in textList){
                i.color = new Color(i.color.r, i.color.g, i.color.b, currOpacity);
            }

            
        }

        IEnumerator AdvanceText(){
            displayText = "";
            dialogueIndicator.enabled = false;

            for(int i = 0; i < line.text.Length; i++){
                if (isDone) {
                    dialogueIndicator.enabled = true;
                    yield break;
                }
                displayText += line.text[displayText.Length];
                dialogueText.SetText(displayText);
                yield return new WaitForSeconds(0.02f);
            }
        }

        public void StartDialogue(Dialogue newDialogue, GameObject newDialogueObject = null){
            StopCoroutine(AdvanceText());
            isActive = true;
            
            dialogueObject = newDialogueObject;
            dialogue = newDialogue;

            displayText = "";
            dNumber = 0;

            NextDialogue();
        }

        private void ExitDialogue(){
            StopCoroutine(AdvanceText());
            isActive = false;
        }

        private void NextDialogue(){
            if (dNumber < dialogue.lines.Length){
                line = dialogue.lines[dNumber];
                dNumber++;

                dialogueName.SetText(line.entity.name);
                UpdatePortrait();

                foreach (InvokableEvent dialogueEvent in line.dialogueEvents){
                    dialogueEvent.Invoke(dialogueObject);
                }
                StartCoroutine(AdvanceText());
            }else if (dialogueBox.activeSelf){
                ExitDialogue();
            }
        }

        private void SkipDialogue(){
            if (!dialogueBox.activeSelf) return;

            if (isDone)
                NextDialogue();
            else
                displayText = line.text;
        } 


        private void UpdatePortrait(){
            EntityCostume costume = line.entity.costume;
            Sprite? chosenPortrait = null;
            switch ((int)line.emotion) {
                default: chosenPortrait = costume.portrait; break;
                case 1: chosenPortrait = costume.determinedPortrait; break;
                case 2: chosenPortrait = costume.hesitantPortrait; break;
                case 3: chosenPortrait = costume.shockedPortrait; break;
                case 4: chosenPortrait = costume.disgustedPortrait; break;
                case 5: chosenPortrait = costume.sadPortrait; break;
                case 6: chosenPortrait = costume.happyPortrait; break;
            }

            if (chosenPortrait != null) dialoguePortrait.sprite = chosenPortrait;
        }

    }
}