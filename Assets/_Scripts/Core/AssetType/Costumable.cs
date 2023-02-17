using System;
using System.Collections.Generic;
using UnityEngine;

using SevenGame.Utility;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEditor;

namespace SeleneGame.Core {

    public abstract class Costumable {

        
        public abstract string internalName { get; }

        public abstract string displayName { get; }
        public abstract string description { get; }

        

        private string GetAnimationPath(string assetName) => @$"Animations/{internalName}/{assetName}";
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
            AsyncOperationHandle<AnimationClip> opHandle = Addressables.LoadAssetAsync<AnimationClip>( path );
            AnimationClip defaultAnim = opHandle.WaitForCompletion();

            if (defaultAnim == null) {
                Debug.LogError($"Error getting default Asset {path}");
                return null;
            }

            return defaultAnim;
        }


        public virtual void Update(){;}
        public virtual void FixedUpdate(){;}
    }

    [Serializable]
    public abstract class Costumable<T, TCostume, TModel> : Costumable where T : Costumable<T, TCostume, TModel> where TCostume : Costume<TCostume> where TModel : CostumeModel<TCostume> {


        private bool disposedValue;


        private static Dictionary<string, T> _identifiedCostumables = new Dictionary<string, T>();
        public static readonly List<Type> _types = new List<Type>();



        protected TCostume _baseCostume;
        [SerializeReference] protected TModel _model;
        


        public TCostume baseCostume {
            get {
                _baseCostume ??= GetBaseCostume() ?? GetDefaultCostume();
                return _baseCostume;
            }
            // protected internal set => _baseCostume = value;
        }

        public TModel model {
            get {
                if ( _model == null )
                    SetCostume(baseCostume);
                return _model;
            }
        }



        static Costumable() {
            _types = new();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in assembly.GetTypes()) {
                    if (typeof(T).IsAssignableFrom(type) && !type.IsAbstract) {
                        _types.Add(type);
                    }
                }
            }
        }

        // protected Costumable(TCostume costume) {
        //     SetCostume(costume ?? baseCostume);
        // }



        public abstract TCostume GetBaseCostume();

        public static TCostume GetDefaultCostume() => Costume<TCostume>.GetDefaultAsset()/*  ?? new TCostume() */; 




        public void SetCostume(string costumeName) {
            SetCostume(Costume<TCostume>.GetAsset(costumeName));
        }

        public abstract void SetCostume(TCostume costume);


    
        public static T GetInstanceWithId(string id) {
            if ( _identifiedCostumables.ContainsKey(id) )
                return _identifiedCostumables[id];

            return null;
        }

        public static void SetInstanceWithId(string id, T costumable) {
            _identifiedCostumables[id] = costumable;
        }
        

        protected void Dispose(bool disposing) {

            if (!disposedValue) {
                if (disposing)
                    DisposeBehavior();

                disposedValue = true;
            }
        }

        protected virtual void DisposeBehavior() {
            _model?.Dispose();
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
    
}
