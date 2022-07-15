using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public class EntityManager : Singleton<EntityManager> {

        public AnimationCurve evadeCurve;
        
        public RuntimeAnimatorController entityAnimatorController;
        
        public List<Entity> entityList = new List<Entity>();

        public Dictionary<string, string> entityCostumes = new Dictionary<string, string>();
        public Dictionary<string, string> weaponCostumes = new Dictionary<string, string>();

        public float stepHeight = 0.35f;

        private void OnEnable() {
            SetCurrent();
        }

        // public void SetCostume(string charName, string costumeName){
        //     GameEvents.SetEntityCostume(charName, costumeName);
        // }

        // public void SetCostumeSeleneBase(){
        //     GameEvents.SetEntityCostume("Selene", "SeleneBase");
        // }

    }
}