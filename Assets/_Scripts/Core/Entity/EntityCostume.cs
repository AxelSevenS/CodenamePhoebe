using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {
    
    [CreateAssetMenu(fileName = "new Entity Costume", menuName = "Costume/Entity")]
    public class EntityCostume : Costume {

        public Sprite determinedPortrait;
        public Sprite hesitantPortrait;
        public Sprite shockedPortrait;
        public Sprite disgustedPortrait;
        public Sprite sadPortrait;
        public Sprite happyPortrait;

        public static EntityCostume GetEntityBaseCostume(Entity entity){
            string defaultCostume = entity.GetType().Name.Replace("Entity", "Base");
            return GetEntityCostume(defaultCostume);
        }
        public static EntityCostume TryGetEntityCostumeOrBase(Entity entity, string costumeName) {
            string path = $"Costume/Entity/{costumeName}";
            EntityCostume costume = Resources.Load<EntityCostume>(path);
            if (costume == null) {
                string baseCostumeName = entity.GetType().Name.Replace("Entity", "Base");
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