using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Entities;

namespace SeleneGame.Core {
    
    public interface IInteractable {

        string interactionDescription {
            get;
        }

        void Interact(Entity entity);
    }
}