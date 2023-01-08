using UnityEngine;

namespace SeleneGame.Core.UI {
    
    public class DialogueInteractable : MonoBehaviour, IInteractable{

        [SerializeField] private DialogueLine dialogue;


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