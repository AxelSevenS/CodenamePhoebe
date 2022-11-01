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

            CreateProceduralConstructor(typeof(State), typeof(HumanoidGroundedState), "SeleneGame.States", @"Assets\_Scripts\Content\State\");

        }

        [MenuItem("Utility/UpdateAddressables")]
        public static void UpdateAddressables()
        {

            AddressableAssetSettings addressablesSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);

            UpdateAddressableAddress<WeaponCostume>(addressablesSettings, "Assets/Data/Weapons/Costumes");

            UpdateAddressableAddressAndCreateBaseCostume<Weapon, WeaponCostume>(addressablesSettings, "Assets/Data/Weapons", "Assets/Data/Weapons/Costumes", (weapon, weaponCostume) => {
                
                    weapon.baseCostume = weaponCostume;
                    Debug.Log($"Created missing base costume for {weapon.name}, {weaponCostume}");
                }
            );

            
            UpdateAddressableAddress<CharacterCostume>(addressablesSettings, "Assets/Data/Characters/Costumes");
            
            UpdateAddressableAddressAndCreateBaseCostume<Character, CharacterCostume>(addressablesSettings, "Assets/Data/Characters", "Assets/Data/Characters/Costumes", (character, characterCostume) => {
                
                    character.baseCostume = characterCostume;
                    Debug.Log($"Created missing base costume for {character.name}, {characterCostume}");
                }
            );

        }



        private static void UpdateAssetEntryAddress<TAsset>(AddressableAssetEntry assetEntry) where TAsset : AddressableAsset<TAsset> {
            try {
                TAsset asset = AssetDatabase.LoadAssetAtPath(assetEntry.AssetPath, typeof(TAsset)) as TAsset;

                assetEntry.address = AddressableAsset<TAsset>.GetPath(asset.name);
            }
            catch (Exception) {
            }
        }

        private static void UpdateAddressableAddress<TAsset>(AddressableAssetSettings addressablesSettings, string filePath, Action<AddressableAssetEntry> callback = null) where TAsset : AddressableAsset<TAsset> {
            string typeName = typeof(TAsset).Name;
            string[] assetGUIDs = AssetDatabase.FindAssets($"t:{typeName}", new string[] { filePath });
            foreach (string assetGUID in assetGUIDs)
            {

                AddressableAssetEntry assetEntry = addressablesSettings.CreateOrMoveEntry(assetGUID, addressablesSettings.DefaultGroup);
                assetEntry.labels.Add(typeName);
                UpdateAssetEntryAddress<TAsset>(assetEntry);

                callback?.Invoke(assetEntry);
            }
        }

        private static void UpdateAddressableAddressAndCreateBaseCostume<TAsset, TCostume>(AddressableAssetSettings addressablesSettings, string filePath, string costumeFilePath, Action<TAsset, TCostume> callback = null) where TAsset : AddressableAsset<TAsset> where TCostume : AddressableAsset<TCostume> {

            UpdateAddressableAddress<TAsset>(addressablesSettings, filePath, (assetEntry) => {
                    string entryName = Path.GetFileNameWithoutExtension(assetEntry.AssetPath);
                    string costumeAssetPath = Path.Combine(costumeFilePath, $"{entryName}_Base.asset");
                    TCostume costumeAsset = AssetDatabase.LoadAssetAtPath<TCostume>(costumeAssetPath);

                    if (costumeAsset == null) {
                        costumeAsset = ScriptableObject.CreateInstance<TCostume>();
                        AssetDatabase.CreateAsset(costumeAsset, costumeAssetPath);
                        AssetDatabase.SaveAssets();

                        string assetGUID = AssetDatabase.AssetPathToGUID(costumeAssetPath);
                        AddressableAssetEntry costumeEntry = addressablesSettings.CreateOrMoveEntry(assetGUID, addressablesSettings.DefaultGroup);
                        costumeEntry.labels.Add(typeof(TCostume).Name);

                        UpdateAssetEntryAddress<TCostume>(costumeEntry);

                        TAsset characterAsset = AssetDatabase.LoadAssetAtPath<TAsset>(assetEntry.AssetPath);
                        
                        callback?.Invoke(characterAsset, costumeAsset);
                    }
                }
            );

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