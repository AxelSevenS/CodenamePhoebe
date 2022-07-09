using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SeleneGame.Core {

    public class EntityManager : MonoBehaviour{
        
        public static EntityManager current;

        public AnimationCurve evadeCurve;
        
        public RuntimeAnimatorController entityAnimatorController;
        
        public List<Entity> entityList = new List<Entity>();

        public Dictionary<string, string> entityCostumes = new Dictionary<string, string>();
        public Dictionary<string, string> weaponCostumes = new Dictionary<string, string>();

        public float stepHeight = 0.35f;

        void OnEnable(){
            if (current != null)
                Destroy(current);
            current = this;
        }

        // public void SetCostume(string charName, string costumeName){
        //     GameEvents.SetEntityCostume(charName, costumeName);
        // }

        // public void SetCostumeSeleneBase(){
        //     GameEvents.SetEntityCostume("Selene", "SeleneBase");
        // }

    }
}