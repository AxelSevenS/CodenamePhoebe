using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {

    public class GenerateIcosphere : MonoBehaviour{
        
        public float planetSize = 1f;
        Mesh planetMesh;
        Vector3[] planetVertices;
        int[] planetTriangles;
        MeshRenderer planetMeshRenderer;
        MeshFilter planetMeshFilter;
        MeshCollider planetMeshCollider;

        void Awake(){
            planetMeshFilter = GetComponent<MeshFilter>();
            planetMeshRenderer = GetComponent<MeshRenderer>();
            planetMeshCollider = GetComponent<MeshCollider>();

            CreateIcoSphereMesh();
        }

        void CreateIcoSphereMesh()
        {
            //need to set the material up top
            transform.localScale = new Vector3(planetSize, planetSize, planetSize);
            IcoSphere.Create(planetMeshFilter, 3);
            planetMeshFilter.mesh.RecalculateBounds();
            planetMeshFilter.mesh.RecalculateTangents();
            planetMeshFilter.mesh.RecalculateNormals();
        }
    }
}