using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SeleneGame.Entities;
using SeleneGame.States;

namespace SeleneGame {

    public class SpeedlinesEffect : ObjectFollower {
        
        public Entity entity;
        [SerializeField] private ParticleSystem weakLines;
        [SerializeField] private ParticleSystem strongLines;

        // private bool weakEnabled => entity != null && entity.state is MaskedState && entity.sliding;
        // private bool strongEnabled => weakEnabled && entity.walkSpeed == Entity.WalkSpeed.sprint;

        public override void SetFollowedObject(GameObject newTarget) {
            followedObject = newTarget;
            if ( newTarget.TryGetComponent<Entity>(out Entity entityComponent) ){
                entity = entityComponent;
                name = entity.name + name.Replace("(Clone)", "");
            }
        }

        // Update is called once per frame
        void Update(){
            // if (weakEnabled) FollowObject();
            // weakLines.enableEmission = weakEnabled;
            // strongLines.enableEmission = strongEnabled;
        }
    }
}