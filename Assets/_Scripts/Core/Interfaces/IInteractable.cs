using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public interface IInteractable {

        string InteractDescription{ get; }

        bool IsInteractable{ get; }

        void Interact(Entity entity);
    }
}