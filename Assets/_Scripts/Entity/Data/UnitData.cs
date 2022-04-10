using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SeleneGame {
    
    public abstract class UnitData : ScriptableObject {

        public static TData GetDataByName<TData>(string fileName) where TData : UnitData {
            var type = typeof(TData);
            string dataPath = GetDataPath(type);

            var returnVal = Resources.Load<TData>($"{dataPath}/{fileName}");

            if (returnVal != null) return returnVal;
            
            Debug.LogError($"A {type.ToString()} named {fileName} does not exist, make sure you spelled it right or it is implemented correctly.");

            string defaultData = GetDefaultData(type);
            Debug.Log(type.ToString() + "  :  " + $"{dataPath}/{defaultData}");
            return Resources.Load<TData>($"{dataPath}/{defaultData}");
        }

        protected static string GetDataPath(System.Type type){
            string folder = type.Name.Replace("Data", "");
            return $"Data/{folder}";
        }

        protected static string GetDefaultData(System.Type type){
            if (type is EntityData){
                return "Selene";
            }else if (type is WeaponData){
                return "Unarmed";
            }
            return "Null";
        }
        
    }

    public abstract class UnitData<TCostume> : UnitData where TCostume : UnitCostume{

        public string displayName;
        
        public TCostume costume;

        public System.Action changeCostume;

        public void SetCostume(string costumeName){
            string dataPath = GetDataPath(this.GetType());
            costume = (Resources.Load<TCostume>($"{dataPath}/{name}/{costumeName}"));
            if (changeCostume != null) changeCostume();
        }
    }
}