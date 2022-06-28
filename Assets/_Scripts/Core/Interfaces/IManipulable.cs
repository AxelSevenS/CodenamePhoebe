using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public interface IManipulable{
        void Grab();
        void Grabbed();
        void Hold();
    }
}