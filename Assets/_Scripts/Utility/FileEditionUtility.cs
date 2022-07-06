using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SeleneGame.Utility {
    public static class FileEditionUtility {
        
        public static string GetPathToResource(Object assetObject){
            StringBuilder sb = new StringBuilder( System.IO.Path.ChangeExtension( AssetDatabase.GetAssetPath(assetObject), null ) );
            sb.Replace("Assets/Resources/", "");
            return sb.ToString();
        }

    }
}
