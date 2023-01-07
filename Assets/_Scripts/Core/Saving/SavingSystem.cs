using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// #if UNITY_EDITOR

// using UnityEditor;

// #endif

namespace SeleneGame.Core {

    public class SavingSystem<T> where T : SaveData, new() {

        public static T loadedData;




        // #if UNITY_EDITOR

        //     [MenuItem("Saving/Save to Slot 1")]
        //     public static void SaveToSlot1() {
        //         SavePlayerData(1);
        //     }
        //     [MenuItem("Saving/Save to Slot 2")]
        //     public static void SaveToSlot2() {
        //         SavePlayerData(2);
        //     }
        //     [MenuItem("Saving/Save to Slot 3")]
        //     public static void SaveToSlot3() {
        //         SavePlayerData(3);
        //     }

        //     [MenuItem("Loading/Load from Slot 1")]
        //     public static void LoadFromSlot1() {
        //         LoadPlayerData(1);
        //     }
        //     [MenuItem("Loading/Load from Slot 2")]
        //     public static void LoadFromSlot2() {
        //         LoadPlayerData(2);
        //     }
        //     [MenuItem("Loading/Load from Slot 3")]
        //     public static void LoadFromSlot3() {
        //         LoadPlayerData(3);
        //     }

        // #endif

        public static void SavePlayerData(uint slot){

            T data = loadedData == null ? new T() : loadedData;
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
        public static void LoadPlayerData(T loadedData){

            if (loadedData != null){
                loadedData.Load();
                Debug.Log($"Data Loaded");
            }else{
                Debug.Log($"Data could not be loaded.");
            }
        }



        public static void SaveDataToFile(T data, uint slot) {
            string path = $"{Application.persistentDataPath}/SaveDat{slot}.dat";

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, data);
            stream.Close();
        }
        
        public static T LoadDataFromFile(uint slot) {
            string path = $"{Application.persistentDataPath}/SaveDat{slot}.dat";

            if (File.Exists(path)){
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(path, FileMode.Open);

                try {
                    return (T)formatter.Deserialize(stream);
                } catch {
                    return null;
                } finally {
                    stream.Close();
                }
                
            } else {
                return null;
            }
        }
    }

}