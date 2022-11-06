using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

using SevenGame.Utility;

namespace SeleneGame.Core.UI {

    public class DialogueController : UI<DialogueController> {


        
        [SerializeField] private GameObject dialogueBox;

        [Space(15)]
        
        [SerializeField] private TextMeshProUGUI dialogueName;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Image dialoguePortrait;
        [SerializeField] private Image dialogueIndicator;

        [Space(15)]

        [SerializeField] private Dialogue dialogue;
        [SerializeField] private Dialogue.Line line;
        [SerializeField] private GameObject dialogueObject;


        [Space(15)]
        [SerializeField] private string displayText;
        private int lineIndex;

        [SerializeField] private float currOpacity;
        private Image[] imageList;
        private TextMeshProUGUI[] textList;


        private bool isTyping => displayText.Length < line.text.Length;


        public override void Enable(){
            
            if (Enabled) return;
            Enabled = true;

            StopCoroutine(AdvanceText());
            dialogueBox.SetActive( true );

            displayText = "";
            lineIndex = 0;
        }

        public override void Disable(){

            if (!Enabled) return;
            Enabled = false;

            StopCoroutine(AdvanceText());
            Enabled = false;
            dialogueBox.SetActive( false );
        }


        public void StartDialogue(Dialogue newDialogue, GameObject newDialogueObject = null){
            Enable();
            
            dialogueObject = newDialogueObject;
            dialogue = newDialogue;

            NextLine();
        }

        private void EndDialogue(){
            Disable();
        }


        private void UpdateOpacity(){

            currOpacity = Mathf.MoveTowards(currOpacity, (Enabled ? 1f : 0f), 7f * GameUtility.timeDelta);

            foreach (Image i in imageList)
                i.color = new Color(i.color.r, i.color.g, i.color.b, currOpacity);

            foreach (TextMeshProUGUI i in textList)
                i.color = new Color(i.color.r, i.color.g, i.color.b, currOpacity);
        }

        IEnumerator AdvanceText(){
            displayText = "";
            dialogueIndicator.enabled = false;

            for(int i = 0; i < line.text.Length; i++){
                if ( !isTyping ) {
                    dialogueIndicator.enabled = true;
                    yield break;
                }
                displayText += line.text[displayText.Length];
                dialogueText.SetText(displayText);
                yield return new WaitForSeconds(0.02f);
            }
        }

        private void NextLine(){
            if (!dialogueBox.activeSelf) return;

            if (lineIndex < dialogue.lines.Length) {
                line = dialogue.lines[lineIndex];
                lineIndex++;

                dialogueName.SetText( line.entity.name );
                dialoguePortrait.sprite = line.entity.character.costume.GetPortrait(line.emotion);

                foreach (InvokableEvent dialogueEvent in line.dialogueEvents){
                    dialogueEvent.Invoke(dialogueObject);
                }
                StartCoroutine(AdvanceText());
            } else 
                EndDialogue();
        }

        private void EndLine(){
            displayText = line.text;
            dialogueText.SetText(displayText);
        }

        private void SkipLine(){
            if (!dialogueBox.activeSelf) return;

            if ( isTyping )
                EndLine();
            else
                NextLine();
        }



        private void Awake(){
            imageList = dialogueBox.transform.GetComponentsInChildren<Image>();
            textList = dialogueBox.transform.GetComponentsInChildren<TextMeshProUGUI>();
        }
        
        private void Update(){

            UpdateOpacity();

            if (PlayerEntityController.current == null) return;

            if (PlayerEntityController.current.lightAttackInput.started || PlayerEntityController.current.heavyAttackInput.started || PlayerEntityController.current.jumpInput.started)
                SkipLine();

        }

    }
}