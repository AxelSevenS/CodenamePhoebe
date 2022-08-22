using System;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;

namespace SeleneGame.Core {

    public abstract class Character : ScriptableObject {

        [System.Serializable]
        public class Instance {
            [ReadOnly] public Character character;
            [ReadOnly] public Entity entity;
            public CharacterCostume costume;
            public GameObject model;
            public CostumeData costumeData;


            public string name => character.name;
            public CharacterCostume defaultCostume => character.defaultCostume;
            public float maxHealth => character.maxHealth;
            public Vector3 size => character.size;
            public float stepHeight => character.stepHeight;
            public float acceleration => character.acceleration;
            public float weight => character.weight;
            public float jumpHeight => character.jumpHeight;


            public float baseSpeed => character.baseSpeed;
            public float sprintMultiplier => character.sprintMultiplier;
            public float slowMultiplier => character.slowMultiplier;
            public float swimMultiplier => character.swimMultiplier;


            public float evadeSpeed => character.evadeSpeed;
            public float evadeDuration => character.evadeDuration;
            public float evadeCooldown => character.evadeCooldown;
            public float totalEvadeDuration => character.totalEvadeDuration;

            public Instance(Entity entity, Character character, CharacterCostume costume = null) {
                this.entity = entity;
                this.character = character;
                SetCostume( costume == null ? character.defaultCostume : costume );
                character.CharacterCreation(this);
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
                    model.name = $"{character.name}CharacterModel";

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


            public void Update() => character.CharacterUpdate(entity);
            public void FixedUpdate() => character.CharacterFixedUpdate(entity);
        }

        public string displayName = "Default Entity Name";
        public CharacterCostume defaultCostume;
        
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
        public float totalEvadeDuration => evadeDuration + evadeCooldown;

        const string defaultPath = "Characters/Selene";

        private static string CharacterNameToPath(string weaponName){
            return $"Characters/{weaponName}";
        }

        public static Character Get(string characterName) {
            AsyncOperationHandle<Character> opHandle = Addressables.LoadAssetAsync<Character>( CharacterNameToPath(characterName) );
            Character result = opHandle.WaitForCompletion();
            if (result == null) {
                Debug.LogError($"Error getting weapon {characterName}");
                return GetDefault();
            }
            return result;
        }
        public static Character GetDefault() {
            AsyncOperationHandle<Character> opHandle = Addressables.LoadAssetAsync<Character>( defaultPath );
            return opHandle.WaitForCompletion();
        }

        public static void GetAsync(string characterName, Action<Character> callback) {
            AsyncOperationHandle<Character> opHandle = Addressables.LoadAssetAsync<Character>( CharacterNameToPath(characterName) );
            opHandle.Completed += operation => {
                if (operation.Status == AsyncOperationStatus.Succeeded) {
                    callback(operation.Result);
                } else {
                    GetDefaultAsync(callback);
                }
            };
        }
        public static void GetDefaultAsync(Action<Character> callback) {
            AsyncOperationHandle<Character> opHandle = Addressables.LoadAssetAsync<Character>( defaultPath );
            opHandle.Completed += operation => {
                callback(operation.Result);
            };
        }
        public virtual void CharacterCreation( Instance instance ){;}
        public virtual void CharacterUpdate( Entity entity ){;}
        public virtual void CharacterFixedUpdate( Entity entity ){;}

    }
}
