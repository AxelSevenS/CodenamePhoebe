using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    [CreateAssetMenu(fileName = "new Entity Costume", menuName = "Costume/Entity")]
    public class EntityCostume : Costume {

        public enum Emotion {neutral, determined, hesitant, shocked, disgusted, sad, happy};

        public Sprite determinedPortrait;
        public Sprite hesitantPortrait;
        public Sprite shockedPortrait;
        public Sprite disgustedPortrait;
        public Sprite sadPortrait;
        public Sprite happyPortrait;

        public Sprite GetPortrait(Emotion emotion) {
            switch (emotion) {
                default: return portrait;
                case Emotion.determined: return determinedPortrait;
                case Emotion.hesitant: return hesitantPortrait;
                case Emotion.shocked: return shockedPortrait;
                case Emotion.disgusted: return disgustedPortrait;
                case Emotion.sad: return sadPortrait;
                case Emotion.happy: return happyPortrait;
            }
        }

        public static EntityCostume GetEntityBaseCostume(string entityTypeName){
            string defaultCostume = entityTypeName.Replace("Entity", "Base");
            return GetEntityCostume(defaultCostume);
        }
        public static EntityCostume GetEntityBaseCostume(System.Type entityType){
            string defaultCostume = entityType.Name.Replace("Entity", "Base");
            return GetEntityCostume(defaultCostume);
        }
        public static EntityCostume TryGetEntityCostumeOrBase(System.Type entityType, string costumeName) {
            string path = $"Costume/Entity/{costumeName}";
            EntityCostume costume = Resources.Load<EntityCostume>(path);
            if (costume == null) {
                string baseCostumeName = entityType.Name.Replace("Entity", "Base");
                string basePath = $"Costume/Entity/{baseCostumeName}";
                Debug.LogError($"No Costume was found at Path {path} ; Using entity base costume at Path {basePath}");
                costume = GetEntityCostume(baseCostumeName);
            }
            return costume;
        }
        public static EntityCostume GetEntityCostume(string costumeName) {
            string path = $"Costume/Entity/{costumeName}";
            EntityCostume costume = Resources.Load<EntityCostume>(path);
            if (costume == null) {
                const string defaultPath = "Costume/Entity/SeleneBase";
                Debug.LogError($"No Costume was found at Path {path} ; Using default costume at Path {defaultPath}");
                costume = Resources.Load<EntityCostume>(defaultPath);
            }
            return costume;
        }
        
    }
}