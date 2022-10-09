using System;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [System.Serializable]
    public abstract class NoGravityState : State {

        public sealed override float gravityMultiplier => 0f; 

        public sealed override Vector3 jumpDirection => Vector3.zero;
        public sealed override bool canJump => false;

    }
}