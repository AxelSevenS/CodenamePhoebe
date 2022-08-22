using System;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace SeleneGame.Core {

    [CreateAssetMenu(fileName = "new Character Costume", menuName = "Costume/Character")]
    public class CharacterCostume : ScriptableObject {

        public enum Emotion {neutral, determined, hesitant, shocked, disgusted, sad, happy};

        public Sprite neutralPortrait;
        public Sprite determinedPortrait;
        public Sprite hesitantPortrait;
        public Sprite shockedPortrait;
        public Sprite disgustedPortrait;
        public Sprite sadPortrait;
        public Sprite happyPortrait;
        public Sprite GetPortrait(Emotion emotion) {
            switch (emotion) {
                default: return neutralPortrait;
                case Emotion.determined: return determinedPortrait;
                case Emotion.hesitant: return hesitantPortrait;
                case Emotion.shocked: return shockedPortrait;
                case Emotion.disgusted: return disgustedPortrait;
                case Emotion.sad: return sadPortrait;
                case Emotion.happy: return happyPortrait;
            }
        }

        public GameObject model;

        public string displayName = "Default Costume Name";

        const string defaultPath = "Costume/SeleneBase";
        



        
        private static string GetPath(string characterName, string costumeName) {
            return $"Characters/Costumes/{characterName}/{costumeName}";
        }

        public static CharacterCostume Get(string characterName, string costumeName) {
            // Get Requested Character Costume
            AsyncOperationHandle<CharacterCostume> opHandle = Addressables.LoadAssetAsync<CharacterCostume>( GetPath(characterName, costumeName) );
            CharacterCostume result = opHandle.WaitForCompletion();
            if (result == null) {
                // If not found, get Base costume of this Character
                Debug.LogWarning($"Error getting costume {costumeName} for character {characterName}; Using Base Costume instead.");
                return GetBase(characterName);
            }

            return result;
        }
        public static CharacterCostume GetBase(string characterName) {
            AsyncOperationHandle<CharacterCostume> opHandle = Addressables.LoadAssetAsync<CharacterCostume>( GetPath(characterName, "Base") );
            CharacterCostume result = opHandle.WaitForCompletion();
            if (result == null) {
                // If not found, get Default costume.
                // This should never happen.
                Debug.LogError($"Error getting Base Costume for character {characterName}");
                result = GetDefault();
            }

            return result;
        }
        public static CharacterCostume GetDefault() {
            AsyncOperationHandle<CharacterCostume> opHandle = Addressables.LoadAssetAsync<CharacterCostume>( defaultPath );
            return opHandle.WaitForCompletion();
        }



        public static void GetAsync(string characterName, string costumeName, Action<CharacterCostume> callback) {
            // Get Requested Character Costume
            AsyncOperationHandle<CharacterCostume> opHandle = Addressables.LoadAssetAsync<CharacterCostume>( GetPath(characterName, costumeName) );
            opHandle.Completed += operation => {
                if (operation.Status == AsyncOperationStatus.Failed) {
                    // If not found, get Base costume of this Character
                    Debug.LogWarning($"Error getting costume {costumeName} for character {characterName}; Using Base Costume instead.");
                    GetBaseAsync(characterName, callback);
                    return;
                }

                callback(operation.Result);
            };
        }
        public static void GetBaseAsync(string characterName, Action<CharacterCostume> callback) {
            // Get Character Base Costume
            AsyncOperationHandle<CharacterCostume> opHandle = Addressables.LoadAssetAsync<CharacterCostume>( GetPath(characterName, "Base") );
            opHandle.Completed += operation => {
                if (operation.Status == AsyncOperationStatus.Failed) {
                    // If not found, get Default costume.
                    // This should never happen.
                    Debug.LogError($"Error getting Base Costume for character {characterName}");
                    GetDefaultAsync(callback);
                    return;
                }

                callback(operation.Result);
            };
        }
        public static void GetDefaultAsync(Action<CharacterCostume> callback) {
            AsyncOperationHandle<CharacterCostume> opHandle = Addressables.LoadAssetAsync<CharacterCostume>( defaultPath );
            opHandle.Completed += operation => {
                callback(operation.Result);
            };
            // TODO: maybe create a new default costume?
        }
        
    }
}