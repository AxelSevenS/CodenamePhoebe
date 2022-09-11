using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public interface IInteractable {

        string InteractDescription{ get; set; }

        bool IsInteractable{ get; set; }

        void Interact(Entity entity);
    }
}