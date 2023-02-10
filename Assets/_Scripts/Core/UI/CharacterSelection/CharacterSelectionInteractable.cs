using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SeleneGame.Core.UI {

    public class CharacterSelectionInteractable : MonoBehaviour, IInteractable{


        public string InteractDescription {
            get {
                return "Open Loadout";
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
            CharacterSelectionMenuController.current.ReplaceCharacter(entity);
        }
    }
}