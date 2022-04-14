using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame {

    public class EventInteractable : MonoBehaviour, IInteractable{
        
        public string interactionText;
        public string interactionDescription{
            get => interactionText;
        }
        public List<InvokableEvent> interactEvents;

        public void Interact(Entity entity){
            foreach (InvokableEvent interactEvent in interactEvents){
                interactEvent.Invoke(gameObject);
            }
        }
    }
}