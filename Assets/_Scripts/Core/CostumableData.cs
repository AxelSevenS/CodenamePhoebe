using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SeleneGame.Core {

    [Serializable]
    public abstract class CostumableData<TCostume> : ScriptableObject where TCostume : Costume {

        public string displayName;

        public string description;

        [SerializeField] protected TCostume _baseCostume;



        public TCostume baseCostume {
            get => _baseCostume;
        }


        
        private string GetAnimationPath(string assetName) => @$"Animations/{name}/{assetName}";
        private string GetDefaultAnimationPath(string assetName) => @$"Animations/Default/{assetName}";
        
        public AnimationClip GetAnimation(string assetName) {
            
            AnimationClip result = null;

            string path = GetAnimationPath(assetName);

            foreach (var locator in Addressables.ResourceLocators) {
                if (locator.Locate(path, typeof(AnimationClip), out var locations)) {
                    // Debug.Log($"Getting Animation {path}");
                    result = Addressables.LoadAssetAsync<AnimationClip>(locations[0]).WaitForCompletion();
                    break;
                }
            }

            // If not found, get Default Asset
            if (result == null) {
                // Debug.Log(message: $"Couldn't find Asset {path}");
                return GetDefaultAnimation(assetName);
            }

            return result;
        }
        public AnimationClip GetDefaultAnimation(string assetName) {

            string path = GetDefaultAnimationPath(assetName);

            Debug.Log($"Getting Default Animation {path}");

            AnimationClip defaultAnim = null;
            try {
                AsyncOperationHandle<AnimationClip> opHandle = Addressables.LoadAssetAsync<AnimationClip>( path );
                defaultAnim = opHandle.WaitForCompletion();
            } catch {
                Debug.LogError($"Error getting default Asset {path}");
                return null;
            }

            return defaultAnim;
        }
    }
}
