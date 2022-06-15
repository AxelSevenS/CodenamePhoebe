using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.States;

namespace SeleneGame.Core {
    
    [CreateAssetMenu(fileName = "new Entity Data", menuName = "Data/Entity")]
    public class EntityData : ScriptableObject, IData {

        public enum EntityType {
            Humanoid,
            Vehicle,
            Flying,
            Other
        }

        public static State TypeToDefaultState(EntityType type) {
            switch (type) {
                case EntityType.Humanoid:
                    return new WalkingState();
                case EntityType.Vehicle:
                    return new VehicleState();
                // case EntityType.Flying:
                //     return new FlyingState();
                //     // Need to create proper flying state.
                default:
                    return new WalkingState();
            }
        }

        public static string defaultData => "Selene";

        public string displayName;
        
        public EntityCostume costume;

        public System.Action onChangeCostume{ get; set;}

        public void SetCostume(string costumeName){
            costume = Resources.Load<EntityCostume>($"{DataGetter.GetDataPath<EntityData>()}/{name}/{costumeName}");
            onChangeCostume?.Invoke();
        }

        // public static EntityData GetData(string fileName){
        //     EntityData returnData = Resources.Load<EntityData>($"{GetDataPath}/{fileName}");

        //     if (returnData == null)
        //         returnData = Resources.Load<EntityData>($"{GetDataPath}/{defaultData}");
        //     return returnData;
        // }
        // public static string GetDataPath() => "Data/Entity";

        public EntityType entityType;
        public float maxHealth;
        public Vector3 size = new Vector3(1f, 1f, 1f);
        public float stepHeight = 1f;
        public float moveIncrement = 20f;
        public float weight = 15f;
        public float jumpHeight= 1f;

        [Header("Movement Speed")]
        public float baseSpeed = 1f;
        public float sprintSpeed = 1f;
        public float slowSpeed = 1f;
        public float swimSpeed;

        [Header("Evade")]
        public float evadeSpeed;
        public float evadeDuration;
        public float evadeCooldown;
        public float totalEvadeDuration => evadeDuration + evadeCooldown;
        public AnimationCurve evadeCurve;
        
    }
}