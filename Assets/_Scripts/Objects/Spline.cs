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

        public ref OrientedPoint controlPoint1 => ref splineCurve.controlPoint1;
        public ref OrientedPoint controlPoint2 => ref splineCurve.controlPoint2;
        public ref OrientedPoint handle1 => ref splineCurve.handle1;
        public ref OrientedPoint handle2 => ref splineCurve.handle2;
        public OrientedPoint transformedControlPoint1 => transform.TransformPoint(splineCurve.controlPoint1);
        public OrientedPoint transformedControlPoint2 => transform.TransformPoint(splineCurve.controlPoint2);
        public OrientedPoint transformedHandle1 => transform.TransformPoint(splineCurve.handle1);
        public OrientedPoint transformedHandle2 => transform.TransformPoint(splineCurve.handle2);

        public BezierCurve splineCurve;

        public OrientedPoint GetBezier(float tVal) => transform.TransformPoint(splineCurve.GetPoint(tVal));

        public RepeatableMesh mesh2D;
        [Range(3, 128)]
        public int ringCount = 4;
        public float scale = 1f;
        private int segNbr = 1;


        
        void OnEnable() => Global.objectManager.objectList.Add( this.gameObject );
        void OnDisable() => Global.objectManager.objectList.Remove( this.gameObject );

        private void Awake() {
            UpdateMesh();
        }

        private void Reset(){
            Vector3 cp1 = Vector3.forward * 0f;
            Vector3 cp2 = Vector3.forward * 75f;
            Vector3 h1 = Vector3.forward * 25f;
            Vector3 h2 = Vector3.forward * 50f;
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
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> triangles = new List<int>();

            for (int ring = 0; ring < ringCount; ring++){

                float t = ring / (ringCount-1f);
                OrientedPoint op = splineCurve.GetPoint(t);

                for (int j = 0; j < mesh2D.vertexCount; j++){
                    vertices.Add(op.position + (op.rotation * mesh2D.vertices[j].point)*scale);
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
                nextSegment.controlPoint1.Set(nextSegment.transform.InverseTransformPoint(transformedControlPoint2));
                Vector3 displacement = transformedControlPoint2.position - transformedHandle2.position;
                nextSegment.handle1.Set( nextSegment.transform.InverseTransformPoint(nextSegment.transformedControlPoint1.position + displacement) );
                nextSegment.UpdateMesh();
            }
            if(prevSegment != null){
                prevSegment.controlPoint2.Set(prevSegment.transform.InverseTransformPoint(transformedControlPoint1));
                Vector3 displacement = transformedControlPoint1.position - transformedHandle1.position;
                prevSegment.handle2.Set( prevSegment.transform.InverseTransformPoint(prevSegment.transformedControlPoint2.position + displacement) );
                prevSegment.UpdateMesh();
            }
        }

        public void AddNext(){
            Vector3 displacement = controlPoint2.position - controlPoint1.position;
            nextSegment = Instantiate(gameObject, transform.position + displacement, transform.rotation, transform.parent).GetComponent<Spline>();
            nextSegment.prevSegment = this;

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

        private void OnValidate(){
            UpdateOtherSegments();
        }

    }
}