using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    public interface IPaintable {
        
        void Paint(Vector3 position, float radius, Color color);
    }
}
