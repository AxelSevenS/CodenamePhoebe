using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SeleneGame.Core {

    public interface IData {

        event System.Action onChangeCostume;

        void SetCostume(string costumeName);
    }
    
}