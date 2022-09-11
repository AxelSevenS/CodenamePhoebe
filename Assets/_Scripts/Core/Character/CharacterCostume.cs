using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace SeleneGame.Core {

    [CreateAssetMenu(fileName = "new Character Costume", menuName = "Costume/Character")]
    public class CharacterCostume : ScriptableObject {


        const string defaultPath = "Costume/SeleneBase";


        [Tooltip("The Portrait of the Character Costume, used as a preview in menus.")]
        public Sprite portrait;

        [Tooltip("The Display Name of the Character Costume, used in menus.")]
        public string displayName = "Default Costume Name";

        [Tooltip("The description of the Character Costume, only appears when it is not the Base Costume of the Selected Character.")]
        [TextArea] 
        public string description = "Default Costume Description";


        public GameObject model;
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


        

        public static void GetCostumesFor(Character character, Action<CharacterCostume> callback) {

            AsyncOperationHandle<IList<CharacterCostume>> opHandle = Addressables.LoadAssetsAsync<CharacterCostume>(
                "CharacterCostume",
                (costume) => {

                    if ( costume == null )
                        return;

                    // Only accept a base costume if it's the base of this character, not if it's the base of another character.
                    // for the Character Selene : Selene_Base is accepted but not Helios_Base...
                    if ( costume.name.Contains(character.name) || !costume.name.Contains("_Base") )
                        callback?.Invoke(costume);

                }
            );
        }
        
        private static string GetPath(string costumeName) {
            return $"Characters/Costumes/{costumeName}";
        }
        private static string GetBasePath(string characterName) {
            return GetPath($"{characterName}_Base");
        }

        public static CharacterCostume Get(string characterName, string costumeName) {
            // Get Requested Character Costume
            AsyncOperationHandle<CharacterCostume> opHandle = Addressables.LoadAssetAsync<CharacterCostume>( GetPath(costumeName) );
            CharacterCostume result = opHandle.WaitForCompletion();

            // If not found, get Base costume of this Character
            if (result == null) {
                Debug.LogWarning($"Error getting costume {costumeName} for character {characterName}; Using Base Costume instead.");
                return GetBase(characterName);
            }

            return result;
        }
        public static CharacterCostume GetBase(string characterName) {
            // Get Character Base Costume
            AsyncOperationHandle<CharacterCostume> opHandle = Addressables.LoadAssetAsync<CharacterCostume>( GetBasePath(characterName) );
            CharacterCostume result = opHandle.WaitForCompletion();

            // If not found, get Default costume.
            // This should never happen.
            if (result == null) {
                Debug.LogError($"Error getting Base Costume for character {characterName}");
                return GetDefault();
            }

            return result;
        }
        public static CharacterCostume GetDefault() {
            AsyncOperationHandle<CharacterCostume> opHandle = Addressables.LoadAssetAsync<CharacterCostume>( defaultPath );
            return opHandle.WaitForCompletion();
        }



        public static void GetAsync(string characterName, string costumeName, Action<CharacterCostume> callback) {
            // Get Requested Character Costume
            AsyncOperationHandle<CharacterCostume> opHandle = Addressables.LoadAssetAsync<CharacterCostume>( GetPath(costumeName) );
            opHandle.Completed += operation => {

                // If not found, get Base costume of this Character
                if (operation.Status == AsyncOperationStatus.Failed) {
                    Debug.LogWarning($"Error getting costume {costumeName} for character {characterName}; Using Base Costume instead.");
                    GetBaseAsync(characterName, callback);
                    return;
                }

                callback(operation.Result);
            };
        }
        public static void GetBaseAsync(string characterName, Action<CharacterCostume> callback) {
            // Get Character Base Costume
            AsyncOperationHandle<CharacterCostume> opHandle = Addressables.LoadAssetAsync<CharacterCostume>( GetBasePath(characterName) );
            opHandle.Completed += operation => {

                // If not found, get Default costume.
                // This should never happen.
                if (operation.Status == AsyncOperationStatus.Failed) {
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
            // TODO: maybe create a new default costume if a default costume can't be found?
        }

        public enum Emotion {neutral, determined, hesitant, shocked, disgusted, sad, happy};
        
    }
}