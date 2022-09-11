using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

using SeleneGame.Core;
using SeleneGame.Entities;
using SeleneGame.States;
using SeleneGame.Weapons;

using SevenGame.Utility;


namespace SeleneGame {
    
    public static class EditorAutomation{

        public static Assembly[] assemblies;
        public static List<Type> types;


        [InitializeOnLoadMethod]
        private static void OnLoaded() {
            
            UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler = (op, ex) => {;};

            assemblies = new Assembly[] {
                Assembly.Load("SeleneGame.Core"),
                Assembly.Load("SeleneGame.Content")
            };

            types = new List<Type>();
            foreach (Assembly assembly in assemblies) {
                foreach (Type assemblyType in assembly.GetTypes()) {
                    types.Add(assemblyType);
                }
            }


            // UpdateAddressables();

            CreateProceduralConstructor(typeof(State), typeof(WalkingState), "SeleneGame.States", @"Assets\_Scripts\Content\EntityState\");

        }

        [MenuItem("Utility/UpdateAddressables")]
        public static void UpdateAddressables() {

            AddressableAssetSettings addressablesSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);

            string[] weaponCostumeGUIDs = AssetDatabase.FindAssets("t:WeaponCostume", new string[] {"Assets/Data/Weapons/Costumes"});
            foreach (string weaponCostumeGUID in weaponCostumeGUIDs) {
                string weaponCostumeAssetPath = AssetDatabase.GUIDToAssetPath(weaponCostumeGUID);
                WeaponCostume weaponCostume = AssetDatabase.LoadAssetAtPath<WeaponCostume>(weaponCostumeAssetPath);
                
                AddressableAssetEntry weaponCostumeEntry = addressablesSettings.CreateOrMoveEntry(weaponCostumeGUID, addressablesSettings.DefaultGroup);
                weaponCostumeEntry.labels.Add("WeaponCostume");
                UpdateWeaponCostumeAddress(weaponCostumeEntry, addressablesSettings);
            }

            string[] weaponGUIDs = AssetDatabase.FindAssets("t:Weapon", new string[] {"Assets/Data/Weapons"});
            foreach (string weaponGUID in weaponGUIDs) {
                string weaponAssetPath = AssetDatabase.GUIDToAssetPath(weaponGUID);
                Weapon weapon = AssetDatabase.LoadAssetAtPath<Weapon>(weaponAssetPath);
                
                AddressableAssetEntry weaponEntry = addressablesSettings.CreateOrMoveEntry(weaponGUID, addressablesSettings.DefaultGroup);
                weaponEntry.labels.Add("Weapon");
                UpdateWeaponAddress(weaponEntry, addressablesSettings);

                CreateMissingWeaponBaseCostume(weaponEntry, addressablesSettings);
            }

            string[] characterCostumeGUIDs = AssetDatabase.FindAssets("t:CharacterCostume", new string[] {"Assets/Data/Characters/Costumes"});
            foreach (string characterCostumeGUID in characterCostumeGUIDs) {
                string characterCostumeAssetPath = AssetDatabase.GUIDToAssetPath(characterCostumeGUID);
                CharacterCostume characterCostume = AssetDatabase.LoadAssetAtPath<CharacterCostume>(characterCostumeAssetPath);
                
                AddressableAssetEntry characterCostumeEntry = addressablesSettings.CreateOrMoveEntry(characterCostumeGUID, addressablesSettings.DefaultGroup);
                characterCostumeEntry.labels.Add("CharacterCostume");
                UpdateCharacterCostumeAddress(characterCostumeEntry, addressablesSettings);
            }

            string[] characterGUIDs = AssetDatabase.FindAssets("t:Character", new string[] {"Assets/Data/Characters"});
            foreach (string characterGUID in characterGUIDs) {
                string characterAssetPath = AssetDatabase.GUIDToAssetPath(characterGUID);
                Character character = AssetDatabase.LoadAssetAtPath<Character>(characterAssetPath);
                
                AddressableAssetEntry characterEntry = addressablesSettings.CreateOrMoveEntry(characterGUID, addressablesSettings.DefaultGroup);
                characterEntry.labels.Add("Character");
                UpdateCharacterAddress(characterEntry, addressablesSettings);

                CreateMissingCharacterBaseCostume(characterEntry, addressablesSettings);
            }
        }
        
        // public static void SetAddressableAddresses( AddressableAssetSettings addressablesSettings, List<AddressableAssetEntry> entries ) {

        //     foreach (AddressableAssetEntry entry in entries) {
        //         if ( entry.labels.Contains("WeaponCostume") ) {
        //             UpdateWeaponCostumeAddress(entry, addressablesSettings);
        //             continue;
        //         }
        //         if ( entry.labels.Contains("Weapon") ) {
        //             UpdateWeaponAddress(entry, addressablesSettings);
        //             // CreateMissingWeaponBaseCostume(addressablesSettings, entry);
        //             continue;
        //         }
        //         if ( entry.labels.Contains("CharacterCostume") ) {
        //             UpdateCharacterCostumeAddress(entry, addressablesSettings);
        //             continue;
        //         }
        //         if ( entry.labels.Contains("Character") ) {
        //             UpdateCharacterAddress(entry, addressablesSettings);
        //             continue;
        //         }
        //     }

        // }

        public static void UpdateWeaponCostumeAddress(AddressableAssetEntry entry, AddressableAssetSettings addressablesSettings) {
            try {
                WeaponCostume asset = AssetDatabase.LoadAssetAtPath(entry.AssetPath, typeof(WeaponCostume)) as WeaponCostume;

                entry.address = $"Weapons/Costumes/{asset.name}";
            } catch (Exception) {
                return;
            }
        }

