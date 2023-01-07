using System;
using System.Collections;

using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] private Dialogue.Line currentLine;
        [SerializeField] private GameObject dialogueObject;


        [Space(15)]
        private int lineIndex;

        [SerializeField] private float alpha;
        private SpriteRenderer[] sprites;


        private bool isTyping => dialogueText.text.Length < currentLine.text.Length;


        public override void Enable(){
            
            if (Enabled) return;

            Enabled = true;
            dialogueBox.SetActive( true );

            dialogueText.text = "";
            lineIndex = 0;
        }

        public override void Disable(){

            if (!Enabled) return;
            
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
            EndLine();

            dialogueObject = null;
            dialogue = null;

            Disable();
        }

        private IEnumerator AdvanceText(){
            dialogueText.text = "";
            dialogueIndicator.enabled = false;

            float time = 0.02f;
            bool writingTag = false;

            while (isTyping){

                do {
                    char nextCharacter = currentLine.text[dialogueText.text.Length];
                    
                    switch (nextCharacter) {
                        case '<':
                            writingTag = true;
                            break;
                        case '>':
                            writingTag = false;
                            break;
                    }

                    dialogueText.text += nextCharacter;
                } 
                while (isTyping && writingTag); // write the whole rich text tag

                yield return new WaitForSeconds(time);
            }

            EndLine();
        }

        private void NextLine(){
            if (!dialogueBox.activeSelf) return;

            EndLine();

            if (lineIndex < dialogue.lines.Length) {
                currentLine = dialogue.lines[lineIndex];

                dialogueName.SetText( currentLine.entity.name );
                dialoguePortrait.sprite = currentLine.entity.character.costume.GetPortrait(currentLine.emotion);

                foreach (InvokableEvent dialogueEvent in currentLine.dialogueEvents){
                    dialogueEvent.Invoke(dialogueObject);
                }
                StartCoroutine(AdvanceText());
                lineIndex++;
            } else  {
                EndDialogue();
            }
        }

        private void EndLine(){
            dialogueText.text = currentLine.text;
            dialogueIndicator.enabled = true;
            StopCoroutine(AdvanceText());
        }

        private void SkipLine(){
            if (!dialogueBox.activeSelf) return;

            if ( isTyping ) {
                EndLine();
            } else{
                NextLine();
            }
        }



        private void Awake(){
            sprites = dialogueBox.transform.GetComponentsInChildren<SpriteRenderer>();
        }

        protected override void OnEnable(){
            base.OnEnable();
            EndDialogue();
        }

        protected override void OnDisable(){
            base.OnDisable();
            EndDialogue();
        }
        
        private void Update(){
            if (!Enabled) return;

            alpha = Mathf.MoveTowards(alpha, (Enabled ? 1f : 0f), 7f * GameUtility.timeDelta);

            Color newColor;
            foreach(SpriteRenderer child in sprites) {
                newColor = child.color;
                newColor.a = alpha;
                child.color = newColor;
            }


            if (PlayerEntityController.current == null) return;

            if (PlayerEntityController.current.lightAttackInput.started || PlayerEntityController.current.heavyAttackInput.started || PlayerEntityController.current.jumpInput.started)
                SkipLine();

        }

    }
}