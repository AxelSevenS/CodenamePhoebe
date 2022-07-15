using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame {
    
    public class DialogueInteractable : MonoBehaviour, IInteractable{

        public string InteractDescription() => "Talk";
        public bool IsInteractable() => true;
        [SerializeField] private Dialogue dialogue;

        public void Interact(Entity entity){
            GameEvents.StartDialogue(dialogue, gameObject); 
        }
    }
}