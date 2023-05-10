using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core.UI {
    public interface IUIModal : IUIMenu {

        IUIMenu previousModal { get; }
    }
}
