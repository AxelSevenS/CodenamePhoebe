using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;


namespace SeleneGame.Core {

    public abstract class InstantiableAsset<T> : AddressableAsset<T> where T : InstantiableAsset<T> {

        public static T GetInstanceOf(T asset){
            T assetInstance = ScriptableObject.Instantiate( asset );
            assetInstance.name = assetInstance.name.Substring(0, assetInstance.name.Length - 7);
            return assetInstance;
        }
    

        public static T GetInstance(string assetName) {

            return GetInstanceOf( GetAsset(assetName) );
        }
        public static T GetDefaultInstance() {

            return GetInstanceOf( GetDefaultAsset() );
        }

        public static void GetInstanceAsync(string assetName, Action<T> callback) {
            GetAssetAsync(assetName, asset => {
                callback?.Invoke( GetInstanceOf(asset) );
            });
        }
        public static void GetDefaultInstanceAsync(Action<T> callback) {
            GetDefaultAssetAsync(asset => {
                callback?.Invoke( GetInstanceOf(asset) );
            });
        }

        public static void GetInstances(Action<T> callback) {

            GetAssets(asset => {
                callback?.Invoke( GetInstanceOf(asset) );
            });
        }

    }
}