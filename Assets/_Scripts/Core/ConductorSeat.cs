using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EasySplines;

namespace SeleneGame.Core {

    public class ConductorSeat : EntitySeat {

        [SerializeField] private FollowSpline _splineFollower;
        [SerializeField] private FollowSpline.MovementDirection previousDirection = FollowSpline.MovementDirection.None;

        public override bool IsInteractable => true; 

        protected override string seatedInteractionText {
            get {
                if (previousDirection == FollowSpline.MovementDirection.None)
                    return "Stop Moving";

                return "Start Moving";
            }
        } 


        protected override void SeatedInteract(Entity entity){
            FollowSpline.MovementDirection oldDirection = previousDirection;
            previousDirection = _splineFollower.movementDirection;
            _splineFollower.movementDirection = oldDirection;
        }

        private void Start() {
            previousDirection = _splineFollower.movementDirection == FollowSpline.MovementDirection.None ? FollowSpline.MovementDirection.Forward : FollowSpline.MovementDirection.None;
        }

    }

}
