using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Entities;

namespace SeleneGame {

    public class EventInteractable : MonoBehaviour, IInteractable{
        
        [SerializeField] private string interactionText;
        public string InteractDescription() => interactionText;
        public bool IsInteractable() => true;
        public List<InvokableEvent> interactEvents;

        public void Interact(Entity entity){
            foreach (InvokableEvent interactEvent in interactEvents){
                interactEvent.Invoke(gameObject);
            }
        }
    }
}