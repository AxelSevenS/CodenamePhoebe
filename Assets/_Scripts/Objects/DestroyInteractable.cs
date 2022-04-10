using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {

    public class DestroyInteractable : MonoBehaviour, IInteractable{
        
        public string interactionDescription {
            get => "Destroy";
        }

        public void Interact(Entity entity){
            Destroy(this.gameObject);
        }
    }
}