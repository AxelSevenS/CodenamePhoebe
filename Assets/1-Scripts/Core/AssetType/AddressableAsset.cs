using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;


namespace SeleneGame.Core {

    public abstract class AddressableAsset<T> : ScriptableObject where T : AddressableAsset<T> {

        [SerializeField] private bool _accessibleInGame = true;

        public static T defaultAsset { get; private set; }



        public bool accessibleInGame => _accessibleInGame;
        
        

        public static string GetPath(string assetName){
            return $"{typeof(T).Name}/{assetName}";
        }


        public static T GetAsset(string assetName) {
            // Get Requested Asset
            AsyncOperationHandle<T> opHandle = Addressables.LoadAssetAsync<T>( GetPath(assetName) );

            T result = opHandle.WaitForCompletion();

            // If not found, get Default Asset
            if (result == null || !result._accessibleInGame) {
                Debug.LogWarning($"Error getting Asset {assetName}");
                return GetDefaultAsset();
            }

            return result;
        }
        public static T GetDefaultAsset() {
            if (defaultAsset != null) 
                return defaultAsset;

            AsyncOperationHandle<T> opHandle = Addressables.LoadAssetAsync<T>( "default" );
            defaultAsset = opHandle.WaitForCompletion();

            if (defaultAsset == null) {
                Debug.LogError($"Error getting default Asset");
                return null;
            }

            return defaultAsset;
        }
        
        public static void GetAssetAsync(string assetName, Action<T> callback) {
            // Get Requested Asset
            AsyncOperationHandle<T> opHandle = Addressables.LoadAssetAsync<T>( GetPath(assetName) );
            opHandle.Completed += operation => {

                // If not found, get Default Asset
                if (operation.Status == AsyncOperationStatus.Failed) {
                    Debug.LogWarning($"Error getting Asset {assetName}");
                    GetDefaultAssetAsync(callback);
                    return;
                }

                callback?.Invoke( operation.Result );
            };
        }
        public static void GetDefaultAssetAsync(Action<T> callback) {
            if (defaultAsset != null) {
                callback?.Invoke( defaultAsset );
                return;
            }

            AsyncOperationHandle<T> opHandle = Addressables.LoadAssetAsync<T>( "default" );
            opHandle.Completed += operation => {
                if (operation.Status == AsyncOperationStatus.Failed) {
                    Debug.LogWarning($"Error getting default Asset");
                    return;
                }

                defaultAsset = operation.Result;
                callback?.Invoke( operation.Result );
            };
        }

        public static void GetAssets(Action<T> callback) {

            AsyncOperationHandle<IList<T>> opHandle = Addressables.LoadAssetsAsync<T>(
                typeof(T).Name,
                (asset) => {

                    callback?.Invoke( asset );

                }
            );
        }

    }
}