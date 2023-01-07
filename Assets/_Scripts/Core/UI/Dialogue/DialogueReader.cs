using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SeleneGame.Core.UI {

    public abstract class DialogueReader<T> : UI<DialogueReader<T>> where T : DialogueReader<T> {
        

        [SerializeField] private GameObject dialogueBox;

        [Space(15)]
        
        [SerializeField] private TextMeshProUGUI dialogueName;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Image dialoguePortrait;

        // [SerializeField] private float alpha;
        // private Image[] sprites;

        // private Task dialogueOpacityTask;

        [Space(15)]

        private CancellationTokenSource cancellationTokenSource;
        private Task lineTextTask;
        private string displayedText;

        [SerializeField] protected Dialogue dialogue;
        [SerializeField] protected Dialogue.Line currentLine;
        [SerializeField] protected GameObject dialogueObject;

        protected int lineIndex;



        public bool isTyping => displayedText.Length < currentLine.text.Length;



        public override void Enable(){
            
            if (Enabled) return;

            Reset();
            dialogueBox.SetActive( true );
            Enabled = true;
        }

        public override void Disable(){

            if (!Enabled) return;

            dialogueBox.SetActive( false );
            Enabled = false;
            Reset();
        }


        public virtual void StartDialogue(Dialogue newDialogue, GameObject newDialogueObject = null){
            Enable();
            dialogueObject = newDialogueObject;
            dialogue = newDialogue;

            // TODO : create a fade in animation 
            // dialogueOpacityTask = FadeDialogue(1f);
            NextLine();
        }

        public virtual void EndDialogue(){
            EndLine();

            // dialogueOpacityTask = FadeDialogue(0);
            // await dialogueOpacityTask;
            Disable();
        }

        // private async Task FadeDialogue(float alpha){
        //     while (this.alpha != alpha){
        //         this.alpha = Mathf.MoveTowards(this.alpha, alpha, 0.16f);
        //         Color newColor;
        //         foreach(Image child in sprites) {
        //             newColor = child.color;
        //             newColor.a = this.alpha;
        //             child.color = newColor;
        //         }
        //         await Task.Delay(10);
        //     }
        // }

        private void DisplayLineText(Dialogue.Line line){

            // if (lineTextTask != null && !lineTextTask.IsCompleted) {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();
            // }

            dialogueName.SetText( line.entity.name );
            dialoguePortrait.sprite = line.entity.character.costume.GetPortrait(line.emotion);
            displayedText = String.Empty;
            dialogueText.SetText(displayedText);

            foreach (InvokableEvent dialogueEvent in line.dialogueEvents){
                dialogueEvent.Invoke(dialogueObject);
            }
            
            lineTextTask = Task.Run(() => ProcessDialogueText(line), cancellationTokenSource.Token);

        }

        private async Task ProcessDialogueText(Dialogue.Line line) {
            int displayedTextLength = 0;
            int time = 20;
            bool writingTag = false;

            try {
                while (displayedText.Length < line.text.Length) {

                    char nextCharacter = line.text[displayedTextLength];

                    switch (nextCharacter) {
                        case '<':
                            writingTag = true;
                            break;
                        case '>':
                            writingTag = false;
                            break;
                    }

                    displayedTextLength++;

                    if (writingTag) continue; // Don't Delay or change the displayed text when writing a tag
                    
                    // TODO : play character sound effect

                    displayedText = line.text.Substring(0, displayedTextLength);
                    await Task.Delay(time);
                }
            }
            catch(TaskCanceledException) {
                // Task was cancelled, don't do anything
            }
            finally {
                EndLine();
            }
        }

        protected virtual void NextLine(){
            if (!dialogueBox.activeSelf) return;


            if (lineIndex < dialogue?.lines.Length) {

                currentLine = dialogue.lines[lineIndex];
                DisplayLineText(currentLine);
                lineIndex++;

            } else  {
                EndDialogue();
            }
        }

        protected virtual void EndLine(){
            displayedText = currentLine?.text ?? String.Empty;
        }

        protected virtual void SkipLine(){
            if (!dialogueBox.activeSelf) return;

            if ( isTyping ) {
                EndLine();
            } else{
                NextLine();
            }
        }

        protected virtual void Reset() {
            dialogueBox?.SetActive(false);
            dialogueName?.SetText(String.Empty);
            dialogueText?.SetText(String.Empty);
            dialoguePortrait.sprite = null;
            displayedText = String.Empty;
            dialogue = null;
            currentLine = null;
            lineIndex = 0;
        }


        protected virtual void Awake(){
            // sprites = dialogueBox.transform.GetComponentsInChildren<Image>();
        }

        protected override void OnEnable(){
            base.OnEnable();
            EndDialogue();
        }

        protected override void OnDisable(){
            base.OnDisable();
            EndDialogue();
        }

        protected virtual void Update() {
            if (!Enabled) return;

            dialogueText.SetText(displayedText);
        }
    }
}
