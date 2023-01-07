using System;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SevenGame.Utility;
using System.Threading;

namespace SeleneGame.Core.UI {

    public class DialogueController : UI<DialogueController> {


        
        [SerializeField] private GameObject dialogueBox;

        [Space(15)]
        
        [SerializeField] private TextMeshProUGUI dialogueName;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Image dialoguePortrait;
        [SerializeField] private Image dialogueIndicator;

        // [SerializeField] private float alpha;
        // private Image[] sprites;

        // private Task dialogueOpacityTask;

        [Space(15)]

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private Task lineTextTask;
        private string displayedText;

        [SerializeField] private Dialogue dialogue;
        [SerializeField] private Dialogue.Line currentLine;
        [SerializeField] private GameObject dialogueObject;

        private int lineIndex;



        private bool isTyping => displayedText.Length < currentLine.text.Length;


        public override void Enable(){
            
            if (Enabled) return;

            Enabled = true;
            dialogueBox.SetActive( true );

            // TODO : create a fade in animation 
            // dialogueOpacityTask = FadeDialogue(1f);

            displayedText = "";
            lineIndex = 0;
        }

        public async override void Disable(){

            if (!Enabled) return;
            
            // dialogueOpacityTask = FadeDialogue(0);
            // await dialogueOpacityTask;

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

            if (lineTextTask != null && !lineTextTask.IsCompleted)
                cancellationTokenSource.Cancel();

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
        }

        private void NextLine(){
            if (!dialogueBox.activeSelf || !Enabled) return;


            if (lineIndex < dialogue?.lines.Length) {

                currentLine = dialogue.lines[lineIndex];
                DisplayLineText(currentLine);
                lineIndex++;

            } else  {
                EndDialogue();
            }
        }

        private void EndLine(){
            displayedText = currentLine?.text ?? String.Empty;
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
        
        private void Update(){
            if (!Enabled) return;

            dialogueText.SetText( displayedText );
            dialogueIndicator.enabled = !isTyping;


            if (PlayerEntityController.current == null) return;

            if (
                PlayerEntityController.current.lightAttackInput.started || 
                PlayerEntityController.current.heavyAttackInput.started || 
                PlayerEntityController.current.jumpInput.started
            )
                SkipLine();

        }

    }
}