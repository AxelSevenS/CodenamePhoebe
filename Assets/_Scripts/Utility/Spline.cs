using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace SevenGame.Utility {
    
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class Spline : MonoBehaviour {

        private Mesh mesh;

        private MeshFilter _meshFilter;
        public MeshFilter meshFilter {
            get {
                if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
                return _meshFilter;
            }
        }
        private MeshCollider _meshCollider;
        public MeshCollider meshCollider {
            get {
                if (_meshCollider == null) _meshCollider = GetComponent<MeshCollider>();
                return _meshCollider;
            }
        }
        public Spline prevSegment, nextSegment;

        public ref OrientedPoint controlPoint1 => ref splineCurve.controlPoint1;
        public ref OrientedPoint controlPoint2 => ref splineCurve.controlPoint2;
        public ref OrientedPoint handle1 => ref splineCurve.handle1;
        public ref OrientedPoint handle2 => ref splineCurve.handle2;

        public OrientedPoint TransformPoint(OrientedPoint point) => transform.TransformPoint(point);
        public OrientedPoint InverseTransformPoint(OrientedPoint point) => transform.InverseTransformPoint(point);

        public BezierCubic splineCurve;

        public OrientedPoint GetBezier(float tVal) => TransformPoint(splineCurve.GetPoint(tVal));

        public RepeatableMesh mesh2D;
        [Range(3, 128)]
        public int ringCount = 4;
        public float scale = 1f;



        private void Awake() {
            // UpdateMesh();
        }

        private void Reset(){
            splineCurve = new BezierCubic();
            splineCurve.Reset();
            Awake();
        }

        public void UpdateMesh(){

            if (mesh2D == null) {
                meshFilter.sharedMesh = null;
                meshCollider.sharedMesh = null;
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

            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        public void UpdateOtherSegments(){
            UpdateMesh();
            if(nextSegment != null){
                nextSegment.controlPoint1.Set(nextSegment.InverseTransformPoint(TransformPoint(controlPoint2)));
                Vector3 displacement = TransformPoint(controlPoint2).position - TransformPoint(handle2).position;
                nextSegment.handle1.Set( nextSegment.InverseTransformPoint(nextSegment.TransformPoint(nextSegment.controlPoint1) + displacement) );
                nextSegment.UpdateMesh();
            }
            if(prevSegment != null){
                prevSegment.controlPoint2.Set(prevSegment.InverseTransformPoint(TransformPoint(controlPoint1)));
                Vector3 displacement = TransformPoint(controlPoint1).position - TransformPoint(handle1).position;
                prevSegment.handle2.Set( prevSegment.InverseTransformPoint(prevSegment.TransformPoint(prevSegment.controlPoint2) + displacement) );
                prevSegment.UpdateMesh();
            }
        }

        public void AddNext(){
            Vector3 displacement = controlPoint2.position - controlPoint1.position;
            nextSegment = Instantiate(gameObject, transform.position + displacement, transform.rotation, transform.parent).GetComponent<Spline>();
            nextSegment.prevSegment = this;

            nextSegment.gameObject.name = "Segment";

            UpdateOtherSegments();
        }
        public void RemoveNext() {
            if (nextSegment.nextSegment != null)
                nextSegment.nextSegment.prevSegment = null;
            GameUtility.SafeDestroy(nextSegment.gameObject);
            nextSegment = null;
        }

        public void AddPrev(){
            Vector3 displacement = controlPoint1.position - controlPoint2.position;
            prevSegment = Instantiate(gameObject, transform.position + displacement, transform.rotation, transform.parent).GetComponent<Spline>();
            prevSegment.nextSegment = this;

            prevSegment.gameObject.name = "Segment";
            
            UpdateOtherSegments();
        }
        public void RemovePrev(){
            if (prevSegment.prevSegment != null)
                prevSegment.prevSegment.nextSegment = null;
            GameUtility.SafeDestroy(prevSegment.gameObject);
            prevSegment = null;
        }

        // private void OnValidate(){
        //     UpdateOtherSegments();
        // }

    }
}