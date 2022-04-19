using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SeleneGame.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SeleneGame {
    
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class Spline : MonoBehaviour {

        private Mesh mesh;
        public MeshFilter _meshFilter;
        public MeshCollider _meshCollider;
        public Spline prevSegment, nextSegment;

        public OrientedPoint controlPoint1 => splineCurve.controlPoint1;
        public OrientedPoint controlPoint2 => splineCurve.controlPoint2;
        public OrientedPoint handle1 => splineCurve.handle1;
        public OrientedPoint handle2 => splineCurve.handle2;

        public BezierCurve splineCurve;

        public OrientedPoint GetBezier(float tVal) => splineCurve.GetPoint(tVal);

        public RepeatableMesh mesh2D;
        [Range(3, 128)]
        public int ringCount = 4;
        public float scale = 1f;
        private int segNbr = 1;

        public Vector3 oldPos;

        
        void OnEnable() => Global.objectManager.objectList.Add( this.gameObject );
        void OnDisable() => Global.objectManager.objectList.Remove( this.gameObject );

        private void Awake() {
            UpdateMesh();
        }

        private void Reset(){
            Vector3 cp1 = transform.position - Vector3.forward * 3;
            Vector3 cp2 = transform.position + Vector3.forward * 3;
            Vector3 h1 = transform.position - Vector3.forward * 2;
            Vector3 h2 = transform.position + Vector3.forward * 2;
            splineCurve = new BezierCurve(cp1, cp2, h1, h2);
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
                // OrientedPoint op = GetBezierCurve(new OrientedPoint(controlPoint1), new OrientedPoint(controlPoint2), new OrientedPoint(handle1), new OrientedPoint(handle2), t);
                OrientedPoint op = GetBezier(t);

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
            UpdateMesh();
            if(nextSegment != null){
                nextSegment.controlPoint1.Set(controlPoint2);
                nextSegment.handle1.Set(nextSegment.controlPoint1.position + (controlPoint2.position - handle2.position));
                nextSegment.UpdateMesh();
            }
            if(prevSegment != null){
                prevSegment.controlPoint2.Set(controlPoint1);
                prevSegment.handle2.Set(prevSegment.controlPoint2.position + (controlPoint1.position - handle1.position));
                prevSegment.UpdateMesh();
            }
        }

        public void AddNext(){
            Vector3 displacement = controlPoint2.position - controlPoint1.position;
            nextSegment = Instantiate(gameObject, transform.position + displacement, transform.rotation, transform.parent).GetComponent<Spline>();
            nextSegment.prevSegment = this;

            nextSegment.splineCurve.Move(displacement);
            nextSegment.segNbr = segNbr + 1;
            nextSegment.gameObject.name = "Segment" + nextSegment.segNbr;

            UpdateOtherSegments();
        }
        public void RemoveNext() {
            if (nextSegment.nextSegment != null)
                nextSegment.nextSegment.prevSegment = null;
            Global.SafeDestroy(nextSegment.gameObject);
            nextSegment = null;
        }

        public void AddPrev(){
            Vector3 displacement = controlPoint1.position - controlPoint2.position;
            prevSegment = Instantiate(gameObject, transform.position + displacement, transform.rotation, transform.parent).GetComponent<Spline>();
            prevSegment.nextSegment = this;

            prevSegment.splineCurve.Move(displacement);
            prevSegment.segNbr = segNbr - 1;
            prevSegment.gameObject.name = "Segment" + prevSegment.segNbr;
            
            UpdateOtherSegments();
        }
        public void RemovePrev(){
            if (prevSegment.prevSegment != null)
                prevSegment.prevSegment.nextSegment = null;
            Global.SafeDestroy(prevSegment.gameObject);
            prevSegment = null;
        }


        #if UNITY_EDITOR
        public void OnDrawGizmosSelected(){

            Handles.DrawBezier( controlPoint1.position, controlPoint2.position, handle1.position, handle2.position, Color.white, EditorGUIUtility.whiteTexture, 1f );


            for (int i = 0; i < ringCount; i++){
                OrientedPoint pointAlongTessel = GetBezier( (float)i/(float)ringCount );
                // Vector3 velocityAlongTessel = GetVelocity( (float)i/(float)ringCount );
                // Vector3 accelerationAlongTessel = GetAcceleration( (float)i/(float)ringCount );

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

            if ( transform.hasChanged ){
                Vector3 movement = transform.position - oldPos;

                splineCurve.Move(movement);
                oldPos += movement;

                transform.hasChanged = false;

                UpdateOtherSegments();
            }
        }

        private void OnValidate(){
            UpdateOtherSegments();
        }

        #endif
    }
}