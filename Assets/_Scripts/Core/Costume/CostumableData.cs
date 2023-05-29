using System;
using System.Collections.Generic;
using Animancer;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace SeleneGame.Core {

    [Serializable]
    public abstract class CostumableData<TCostume> : ScriptableObject where TCostume : Costume {

        public LocalizedString displayName;

        public LocalizedString description;

        [SerializeField] protected TCostume _baseCostume;



        public TCostume baseCostume {
            get => _baseCostume;
        }


        
        public AnimancerTransitionAssetBase GetTransition(string assetName) {
            return AddressablesUtils.GetOverridableAsset<AnimancerTransitionAssetBase>(name, assetName);
        }
        public AnimancerTransitionAssetBase GetDefaultTransition(string assetName) {
            return AddressablesUtils.GetDefaultOverridableAsset<AnimancerTransitionAssetBase>(assetName);
        }

        public void GetTransitionAsync(string assetName, Action<AnimancerTransitionAssetBase> callback) {
            AddressablesUtils.GetOverridableAssetAsync<AnimancerTransitionAssetBase>(name, assetName, callback);
        }
        public void GetDefaultTransitionAsync(string assetName, Action<AnimancerTransitionAssetBase> callback) {
            AddressablesUtils.GetDefaultOverridableAssetAsync<AnimancerTransitionAssetBase>(assetName, callback);
        }

        public AnimationClip GetAnimation(string assetName) {
            return AddressablesUtils.GetOverridableAsset<AnimationClip>(name, assetName);
        }
        public AnimationClip GetDefaultAnimation(string assetName) {
            return AddressablesUtils.GetDefaultOverridableAsset<AnimationClip>(assetName);
        }

        public void GetAnimationAsync(string assetName, Action<AnimationClip> callback) {
            AddressablesUtils.GetOverridableAssetAsync<AnimationClip>(name, assetName, callback);
        }
        public void GetDefaultAnimationAsync(string assetName, Action<AnimationClip> callback) {
            AddressablesUtils.GetDefaultOverridableAssetAsync<AnimationClip>(assetName, callback);
        }
    }
}
