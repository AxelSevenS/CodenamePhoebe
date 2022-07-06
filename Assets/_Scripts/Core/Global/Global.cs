using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SeleneGame.Core {

    public static class Global {
        
        public static GameController gameController => GameObject.FindWithTag("GameController").GetComponent<GameController>();
        public static GameObject effects = gameController.transform.Find("Effects").gameObject;
        public static GameObject weather = gameController.transform.Find("Weather").gameObject;
        public static GameObject ui = gameController.transform.Find("UI").gameObject;
        public static EntityManager entityManager = gameController.transform.GetComponentInChildren<EntityManager>();
        public static ObjectManager objectManager = gameController.transform.GetComponentInChildren<ObjectManager>();
        public static WeatherManager weatherManager = gameController.transform.GetComponentInChildren<WeatherManager>();
        public static MusicManager musicManager = gameController.transform.GetComponentInChildren<MusicManager>();
        public static SceneManager sceneManager = gameController.transform.GetComponentInChildren<SceneManager>(); 


        public static LayerMask GroundMask = LayerMask.GetMask("Default");
        public static LayerMask WaterMask = LayerMask.GetMask("Water");
        public static LayerMask ObjectEntityMask = LayerMask.GetMask("ObjectEntity");



        public static Conversation LoadConversation(string fileName) => Resources.Load<Conversation>($"Conversation/{fileName}");
        public static GameObject LoadParticle(string fileName) => Resources.Load<GameObject>($"Particle/{fileName}");

        public static float timeDelta => Time.inFixedTimeStep ? Time.fixedDeltaTime : Time.deltaTime;
        public static float timeUnscaledDelta => Time.inFixedTimeStep ? Time.fixedUnscaledDeltaTime : Time.unscaledDeltaTime;


        public static bool IsBindPressed(this InputActionMap controlMap, string bindName) => controlMap[bindName].ReadValue<float>() > 0;
        public static bool IsActuated(this InputAction action) => action.ReadValue<float>() > 0;

        public static string Nicify(this string t){
            
            System.Text.StringBuilder result = new System.Text.StringBuilder("", t.Length);
            const char spaceChar = ' ';
        
            for(int i = 0; i < t.Length; i++){
                if(char.IsUpper(t[i]) == true && i != 0){
                    result.Append(spaceChar);
                }
                result.Append(t[i]);
            }
            return result.ToString();
        }

        public static T SafeDestroy<T>(T obj) where T : Object{
            if (Application.isEditor)
                Object.DestroyImmediate(obj);
            else
                Object.Destroy(obj);
            
            return null;
        }
        
        public static void SetLayerRecursively(this GameObject gameObject, int newLayer) {
            gameObject.layer = newLayer;
        
            foreach ( Transform child in gameObject.transform ) {
                SetLayerRecursively( child.gameObject, newLayer );
            }
        }

        public static T GetPropertyValue<T>(this System.Type type, string name) {
            if (type == null) return default(T);

            BindingFlags flags = BindingFlags.Static | BindingFlags.Public;

            PropertyInfo info = type.GetProperty(name, flags);

            if (info == null) {
                FieldInfo fieldInfo = type.GetField(name, flags);
                if (fieldInfo == null) return default(T);

                return (T)fieldInfo.GetValue(null);
            }

            return (T)info.GetValue(null, null);
        }

        // public static void SendIEntityMessage<T>(this GameObject gameObject, string methodName) where T : IMessageable{
        //     T[] components = gameObject.GetComponents<T>();
        //     if( typeof(T) is IEntity ){
        //         foreach (IEntity component in components){
        //             component.
        //         }
        //     }
        // }


        public static Weapon CreateWeapon(string weaponType){
            weaponType = weaponType.Replace("Weapon", "");
            switch (weaponType) {
                default:
                    return new SeleneGame.Weapons.UnarmedWeapon();
                case "Eris":
                    return new SeleneGame.Weapons.ErisWeapon();
                case "Hypnos":
                    return new SeleneGame.Weapons.HypnosWeapon();
            }
        }
    }
}