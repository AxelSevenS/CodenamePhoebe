using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    public class FloatingEvadeBehaviour : EntityEvadeBehaviour {
        
        public sealed class Builder : Builder<FloatingEvadeBehaviour> {

            public static readonly Builder Default = new ();
        }
    }
}
