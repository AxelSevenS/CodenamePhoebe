using System;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

using TMPro;

using SevenGame.UI;
using Scribe;

namespace SeleneGame.Core.UI {

    public abstract class DialogueReader<T> : UIObject<DialogueReader<T>>, IDialogueReader where T : DialogueReader<T> {
        

        [SerializeField] private GameObject dialogueBox;

        [Space(15)]
        
        [SerializeField] private TextMeshProUGUI dialogueName;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Image dialoguePortrait;
        [SerializeField] private string displayText;

        // [SerializeField] private float alpha;
        // private Image[] sprites;

        // private Task dialogueOpacityTask;

        [Space(15)]
        [SerializeField] protected DialogueLine currentLine;
        [SerializeField] protected GameObject dialogueObject;

        private float timeOfLastCharacter = 0f;


        public bool isTyping => dialogueText.text.Length < (displayText.Length);



        public void StartDialogue(DialogueLine newDialogue, GameObject newDialogueObject = null){

            Enable();

            dialogueObject = newDialogueObject;
            currentLine = newDialogue;
            displayText = currentLine.localizedText.GetLocalizedString();

            foreach (GameEvent gameEvent in currentLine.gameEvents){
                if (gameEvent.Evaluate())
                    gameEvent.Invoke(dialogueObject);
            }

            dialogueName.SetText( currentLine.entity.name );
            dialoguePortrait.sprite = currentLine.entity.character.model.costume.GetPortrait(currentLine.emotion);
            dialogueText.SetText(String.Empty);
            timeOfLastCharacter = Time.unscaledTime;

        }

        public virtual void EndDialogue(){
            Disable();
        }

        // public virtual void SkipToLine(DialogueSource source) {
        //     currentLine = source?.GetDialogue();
        // }

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
            currentLine.SetFlag(DialogueFlag.Interrupted);
            
            EndDialogue();
        }

        protected virtual void NextLine(){
            if (!dialogueBox.activeSelf) return;


            if (currentLine?.nextLine != null) {

                StartDialogue(currentLine.nextLine.GetDialogue(), dialogueObject);

            } else {
                EndLine();
                EndDialogue();
            }
        }

        protected virtual void EndLine(){
            dialogueText.text = displayText;
        }

        protected virtual void SkipLine(){
            if (!dialogueBox.activeSelf) return;

            if ( isTyping ) {
                EndLine();
            } else{
                NextLine();
            }
        }


        public override void Enable() {
            Reset();

            if (Enabled) return;

            base.Enable();

            dialogueBox.SetActive( true );
            UIController.currentDialogueReader = this;
        }

        public override void Disable() {
            Reset();

            if (!Enabled) return;

            base.Disable();

            dialogueBox.SetActive( false );
            UIController.currentDialogueReader = null;
        }
        

        protected virtual void Reset() {
            dialogueName?.SetText(String.Empty);
            dialogueText?.SetText(String.Empty);
            dialoguePortrait.sprite = null;
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
            if (!Enabled || currentLine == null) return;

            int displayedTextLength = dialogueText.text.Length;
            const float textDelta = 20 * 0.001f;
            bool writingTag = false;

            if (Time.unscaledTime >= timeOfLastCharacter + textDelta && displayedTextLength < displayText.Length) {

                do {

                    char nextCharacter = displayText[displayedTextLength];

                    switch (nextCharacter) {
                        case '<':
                            writingTag = true;
                            break;
                        case '>':
                            writingTag = false;
                            break;
                    }

                    displayedTextLength++;
                    
                } while ( writingTag && displayedTextLength < displayText.Length );
                
                // TODO : play character sound effect

                dialogueText.text = displayText.Substring(0, displayedTextLength);

                timeOfLastCharacter += textDelta;
            }
        }
    }
}
