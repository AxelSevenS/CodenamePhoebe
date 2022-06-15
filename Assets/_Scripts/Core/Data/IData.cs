using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SeleneGame.Core {

    public interface IData {

        System.Action onChangeCostume{ get; set;}

        void SetCostume(string costumeName);
    }
    
}