using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class ShaderGraphConversionTool : Editor {
    const string PBRForwardPassInclude = "#include \"Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/PBRForwardPass.hlsl\"";

    const string CustomInclude = "#include \"CelForwardPass.hlsl\"";

    [MenuItem("Tools/Convert Shader in CopyPaste Buffer")]
    // static void ConvertShaderInBuffer() {
    //     var shader = GUIUtility.systemCopyBuffer;
    //     string convertedShader = ConvertShader(shader);
    //     GUIUtility.systemCopyBuffer = convertedShader;
    //     WriteShaderToFile(convertedShader, "ConvertedShader");
    // }

    static string ConvertShader(string shader) {
        return shader.Replace(PBRForwardPassInclude, CustomInclude);
    }

    static void WriteShaderToFile(string shader, string path, string defaultFileName) {
        string filePath = $"{path}".Replace(".shadergraph", ".shader");

        if (!string.IsNullOrEmpty(filePath)) {
            File.WriteAllText(filePath, shader);
            AssetDatabase.Refresh();
        }
    }

    public static void ConvertShaderGraphWithGuid(GUID guid) {
        var path = AssetDatabase.GUIDToAssetPath(guid);
        var assetImporter = AssetImporter.GetAtPath(path); 
        if (assetImporter.GetType().FullName != "UnityEditor.ShaderGraph.ShaderGraphImporter") {
            Debug.Log("Not a shader graph importer");
            return;
        }

        string shaderGraphName = Path.GetFileNameWithoutExtension(path);

        Assembly shaderGraphImporterAssembly = AppDomain.CurrentDomain.GetAssemblies().First(assembly => {
                return assembly.GetType("UnityEditor.ShaderGraph.ShaderGraphImporterEditor") != null;
            }
        );
        Type shaderGraphImporterEditorType = shaderGraphImporterAssembly.GetType("UnityEditor.ShaderGraph.ShaderGraphImporterEditor");

        MethodInfo getGraphDataMethod = shaderGraphImporterEditorType
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .First(info => { return info.Name.Contains("GetGraphData"); });

        var graphData = getGraphDataMethod.Invoke(null, new object[] {assetImporter});
        Type generatorType = shaderGraphImporterAssembly.GetType("UnityEditor.ShaderGraph.Generator");
        ConstructorInfo generatorConstructor = generatorType.GetConstructors().First();
        var generator = generatorConstructor.Invoke(new object[]
            {graphData, null, 1, $"CelShaded/{shaderGraphName}", null});

        MethodInfo generatedShaderMethod = generator.GetType().GetMethod("get_generatedShader",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var generatedShader = generatedShaderMethod.Invoke(generator, new object[] { });

        WriteShaderToFile(ConvertShader((string) generatedShader), path, shaderGraphName);
    }
}