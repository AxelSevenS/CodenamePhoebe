using System.Collections;
using System.Collections.Generic;

using UnityEngine.SceneManagement;
using UnityEngine;

using SevenGame.Utility;

namespace SeleneGame.Core {
    
    public class SceneManager : Singleton<SceneManager> {
        private void OnEnable() {
            SetCurrent();
        }

        private static Scene currentScene;

        private void Awake() {
            // LoadScene("SampleScene");
        }

        public static void LoadScene(string sceneName) {
            Debug.Log(currentScene);
            if ( currentScene.IsValid() && currentScene.isLoaded )
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(currentScene);

            AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncOperation.completed += (operation) => {
                currentScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(currentScene);
            };
        }
    }
}