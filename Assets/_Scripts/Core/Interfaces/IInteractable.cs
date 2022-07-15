using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public interface IInteractable {

        string InteractDescription();

        bool IsInteractable();

        void Interact(Entity entity);
    }
}