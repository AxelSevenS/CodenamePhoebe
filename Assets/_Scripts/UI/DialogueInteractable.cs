using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame.UI {
    
    public class DialogueInteractable : MonoBehaviour, IInteractable{

        public string InteractDescription() => "Talk";
        public bool IsInteractable() => true;
        [SerializeField] private Dialogue dialogue;

        public void Interact(Entity entity){
            DialogueController.current.StartDialogue(dialogue, gameObject); 
        }
    }
}