using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using SeleneGame.Core;

namespace SeleneGame.UI {

    public class WeaponInventoryInteractable : MonoBehaviour, IInteractable{


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
            if (entity is ArmedEntity armed)
                WeaponInventoryMenuController.current.OpenInventory(armed);
        }
    }
}