using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    public interface IGrabbable {

        bool isGrabbable { get; }
        Transform grabTransform { get; }

        void Grab();
        void Throw(Vector3 direction);

    }
}
