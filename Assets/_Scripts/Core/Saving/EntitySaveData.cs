using System.Reflection;
using System.Collections.Generic;

using UnityEngine;

using SevenGame.Utility;
using System;

namespace SeleneGame.Core {

    [System.Serializable]
    public class EntitySaveData {


        private const string ENTITY_DEFAULT_TYPE_NAME = "SeleneGame.Core.Entity";
        private const string CHARACTER_DEFAULT_TYPE_NAME = "SeleneGame.Content.SeleneCharacter";
        private const string CHARACTER_COSTUME_DEFAULT_NAME = "Base";

        public string entityTypeName = ENTITY_DEFAULT_TYPE_NAME;
        public string characterName = CHARACTER_DEFAULT_TYPE_NAME;
        public string characterCostumeName = CHARACTER_COSTUME_DEFAULT_NAME;
        public float[] position = new float[3]{0f, 0f, 0f};
        public float[] rotation = new float[4]{0f, 0f, 0f, 0f};
        public float[] gravity = new float[3]{0f, -1f, 0f};
        
        public WeaponSaveData[] weaponData = new WeaponSaveData[0];

        public void Save(Entity entity){
            System.Type entityType = entity?.GetType();
            entityTypeName = entityType?.AssemblyQualifiedName ?? ENTITY_DEFAULT_TYPE_NAME;

            System.Type characterType = entity?.Character?.GetType();
            characterName = characterType?.AssemblyQualifiedName ?? "";

            characterCostumeName = entity.Character?.Model?.costume?.name ?? "";
            
            position = new float[3]{entity.transform.position.x, entity.transform.position.y, entity.transform.position.z};
            rotation = new float[4]{entity.transform.rotation.x, entity.transform.rotation.y, entity.transform.rotation.z, entity.transform.rotation.w};
            gravity = new float[3]{entity.gravityDown.x, entity.gravityDown.y, entity.gravityDown.z};


            if ( typeof(ArmedEntity).IsAssignableFrom(entityType) ) {
                ArmedEntity armed = entity as ArmedEntity;

                int weaponCount = armed.Weapons.Count;
                weaponData = new WeaponSaveData[weaponCount];
                for (int i = 0; i < weaponCount; i++) {
                    weaponData[i] = new WeaponSaveData(armed.Weapons[i]);
                }
            }

        }

        public Entity Load(Entity oldEntity){
            System.Type entityType = Type.GetType(entityTypeName);
            Debug.Log(entityTypeName);
            Debug.Log(entityType);

            if (oldEntity != null) 
                GameUtility.SafeDestroy(oldEntity.gameObject);

                
            Entity entity = Entity.CreatePlayerEntity(
                entityType,
                new Vector3(position[0], position[1], position[2]), 
                new Quaternion(rotation[0], rotation[1], rotation[2], rotation[3]),
                AddressablesUtils.GetAsset<CharacterData>(characterName),
                AddressablesUtils.GetAsset<CharacterCostume>(characterCostumeName)
            );
            entity.gravityDown = new Vector3(gravity[0], gravity[1], gravity[2]);


            if ( typeof(ArmedEntity).IsAssignableFrom(entityType) ) {
                ArmedEntity armed = (ArmedEntity)entity;

                armed.ResetWeapons();
                for (int i = 0; i < armed.Weapons.Count; i++) {
                    
                    // We have to do this or the index will be wrong when the async operation is completed.
                    int currIndex = i;
                    WeaponSaveData currWeaponData = weaponData[currIndex];
                    
                    WeaponCostume costume = AddressablesUtils.GetAsset<WeaponCostume>(currWeaponData.costumeName);
                    armed.Weapons.Set(currIndex, AddressablesUtils.GetAsset<WeaponData>(currWeaponData.weaponName), costume);
                }
            }
            return entity;
        }

    }
}
