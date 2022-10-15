using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public abstract class Weapon : ScriptableObject {
        

        const string defaultPath = "Weapons/Unarmed";




        public WeaponCostume baseCostume;

        public WeaponType weaponType;

        [SerializeField] private string _displayName;
        
        [SerializeField] [TextArea] private string _description;


        public float weightModifier = 1f;


        public ArmedEntity entity;
        public WeaponCostume costume;




        public string displayName => _displayName;
        public string description => _description;




        private static string WeaponNameToPath(string weaponName){
            return $"Weapons/{weaponName}";
        }

        public static void GetWeapons(Action<Weapon> callback) {

            AsyncOperationHandle<IList<Weapon>> opHandle = Addressables.LoadAssetsAsync<Weapon>(
                "Weapon",
                (weapon) => {

                    if ( weapon == null )
                        return;

                    Weapon weaponInstance = ScriptableObject.Instantiate(weapon);
                    weaponInstance.name = weaponInstance.name.Replace("(Clone)", "");

                    callback?.Invoke( weaponInstance );

                }
            );
        }

        public static Weapon Get(string weaponName) {
            // Get Requested Weapon
            AsyncOperationHandle<Weapon> opHandle = Addressables.LoadAssetAsync<Weapon>( WeaponNameToPath(weaponName) ); 

            Weapon weaponInstance = opHandle.WaitForCompletion();

            // If not found, get Default Weapon : Unarmed
            if (weaponInstance == null) {
                Debug.LogWarning($"Error getting weapon {weaponName}");
                return GetDefault();
            }

            weaponInstance = ScriptableObject.Instantiate( weaponInstance );
            weaponInstance.name = weaponInstance.name.Replace("(Clone)", "");

            return weaponInstance;
        }
        public static Weapon GetDefault() {
            AsyncOperationHandle<Weapon> opHandle = Addressables.LoadAssetAsync<Weapon>( defaultPath );

            Weapon weaponInstance = ScriptableObject.Instantiate( opHandle.WaitForCompletion() );
            weaponInstance.name = weaponInstance.name.Replace("(Clone)", "");

            return weaponInstance;
        }

        public static void GetAsync(string weaponName, Action<Weapon> callback) {
            // Get Requested Weapon
            AsyncOperationHandle<Weapon> opHandle = Addressables.LoadAssetAsync<Weapon>( WeaponNameToPath(weaponName) );
            opHandle.Completed += operation => {

                // If not found, get Default Weapon : Unarmed
                if (operation.Status == AsyncOperationStatus.Failed) {
                    Debug.LogWarning($"Error getting Weapon {weaponName}");
                    GetDefaultAsync(callback);
                    return;
                }

                Weapon weaponInstance = ScriptableObject.Instantiate( operation.Result );
                weaponInstance.name = weaponInstance.name.Replace("(Clone)", "");

                callback?.Invoke(weaponInstance);
            };
        }
        public static void GetDefaultAsync(Action<Weapon> callback) {
            AsyncOperationHandle<Weapon> opHandle = Addressables.LoadAssetAsync<Weapon>( defaultPath );
            opHandle.Completed += operation => {

                Weapon weaponInstance = ScriptableObject.Instantiate( operation.Result );
                weaponInstance.name = weaponInstance.name.Replace("(Clone)", "");

                callback?.Invoke(weaponInstance);
            };
        }


        public void SetCostume(WeaponCostume costume){
            this.costume = costume;
            
            LoadModel();

        }

        public virtual void OnEquip(){
            Display();
        }
        public virtual void OnUnequip(){
            Hide();
        }

        public abstract void LoadModel();
        public abstract void UnloadModel();

        public abstract void Display();
        public abstract void Hide();


        public virtual void Initialize( ArmedEntity entity, WeaponCostume costume = null) {
            this.entity = entity;
            SetCostume( costume ?? baseCostume );
        }

        public virtual void Update( ){;}
        public virtual void FixedUpdate( ){;}
        



        public enum WeaponType {
            OneHanded = 1,
            TwoHanded,
            Staff,
            DoubleOneHanded,
            Sparring
        };

    }
}