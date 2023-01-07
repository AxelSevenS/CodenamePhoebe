using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;

using SeleneGame.Core;

using SevenGame.Utility;

namespace SeleneGame.Core.UI {
    
    public abstract class UIPausedMenu<T> : UIMenu<T>, IUIPausedMenu where T : UIMenu<T> {
        

    }

}