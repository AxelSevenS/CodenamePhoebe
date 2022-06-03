using System.IO;
using UnityEditor;

namespace SeleneGame.EditorUI {
    public class ShaderGraphModificationProcessor : AssetPostprocessor {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            foreach (string importedAsset in importedAssets) {
                if (Path.GetExtension(importedAsset).ToLower() == ".shadergraph") {
                    GUID guid = new GUID(AssetDatabase.AssetPathToGUID(importedAsset));
                    //TODO: Convert Shader Graph
                    ShaderGraphConversionTool.ConvertShaderGraphWithGuid(guid);
                }
            }
        }
    }
}