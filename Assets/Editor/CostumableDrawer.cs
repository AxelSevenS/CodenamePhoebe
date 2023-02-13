using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SeleneGame.Core {


    public abstract class CostumableDrawer<TCostumable, TCostume, TModel> : PropertyDrawer where TCostumable : Costumable<TCostumable, TCostume, TModel> where TCostume : Costume<TCostume> where TModel : CostumeModel<TCostume> {

        private bool foldout = false;
        protected TCostumable targetCostumable;

        private static string[] typeOptions;
        int typeIndex = 0;


        static CostumableDrawer() {
            
            typeOptions = new string[Costumable<TCostumable, TCostume, TModel>._types.Count + 1];
            typeOptions[0] = "None";
            for (int i = 0; i < Costumable<TCostumable, TCostume, TModel>._types.Count; i++) {
                typeOptions[i + 1] = Costumable<TCostumable, TCostume, TModel>._types[i].Name;
            }
            
        }
            
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {


            targetCostumable = property.managedReferenceValue as TCostumable;


            EditorGUI.BeginProperty( position, label, property );




            if (targetCostumable == null)
                typeIndex = 0;
            else 
                typeIndex = Costumable<TCostumable, TCostume, TModel>._types.IndexOf( targetCostumable.GetType() ) + 1;


            Rect valueRect = new Rect(position.x, position.y, position.width, position.height);
            Rect buttonRect = new Rect(position.x + position.width/2f, position.y, position.width/2f, EditorGUIUtility.singleLineHeight);
            

            EditorGUI.BeginChangeCheck();

            typeIndex = EditorGUI.Popup(
                buttonRect,
                typeIndex,
                typeOptions);

            if (EditorGUI.EndChangeCheck()) {
                SetValue(property, typeIndex - 1);
            }

            Rect rectType = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            if (targetCostumable != null) {

                foldout = EditorGUI.Foldout(rectType, foldout, label, true);
                if ( foldout) {
                    
                    EditorGUI.indentLevel++;
                    rectType.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;


                    EditorGUI.BeginChangeCheck();

                    TCostume selectedCostume = targetCostumable?.model?.costume ?? null;
                    selectedCostume = EditorGUI.ObjectField(rectType, new GUIContent("Costume"), selectedCostume, typeof(TCostume), false) as TCostume;

                    if ( EditorGUI.EndChangeCheck() ) {
                        targetCostumable.SetCostume( selectedCostume ?? targetCostumable.baseCostume);
                    }

                    rectType.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    int depth = property.depth + 1;
                    foreach (SerializedProperty prop in property) {
                        
                        
                        if (prop.name == "m_Script" || prop.depth > depth) {
                            // Debug.Log("continue");
                            continue;
                        }

                        // Draw the property.
                        EditorGUI.PropertyField(rectType, prop, true);
                        rectType.y += EditorGUI.GetPropertyHeight(prop, true) + EditorGUIUtility.standardVerticalSpacing;
                    }

                    
                    EditorGUI.indentLevel--;
                }
            
            } else {
                EditorGUI.LabelField(rectType, label);
            }



            EditorGUI.EndProperty();


        }

        public abstract void SetValue(SerializedProperty property, int typeIndex);

        public virtual void PreWork(Rect position, SerializedProperty property, GUIContent label) {

        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float height = EditorGUIUtility.singleLineHeight;
            int depth = property.depth + 1;
            if (foldout) {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                foreach (SerializedProperty prop in property) {
                    if (prop.name == "m_Script" || prop.depth > depth)
                        continue;
                    height += EditorGUI.GetPropertyHeight(prop, true) + EditorGUIUtility.standardVerticalSpacing;
                }
            }
            return height;
        }

	
        public static object GetParent(SerializedProperty prop) {
            var path = prop.propertyPath.Replace(".Array.data[", "[");
            object obj = prop.serializedObject.targetObject;
            var elements = path.Split('.');
            foreach(var element in elements.Take(elements.Length-1))
            {
                if(element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[","").Replace("]",""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }
    
        public static object GetValue(object source, string name) {
            if(source == null)
                return null;
            var type = source.GetType();
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if(f == null)
            {
                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if(p == null)
                    return null;
                return p.GetValue(source, null);
            }
            return f.GetValue(source);
        }

        public static object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as IEnumerable;
            var enm = enumerable.GetEnumerator();
            while(index-- >= 0)
                enm.MoveNext();
            return enm.Current;
        }
    }
}
