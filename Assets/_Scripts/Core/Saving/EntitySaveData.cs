using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {

    [System.Serializable]
    public class EntitySaveData {

        public string entityTypeName = "SeleneGame.Core.Entity";
        public string characterName = "Selene";
        public string characterCostumeName = "Base";
        public float[] position = new float[3]{0f, 0f, 0f};
        public float[] rotation = new float[4]{0f, 0f, 0f, 0f};
        public float[] gravity = new float[3]{0f, -1f, 0f};
        
        public WeaponSaveData[] weaponData = new WeaponSaveData[0];

        public void Save(Entity entity){
            System.Type entityType = entity.GetType();
            entityTypeName = entityType.AssemblyQualifiedName;
            characterName = entity.character.name ?? "Selene";
            characterCostumeName = entity.character?.costume?.name ?? "Base";
            position = new float[3]{entity.transform.position.x, entity.transform.position.y, entity.transform.position.z};
            rotation = new float[4]{entity.rotation.x, entity.rotation.y, entity.rotation.z, entity.rotation.w};
            gravity = new float[3]{entity.gravityDown.x, entity.gravityDown.y, entity.gravityDown.z};


            if ( typeof(ArmedEntity).IsAssignableFrom(entityType) ) {
                ArmedEntity armed = entity as ArmedEntity;

                int weaponCount = armed.weapons.Count;
                weaponData = new WeaponSaveData[weaponCount];
                for (int i = 0; i < weaponCount; i++) {
                    weaponData[i] = new WeaponSaveData(armed.weapons[i]);
                }
            }

        }

        public Entity Load(Entity oldEntity){
            System.Type entityType = System.Type.GetType(entityTypeName);
            Debug.Log(entityTypeName);
            Debug.Log(entityType);

            if (oldEntity != null) 
                GameUtility.SafeDestroy(oldEntity.gameObject);
                
            Entity entity = Entity.CreatePlayerEntity(
                entityType,
                Character.GetInstance(characterName),
                new Vector3(position[0], position[1], position[2]), 
                new Quaternion(rotation[0], rotation[1], rotation[2], rotation[3]), 
                CharacterCostume.GetAsset(characterCostumeName)
            );
            entity.gravityDown = new Vector3(gravity[0], gravity[1], gravity[2]);


            if ( typeof(ArmedEntity).IsAssignableFrom(entityType) ) {
                ArmedEntity armed = (ArmedEntity)entity;

                armed.ResetWeapons();
                for (int i = 0; i < armed.weapons.Count; i++) {
                    
                    // We have to do this or the index will be wrong when the async operation is completed.
                    int currIndex = i;
                    WeaponSaveData currWeaponData = weaponData[currIndex];
                    
                    Weapon.GetInstanceAsync(currWeaponData.name, (weapon) => {
                        WeaponCostume.GetAssetAsync(currWeaponData.costumeName, costume => {
                            Debug.Log(costume);
                            armed.weapons.Set( currIndex, weapon, costume );
                        });
                    });
                }
            }
            return entity;
        }

    }
}
