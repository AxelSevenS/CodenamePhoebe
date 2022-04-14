using UnityEngine;
using SeleneGame.Core;
using SeleneGame.Entities;

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