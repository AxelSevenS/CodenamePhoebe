using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {

    [RequireComponent(typeof(WaterController))] 
    [RequireComponent(typeof(MeshFilter))]
    public class WaterPlaneGenerator : MonoBehaviour{
        
        private MeshFilter filter;
        private new BoxCollider collider;
        private WaterController waterController;
        [Range(1, 1000)] public int meshSize;
        public float squareSize;
        public float squareHeight;
        public float waterMargin = 5f;

        void Awake(){
            collider = GetComponent<BoxCollider>();
            filter = GetComponent<MeshFilter>();
            waterController = GetComponent<WaterController>();

            filter.mesh = GenerateMesh();
        }

        private void OnValidate(){
            Awake();
        }

        void Start(){
        }

        private Mesh GenerateMesh(){
            Mesh plane = new Mesh();
            plane.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();

            var vertexCount = meshSize + 1;
            var triangles = new List<int>();

            for (int x = 0; x < vertexCount; x++){
                for (int y = 0; y < vertexCount; y++){
                    vertices.Add(new Vector3(-meshSize * 0.5f * squareSize + meshSize *(x / ((float)meshSize))* squareSize, 0, -meshSize * 0.5f * squareSize + meshSize *(y / ((float)meshSize))* squareSize));
                    normals.Add(Vector3.up);
                    uvs.Add(new Vector3(x / (float)meshSize, y / (float)meshSize));
                }
            }
            for(int i = 0; i < vertexCount * vertexCount - vertexCount; i++){
                if ((i+1) % vertexCount == 0){
                    continue;
                }
                triangles.AddRange(new List<int>(){
                    i + 1 + vertexCount, i+vertexCount, i,
                    i, i+1, i+vertexCount+1

                });
            }

            plane.SetVertices(vertices);
            plane.SetNormals(normals);
            plane.SetUVs(0, uvs);
            plane.SetTriangles(triangles, 0);

            if (waterController != null){
                collider.size = new Vector3(squareSize*meshSize,squareHeight + Mathf.Abs(waterController.waveStrength) + waterController.noiseStrength + waterMargin,squareSize*meshSize);
                collider.center = new Vector3(collider.center.x,(-squareHeight + Mathf.Abs(waterController.waveStrength) + waterController.noiseStrength + waterMargin)/2f,collider.center.z);
            }else{
                collider.size = new Vector3(squareSize*meshSize, squareHeight + waterMargin, squareSize*meshSize);
                collider.center = new Vector3(collider.center.x,(-squareHeight + waterMargin)/2f,collider.center.z);
            }

            return plane;
        }
    }
}