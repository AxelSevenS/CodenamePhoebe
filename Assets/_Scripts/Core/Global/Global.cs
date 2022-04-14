using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SeleneGame.Core {

    public static class Global{
        
        public static GameObject manager = GameObject.FindWithTag("GameController");
        public static GameObject effects = manager.transform.Find("Effects").gameObject;
        public static GameObject weather = manager.transform.Find("Weather").gameObject;
        public static GameObject ui = GameObject.FindWithTag("UI");
        public static EntityManager entityManager = manager.transform.GetComponentInChildren<EntityManager>();
        public static ObjectManager objectManager = manager.transform.GetComponentInChildren<ObjectManager>();
        public static WeatherManager weatherManager = manager.transform.GetComponentInChildren<WeatherManager>();
        public static MusicManager musicManager = manager.transform.GetComponentInChildren<MusicManager>();
        public static SceneManager sceneManager = manager.transform.GetComponentInChildren<SceneManager>();


        public static LayerMask GroundMask = LayerMask.GetMask("Default");
        public static LayerMask WaterMask = LayerMask.GetMask("Water");
        public static LayerMask ObjectEntityMask = LayerMask.GetMask("ObjectEntity");



        public static Conversation LoadConversation(string fileName) => Resources.Load<Conversation>($"Conversation/{fileName}");
        public static GameObject LoadParticle(string fileName) => Resources.Load<GameObject>($"Particle/{fileName}");


        public static bool IsBindPressed(this InputActionMap controlMap, string bindName) => controlMap[bindName].ReadValue<float>() > 0;
        public static bool IsActuated(this InputAction action) => action.ReadValue<float>() > 0;

        public static string Nicify(this string t){
            string result = "";
        
            for(int i = 0; i < t.Length; i++){
                if(char.IsUpper(t[i]) == true && i != 0){
                    result += " ";
                }
                result += t[i];
            }
            return result;
        }

        public static T SafeDestroy<T>(T obj) where T : Object{
            if (Application.isEditor)
                Object.DestroyImmediate(obj);
            else
                Object.Destroy(obj);
            
            return null;
        }

        // public static void SendIEntityMessage<T>(this GameObject gameObject, string methodName) where T : IMessageable{
        //     T[] components = gameObject.GetComponents<T>();
        //     if( typeof(T) is IEntity ){
        //         foreach (IEntity component in components){
        //             component.
        //         }
        //     }
        // }

    }
}