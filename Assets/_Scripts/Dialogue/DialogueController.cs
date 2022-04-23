using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using SeleneGame.Core;

namespace SeleneGame {

    public class DialogueController : MonoBehaviour{
        
        [SerializeField] private GameObject dialogueBox;

        [Space(15)]
        
        [SerializeField] private TextMeshProUGUI dialogueName;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Image dialoguePortrait;
        [SerializeField] private Image dialogueIndicator;

        [Space(15)]

        [SerializeField] private Conversation conversation;
        [SerializeField] private Dialogue dialogue;
        [SerializeField] private GameObject dialogueObject;

        [Space(15)]

        [SerializeField] private bool isDone;
        [SerializeField] private string displayText;
        private int dNumber;

        [SerializeField] private float opacity, currOpacity;
        private Image[] imageList;
        private TextMeshProUGUI[] textList;

        void OnEnable(){
            GameEvents.onPlayerInput += SkipDialogue; 
            GameEvents.onStartDialogue += (newDialogue, newDialogueObject) => {
                dialogueObject = newDialogueObject;
                StartConversation(newDialogue);
            };
        }

        void OnDisable(){
            GameEvents.onPlayerInput -= SkipDialogue; 
            GameEvents.onStartDialogue -= (newDialogue, newDialogueObject) => {
                dialogueObject = newDialogueObject;
                StartConversation(newDialogue);
            };
        }

        void Awake(){
            imageList = dialogueBox.transform.GetComponentsInChildren<Image>();
            textList = dialogueBox.transform.GetComponentsInChildren<TextMeshProUGUI>();
        }
        
        void Update(){
            currOpacity = Mathf.MoveTowards(currOpacity, opacity, 7f * Global.timeDelta);

            foreach (Image i in imageList){
                i.color = new Color(i.color.r, i.color.g, i.color.b, currOpacity);
            }
            foreach (TextMeshProUGUI i in textList){
                i.color = new Color(i.color.r, i.color.g, i.color.b, currOpacity);
            }

            dialogueBox.SetActive(currOpacity != 0f);

            if (dialogueBox.activeSelf){
                dialogueName.SetText(dialogue.entityData.name);
                UpdatePortrait();
                dialogueIndicator.enabled = isDone && dialogueBox.activeSelf;
                dialogueText.SetText(displayText);
                isDone = (displayText.Length >= dialogue.text.Length);
            }
            
        }

        private void UpdatePortrait(){
            EntityCostume costume = dialogue.entityData.costume;
            Sprite[] portraits = new Sprite[7]{costume.portrait, costume.determinedPortrait, costume.hesitantPortrait, costume.shockedPortrait, costume.disgustedPortrait, costume.sadPortrait, costume.happyPortrait};
            dialoguePortrait.sprite = portraits[ (int)dialogue.emotion ];
        }

        public void StartConversation(Conversation newConversation){
            Player.current.talking = true;
            conversation = newConversation;
            dNumber = 0;
            NextDialogue();
            opacity = 1f;
        }

        private void ExitConversation(){
            Player.current.talking = false;
            opacity = 0;
        }


        IEnumerator AdvanceText(){
            isDone = false;
            displayText = "";
            for(int i = 0; i < dialogue.text.Length; i++){
                if (isDone) yield break;
                displayText += dialogue.text[i];
                yield return new WaitForSeconds(0.02f);
            }
        }

        private void NextDialogue(){
            if (dNumber < conversation.dialogues.Length){
                dialogue = conversation.dialogues[dNumber];
                dNumber += 1;
                foreach (InvokableEvent dialogueEvent in dialogue.dialogueEvents){
                    dialogueEvent.Invoke(dialogueObject);
                }
                StartCoroutine("AdvanceText");
            }else if (dialogueBox.activeSelf){
                ExitConversation();
            }
        }

        private void SkipDialogue(){
            if (!dialogueBox.activeSelf) return;

            if (isDone){
                NextDialogue();
            }else{
                isDone = true;
                displayText = dialogue.text;
            }
        } 
    }
}