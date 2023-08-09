using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core.UI;

using Animancer;

using SevenGame.Utility;
using System;

namespace SeleneGame.Core {
    
    [Serializable]
    public sealed partial class GroundedBehaviour : EntityBehaviour {

        public override float GravityMultiplier => 1f;

        public override CameraController.CameraStyle CameraType => CameraController.CameraStyle.ThirdPersonGrounded;


        private bool HandleWaterHover() {
            if ( Entity.Character.Model.ColliderCast( Vector3.zero, Entity.gravityDown, out RaycastHit hit, out _, 0.15f, CollisionUtils.WaterMask ) ) {
                Entity.groundHit = hit;
                Entity.onGround.SetVal(true);
                return true;
            }

            return false;
        }



        public class Builder : Builder<GroundedBehaviour> {

            public static Builder Default => new ();

            protected override void ConfigureBehaviour(Context context) {
                base.ConfigureBehaviour(context);

                context.Behaviour.SetMoveBehaviour( GroundedMoveBehaviour.Builder.Default, context.PreviousBehaviour?.MoveBehaviour );
                context.Behaviour.SetEvadeBehaviour( GroundedEvadeBehaviour.Builder.Default, context.PreviousBehaviour?.EvadeBehaviour );
                context.Behaviour.SetJumpBehaviour( GroundedJumpBehaviour.Builder.Default, context.PreviousBehaviour?.JumpBehaviour );
            }
        }

    }
}