using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {
    
    [CreateAssetMenu(fileName = "new Entity Costume", menuName = "Costume/Entity")]
    public class EntityCostume : UnitCostume {

        public Sprite determinedPortrait;
        public Sprite hesitantPortrait;
        public Sprite shockedPortrait;
        public Sprite disgustedPortrait;
        public Sprite sadPortrait;
        public Sprite happyPortrait;
        public List<StringPair> limbsPaths = new List<StringPair>();
        
    }
}