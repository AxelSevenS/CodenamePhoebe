using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {
    
    public interface IInteractable {

        string interactionDescription {
            get;
        }

        void Interact(Entity entity);
    }
}