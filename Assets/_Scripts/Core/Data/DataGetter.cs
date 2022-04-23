using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame.Core {

    public static class DataGetter {

        public static TData GetData<TData>(string fileName) where TData : ScriptableObject, IData {

            string dataPath = GetDataPath<TData>();
            TData returnData = Resources.Load<TData>($"{dataPath}/{fileName}");

            if (returnData != null) return returnData;

            Debug.LogError($"{fileName} not found, using default data instead");

            string defaultData = GetDefaultData<TData>();
            returnData = Resources.Load<TData>($"{dataPath}/{defaultData}");

            if (returnData != null) return returnData;

            throw new System.Exception($"default data of type {typeof(TData).Name} not found");
        }

        public static string GetDataPath<TData>() where TData : ScriptableObject, IData {
            return "Data/" + typeof(TData).Name.Replace("Data", "");
        }

        public static string GetDefaultData<TData>() where TData : ScriptableObject, IData => typeof(TData).GetPropertyValue<string>("defaultData");
    }
}
