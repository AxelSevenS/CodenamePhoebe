using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace SeleneGame.Core {

    public static class AddressablesUtils {


        public static T GetAsset<T>(string assetName) {

            foreach (var locator in Addressables.ResourceLocators) {
                if (locator.Locate(assetName, typeof(T), out var locations)) {
                    return Addressables.LoadAssetAsync<T>(locations[0]).WaitForCompletion();
                }
            }

            return GetDefaultAsset<T>();

        }
    
        public static T GetDefaultAsset<T>() {

            AsyncOperationHandle<T> opHandle = Addressables.LoadAssetAsync<T>( "default" );
            T asset = opHandle.WaitForCompletion();

            if (asset == null) {
                Debug.LogError($"Error getting default Asset");
                return default(T);
            }

            return asset;
        }
        
        public static void GetAssetAsync<T>(string assetName, Action<T> callback) {

            foreach (var locator in Addressables.ResourceLocators) {
                if (locator.Locate(assetName, typeof(T), out var locations)) {
                    AsyncOperationHandle<T> opHandle = Addressables.LoadAssetAsync<T>(locations[0]);
                    opHandle.Completed += operation => {
                        callback?.Invoke( operation.Result );
                    };
                    return;
                }
            }

            
            GetDefaultAssetAsync<T>(callback);
        }
        public static void GetDefaultAssetAsync<T>(Action<T> callback) {

            AsyncOperationHandle<T> opHandle = Addressables.LoadAssetAsync<T>( "default" );
            opHandle.Completed += operation => {
                if (operation.Status == AsyncOperationStatus.Failed) {
                    Debug.LogWarning($"Error getting default Asset");
                    return;
                }

                T asset = operation.Result;
                callback?.Invoke( operation.Result );
            };
        }

        public static T GetOverridableAsset<T>(string overrideName, string assetName) {

            string path = $"{overrideName}/{assetName}";

            foreach (var locator in Addressables.ResourceLocators) {
                if (locator.Locate(path, typeof(T), out var locations)) {
                    return Addressables.LoadAssetAsync<T>(locations[0]).WaitForCompletion();
                }
            }

            return GetDefaultOverridableAsset<T>(assetName);

        }
    
        public static T GetDefaultOverridableAsset<T>(string assetName) {

            string path = $"Default/{assetName}";

            AsyncOperationHandle<T> opHandle = Addressables.LoadAssetAsync<T>( path );
            T asset = opHandle.WaitForCompletion();

            if (asset == null) {
                Debug.LogError($"Error getting default Asset");
                return default(T);
            }

            return asset;
        }
        
        public static void GetOverridableAssetAsync<T>(string overrideName, string assetName, Action<T> callback) {

            string path = $"{overrideName}/{assetName}";

            foreach (var locator in Addressables.ResourceLocators) {
                if (locator.Locate(path, typeof(T), out var locations)) {
                    AsyncOperationHandle<T> opHandle = Addressables.LoadAssetAsync<T>(locations[0]);
                    opHandle.Completed += operation => {
                        callback?.Invoke( operation.Result );
                    };
                    return;
                }
            }

            
            GetDefaultOverridableAssetAsync<T>(assetName, callback);
        }
        public static void GetDefaultOverridableAssetAsync<T>(string assetName, Action<T> callback) {

            string path = $"Default/{assetName}";

            AsyncOperationHandle<T> opHandle = Addressables.LoadAssetAsync<T>( path );
            opHandle.Completed += operation => {
                if (operation.Status == AsyncOperationStatus.Failed) {
                    Debug.LogWarning($"Error getting default Asset");
                    return;
                }

                T asset = operation.Result;
                callback?.Invoke( operation.Result );
            };
        }

        public static void GetAssets<T>(Action<T> callback) {

            AsyncOperationHandle<IList<T>> opHandle = Addressables.LoadAssetsAsync<T>(
                typeof(T).Name,
                (asset) => {

                    callback?.Invoke( asset );

                }
            );
        }

        public static void GetAssets<T>(Action<T> callback, string key) {

            AsyncOperationHandle<IList<T>> opHandle = Addressables.LoadAssetsAsync<T>(
                key,
                (asset) => {

                    callback?.Invoke( asset );

                }
            );
        }

    }

}