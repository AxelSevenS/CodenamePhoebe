using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public sealed class SwimmingBehaviour : EntityBehaviour {

        public const float swimTreshold = 1f;
        public const float surfaceTreshold = 2f;


        public sealed override float GravityMultiplier => 0f;



        private void Update() {

            Entity.Inertia *= 0.95f;

        }

        public sealed class Builder : Builder<SwimmingBehaviour> {

            public static readonly Builder Default = new();

            protected override void ConfigureBehaviour(Context context) {
                base.ConfigureBehaviour(context);
                context.Entity.gravityDown = Vector3.down;

                context.Behaviour.SetMoveBehaviour( FloatingMoveBehaviour.Builder.Default, context.PreviousBehaviour?.MoveBehaviour );
                context.Behaviour.SetEvadeBehaviour( FloatingEvadeBehaviour.Builder.Default, context.PreviousBehaviour?.EvadeBehaviour );
            }
        }
    }
}