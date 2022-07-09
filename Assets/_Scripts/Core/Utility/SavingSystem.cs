using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using SeleneGame.Core;

namespace SeleneGame.Saving {

    public static class SavingSystem{
        
        public static System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        public static SaveData loadedData;
        public static System.DateTime loadedTime = System.DateTime.UtcNow;
        public static System.TimeSpan timeSinceEpochStart => (System.DateTime.UtcNow - epochStart);

        public static void SavePlayerData(uint slot){

            SaveData data = loadedData == null ? new SaveData() : loadedData;
            data.Save();

            SaveDataToFile(data, slot);
            Debug.Log($"Data Saved in Slot {slot} at {Application.persistentDataPath}");
        }

        public static void LoadPlayerData(uint slot){
            
            loadedData = LoadDataFromFile(slot);

            if (loadedData != null){
                loadedData.Load();
                Debug.Log($"Data Loaded from Slot {slot}");
            }else{
                Debug.Log($"Data in Slot {slot} could not be loaded.");
            }
        }

        public static void SaveDataToFile(SaveData data, uint slot) {
            string path = $"{Application.persistentDataPath}/SaveDat{slot}.dat";

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, data);
            stream.Close();
        }
        
        public static SaveData LoadDataFromFile(uint slot) {
            string path = $"{Application.persistentDataPath}/SaveDat{slot}.dat";

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

        // public static void SaveControlsToFile(){
        //     Dictionary<System.Guid, string> overrides = new Dictionary<System.Guid, string>();
        //     foreach (var map in Player.current.controls.actionMaps){
        //         foreach (var binding in map.bindings){
        //             if (!string.IsNullOrEmpty(binding.overridePath))
        //                 overrides[binding.id] = binding.overridePath;
        //         }
        //     }
        //     string path = $"{Application.persistentDataPath}/controls.dat";

        //     BinaryFormatter formatter = new BinaryFormatter();
        //     FileStream stream = new FileStream(path, FileMode.Create);

        //     formatter.Serialize(stream, overrides);
        //     stream.Close();
        // }

        // public static void LoadControlsFromFile(){

        //     return;

        //     string path = $"{Application.persistentDataPath}/controls.dat";

        //     if (File.Exists(path)){
        //         BinaryFormatter formatter = new BinaryFormatter();
        //         FileStream stream = new FileStream(path, FileMode.Open);

        //         Dictionary<System.Guid, string> data = (Dictionary<System.Guid, string>)formatter.Deserialize(stream);
        //         stream.Close();

        //         foreach (var map in Player.current.controls.actionMaps){
        //             var bindings = map.bindings;
        //             for (int i = 0; i < bindings.Count; ++i){

        //                 if (data.TryGetValue(bindings[i].id, out string overridePath)){
        //                     map.ApplyBindingOverride(i, new InputBinding { overridePath = overridePath });
        //                 }
        //             }
        //         }

        //         Debug.Log("Loaded Controls");

        //     }else{
        //         return;
        //     }
            
        //     Player.current.controls.Enable();
        // }
    }

}