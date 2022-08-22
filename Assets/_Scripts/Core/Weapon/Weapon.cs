using System;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public abstract class Weapon : ScriptableObject {

        [System.Serializable]
        public class Instance {
            [ReadOnly] public Weapon weapon;
            [ReadOnly] public ArmedEntity entity;
            public WeaponCostume costume;
            public GameObject rightHandModel;
            public GameObject leftHandModel;

            public string name => weapon.name;
            public WeaponType weaponType => weapon.weaponType;
            public float weightModifier => weapon.weightModifier;
            public WeaponCostume defaultCostume => weapon.defaultCostume;

            public Instance(ArmedEntity entity, Weapon weapon, WeaponCostume costume = null) {
                this.entity = entity;
                this.weapon = weapon;
                SetCostume( costume == null ? weapon.defaultCostume : costume );
                weapon.WeaponCreation(this);
            }


            public void SetCostume(WeaponCostume costume){
                this.costume = costume;
                
                LoadModel();
                Hide();

            }

            public void LoadModel(){
                if (entity == null || costume == null) return;

                UnloadModel();

                if (costume.rightHandModel != null) {
                    Transform rightWeapon = entity["weaponRight"].transform;
                    rightHandModel = GameObject.Instantiate(costume.rightHandModel, rightWeapon.position, rightWeapon.rotation, rightWeapon);
                    rightHandModel.name = $"{weapon.name}WeaponModel";
                }

                if (costume.leftHandModel != null) {
                    Transform leftWeapon = entity["weaponLeft"].transform;
                    leftHandModel = GameObject.Instantiate(costume.leftHandModel, leftWeapon.position, leftWeapon.rotation, leftWeapon);
                    leftHandModel.name = $"{weapon.name}WeaponModel";
                }
                    
            }
            public void UnloadModel(){
                rightHandModel = GameUtility.SafeDestroy(rightHandModel);
                leftHandModel = GameUtility.SafeDestroy(leftHandModel);
            }


            public void Display(){
                if (entity == null) return;

                if (rightHandModel != null) rightHandModel.SetActive(true);
                if (leftHandModel != null) leftHandModel.SetActive(true);
            }
            public void Hide(){
                if (entity == null) return;

                if (rightHandModel != null) rightHandModel.SetActive(false);
                if (leftHandModel != null) leftHandModel.SetActive(false);
            }

            public void Update() => weapon.WeaponUpdate(entity);
            public void FixedUpdate() => weapon.WeaponFixedUpdate(entity);
        }

        public enum WeaponType {lightSword, heavySword, spear, swordAndShield, sparring};

        public WeaponType weaponType;

        public float weightModifier = 1f;
        public string displayName;

        [SerializeField] private WeaponCostume defaultCostume;

        const string defaultPath = "Weapons/Unarmed";

        private static string WeaponNameToPath(string weaponName){
            return $"Weapons/{weaponName}";
        }

        public static Weapon Get(string weaponName) {
            AsyncOperationHandle<Weapon> opHandle = Addressables.LoadAssetAsync<Weapon>( WeaponNameToPath(weaponName) );
            Weapon result = opHandle.WaitForCompletion();
            if (result == null) {
                Debug.LogError($"Error getting weapon {weaponName}");
                return GetDefault();
            }
            return result;
        }
        public static Weapon GetDefault() {
            AsyncOperationHandle<Weapon> opHandle = Addressables.LoadAssetAsync<Weapon>( defaultPath );
            return opHandle.WaitForCompletion();
        }

        public static void GetAsync(string weaponName, Action<Weapon> callback) {
            AsyncOperationHandle<Weapon> opHandle = Addressables.LoadAssetAsync<Weapon>( WeaponNameToPath(weaponName) );
            opHandle.Completed += operation => {
                if (operation.Status == AsyncOperationStatus.Succeeded) {
                    callback(operation.Result);
                } else {
                    GetDefaultAsync(callback);
                }
            };
        }
        public static void GetDefaultAsync(Action<Weapon> callback) {
            AsyncOperationHandle<Weapon> opHandle = Addressables.LoadAssetAsync<Weapon>( defaultPath );
            opHandle.Completed += operation => {
                callback(operation.Result);
            };
        }

        public virtual void WeaponCreation( Instance instance ){;}

        public virtual void WeaponUpdate( Entity entity ){;}
        public virtual void WeaponFixedUpdate( Entity entity ){;}


    }
}