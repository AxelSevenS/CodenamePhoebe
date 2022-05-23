using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

namespace SeleneGame.Core {
    
    public static class EditorGlobal{

        private static System.Type[] types;

        #if UNITY_EDITOR

        [InitializeOnLoadMethod]
        private static void OnLoaded(){

            types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            CreateAssetOfInheritingScripts(typeof(Weapon), typeof(WeaponData), @"Assets\Resources\Data\Weapon", "Weapon");
            CreateAssetOfInheritingScripts(typeof(Entity), typeof(EntityData), @"Assets\Resources\Data\Entity", "Entity");


        }

        // // The String {{TypeName}} in the template is replaced by the TypeName when generating Classes.
        // private static void CreateDrawerForAllGenericClasses(string nameFilter, string templatePath){
        //     var inheriting = types.Where(
        //         t => t.BaseType != null && t.BaseType.IsGenericType && t.ToString().Contains(nameFilter)
        //     );

        //     string projectFolderPath = Directory.GetCurrentDirectory();
        //     string template = File.ReadAllText($@"{projectFolderPath}{templatePath}");
        //     string folderPath = ($@"{projectFolderPath}\Assets\Editor\");

        //     foreach(var inheritingType in inheriting){
        //         string fileName = $"{inheritingType.ToString()}Drawer.cs";
        //         string filePath = Path.Combine(folderPath, fileName);

        //         // Generate Contents
        //         string fileContents = template;
        //         fileContents = fileContents.Replace("{{TypeName}}", inheritingType.ToString());

        //         File.WriteAllText( filePath, fileContents );
        //     }
        // }

        private static void CreateAssetOfInheritingScripts(System.Type scriptType, System.Type dataType, string path, string suffixToBeRemoved = ""){
            System.Type[] inheriting = (from System.Type checkedType in types where checkedType.IsSubclassOf(scriptType) select checkedType).ToArray();
            foreach(var inheritingType in inheriting){
                string assetName = suffixToBeRemoved != "" ? inheritingType.Name.Replace(suffixToBeRemoved,"") : inheritingType.Name; 
                string assetPath = $@"{path}\{assetName}.asset";

                var assetGuids = AssetDatabase.FindAssets($"{assetName} t:{dataType.ToString()}", new[] {path});
        
                if (assetGuids.Length != 0) continue;

                var instance = ScriptableObject.CreateInstance(dataType);
                AssetDatabase.CreateAsset(instance, assetPath); 

                Debug.Log($"Created {assetName} asset at {assetPath}");

            }
        }

        #endif
    }
}