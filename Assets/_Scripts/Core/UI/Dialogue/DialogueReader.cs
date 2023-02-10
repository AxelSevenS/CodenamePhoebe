using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SeleneGame.Core.UI {

    public abstract class DialogueReader<T> : UI<DialogueReader<T>>, IDialogueReader where T : DialogueReader<T> {
        

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

        [SerializeField] protected DialogueLine currentLine;
        [SerializeField] protected GameObject dialogueObject;

        private bool lineWasChanged = false;


        public bool isTyping => displayedText.Length < currentLine.text.Length;



        public override void Enable(){
            Reset();

            if (Enabled) return;
            
            if (UIController.currentDialogueReader != null)
                UIController.currentDialogueReader.EndDialogue();

            dialogueBox.SetActive( true );
            Enabled = true;
            UIController.currentDialogueReader = this;
        }

        public override void Disable(){
            Reset();

            if (!Enabled) return;

            UIController.currentDialogueReader = null;

            dialogueBox.SetActive( false );
            Enabled = false;
        }


        public virtual void StartDialogue(IDialogueSource source, GameObject newDialogueObject = null){
            Enable();
            dialogueObject = newDialogueObject;
            currentLine = source.GetDialogue();

            // TODO : create a fade in animation 
            // dialogueOpacityTask = FadeDialogue(1f);
            DisplayLineText();
        }

        public virtual void EndDialogue(){
            EndLine();

            // dialogueOpacityTask = FadeDialogue(0);
            // await dialogueOpacityTask;
            Disable();
        }

        public virtual void SkipToLine(IDialogueSource source) {
            currentLine = source.GetDialogue();
            lineWasChanged = true;
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

        public virtual void InterruptDialogue(){
            foreach (GameEvent interruptionEvent in currentLine.interruptionEvents) {
                interruptionEvent.Invoke(dialogueObject);
            }
            EndDialogue();
        }

        private void DisplayLineText(){
            lineWasChanged = false;
            foreach (GameEvent gameEvent in currentLine.gameEvents){
                if (gameEvent.Evaluate())
                    gameEvent.Invoke(dialogueObject);

                if (lineWasChanged) {
                    lineWasChanged = false;
                    DisplayLineText();
                    return;
                }
            }

            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            dialogueName.SetText( currentLine.entity.name );
            dialoguePortrait.sprite = currentLine.entity.character.model.costume.GetPortrait(currentLine.emotion);
            displayedText = String.Empty;
            dialogueText.SetText(displayedText);
            
            lineTextTask = Task.Run(() => ProcessDialogueText(currentLine), cancellationTokenSource.Token);

        }

        private async Task ProcessDialogueText(DialogueLine line) {
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


            if (currentLine.nextLine != null) {

                currentLine = currentLine.nextLine.GetDialogue();
                DisplayLineText();

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
            currentLine = null;
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
