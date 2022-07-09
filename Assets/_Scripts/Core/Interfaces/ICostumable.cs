using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    public interface ICostumable<TCostume> where TCostume : Costume {

        // TCostume costume { get; }
        // GameObject model { get; }
        void SetCostume(TCostume newCostume);
        void LoadModel();
        void DestroyModel();
    }
}