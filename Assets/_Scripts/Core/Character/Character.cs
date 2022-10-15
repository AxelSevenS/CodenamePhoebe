using System;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public abstract class Character : ScriptableObject {


        const string defaultPath = "Characters/Selene";



        public string displayName = "Default Entity Name";
        public CharacterCostume baseCostume;
        
        public float maxHealth;
        public Vector3 size;
        public float stepHeight;
        public float acceleration;
        public float weight;
        public float jumpHeight;

        [Header("Movement Speed")]
        public float baseSpeed;
        public float sprintMultiplier;
        public float slowMultiplier;
        public float swimMultiplier;

        [Header("Evade")]
        public float evadeSpeed;
        public float evadeDuration;
        public float evadeCooldown;


        protected Entity entity;
        public CharacterCostume costume;
        public GameObject model;
        public CostumeData costumeData;



        public float totalEvadeDuration => evadeDuration + evadeCooldown;



        private static string CharacterNameToPath(string characterName){
            return $"Characters/{characterName}";
        }

        public static Character Get(string characterName) {
            // Get Requested Character
            AsyncOperationHandle<Character> opHandle = Addressables.LoadAssetAsync<Character>( CharacterNameToPath(characterName) );

            Character characterInstance = opHandle.WaitForCompletion();

            // If not found, get Default Character : Unarmed
            if (characterInstance == null) {
                Debug.LogWarning($"Error getting character {characterName}");
                return GetDefault();
            }

            characterInstance = ScriptableObject.Instantiate( characterInstance );
            characterInstance.name = characterInstance.name.Replace("(Clone)", "");

            return characterInstance;
        }
        public static Character GetDefault() {
            AsyncOperationHandle<Character> opHandle = Addressables.LoadAssetAsync<Character>( defaultPath );

            Character characterInstance = ScriptableObject.Instantiate( opHandle.WaitForCompletion() );
            characterInstance.name = characterInstance.name.Replace("(Clone)", "");

            return characterInstance;
        }

        public static void GetAsync(string characterName, Action<Character> callback) {
            // Get Requested Character
            AsyncOperationHandle<Character> opHandle = Addressables.LoadAssetAsync<Character>( CharacterNameToPath(characterName) );
            opHandle.Completed += operation => {

                // If not found, get Default Character : Selene
                if (operation.Status == AsyncOperationStatus.Failed) {
                    Debug.LogWarning($"Error getting Character {characterName}");
                    GetDefaultAsync(callback);
                    return;
                }

                Character characterInstance = ScriptableObject.Instantiate( operation.Result );
                characterInstance.name = characterInstance.name.Replace("(Clone)", "");

                callback?.Invoke(characterInstance);
            };
        }
        public static void GetDefaultAsync(Action<Character> callback) {
            AsyncOperationHandle<Character> opHandle = Addressables.LoadAssetAsync<Character>( defaultPath );
            opHandle.Completed += operation => {

                Character characterInstance = ScriptableObject.Instantiate( operation.Result );
                characterInstance.name = characterInstance.name.Replace("(Clone)", "");

                callback?.Invoke(characterInstance);
            };
        }



        public virtual void Initialize( Entity entity, CharacterCostume costume = null) {
            this.entity = entity;
            SetCostume( costume ?? baseCostume );
        }

        public void SetCostume(string costumeName) {
            SetCostume(CharacterCostume.Get(name, costumeName));
        }

        public void SetCostume(CharacterCostume costume){
            this.costume = costume;
            
            LoadModel();
        }

        public void LoadModel(){
            UnloadModel();

            if (entity == null || costume == null) return;

            if (costume.model != null) {
                Transform entityTransform = entity.transform;
                model = GameObject.Instantiate(costume.model, entityTransform.position, entityTransform.rotation, entityTransform);
                model.name = $"{name}CharacterModel";

                costumeData = model.GetComponent<CostumeData>();
            }

            entity.animator.runtimeAnimatorController = costumeData.animatorController;
            entity.animator.avatar = costumeData.animatorAvatar;
            entity.animator.Rebind();

        }
        public void UnloadModel(){
            model = GameUtility.SafeDestroy(model);
            costumeData = null;
        }

        public virtual void CharacterUpdate( Entity entity ){;}
        public virtual void CharacterFixedUpdate( Entity entity ){;}

    }
}
