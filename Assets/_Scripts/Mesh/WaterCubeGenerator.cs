using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {

    [RequireComponent(typeof(WaterController))] 
    [RequireComponent(typeof(MeshFilter))]
    public class WaterCubeGenerator : MonoBehaviour{
        
        private MeshFilter filter;
        private new BoxCollider collider;
        private WaterController waterController;
        [SerializeField] [Range(1, 1000)] private int meshSize;
        /* [SerializeField]  */[Range(1, 100)] private int meshHeight;
        public float squareSize;
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

            meshHeight = meshSize;

            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();

            var faceVertexCount = meshSize + 1;
            var faceTriangleCount = faceVertexCount * faceVertexCount - faceVertexCount;
            var triangles = new List<int>();

            for (int x = 0; x < faceVertexCount; x++){
                for (int y = 0; y < faceVertexCount; y++){
                    vertices.Add(new Vector3(-meshSize * 0.5f * squareSize + meshSize *(x / ((float)meshSize))* squareSize, 0, -meshSize * 0.5f * squareSize + meshSize *(y / ((float)meshSize))* squareSize));
                    normals.Add(Vector3.up);
                    uvs.Add(new Vector3(x / (float)meshSize, y / (float)meshSize));
                }
            }
            for(int i = 0; i < (faceTriangleCount); i++){
                if ((i+1) % faceVertexCount == 0){
                    continue;
                }
                triangles.AddRange(new List<int>(){
                    i + 1 + faceVertexCount, i+faceVertexCount, i,
                    i, i+1, i+faceVertexCount+1

                });
            }
            
            for (int x = 0; x < faceVertexCount; x++){
                for (int y = 0; y < faceVertexCount; y++){
                    vertices.Add(new Vector3(meshSize * squareSize * 0.5f, meshHeight *(x / ((float)meshHeight))* -squareSize, -meshSize * 0.5f * squareSize + meshHeight *(y / ((float)meshHeight))* squareSize));
                    normals.Add(Vector3.right);
                    uvs.Add(new Vector3(x / (float)meshSize, y / (float)meshSize));
                }
            }
            
            for(int i = faceTriangleCount+faceVertexCount; i < faceTriangleCount*2+faceVertexCount*1; i++){
                if ((i+1) % faceVertexCount == 0){
                    continue;
                }
                triangles.AddRange(new List<int>(){
                    i + 1 + faceVertexCount, i+faceVertexCount, i,
                    i, i+1, i+faceVertexCount+1

                });
            }

            
            for (int x = 0; x < faceVertexCount; x++){
                for (int y = 0; y < faceVertexCount; y++){
                    vertices.Add(new Vector3(-meshSize * squareSize * 0.5f, meshHeight *(x / ((float)meshHeight))* -squareSize, -meshSize * 0.5f * squareSize + meshHeight *(y / ((float)meshHeight))* squareSize));
                    normals.Add(-Vector3.right);
                    uvs.Add(new Vector3(x / (float)meshSize, y / (float)meshSize));
                }
            }
            for(int i = (faceTriangleCount+faceVertexCount)*2; i < faceTriangleCount*3+faceVertexCount*2; i++){
                if ((i+1) % faceVertexCount == 0){
                    continue;
                }
                triangles.AddRange(new List<int>(){
                    i + 1 + faceVertexCount, i+faceVertexCount, i,
                    i, i+1, i+faceVertexCount+1

                });
            }

            
            for (int x = 0; x < faceVertexCount; x++){
                for (int y = 0; y < faceVertexCount; y++){
                    vertices.Add(new Vector3( -meshSize * 0.5f * squareSize + meshHeight *(y / ((float)meshHeight))* squareSize, meshHeight *(x / ((float)meshHeight))* -squareSize, meshSize * squareSize * 0.5f ) );
                    normals.Add(Vector3.forward);
                    uvs.Add(new Vector3(x / (float)meshSize, y / (float)meshSize));
                }
            }
            for(int i = (faceTriangleCount+faceVertexCount)*3; i < faceTriangleCount*4+faceVertexCount*3; i++){
                if ((i+1) % faceVertexCount == 0){
                    continue;
                }
                triangles.AddRange(new List<int>(){
                    i + 1 + faceVertexCount, i+faceVertexCount, i,
                    i, i+1, i+faceVertexCount+1

                });
            }

            for (int x = 0; x < faceVertexCount; x++){
                for (int y = 0; y < faceVertexCount; y++){
                    vertices.Add(new Vector3( -meshSize * 0.5f * squareSize + meshHeight *(y / ((float)meshHeight))* squareSize, meshHeight *(x / ((float)meshHeight))* -squareSize, -meshSize * squareSize * 0.5f ) );
                    normals.Add(new Vector3(0,0,-1));
                    uvs.Add(new Vector3(x / (float)meshSize, y / (float)meshSize));
                }
            }
            for(int i = (faceTriangleCount+faceVertexCount)*4; i < faceTriangleCount*5+faceVertexCount*4; i++){
                if ((i+1) % faceVertexCount == 0){
                    continue;
                }
                triangles.AddRange(new List<int>(){
                    i + 1 + faceVertexCount, i+faceVertexCount, i,
                    i, i+1, i+faceVertexCount+1

                });
            }


            for (int x = 0; x < faceVertexCount; x++){
                for (int y = 0; y < faceVertexCount; y++){
                    vertices.Add(new Vector3(-meshSize * 0.5f * squareSize + meshSize *(x / ((float)meshSize))* squareSize, -meshHeight * squareSize, -meshSize * 0.5f * squareSize + meshSize *(y / ((float)meshSize))* squareSize));
                    normals.Add(Vector3.down);
                    uvs.Add(new Vector3(x / (float)meshSize, y / (float)meshSize));
                }
            }
            for(int i = (faceTriangleCount+faceVertexCount)*5; i < faceTriangleCount*6+faceVertexCount*5; i++){
                if ((i+1) % faceVertexCount == 0){
                    continue;
                }
                triangles.AddRange(new List<int>(){
                    i + 1 + faceVertexCount, i+faceVertexCount, i,
                    i, i+1, i+faceVertexCount+1

                });
            }

            plane.SetVertices(vertices);
            plane.SetNormals(normals);
            plane.SetUVs(0, uvs);
            plane.SetTriangles(triangles, 0);

            if (waterController != null){
                collider.size = new Vector3(squareSize*meshSize,(meshHeight*squareSize) + Mathf.Abs(waterController.waveStrength) + waterController.noiseStrength + waterMargin,squareSize*meshSize);
                collider.center = new Vector3(collider.center.x,(-(meshHeight*squareSize) + Mathf.Abs(waterController.waveStrength) + waterController.noiseStrength + waterMargin)/2f,collider.center.z);
            }else{
                collider.size = new Vector3(squareSize*meshSize, (meshHeight*squareSize) + waterMargin, squareSize*meshSize);
                collider.center = new Vector3(collider.center.x,(-(meshHeight*squareSize) + waterMargin)/2f,collider.center.z);
            }

            return plane;
        }
    }
}