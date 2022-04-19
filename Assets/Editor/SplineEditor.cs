using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SeleneGame.Core;

namespace SeleneGame.EditorUI {

    [CanEditMultipleObjects]
    [CustomEditor(typeof(Spline))]
    public class SplineEditor : Editor{

        private Vector3 oldPos; 
        private Spline targetSpline;

        private SerializedObject so;
        private SerializedProperty propSplineCurve;
        private SerializedProperty propControlPoint1;
        private SerializedProperty propControlPoint2;
        private SerializedProperty propHandle1;
        private SerializedProperty propHandle2;
        
        private SerializedProperty propNextSegment;
        private SerializedProperty propPrevSegment;
        
        private SerializedProperty propMesh2D;
        private SerializedProperty propCount;
        private SerializedProperty propScale;
        
        void OnEnable(){
            so = serializedObject;
            propSplineCurve = so.FindProperty( "splineCurve" );
            propControlPoint1 = propSplineCurve.FindPropertyRelative( "controlPoint1" );
            propControlPoint2 = propSplineCurve.FindPropertyRelative( "controlPoint2" ); 
            propHandle1 = propSplineCurve.FindPropertyRelative( "handle1" );
            propHandle2 = propSplineCurve.FindPropertyRelative( "handle2" );

            propNextSegment = so.FindProperty( "nextSegment" );
            propPrevSegment = so.FindProperty( "prevSegment" );

            propMesh2D = so.FindProperty( "mesh2D" );
            propCount = so.FindProperty( "ringCount" );
            propScale = so.FindProperty( "scale" );

            targetSpline = (Spline)target;
            targetSpline.oldPos = targetSpline.transform.position;
        }
        
        public override void OnInspectorGUI(){
            /* DrawDefaultInspector(); */

            GUILayout.Label( "Spline Control Points", EditorStyles.boldLabel );
            using ( new GUILayout.HorizontalScope( EditorStyles.helpBox ) ){
                EditorGUIUtility.labelWidth = 100;
                using ( new GUILayout.VerticalScope() ){ 
                    EditorGUILayout.PropertyField( propControlPoint1, new GUIContent("Control Point 1") );
                    EditorGUILayout.PropertyField( propHandle1, new GUIContent("Handle 1") );
                }
                using ( new GUILayout.VerticalScope() ){
                    EditorGUILayout.PropertyField( propControlPoint2, new GUIContent("Control Point 2") );
                    EditorGUILayout.PropertyField( propHandle2, new GUIContent("Handle 2") );
                }
            }

            GUILayout.Space( 15 );

            GUILayout.Label( "Spline Segments", EditorStyles.boldLabel );
            using ( new GUILayout.HorizontalScope( EditorStyles.helpBox ) ){
                EditorGUIUtility.labelWidth = 150;
                using ( new GUILayout.VerticalScope( EditorStyles.label ) ){
                    GUILayout.Label( "Previous Segment", EditorStyles.boldLabel );
                    EditorGUILayout.PropertyField( propPrevSegment, GUIContent.none );

                    GUILayout.Space( 5 );

                    if (targetSpline.prevSegment == null){
                        if (GUILayout.Button( "Add Previous Segment" )) targetSpline.AddPrev();
                    }else{
                        if (GUILayout.Button( "Remove Previous Segment" )) targetSpline.RemovePrev();
                    }
                }
                using ( new GUILayout.VerticalScope( EditorStyles.label ) ){
                    GUILayout.Label( "Next Segment", EditorStyles.boldLabel );
                    EditorGUILayout.PropertyField( propNextSegment, GUIContent.none );

                    GUILayout.Space( 5 );

                    if (targetSpline.nextSegment == null){
                        if (GUILayout.Button( "Add Next Segment" )) targetSpline.AddNext();
                    }else {
                        if (GUILayout.Button( "Remove Next Segment" )) targetSpline.RemoveNext();
                    }
                }
            }

            GUILayout.Space( 15 );

            GUILayout.Label( "Procedural Mesh", EditorStyles.boldLabel );
            using ( new GUILayout.VerticalScope( EditorStyles.helpBox ) ){
                EditorGUILayout.PropertyField( propMesh2D );

                if (targetSpline.mesh2D != null){

                    EditorGUILayout.PropertyField( propCount, new GUIContent("Repeat Count") );
                    EditorGUILayout.PropertyField( propScale, new GUIContent("Scale of Mesh") );
                }
            }

            so.ApplyModifiedProperties();
        }

        public void OnSceneGUI(){

            BezierCurve splineCurve = targetSpline.splineCurve;
            
		    EditorGUI.BeginChangeCheck();

            OrientedPoint newControlPoint1 = new OrientedPoint(splineCurve.controlPoint1);
            OrientedPoint newControlPoint2 = new OrientedPoint(splineCurve.controlPoint2);

            GUIStyle style = GUIStyle.none;
            style.fontSize = 15;
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;


            Handles.Label(newControlPoint1.position, "Control Point 1", style );
            Handles.Label(newControlPoint2.position, "Control Point 2", style );
            Handles.Label(splineCurve.handle1.position, "Handle 1", style );
            Handles.Label(splineCurve.handle2.position, "Handle 2", style );

            Handles.TransformHandle(ref newControlPoint1.position, ref newControlPoint1.rotation);
            Handles.TransformHandle(ref newControlPoint2.position, ref newControlPoint2.rotation);
            Vector3 newHandle1Pos = Handles.PositionHandle(splineCurve.handle1.position, Quaternion.identity);
            Vector3 newHandle2Pos = Handles.PositionHandle(splineCurve.handle2.position, Quaternion.identity);

            if ( EditorGUI.EndChangeCheck() ){

                Undo.RecordObject( target, "Edited Spline" ); 

                targetSpline.controlPoint1.Set(newControlPoint1);
                targetSpline.controlPoint2.Set(newControlPoint2);
                targetSpline.handle1.Set(newHandle1Pos);
                targetSpline.handle2.Set(newHandle2Pos);

                targetSpline.UpdateOtherSegments();

            }

            // // Check if a parent was moved recursively.
            // var currentParent = targetSpline.transform;
            // bool hasChanged = currentParent.hasChanged;
            // Debug.Log(hasChanged);
            // while (!hasChanged) {
            //     currentParent = currentParent.parent;
            //     if (currentParent == null) break;
            //     hasChanged = currentParent.hasChanged;
            // }

            // if ( hasChanged ){
            //     Debug.Log("pute");
            //     Vector3 movement = targetSpline.transform.position - oldPos;

            //     targetSpline.splineCurve.Move(movement);
            //     oldPos += movement;

            //     currentParent.hasChanged = false;

            //     targetSpline.UpdateOtherSegments();
            // }

            if ( targetSpline.transform.hasChanged && oldPos != Vector3.zero){
                Vector3 movement = targetSpline.transform.position - oldPos;

                targetSpline.splineCurve.Move(movement);
                oldPos += movement;

                targetSpline.transform.hasChanged = false;

                targetSpline.UpdateOtherSegments();
            }
        }
    }
}