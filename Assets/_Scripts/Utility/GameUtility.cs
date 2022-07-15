using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SevenGame.Utility {
    public static class GameUtility {
        
        
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

    }
}
