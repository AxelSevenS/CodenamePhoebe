using System;

using UnityEngine;

using Animancer;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [Serializable]
    public sealed partial class GroundedEvadeBehaviour : EntityEvadeBehaviour {

        private int evadeCount = 1;

        public override bool CanEvade => base.CanEvade && evadeCount > 0;
        public override Vector3 DefaultDirection => Entity.Behaviour?.MoveBehaviour.Direction.sqrMagnitude == 0f ? -Entity.AbsoluteForward : Entity.AbsoluteForward;


        protected internal override void HandleInput(Player contoller) {
            if ( contoller.evadeInput.started ) {
                Evade();
            }
        }
        protected internal override void Evade(Vector3 direction = default) {
            if ( !CanEvade ) return;

            base.Evade(direction);

            evadeCount--;
                
            if (Entity.Behaviour.GravityMultiplier > 0f) {

                Vector3 newInertia = Entity.Inertia.NullifyInDirection( Entity.gravityDown );
                if (!Entity.onGround){
                    newInertia += -Entity.gravityDown.normalized * 5f;
                }
                Entity.Inertia = newInertia;
            }

        }

        internal void SetMaxEvadeCount(int count) {
            evadeCount = Math.Max(count, 1);
        }


        protected override void Update() {

            base.Update();

            if ( AirEvadeMixer.State != null )
                _airEvadeMixer.State.Parameter = animationDirection;

            if ( Entity.onGround ) {
                evadeCount = 1;
            }
        }



        public sealed class Builder : Builder<GroundedEvadeBehaviour> {

            public static readonly Builder Default = new ();

            private readonly int maxEvadeCount;

            public Builder(int maxEvadeCount = 1) : base() {
                this.maxEvadeCount = Math.Max(maxEvadeCount, 1);
            }

            protected override void ConfigureBehaviour(Context context) {
                base.ConfigureBehaviour(context);
                context.Behaviour.SetMaxEvadeCount(maxEvadeCount);
            }
        }
    }
}
