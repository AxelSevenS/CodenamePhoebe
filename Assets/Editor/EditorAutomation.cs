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

        public static List<Assembly> assemblies;
        public static List<Type> types;
        public static List<Type> weaponTypes;
        public static List<Type> stateTypes;

        [InitializeOnLoadMethod]
        private static void OnLoaded() {

            assemblies = new List<Assembly>();
            assemblies.Add(Assembly.Load("SeleneGame.Core"));
            assemblies.Add(Assembly.Load("SeleneGame.Content"));

            types = new List<Type>();
            foreach (Assembly assembly in assemblies) {
                foreach (Type assemblyType in assembly.GetTypes()) {
                    types.Add(assemblyType);
                }
            }

            UpdateAddressables();


            // CreateAllProceduralConstructors();

        }

        [MenuItem("Utility/UpdateAddressables")]
        public static void UpdateAddressables() {
            var addressablesSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);
            var entries = addressablesSettings.DefaultGroup.entries.ToList();
            foreach (var entry in entries) {
                if ( entry.labels.Contains("WeaponCostume") ) {
                    UpdateWeaponCostumeAddress(entry);
                    continue;
                }
                if ( entry.labels.Contains("Weapon") ) {
                    UpdateWeaponAddress(entry);
                    continue;
                }
                if ( entry.labels.Contains("CharacterCostume") ) {
                    UpdateCharacterCostumeAddress(entry);
                    continue;
                }
                if ( entry.labels.Contains("Character") ) {
                    UpdateCharacterAddress(entry);
                    continue;
                }
            }
        }

        public static void UpdateWeaponCostumeAddress(AddressableAssetEntry entry) {
            string path = AssetDatabase.GUIDToAssetPath(entry.guid);
            try {
                WeaponCostume asset = AssetDatabase.LoadAssetAtPath(path, typeof(WeaponCostume)) as WeaponCostume;
                string name = asset.name;

                string[] split = name.Split('_');
                entry.address = $"Weapons/Costumes/{split[0]}/{split[1]}";
            } catch (Exception) {
                return;
            }
        }

        public static void UpdateWeaponAddress(AddressableAssetEntry entry) {
            string path = AssetDatabase.GUIDToAssetPath(entry.guid);
            try {
                Weapon asset = AssetDatabase.LoadAssetAtPath(path, typeof(Weapon)) as Weapon;
                string name = asset.name;

                entry.address = $"Weapons/{name}";
            } catch (Exception) {
                return;
            }
        }
        public static void UpdateCharacterCostumeAddress(AddressableAssetEntry entry) {
            string path = AssetDatabase.GUIDToAssetPath(entry.guid);
            try {
                CharacterCostume asset = AssetDatabase.LoadAssetAtPath(path, typeof(CharacterCostume)) as CharacterCostume;
                string name = asset.name;

                string[] split = name.Split('_');
                entry.address = $"Characters/Costumes/{split[0]}/{split[1]}";
            } catch (Exception) {
                return;
            }
        }

        public static void UpdateCharacterAddress(AddressableAssetEntry entry) {
            string path = AssetDatabase.GUIDToAssetPath(entry.guid);
            try {
                Character asset = AssetDatabase.LoadAssetAtPath(path, typeof(Character)) as Character;
                string name = asset.name;

                entry.address = $"Characters/{name}";
            } catch (Exception) {
                return;
            }
        }

        // [MenuItem("Utility/Regenerate All Constructors")]
        // private static void CreateAllProceduralConstructors() {

        //     weaponTypes = CreateProceduralConstructor(typeof(Weapon), typeof(UnarmedWeapon), "SeleneGame.Weapons", @"Assets\_Scripts\Content\EntityWeapon\");
        //     stateTypes = CreateProceduralConstructor(typeof(State), typeof(WalkingState), "SeleneGame.States", @"Assets\_Scripts\Content\EntityState\");
        // }

        private static List<Type> CreateProceduralConstructor(Type type, Type defaultType, string nameSpace, string outputPath){
            const string templatePath = @"Assets\ScriptTemplates\Constructor.txt";
            string projectFolderPath = Directory.GetCurrentDirectory();
            
            string templateFilePath = Path.Combine(projectFolderPath, templatePath);
            string outputFolderPath = Path.Combine(projectFolderPath, outputPath);

            string outputFileName = $"{type.Name}Constructor.cs";
            string outputFilePath = Path.Combine(outputFolderPath, outputFileName);
            


            List<Type> inheritingTypes = (from Type checkedType in types where checkedType.IsSubclassOf(type) && !checkedType.IsAbstract && checkedType != defaultType select checkedType).ToList();

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

            return inheritingTypes;
        }

    }
}