using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SeleneGame {
    
    [RequireComponent(typeof(MeshFilter))]
    public class Spline : MonoBehaviour{

        private Mesh mesh;
        public MeshFilter _meshFilter;
        public MeshCollider _meshCollider;
        public Transform controlPoint1;
        public Transform controlPoint2;
        public Transform handle1;
        public Transform handle2;
        public Spline prevSegment, nextSegment;

        private Mathfs.BezierCurve splineCurve => new Mathfs.BezierCurve( controlPoint1, controlPoint2, handle1, handle2);

        public Mathfs.OrientedPoint GetBezier(float tVal) => splineCurve.GetPoint(tVal);

        public RepeatableMesh mesh2D;
        [Range(3, 128)]
        public int ringCount = 4;
        public float scale = 1f;
        private int segNbr = 1;


        
        void OnEnable() => Global.objectManager.objectList.Add( this.gameObject );
        void OnDisable() => Global.objectManager.objectList.Remove( this.gameObject );

        private void Awake(){
            UpdateMesh();
        }

        private void Reset(){
            Awake();
        }

        public void UpdateMesh(){
            if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
            if (_meshCollider == null) _meshCollider = GetComponent<MeshCollider>();

            if (mesh2D == null) {
                _meshFilter.sharedMesh = null;
                _meshCollider.sharedMesh = null;
                return;
            }

            if (mesh != null) {
                mesh.Clear();
            }else{
                mesh = new Mesh();
                mesh.name = $"Procedural {mesh2D.name} mesh";
            }
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();
            var triangles = new List<int>();

            for (int ring = 0; ring < ringCount; ring++){

                float t = ring / (ringCount-1f);
                // Mathfs.OrientedPoint op = Mathfs.GetBezierCurve(new Mathfs.OrientedPoint(controlPoint1), new Mathfs.OrientedPoint(controlPoint2), new Mathfs.OrientedPoint(handle1), new Mathfs.OrientedPoint(handle2), t);
                Mathfs.OrientedPoint op = splineCurve.GetPoint(t);

                for (int j = 0; j < mesh2D.vertexCount; j++){
                    vertices.Add(op.position - this.transform.position + (op.rotation * mesh2D.vertices[j].point)*scale);
                    normals.Add(op.rotation * mesh2D.vertices[j].normal);
                    uvs.Add(new Vector2(mesh2D.vertices[j].UCoord, t));

                }
            }

            for (int ring = 0; ring < (ringCount-1f); ring++){
                int rootIndex = ring * mesh2D.vertexCount;
                int rootIndexNext = (ring+1) * mesh2D.vertexCount;

                for (int line = 0; line < mesh2D.lineCount; line++){
                    int lineIndexA = mesh2D.segmentIndices[line].vert1;
                    int lineIndexB = mesh2D.segmentIndices[line].vert2;

                    int currentA = rootIndex + lineIndexA;
                    int currentB = rootIndex + lineIndexB;
                    int nextA = rootIndexNext + lineIndexA;
                    int nextB = rootIndexNext + lineIndexB;

                    triangles.Add(currentA);
                    triangles.Add(nextA);
                    triangles.Add(nextB);
                    triangles.Add(currentA);
                    triangles.Add(nextB);
                    triangles.Add(currentB);
                }

            }

            triangles.Add(0);
            triangles.Add(6);
            triangles.Add(5);

            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uvs);

            mesh.SetTriangles(triangles, 0);

            _meshFilter.sharedMesh = mesh;
            _meshCollider.sharedMesh = mesh;
        }

        public void UpdateOtherSegments(){
            if(nextSegment != null){
                nextSegment.controlPoint1.transform.position = controlPoint2.transform.position;
                nextSegment.controlPoint1.transform.rotation = controlPoint2.transform.rotation;
                nextSegment.handle1.transform.position = nextSegment.controlPoint1.transform.position + (controlPoint2.transform.position - handle2.transform.position);
                nextSegment.UpdateMesh();
            }
            if(prevSegment != null){
                prevSegment.controlPoint2.transform.position = controlPoint1.transform.position;
                prevSegment.controlPoint2.transform.rotation = controlPoint1.transform.rotation;
                prevSegment.handle2.transform.position = prevSegment.controlPoint2.transform.position + (controlPoint1.transform.position - handle1.transform.position);
                prevSegment.UpdateMesh();
            }
            UpdateMesh();
        }

        public void AddNext(){
            var newSeg = Instantiate(gameObject, controlPoint2.transform.position + (controlPoint2.transform.position - transform.position), transform.rotation, transform.parent).GetComponent<Spline>();
            newSeg.prevSegment = this;
            nextSegment = newSeg;
            newSeg.segNbr = segNbr + 1;
            newSeg.gameObject.name = "Segment" + newSeg.segNbr;
        }
        public void RemoveNext(){
            if (nextSegment.nextSegment != null)
                nextSegment.nextSegment.prevSegment = null;
            Global.SafeDestroy(nextSegment.gameObject);
            nextSegment = null;
        }
        public void AddPrev(){
            var newSeg = Instantiate(gameObject, controlPoint1.transform.position + (controlPoint1.transform.position - transform.position), transform.rotation, transform.parent).GetComponent<Spline>();
            newSeg.nextSegment = this;
            prevSegment = newSeg;
            newSeg.segNbr = segNbr - 1;
            newSeg.gameObject.name = "Segment" + newSeg.segNbr;
        }
        public void RemovePrev(){
            if (prevSegment.prevSegment != null)
                prevSegment.prevSegment.nextSegment = null;
            Global.SafeDestroy(prevSegment.gameObject);
            prevSegment = null;
        }


        #if UNITY_EDITOR
        public void OnDrawGizmos(){
            // Draw Gizmos only if Object or Child of Object is Selected
            if (Selection.Contains(controlPoint1.gameObject) || Selection.Contains(controlPoint2.gameObject) || Selection.Contains(handle1.gameObject) || Selection.Contains(handle2.gameObject) || Selection.Contains(gameObject)){
                DrawGizmos();
                UpdateOtherSegments();
                return;
            }
        }
        
        private void DrawGizmos(){

            Mathfs.BezierCurve currentBezier = splineCurve;

            Gizmos.DrawSphere(currentBezier.controlPoint1.position, 0.3f);
            Gizmos.DrawSphere(currentBezier.controlPoint2.position, 0.3f);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(currentBezier.handle1.position, 0.3f);
            Gizmos.DrawSphere(currentBezier.handle2.position, 0.3f);

            Handles.DrawBezier( currentBezier.controlPoint1.position, currentBezier.controlPoint2.position, currentBezier.handle1.position, currentBezier.handle2.position, Color.white, EditorGUIUtility.whiteTexture, 1f );


            for (int i = 0; i < ringCount; i++){
                Mathfs.OrientedPoint pointAlongTessel = currentBezier.GetPoint( (float)i/(float)ringCount );
                // Vector3 velocityAlongTessel = currentBezier.GetVelocity( (float)i/(float)ringCount );
                // Vector3 accelerationAlongTessel = currentBezier.GetAcceleration( (float)i/(float)ringCount );

                // Gizmos.color = Color.magenta;
                // Gizmos.DrawLine( pointAlongTessel.position, pointAlongTessel.position + velocityAlongTessel );

                // Gizmos.color = Color.blue;
                // Gizmos.DrawLine( pointAlongTessel.position, pointAlongTessel.position + accelerationAlongTessel );


                if (mesh2D == null) continue;
                Gizmos.color = Color.red;

                for (int j = 0; j < mesh2D.vertices.Length-1; j++)
                    Gizmos.DrawLine(pointAlongTessel.position + (pointAlongTessel.rotation * mesh2D.vertices[j].point)*scale, pointAlongTessel.position + (pointAlongTessel.rotation * mesh2D.vertices[j+1].point)*scale);
                Gizmos.DrawLine(pointAlongTessel.position + (pointAlongTessel.rotation * mesh2D.vertices[mesh2D.vertices.Length-1].point)*scale, pointAlongTessel.position + (pointAlongTessel.rotation * mesh2D.vertices[0].point)*scale);
                
                // Handles.PositionHandle(point.pos, point.rot);
            }
        }
        #endif
    }
}