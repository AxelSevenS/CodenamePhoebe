using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;

using SeleneGame.Core;
using SeleneGame.Entities;
using SeleneGame.States;
using SeleneGame.Weapons;


namespace SeleneGame.Procedural {
    
    public static class EditorUtility{

        private static System.Type[] types;

        #if UNITY_EDITOR

        [InitializeOnLoadMethod]
        private static void OnLoaded(){

            // types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            // CreateAssetOfInheritingScripts(typeof(Weapon), typeof(WeaponData), @"Assets\Resources\Data\Weapon", "Weapon");
            // CreateAssetOfInheritingScripts(typeof(Entity), typeof(EntityData), @"Assets\Resources\Data\Entity", "Entity");

            CreateAllProceduralConstructors();

        }

        [MenuItem("Utility/Regenerate All Constructors")]
        private static void CreateAllProceduralConstructors() {

            types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
            const string path = @"\Assets\_Scripts\Core\Procedural\Constructor.txt";
            CreateProceduralConstructor(typeof(Weapon), typeof(UnarmedWeapon), "SeleneGame.Weapons", path);
            CreateProceduralConstructor(typeof(State), typeof(WalkingState), "SeleneGame.States", path);
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

        private static void CreateProceduralConstructor(System.Type type, System.Type defaultType, string nameSpace, string templatePath){
            string projectFolderPath = Directory.GetCurrentDirectory();
            string template = File.ReadAllText($@"{projectFolderPath}{templatePath}");
            string folderPath = ($@"{projectFolderPath}\Assets\_Scripts\Core\Procedural\");

            string fileName = $"{type.Name}Constructor.cs";
            string filePath = Path.Combine(folderPath, fileName);

            System.Type[] inheriting = (from System.Type checkedType in types where checkedType.IsSubclassOf(type) && !checkedType.IsAbstract && checkedType != defaultType select checkedType).ToArray();

            System.Text.StringBuilder cases = new System.Text.StringBuilder($"default: return new {defaultType.FullName}();");
            foreach(var inheritingType in inheriting){
                cases.Append($"\n                case \"{inheritingType.Name}\": return new {inheritingType.FullName}();");
            }

            // Generate Contents
            System.Text.StringBuilder fileContents = new System.Text.StringBuilder(template);
            fileContents.Replace("{{cases}}", cases.ToString());
            fileContents.Replace("{{namespace}}", nameSpace);
            fileContents.Replace("{{typefullname}}", type.FullName);
            fileContents.Replace("{{typename}}", type.Name);

            File.WriteAllText( filePath, fileContents.ToString() );
        }

        private static void CreateAssetOfInheritingScripts(System.Type scriptType, System.Type dataType, string path, string suffixToBeRemoved = ""){
            System.Type[] inheriting = (from System.Type checkedType in types where checkedType.IsSubclassOf(scriptType) && !checkedType.IsAbstract select checkedType).ToArray();
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