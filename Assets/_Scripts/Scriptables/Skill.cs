using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {
    
    [CreateAssetMenu(fileName = "new Skill", menuName = "Skill")]
    public class Skill : ScriptableObject {

        public string description;
        public Sprite image;
        public int cost;
        
    }
}