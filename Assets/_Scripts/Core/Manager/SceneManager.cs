using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
    using UnityEditor.SceneManagement;
#endif

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    [DefaultExecutionOrder(-1000)]
    public class SceneManager : Singleton<SceneManager> {
        private void OnEnable() {
            SetCurrent();
        }

        private static Scene currentScene;

        private void Awake() {
            #if UNITY_EDITOR
            
            #else
                LoadScene("SampleScene");
            #endif
        }

        public static void LoadScene(string sceneName) {
            if ( currentScene.IsValid() && currentScene.isLoaded )
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(currentScene);

            currentScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
            
            // check if the scene is already in the hierarchy
            if ( !currentScene.IsValid() || !currentScene.isLoaded ) {
                AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                asyncOperation.completed += (operation) => SetCurrent();
            } else {
                SetCurrent();
            }

            void SetCurrent() {
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(currentScene);
            }
        }
    }
}