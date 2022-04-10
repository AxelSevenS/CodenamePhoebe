using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SeleneGame {

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Spline))]
    public class SplineEditor : Editor{

        SerializedObject so;
        SerializedProperty propControlPoint1;
        SerializedProperty propHandle1;
        SerializedProperty propControlPoint2;
        SerializedProperty propHandle2;
        SerializedProperty propNextSegment;
        SerializedProperty propPrevSegment;
        SerializedProperty propMesh2D;
        SerializedProperty propCount;
        SerializedProperty propScale;

        void OnEnable(){
            so = serializedObject;
            propControlPoint1 = so.FindProperty( "controlPoint1" );
            propHandle1 = so.FindProperty( "handle1" );
            propControlPoint2 = so.FindProperty( "controlPoint2" );
            propHandle2 = so.FindProperty( "handle2" );

            propNextSegment = so.FindProperty( "nextSegment" );
            propPrevSegment = so.FindProperty( "prevSegment" );

            propMesh2D = so.FindProperty( "mesh2D" );
            propCount = so.FindProperty( "ringCount" );
            propScale = so.FindProperty( "scale" );
        }
        
        public override void OnInspectorGUI(){
            /* DrawDefaultInspector(); */

            so.Update();

            Spline targetSpline = (Spline)target;

            GUILayout.Label( "Spline Control Points", EditorStyles.boldLabel );
            using( new GUILayout.HorizontalScope( EditorStyles.helpBox ) ){
                EditorGUIUtility.labelWidth = 100;
                using( new GUILayout.VerticalScope() ){
                    EditorGUILayout.PropertyField( propControlPoint1, new GUIContent("Control Point 1") );
                    EditorGUILayout.PropertyField( propHandle1, new GUIContent("Handle 1") );
                }
                using( new GUILayout.VerticalScope() ){
                    EditorGUILayout.PropertyField( propControlPoint2, new GUIContent("Control Point 2") );
                    EditorGUILayout.PropertyField( propHandle2, new GUIContent("Handle 2") );
                }
            }

            GUILayout.Space( 15 );

            GUILayout.Label( "Spline Segments", EditorStyles.boldLabel );
            using( new GUILayout.HorizontalScope( EditorStyles.helpBox ) ){
                EditorGUIUtility.labelWidth = 150;
                using( new GUILayout.VerticalScope( EditorStyles.helpBox ) ){
                    EditorGUILayout.PropertyField( propPrevSegment, new GUIContent("Previous Segment") );

                    GUILayout.Space( 5 );

                    if (targetSpline.prevSegment == null){
                        if (GUILayout.Button( "Add Previous Segment" )){
                            targetSpline.AddPrev();
                        }
                    }else{
                        if (GUILayout.Button( "Remove Previous Segment" )){
                            targetSpline.RemovePrev();
                        }
                    }
                }
                using( new GUILayout.VerticalScope( EditorStyles.helpBox ) ){
                    EditorGUILayout.PropertyField( propNextSegment, new GUIContent("Next Segment") );

                    GUILayout.Space( 5 );

                    if (targetSpline.nextSegment == null){
                        if (GUILayout.Button( "Add Next Segment" )){
                            targetSpline.AddNext();
                        }
                    }else{
                        if (GUILayout.Button( "Remove Next Segment" )){
                            targetSpline.RemoveNext();
                        }
                    }
                }
            }

            GUILayout.Space( 15 );

            GUILayout.Label( "Procedural Mesh", EditorStyles.boldLabel );
            using( new GUILayout.VerticalScope( EditorStyles.helpBox ) ){
                EditorGUILayout.PropertyField( propMesh2D );

                if (targetSpline.mesh2D != null){

                    EditorGUILayout.PropertyField( propCount, new GUIContent("Repeat Count") );
                    EditorGUILayout.PropertyField( propScale, new GUIContent("Scale of Mesh") );
                }
            }

            so.ApplyModifiedProperties();
        }
    }
}