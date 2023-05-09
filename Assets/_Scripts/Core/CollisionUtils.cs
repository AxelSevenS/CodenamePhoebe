using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public static class CollisionUtils {


        // public static Vector3 cameraDefaultPosition = new Vector3(1f, 0f, -3.5f);

        public static readonly LayerMask GroundLayer = LayerMask.NameToLayer("Default");
        public static readonly LayerMask WaterLayer = LayerMask.NameToLayer("Water");
        public static readonly LayerMask EntityObjectLayer = LayerMask.NameToLayer("EntityObject");

        public static readonly LayerMask GroundMask = LayerMask.GetMask("Default");
        public static readonly LayerMask WaterMask = LayerMask.GetMask("Water");
        public static readonly LayerMask EntityObjectMask = LayerMask.GetMask("EntityObject");

        public static readonly LayerMask EntityCollisionMask = GroundMask;

    }
}