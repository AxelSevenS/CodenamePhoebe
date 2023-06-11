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

        public override SittingBehaviour Build(Entity entity, EntityBehaviour previousBehaviour, GameObject gameObject) {

            if (seat == null)
                throw new System.ArgumentNullException("Seat cannot be null!");

            bool wasEnabled = gameObject.activeSelf;

            gameObject.SetActive(false);

            SittingBehaviour behaviour = gameObject.AddComponent<SittingBehaviour>();
            behaviour.Initialize(entity, previousBehaviour);
            behaviour.SetSeat(seat);
            behaviour.SetPose(sittingPose);

            gameObject.SetActive(wasEnabled);

            return behaviour;
        }
    }
}
