using UnityEngine;
using SeleneGame.Core;
using SeleneGame.Entities;

using SeleneGame.UI;

namespace SeleneGame {
    
    public class DialogueInteractable : MonoBehaviour, IInteractable{

        public string interactionDescription{
            get => "Talk";
        }
        [SerializeField] private Dialogue dialogue;

        public void Interact(Entity entity){
            GameEvents.StartDialogue(dialogue, gameObject); 
        }
    }
}