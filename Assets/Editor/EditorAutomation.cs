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
using SeleneGame.Content;

using SevenGame.Utility;


namespace SeleneGame.Core {
    
    public static class EditorAutomation {


        [InitializeOnLoadMethod]
        private static void OnLoaded() {
            
            // UnityEngine.ResourceManagement.ResourceManager.ExceptionHandler = (op, ex) => {
            //     // Debug.LogException(ex);
            // };

            // assemblies = new Assembly[] {
            //     Assembly.Load("SeleneGame.Core"),
            //     Assembly.Load("SeleneGame.Content")
            // };

            // types = new List<Type>();
            // foreach (Assembly assembly in assemblies) {
            //     foreach (Type assemblyType in assembly.GetTypes()) {
            //         types.Add(assemblyType);
            //     }
            // }


            // UpdateAddressables();

            // CreateProceduralConstructor(typeof(State), typeof(Grounded), "SeleneGame.Content", @"Assets\1-Scripts\Content\3-State\");

        }

        [MenuItem("Utility/UpdateAddressables")]
        public static void UpdateAddressables() {

            AddressableAssetSettings addressablesSettings = AddressableAssetSettingsDefaultObject.GetSettings(false);

            UpdateAddressableAddress<AnimationClip>(addressablesSettings, "Assets/Animations");
            
            UpdateAddressableAddressWithTypeName<CharacterCostume>(addressablesSettings, "Assets/Costumes/Characters");
            UpdateAddressableAddressWithTypeName<WeaponCostume>(addressablesSettings, "Assets/Costumes/Weapons");
            UpdateAddressableAddressWithTypeName<EidolonMaskCostume>(addressablesSettings, "Assets/Costumes/Masks");
            
            UpdateAddressableAddressWithTypeName<CharacterData>(addressablesSettings, "Assets/Data/Characters");
            UpdateAddressableAddressWithTypeName<WeaponData>(addressablesSettings, "Assets/Data/Weapons");
            UpdateAddressableAddressWithTypeName<EidolonMaskData>(addressablesSettings, "Assets/Data/Masks");


        }


        private static void UpdateAddressableAddress<TAsset>(AddressableAssetSettings addressablesSettings, string assetPath) {

            string typeName = typeof(TAsset).Name;

            // if it doesn't exist, create the label
            if ( !addressablesSettings.GetLabels().Contains(typeName) ) {
                addressablesSettings.AddLabel(typeName);
            }


            string[] assetGUIDs = AssetDatabase.FindAssets($"t:{typeName}", new string[] { assetPath });
            foreach (string assetGUID in assetGUIDs) {
                
                AddressableAssetEntry assetEntry = addressablesSettings.CreateOrMoveEntry(assetGUID, addressablesSettings.DefaultGroup);

                string newAddress = assetEntry.AssetPath.Replace(Path.GetExtension(assetEntry.AssetPath), "");
                newAddress = newAddress.Replace("Assets/", "");
                assetEntry.address = Path.Combine(newAddress);
                // assetEntry.address = Path.GetFileNameWithoutExtension(assetEntry.address);

            }
        }
        
        private static void UpdateAddressableAddressWithTypeName<TAsset>(AddressableAssetSettings addressablesSettings, string assetPath) where TAsset : ScriptableObject {

            string typeName = typeof(TAsset).Name;


            string[] assetGUIDs = AssetDatabase.FindAssets($"t:{typeName}", new string[] { assetPath });
            foreach (string assetGUID in assetGUIDs) {
                
                AddressableAssetEntry assetEntry = addressablesSettings.CreateOrMoveEntry(assetGUID, addressablesSettings.DefaultGroup);

                TAsset asset = AssetDatabase.LoadAssetAtPath(assetEntry.AssetPath, typeof(TAsset)) as TAsset;
                assetEntry.address = AddressablesUtils.GetPath<TAsset>(asset.name);

            }
        }



        // // [MenuItem("Utility/Regenerate All Constructors")]
        // // private static void CreateAllProceduralConstructors() {

        // //     weaponTypes = CreateProceduralConstructor(typeof(Weapon), typeof(UnarmedWeapon), "SeleneGame.Weapons", @"Assets\_Scripts\Content\EntityWeapon\");
        // // }

        // private static void CreateProceduralConstructor(Type type, Type defaultType, string nameSpace, string outputPath){
        //     const string templatePath = @"Assets\ScriptTemplates\Constructor.txt";
        //     string projectFolderPath = Directory.GetCurrentDirectory();
            
        //     string templateFilePath = Path.Combine(projectFolderPath, templatePath);
        //     string outputFolderPath = Path.Combine(projectFolderPath, outputPath);

        //     string outputFileName = $"{type.Name}Constructor.cs";
        //     string outputFilePath = Path.Combine(outputFolderPath, outputFileName);
            


        //     IEnumerable<Type> inheritingTypes = (from Type checkedType in types where checkedType.IsSubclassOf(type) && !checkedType.IsAbstract && checkedType != defaultType select checkedType);

        //     System.Text.StringBuilder stringToConstructor = new System.Text.StringBuilder($"default: return new {defaultType.FullName}();");
        //     foreach(var inheritingType in inheritingTypes){
        //         stringToConstructor.Append($"\n                case \"{inheritingType.Name}\": return new {inheritingType.FullName}();");
        //     }

        //     // Generate Contents
        //     string template = File.ReadAllText( templateFilePath );
        //     System.Text.StringBuilder fileContents = new System.Text.StringBuilder(template);
        //     fileContents.Replace("{{stringtoconstructor}}", stringToConstructor.ToString());
        //     fileContents.Replace("{{namespace}}", nameSpace);
        //     fileContents.Replace("{{typefullname}}", type.FullName);
        //     fileContents.Replace("{{typename}}", type.Name);

        //     File.WriteAllText( outputFilePath, fileContents.ToString() );
        // }

    }
}