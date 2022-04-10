using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {
    
    public interface IManipulable{
        void Grab(Entity entity);
        void Grabbed(Entity entity);
        void Hold(Entity entity);
    }
}