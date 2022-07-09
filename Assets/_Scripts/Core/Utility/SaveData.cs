using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using SeleneGame.Utility;
using SeleneGame.Core;
using SeleneGame.Weapons;
using SeleneGame.UI;

namespace SeleneGame.Saving {


    [System.Serializable]
    public class SaveData {

        public EntitySaveData playerData = new EntitySaveData();

        // Game Progress Save Data


        // General Save Data

        public Dictionary<System.Guid, string> inputOverrides = new Dictionary<System.Guid, string>();
        public uint timeOfLastSave = 0;
        public uint totalTimePlayed = 0;


        public System.DateTime GetTimeOfLastSave(){
            return SavingSystem.epochStart.AddSeconds(timeOfLastSave);
        }
        public System.TimeSpan GetTotalPlaytime(){
            return System.TimeSpan.FromSeconds(totalTimePlayed);
        }

        public void Save() {

            SaveControls();

            playerData.SaveEntityData(Player.current.entity);

            // Saving the time of the last save
            timeOfLastSave = (uint)SavingSystem.timeSinceEpochStart.TotalSeconds;

            // Calculating play time of this session.
            uint sessionPlayTime = (uint)SavingSystem.timeSinceEpochStart.TotalSeconds - (uint)(SavingSystem.loadedTime - SavingSystem.epochStart).TotalSeconds;
            totalTimePlayed += sessionPlayTime;
            Debug.Log( $"{sessionPlayTime} Seconds were added to the play time." );

            // Update the Time of loading this save file so "session playtime" gets reset and isn't added multiple times.
            SavingSystem.loadedTime = System.DateTime.UtcNow;
        }

        public void Load() {
            
            LoadControls();

            Player.current.entity = playerData.LoadEntityData(Player.current.entity);

            // Calculating the time since the last save
            uint timeSinceLastSave = (uint)SavingSystem.timeSinceEpochStart.TotalSeconds - timeOfLastSave;
            Debug.Log( $"{timeSinceLastSave} Seconds have elapsed since the file was saved." );
            Debug.Log( $"The date of the last save was {GetTimeOfLastSave()}." );
            Debug.Log( $"Total Playtime is {GetTotalPlaytime()}." );

            // Time of loading this save file is stored temporarily to calculate play time of this session.
            SavingSystem.loadedTime = System.DateTime.UtcNow;
        }

        public void SaveControls() {
            inputOverrides.Clear();
            // Save Input Overrides
            foreach (var map in Player.current.controls.actionMaps){
                foreach (var binding in map.bindings){
                    if (!string.IsNullOrEmpty(binding.overridePath))
                        inputOverrides[binding.id] = binding.overridePath;
                }
            }
        }
        public void LoadControls() {
            // Load Input Overrides
            foreach (var map in Player.current.controls.actionMaps){
                var bindings = map.bindings;
                for (int i = 0; i < bindings.Count; ++i){

                    if (inputOverrides.TryGetValue(bindings[i].id, out string overridePath)){
                        map.ApplyBindingOverride(i, new InputBinding { overridePath = overridePath });
                    }
                }
            }
            KeyBindingController.current.UpdateKeyBindings();
            Player.current.controls.Enable();
        }

    }



    [System.Serializable]
    public class EntitySaveData {

        // Entity Save Data
        public string entityTypeName = "SeleneGame.Core.Entity";
        public string entityDataPath = "Data/Entity/Selene";
        public string entityCostumePath = "Costume/Entity/SeleneBase";
        public float[] position = new float[3]{0f, 0f, 0f};
        public float[] rotation = new float[4]{0f, 0f, 0f, 0f};
        public float[] gravity = new float[3]{0f, -1f, 0f};

        // Weapon Save Data
        public string[] weaponTypeNames = new string[0];
        public string[] weaponCostumePaths = new string[0];

        public void SaveEntityData(Entity entity){
            entityTypeName = entity.GetType().FullName;
            // entityDataPath = FileEditionUtility.GetPathToResource(entity.data);
            entityCostumePath = entity?.costume.name ?? "SeleneBase";
            position = new float[3]{entity.transform.position.x, entity.transform.position.y, entity.transform.position.z};
            rotation = new float[4]{entity.transform.rotation.x, entity.transform.rotation.y, entity.transform.rotation.z, entity.transform.rotation.w};
            gravity = new float[3]{entity.gravityDown.x, entity.gravityDown.y, entity.gravityDown.z};

            if (entity is ArmedEntity armed) {
                int weaponCount = armed.weapons.Length;
                weaponTypeNames = new string[weaponCount];
                weaponCostumePaths = new string[weaponCount];
                for (int i = 0; i < weaponCount; i++) {
                    weaponTypeNames[i] = armed.weapons[i]?.GetType().Name ?? "UnarmedWeapon";
                    weaponCostumePaths[i] = armed.weapons[i]?.costume.name ?? "UnarmedBase";
                }
            }
        }
        public Entity LoadEntityData(Entity oldEntity){
            Debug.Log(entityDataPath);
            System.Type entityType = System.Type.GetType(entityTypeName);

            Entity entity = Entity.CreateEntity(
                entityType, 
                new Vector3(position[0], position[1], position[2]), 
                new Quaternion(rotation[0], rotation[1], rotation[2], rotation[3]), 
                // Resources.Load<EntityData>(entityDataPath), 
                EntityCostume.TryGetEntityCostumeOrBase(entityType, entityCostumePath)
            );
            entity.gravityDown = new Vector3(gravity[0], gravity[1], gravity[2]);

            if (entity is ArmedEntity armed) {
                int weaponCount = armed.weapons.Length;
                for (int i = 0; i < weaponCount; i++) {
                    System.Type weaponType = System.Type.GetType(weaponTypeNames[i]);

                    armed.weapons.Set( i, WeaponConstructor.CreateWeapon(weaponTypeNames[i]) );
                    armed.weapons[i].SetCostume( WeaponCostume.TryGetWeaponCostumeOrBase(weaponType, weaponCostumePaths[i]) );
                }
            }
            GameUtility.SafeDestroy(oldEntity.gameObject);
            return entity;
        }
    }
}
