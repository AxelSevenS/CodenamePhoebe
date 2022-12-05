using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public static class Global {


        public static Vector3 cameraDefaultPosition = new Vector3(1f, 0f, -3.5f);

        public static LayerMask GroundMask = LayerMask.GetMask("Default");
        public static LayerMask WaterMask = LayerMask.GetMask("Water");
        public static LayerMask EntityObjectMask = LayerMask.GetMask("EntityObject");

    }
}