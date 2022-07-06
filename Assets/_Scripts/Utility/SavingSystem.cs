using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SeleneGame.Core;
using SeleneGame.Entities;
using SeleneGame.Weapons;

namespace SeleneGame.Utility {

    public static class SavingSystem{
        
        public static System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

        public static void SavePlayerData(int slot){

            // Fancy Saving screen stuff here

            CreatePlayerData(slot);
            Debug.Log("Data Saved in Slot " + slot.ToString() + " at " + Application.persistentDataPath);
        }

        public static void LoadPlayerData(int slot){

            // Fancy Loading screen stuff here
            
            SaveData data = GetPlayerData(slot);

            if (data != null){
                data.Load();
                Debug.Log("Data Loaded from Slot " + slot.ToString());
            }else{
                Debug.Log("Data in Slot " + slot.ToString() + " could not be loaded.");
            }
        }

        public static void CreatePlayerData(int slot) {
            string path = Application.persistentDataPath + "/SaveDat" + slot.ToString() + ".dat";

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            SaveData data = new SaveData();

            formatter.Serialize(stream, data);
            stream.Close();
        }
        
        public static SaveData GetPlayerData(int slot) {
            string path = Application.persistentDataPath + "/SaveDat" + slot.ToString() + ".dat";

            if (File.Exists(path)){
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                SaveData data = (SaveData)formatter.Deserialize(stream);
                stream.Close();
                
                return data;
            }else{
                return null;
            }
        }

        public static void SaveControls(){
            var overrides = new Dictionary<System.Guid, string>();
            foreach (var map in Player.current.controls.actionMaps){
                foreach (var binding in map.bindings){
                    if (!string.IsNullOrEmpty(binding.overridePath))
                        overrides[binding.id] = binding.overridePath;
                }
            }
            string path = Application.persistentDataPath + "/controls.dat";

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, overrides);
            stream.Close();
        }

        public static void LoadControls(){

            string path = Application.persistentDataPath + "/controls.dat";

            if (File.Exists(path)){
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                var data = (Dictionary<System.Guid, string>)formatter.Deserialize(stream);
                stream.Close();

                foreach (var map in Player.current.controls.actionMaps){
                    var bindings = map.bindings;
                    for (var i = 0; i < bindings.Count; ++i){

                        if (data.TryGetValue(bindings[i].id, out var overridePath)){
                            map.ApplyBindingOverride(i, new InputBinding { overridePath = overridePath });
                        }
                    }
                }

                Debug.Log("Loaded Controls");

            }else{
                return;
            }
            
            Player.current.controls.Enable();
        }
    }


    [System.Serializable]
    public class SaveData {
        public string entityType = "SeleneGame.Core.Entity";
        public string entityDataPath = "Data/Entity/Selene";
        public string entityCostumePath = "Costume/Entity/SeleneBase";
        public float[] position = new float[3]{0f, 0f, 0f};
        public float[] rotation = new float[4]{0f, 0f, 0f, 0f};
        public float[] gravity = new float[3]{0f, -1f, 0f};

        public string[] weaponTypes = new string[0];
        public string[] weaponCostumePaths = new string[0];

        // public EntitySaveData entityData;
        // public float[] savePos = new float[3]{0f, 0f, 0f};
        // public float[] saveGravity = new float[3]{0f, -1f, 0f};
        // public string entityDataPath;
        // public string entityCostumePath;
        public int lastTimePlayed = 0;
        
        public void SaveEntityData(Entity entity){
            entityType = entity.GetType().FullName;
            entityDataPath = FileEditionUtility.GetPathToResource(entity.data);
            entityCostumePath = FileEditionUtility.GetPathToResource(entity.costume);
            position = new float[3]{entity.transform.position.x, entity.transform.position.y, entity.transform.position.z};
            rotation = new float[4]{entity.transform.rotation.x, entity.transform.rotation.y, entity.transform.rotation.z, entity.transform.rotation.w};
            gravity = new float[3]{entity.gravityDown.x, entity.gravityDown.y, entity.gravityDown.z};

            if (entity is ArmedEntity armed) {
                int weaponCount = armed.weapons.Length;
                weaponTypes = new string[weaponCount];
                weaponCostumePaths = new string[weaponCount];
                for (int i = 0; i < weaponCount; i++) {
                    weaponTypes[i] = armed.weapons[i]?.GetType().Name ?? "UnarmedWeapon";
                    weaponCostumePaths[i] = armed.weapons[i]?.costume.name ?? "UnarmedBase";
                }
            }
        }
        public Entity LoadEntityData(){
            Debug.Log(entityDataPath);

            Entity entity = Entity.CreateEntity(
                System.Type.GetType(entityType), 
                new Vector3(position[0], position[1], position[2]), 
                new Quaternion(rotation[0], rotation[1], rotation[2], rotation[3]), 
                Resources.Load<EntityData>(entityDataPath), 
                Resources.Load<EntityCostume>(entityCostumePath)
            );
            entity.gravityDown = new Vector3(gravity[0], gravity[1], gravity[2]);

            if (entity is ArmedEntity armed) {
                int weaponCount = armed.weapons.Length;
                for (int i = 0; i < weaponCount; i++) {
                    // armed.weapons.Set( i, System.Type.GetType(weaponTypes[i]) );
                    armed.weapons.Set( i, WeaponConstructor.CreateWeapon(weaponTypes[i]) );
                    armed.weapons[i].SetCostume( WeaponCostume.TryGetWeaponCostumeOrBase(armed.weapons[i], weaponCostumePaths[i]) );
                }
            }
            return entity;
        }

        public SaveData() {

            SaveEntityData(Player.current.entity);

            lastTimePlayed = (int)(System.DateTime.UtcNow - SavingSystem.epochStart).TotalSeconds;
        }

        public void Load() {
            
            Entity newEntity = LoadEntityData();
            Entity oldEntity = Player.current.entity;
            Player.current.entity = newEntity;
            Global.SafeDestroy(oldEntity.gameObject);

            Debug.Log( (int)(System.DateTime.UtcNow - SavingSystem.epochStart).TotalSeconds - lastTimePlayed + " Seconds have elapsed since last played on this Save." );
        }
    }
}