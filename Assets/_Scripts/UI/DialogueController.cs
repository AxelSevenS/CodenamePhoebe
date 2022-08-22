using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using SeleneGame.Core;

using SevenGame.Utility;

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

        private void OnEnable() {
            if (current != null && current != this)
                Destroy(current);
            current = this;
        }

        private void Awake(){
            imageList = dialogueBox.transform.GetComponentsInChildren<Image>();
            textList = dialogueBox.transform.GetComponentsInChildren<TextMeshProUGUI>();
        }
        
        private void Update(){

            currOpacity = Mathf.MoveTowards(currOpacity, (isActive ? 1f : 0f), 7f * GameUtility.timeDelta);
            dialogueBox.SetActive(isActive);

            foreach (Image i in imageList){
                i.color = new Color(i.color.r, i.color.g, i.color.b, currOpacity);
            }
            foreach (TextMeshProUGUI i in textList){
                i.color = new Color(i.color.r, i.color.g, i.color.b, currOpacity);
            }
            

            if (PlayerEntityController.current == null) return;

            if (PlayerEntityController.current.lightAttackInput.started || PlayerEntityController.current.heavyAttackInput.started || PlayerEntityController.current.jumpInput.started)
                SkipLine();

            
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

            NextLine();
        }

        private void EndDialogue(){
            StopCoroutine(AdvanceText());
            isActive = false;
        }

        private void NextLine(){
            if (!dialogueBox.activeSelf) return;

            if (dNumber < dialogue.lines.Length){
                line = dialogue.lines[dNumber];
                dNumber++;

                dialogueName.SetText( line.entity.name );
                dialoguePortrait.sprite = line.entity.character.costume.GetPortrait(line.emotion);

                foreach (InvokableEvent dialogueEvent in line.dialogueEvents){
                    dialogueEvent.Invoke(dialogueObject);
                }
                StartCoroutine(AdvanceText());
            }else 
                EndDialogue();
        }

        private void EndLine(){
            displayText = line.text;
            dialogueText.SetText(displayText);
        }

        private void SkipLine(){
            if (!dialogueBox.activeSelf) return;

            if (isDone)
                NextLine();
            else
                EndLine();
        }

    }
}