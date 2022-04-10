using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeleneGame {
    
    [CreateAssetMenu(fileName = "new 2D Mesh", menuName = "2D Mesh")]
    public class RepeatableMesh : ScriptableObject{
        
        [System.Serializable] public class Vertex{
            public Vector2 point;
            public Vector2 normal;
            public float UCoord;
        }
        [System.Serializable] public class Segment{
            public int vert1;
            public int vert2;
        }
        
        public Vertex[] vertices;

        public Segment[] segmentIndices;

        public int vertexCount => vertices.Length;
        public int lineCount => segmentIndices.Length;
    }
}