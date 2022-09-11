using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame.UI {
    
    public class DialogueInteractable : MonoBehaviour, IInteractable{

        [SerializeField] private Dialogue dialogue;


        public string InteractDescription {
            get {
                return "Talk";
            }
            set {;}
        }

        public bool IsInteractable {
            get {
                return true;
            }
            set {;}
        }


        public void Interact(Entity entity){
            DialogueController.current.StartDialogue(dialogue, gameObject); 
        }
    }
}