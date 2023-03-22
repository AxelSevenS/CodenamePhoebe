using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    public class SittingBehaviourBuilder : EntityBehaviourBuilder<SittingBehaviour> {
        private Seat seat;

        public void SetSeat(Seat seat) {
            this.seat = seat;
        }

        public override SittingBehaviour Build(Entity entity, EntityBehaviour previousBehaviour) {
            return new SittingBehaviour(entity, previousBehaviour, seat);
        }
    }
}
