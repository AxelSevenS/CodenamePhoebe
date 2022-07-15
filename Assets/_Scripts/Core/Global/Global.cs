using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

namespace SeleneGame.Core {

    public static class Global {

        public const float HOLDTIME = 0.2f;

        public static Vector3 cameraDefaultPosition = new Vector3(1f, 0f, -3.5f);
        
        // public static GameController gameController => GameObject.FindWithTag("GameController").GetComponent<GameController>();
        // public static GameObject effects = gameController.transform.Find("Effects").gameObject;
        // public static GameObject weather = gameController.transform.Find("Weather").gameObject;
        // public static GameObject ui = gameController.transform.Find("UI").gameObject;


        // public static EntityManager entityManager = gameController.transform.GetComponentInChildren<EntityManager>();
        // public static ObjectManager objectManager = gameController.transform.GetComponentInChildren<ObjectManager>();
        // public static WeatherManager weatherManager = gameController.transform.GetComponentInChildren<WeatherManager>();
        // public static MusicManager musicManager = gameController.transform.GetComponentInChildren<MusicManager>();
        // public static SceneManager sceneManager = gameController.transform.GetComponentInChildren<SceneManager>(); 


        public static LayerMask GroundMask = LayerMask.GetMask("Default");
        public static LayerMask WaterMask = LayerMask.GetMask("Water");
        public static LayerMask EntityObjectMask = LayerMask.GetMask("EntityObject");


        public static GameObject LoadParticle(string fileName) => Resources.Load<GameObject>($"Particle/{fileName}");

    }
}