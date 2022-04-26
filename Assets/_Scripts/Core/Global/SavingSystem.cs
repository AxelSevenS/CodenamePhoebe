using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SeleneGame.Core {

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

        public float[] savePos = new float[3]{0f, 0f, 0f};
        public float[] saveGravity = new float[3]{0f, -1f, 0f};
        public Dictionary<string, string> entityCostumes = new Dictionary<string, string>();
        public Dictionary<string, string> weaponCostumes = new Dictionary<string, string>();
        public int lastTimePlayed = 0;
        
        public SaveData () {

            Vector3 savePosV = Player.current.entity._transform.position;
            savePos = new float[3]{savePosV.x, savePosV.y, savePosV.z};
            
            Vector3 saveGravityV = Player.current.entity.gravityDown;
            saveGravity = new float[3]{saveGravityV.x, saveGravityV.y, saveGravityV.z};

            entityCostumes = EntityManager.current.entityCostumes;

            weaponCostumes = EntityManager.current.weaponCostumes;

            lastTimePlayed = (int)(System.DateTime.UtcNow - SavingSystem.epochStart).TotalSeconds;
        }

        public void Load() {
            Player.current.entity._transform.position = new Vector3( savePos[0], savePos[1], savePos[2]);
            Player.current.entity.gravityDown = new Vector3( saveGravity[0], saveGravity[1], saveGravity[2]);

            // foreach(KeyValuePair<string, string> entry in entityCostumes){
            //     GameEvents.SetEntityCostume(entry.Key, entry.Value);
            // }

            // foreach(KeyValuePair<string, string> entry in weaponCostumes){
            //     GameEvents.SetWeaponCostume(entry.Key, entry.Value);
            // }

            Debug.Log( (int)(System.DateTime.UtcNow - SavingSystem.epochStart).TotalSeconds - lastTimePlayed + " Seconds have elapsed since last played on this Save." );
        }
    }
}