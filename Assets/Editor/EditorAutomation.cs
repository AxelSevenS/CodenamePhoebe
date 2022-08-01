using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using System.IO;

using UnityEngine;
using UnityEditor;

using SeleneGame.Core;
using SeleneGame.Entities;
using SeleneGame.States;
using SeleneGame.Weapons;


namespace SeleneGame {
    
    public static class EditorAutomation{

        public static List<Assembly> assemblies;
        public static List<Type> types;
        public static List<Type> weaponTypes;
        public static List<Type> stateTypes;

        [InitializeOnLoadMethod]
        private static void OnLoaded(){

            assemblies = new List<Assembly>();
            assemblies.Add(Assembly.Load("SeleneGame.Core"));
            assemblies.Add(Assembly.Load("SeleneGame.Content"));
            
            types = new List<Type>();
            foreach (Assembly assembly in assemblies) {
                foreach (Type assemblyType in assembly.GetTypes()) {
                    types.Add(assemblyType);
                }
            } 
            

            CreateAllProceduralConstructors();

        }

        [MenuItem("Utility/Regenerate All Constructors")]
        private static void CreateAllProceduralConstructors() {
            
            weaponTypes = CreateProceduralConstructor(typeof(Weapon), typeof(UnarmedWeapon), "SeleneGame.Weapons", @"Assets\_Scripts\Content\EntityWeapon\");
            stateTypes = CreateProceduralConstructor(typeof(State), typeof(WalkingState), "SeleneGame.States", @"Assets\_Scripts\Content\EntityState\");
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