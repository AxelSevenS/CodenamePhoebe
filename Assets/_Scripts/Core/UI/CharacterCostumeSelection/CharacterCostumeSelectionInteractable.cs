using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SeleneGame.Core.UI {

    public class CharacterCostumeSelectionInteractable : MonoBehaviour, IInteractable{


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
            CharacterCostumeSelectionMenuController.current.ReplaceCharacterCostume(entity.character);
        }
    }
}