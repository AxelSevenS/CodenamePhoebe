using System;
using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace SeleneGame.Core {
    
    public static class PropertyDrawerUtility {

        public static float lineSpace = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

        public static object GetTargetObject(SerializedProperty prop) {

            SerializedObject serializedObject = prop.serializedObject;
            string path = prop.propertyPath;
        
            object propertyObject = serializedObject == null || serializedObject.targetObject == null ? null : serializedObject.targetObject;
            System.Type objectType = propertyObject == null ? null : propertyObject.GetType();
            
            if (!string.IsNullOrEmpty(path) && propertyObject != null) {
                string[] splitPath = path.Split('.');
                Type fieldType = null;

                //work through the given property path, node by node
                for (int i = 0; i < splitPath.Length; i++) {
                    string pathNode = splitPath[i];

                    //both arrays and lists implement the IList interface
                    if (fieldType != null && typeof(IList).IsAssignableFrom(fieldType)) {
                        //IList items are serialized like this: `Array.data[0]`
                        Debug.AssertFormat(pathNode.Equals("Array", StringComparison.Ordinal), serializedObject.targetObject, "Expected path node 'Array', but found '{0}'", pathNode);

                        //just skip the `Array` part of the path
                        pathNode = splitPath[++i];

                        //match the `data[0]` part of the path and extract the IList item index
                        Regex matchArrayElement = new Regex(@"^data\[(\d+)\]$");
                        Match elementMatch = matchArrayElement.Match(pathNode);
                        int index;
                        if (elementMatch.Success && int.TryParse(elementMatch.Groups[1].Value, out index)) {
                            IList objectArray = (IList)propertyObject;
                            bool validArrayEntry = objectArray != null && index < objectArray.Count;
                            propertyObject = validArrayEntry ? objectArray[index] : null;
                            objectType = fieldType.IsArray
                                ? fieldType.GetElementType()          //only set for arrays
                                : fieldType.GenericTypeArguments[0];  //type of `T` in List<T>
                        }
                        else {
                            Debug.LogErrorFormat(serializedObject.targetObject, "Unexpected path format for array item: '{0}'", pathNode);
                        }
                        //reset fieldType, so we don't end up in the IList branch again next iteration
                        fieldType = null;

                    } else {

                        FieldInfo field;
                        Type instanceType = objectType;
                        BindingFlags fieldBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
                        do {
                            field = instanceType.GetField(pathNode, fieldBindingFlags);

                            //b/c a private, serialized field of a subclass isn't directly retrievable,
                            fieldBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
                            //if neccessary, work up the inheritance chain until we find it.
                            instanceType = instanceType.BaseType;
                        }
                        while (field == null && instanceType != typeof(object));

                        //store object info for next iteration or to return
                        propertyObject = field == null || propertyObject == null ? null : field.GetValue(propertyObject);
                        fieldType = field == null ? null : field.FieldType;
                        objectType = fieldType;
                    }
                }
            }
            
            return propertyObject;
            
        }

        public static T GetTargetObject<T>(SerializedProperty prop) where T : class {
            
            SerializedObject serializedObject = prop.serializedObject;
            string path = prop.propertyPath;

            Type fieldType = typeof(T);
        
            object propertyObject = serializedObject == null || serializedObject.targetObject == null ? null : serializedObject.targetObject;
            if ( !(propertyObject.GetType().IsSubclassOf(fieldType) || propertyObject is T) ) return null;

            T TObject = propertyObject as T;

            if (!string.IsNullOrEmpty(path) && TObject != null) {
                string[] splitPath = path.Split('.');

                //work through the given property path, node by node
                for (int i = 0; i < splitPath.Length; i++) {
                    string pathNode = splitPath[i];

                    //both arrays and lists implement the IList interface
                    if (fieldType != null && typeof(IList).IsAssignableFrom(fieldType)) {
                        //IList items are serialized like this: `Array.data[0]`
                        Debug.AssertFormat(pathNode.Equals("Array", StringComparison.Ordinal), serializedObject.targetObject, "Expected path node 'Array', but found '{0}'", pathNode);

                        //just skip the `Array` part of the path
                        pathNode = splitPath[++i];

                        //match the `data[0]` part of the path and extract the IList item index
                        Regex matchArrayElement = new Regex(@"^data\[(\d+)\]$");
                        Match elementMatch = matchArrayElement.Match(pathNode);
                        int index;
                        if (elementMatch.Success && int.TryParse(elementMatch.Groups[1].Value, out index)) {
                            IList objectArray = (IList)TObject;
                            bool validArrayEntry = objectArray != null && index < objectArray.Count;
                            TObject = validArrayEntry ? (T)objectArray[index] : null;
                        }
                        else {
                            Debug.LogErrorFormat(serializedObject.targetObject, "Unexpected path format for array item: '{0}'", pathNode);
                        }
                        //reset fieldType, so we don't end up in the IList branch again next iteration
                        fieldType = null;

                    } else {

                        FieldInfo field;
                        Type instanceType = typeof(T);
                        BindingFlags fieldBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
                        do {
                            field = instanceType.GetField(pathNode, fieldBindingFlags);

                            //b/c a private, serialized field of a subclass isn't directly retrievable,
                            fieldBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
                            //if neccessary, work up the inheritance chain until we find it.
                            instanceType = instanceType.BaseType;
                        }
                        while (field == null && instanceType != typeof(object));

                        //store object info for next iteration or to return
                        TObject = field == null || TObject == null ? null : (T)field.GetValue(TObject);
                        fieldType = field == null ? null : field.FieldType;
                    }
                }
            }
            
            return TObject;
        }
    }
}
