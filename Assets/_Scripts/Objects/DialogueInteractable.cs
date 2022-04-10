using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {
    
    public class DialogueInteractable : MonoBehaviour, IInteractable{

        public string interactionDescription{
            get => "Talk";
        }
        [SerializeField] private Conversation dialogue;

        public void Interact(Entity entity){
            GameEvents.StartDialogue(dialogue, gameObject); 
        }
    }
}