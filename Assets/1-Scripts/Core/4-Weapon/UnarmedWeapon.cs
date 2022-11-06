using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    [CreateAssetMenu(fileName = "Unarmed", menuName = "Weapon/Unarmed")]
    public sealed class UnarmedWeapon : Weapon{

        public override void LoadModel() {;}
        public override void UnloadModel() {;}
        

        public override void Display() {;}
        public override void Hide() {;}

    }
}