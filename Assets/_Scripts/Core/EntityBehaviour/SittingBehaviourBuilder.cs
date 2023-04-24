using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    public class SittingBehaviourBuilder : EntityBehaviourBuilder<SittingBehaviour> {

        private Seat seat;
        private Seat.SittingPose sittingPose;

        public void SetSeat(Seat seat) {
            this.seat = seat;
        }
        public void SetPose(Seat.SittingPose sittingPose) {
            this.sittingPose = sittingPose;
        }

        public override SittingBehaviour Build(Entity entity, EntityBehaviour previousBehaviour) {
            return new SittingBehaviour(entity, previousBehaviour, seat, sittingPose);
        }
    }
}
