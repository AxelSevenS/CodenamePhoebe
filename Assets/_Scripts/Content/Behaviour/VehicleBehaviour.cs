using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SeleneGame.Core;
using SevenGame.Utility;

namespace SeleneGame.Content {
    
    [System.Serializable]
    public class VehicleBehaviour : EntityBehaviour {



        public override float GravityMultiplier => 1f;
        public override CameraController.CameraStyle CameraType => CameraController.CameraStyle.ThirdPersonGrounded;


        public class Builder : Builder<VehicleBehaviour> {

            public readonly static Builder Default = new();

            protected override void ConfigureBehaviour(Context context) {
                base.ConfigureBehaviour(context);

                context.Behaviour.SetMoveBehaviour(VehicleMoveBehaviour.Builder.Default, context.PreviousBehaviour?.MoveBehaviour);
                context.Behaviour.SetJumpBehaviour(GroundedJumpBehaviour.Builder.Default, context.PreviousBehaviour?.JumpBehaviour);
            }
        }
    }
}