        public static void UpdateWeaponAddress(AddressableAssetEntry entry, AddressableAssetSettings addressablesSettings) {
            try {
                Weapon asset = AssetDatabase.LoadAssetAtPath(entry.AssetPath, typeof(Weapon)) as Weapon;

                entry.address = $"Weapons/{asset.name}";
            } catch (Exception) {
                return;
            }
        }
        public static void UpdateCharacterCostumeAddress(AddressableAssetEntry entry, AddressableAssetSettings addressablesSettings) {
            try {
                CharacterCostume asset = AssetDatabase.LoadAssetAtPath(entry.AssetPath, typeof(CharacterCostume)) as CharacterCostume;

                entry.address = $"Characters/Costumes/{asset.name}";
            } catch (Exception) {
                return;
            }
        }

        public static void UpdateCharacterAddress(AddressableAssetEntry entry, AddressableAssetSettings addressablesSettings) {
            try {
                Character asset = AssetDatabase.LoadAssetAtPath(entry.AssetPath, typeof(Character)) as Character;

                entry.address = $"Characters/{asset.name}";
            } catch (Exception) {
                return;
            }
        }

        private static void CreateMissingWeaponBaseCostume(AddressableAssetEntry entry, AddressableAssetSettings addressablesSettings) {
            string entryName = Path.GetFileNameWithoutExtension(entry.AssetPath);
            string costumeAssetPath = $"Assets/Data/Weapons/Costumes/{entryName}_Base.asset";
            WeaponCostume costumeAsset = AssetDatabase.LoadAssetAtPath<WeaponCostume>(costumeAssetPath);
            if (costumeAsset == null) {
                costumeAsset = ScriptableObject.CreateInstance<WeaponCostume>();
                AssetDatabase.CreateAsset(costumeAsset, costumeAssetPath);
                AssetDatabase.SaveAssets();

                string assetGUID = AssetDatabase.AssetPathToGUID(costumeAssetPath);
                AddressableAssetEntry costumeEntry = addressablesSettings.CreateOrMoveEntry(assetGUID, addressablesSettings.DefaultGroup);
                costumeEntry.labels.Add("WeaponCostume");
                UpdateWeaponCostumeAddress(costumeEntry, addressablesSettings);

                Weapon weaponAsset = AssetDatabase.LoadAssetAtPath<Weapon>(entry.AssetPath);
                weaponAsset.baseCostume = costumeAsset;

                Debug.Log($"Created missing base costume for {entryName} at path : {costumeAssetPath}");
            }
        }

        private static void CreateMissingCharacterBaseCostume(AddressableAssetEntry entry, AddressableAssetSettings addressablesSettings) {
            string entryName = Path.GetFileNameWithoutExtension(entry.AssetPath);
            string costumeAssetPath = $"Assets/Data/Characters/Costumes/{entryName}_Base.asset";
            CharacterCostume costumeAsset = AssetDatabase.LoadAssetAtPath<CharacterCostume>(costumeAssetPath);
            if (costumeAsset == null) {
                costumeAsset = ScriptableObject.CreateInstance<CharacterCostume>();
                AssetDatabase.CreateAsset(costumeAsset, costumeAssetPath);
                AssetDatabase.SaveAssets();

                string assetGUID = AssetDatabase.AssetPathToGUID(costumeAssetPath);
                AddressableAssetEntry costumeEntry = addressablesSettings.CreateOrMoveEntry(assetGUID, addressablesSettings.DefaultGroup);
                costumeEntry.labels.Add("CharacterCostume");
                UpdateCharacterCostumeAddress(costumeEntry, addressablesSettings);

                Character characterAsset = AssetDatabase.LoadAssetAtPath<Character>(entry.AssetPath);
                characterAsset.baseCostume = costumeAsset;

                Debug.Log($"Created missing base costume for {entryName} at path : {costumeAssetPath}");
            }
        }

        // [MenuItem("Utility/Regenerate All Constructors")]
        // private static void CreateAllProceduralConstructors() {

        //     weaponTypes = CreateProceduralConstructor(typeof(Weapon), typeof(UnarmedWeapon), "SeleneGame.Weapons", @"Assets\_Scripts\Content\EntityWeapon\");
        // }

        private static void CreateProceduralConstructor(Type type, Type defaultType, string nameSpace, string outputPath){
            const string templatePath = @"Assets\ScriptTemplates\Constructor.txt";
            string projectFolderPath = Directory.GetCurrentDirectory();
            
            string templateFilePath = Path.Combine(projectFolderPath, templatePath);
            string outputFolderPath = Path.Combine(projectFolderPath, outputPath);

            string outputFileName = $"{type.Name}Constructor.cs";
            string outputFilePath = Path.Combine(outputFolderPath, outputFileName);
            


            IEnumerable<Type> inheritingTypes = (from Type checkedType in types where checkedType.IsSubclassOf(type) && !checkedType.IsAbstract && checkedType != defaultType select checkedType);

            System.Text.StringBuilder stringToConstructor = new System.Text.StringBuilder($"default: return new {defaultType.FullName}();");
            foreach(var inheritingType in inheritingTypes){
                stringToConstructor.Append($"\n                case \"{inheritingType.Name}\": return new {inheritingType.FullName}();");
            }

            // Generate Contents
            string template = File.ReadAllText( templateFilePath );
            System.Text.StringBuilder fileContents = new System.Text.StringBuilder(template);
            fileContents.Replace("{{stringtoconstructor}}", stringToConstructor.ToString());
            fileContents.Replace("{{namespace}}", nameSpace);
            fileContents.Replace("{{typefullname}}", type.FullName);
            fileContents.Replace("{{typename}}", type.Name);

            File.WriteAllText( outputFilePath, fileContents.ToString() );
        }

    }
}