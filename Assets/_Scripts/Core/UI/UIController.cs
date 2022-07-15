using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public class UIController : MonoBehaviour{

        public static UIController current;

        private void OnEnable() { 
            if (current != null && current != this)
                Destroy(current);
            current = this;
        }

    }
}