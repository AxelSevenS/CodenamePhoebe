using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    public class ArmedGroundedBehaviour : GroundedBehaviour {

        [SerializeField] private ArmedEntity _armedEntity;

        // public ArmedGroundedBehaviour(ArmedEntity entity, EntityBehaviour previousBehaviour) : base(entity, previousBehaviour) {
        //     _armedEntity = entity;
        // }

        public override void Initialize(Entity entity, EntityBehaviour previousBehaviour) {
            _armedEntity = entity as ArmedEntity;

            base.Initialize(entity, previousBehaviour);
        }

        protected internal override void HandleInput(Player controller) {
            base.HandleInput(controller);
            
            _armedEntity.weapons.HandleInput(controller);
        }

        protected internal override void HandleAI(AIController controller) {
            base.HandleAI(controller);

            // TODO : If the AI is in attack mode, evaluate the weapon's different attack options and chose a favorable one. 

            // Action attackAction = _armedEntity.weapons.EvaluateAttack();
        }
    }

}
