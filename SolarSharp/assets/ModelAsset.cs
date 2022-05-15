using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Assets
{
    public enum VertexLayout
    {
        INVALID = 0,
        P,      // @NOTE: Postion
        P_PAD,  // @NOTE: Postion and a padd
        PNT,    // @NOTE: Postions, normal, texture coords(uv)
        PNTC,   // @NOTE: Postions, normal, texture coords(uv), vertex colour
        PNTM,   // @NOTE: Postions, normal, texture coords(uv), and an instanced model transform matrix
        TEXT,   // @NOTE: Layout for text rendering
        PC,     // @NOTE: Postion and Colour
        COUNT,
    }

    public static class VertexLayoutExtensions
    {
        public static uint GetStride(this VertexLayout layout)
        {
            switch (layout)
            {
                case VertexLayout.P: return 3;
                case VertexLayout.PNT: return (3 + 3 + 2);
                default: Debug.Assert(false); return 0;
            }

            return 0;
        }

        public static int GetStrideBytes(this VertexLayout vertexLayout)
        {
            switch (vertexLayout)
            {
                case VertexLayout.INVALID:
                    return -1;
                case VertexLayout.P:
                    return 4 * 3;
                case VertexLayout.P_PAD:
                    return -1;
                case VertexLayout.PNT:
                    return 4 * (3 + 3 + 2);
                case VertexLayout.PNTC:
                    return -1;
                case VertexLayout.PNTM:
                    return -1;
                case VertexLayout.TEXT:
                    return -1;
                case VertexLayout.PC:
                    return -1;
                case VertexLayout.COUNT:
                    return -1;
                default: Debug.Assert(false); return 0;
            }
            return -1;
        }
    }

    public class ImportedMaterial
    {

    }

    public struct MeshVertex
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector2 uv;
        public Vector3 color;
    }

    public class MeshAsset : EngineAsset
    {
        public string name;
        public List<float> vertices;
        public List<uint> indices;
        public VertexLayout layout;
        public AlignedBox alignedBox;
        public string materialName;  
        
        public List<Triangle> BuildTriangles()
        {
            List<Triangle> triangles = new List<Triangle>();
            // @NOTE: Assumes the mesh is triangluated
            for (int i = 0 ; i < indices.Count; i += 3)
            {
                int index1 = (int)indices[i];
                int index2 = (int)indices[i + 1];
                int index3 = (int)indices[i + 2];

                Triangle triangle = new Triangle();
                triangle.a =  GetMeshVertex(index1).position;
                triangle.b = GetMeshVertex(index2).position;
                triangle.c = GetMeshVertex(index3).position;

                triangles.Add(triangle);
            }

            return triangles;
        }

        public MeshVertex GetMeshVertex(int index)
        {
            MeshVertex vertex = new MeshVertex();

            index *= (int)layout.GetStride();
            switch (layout)
            {
                case VertexLayout.PNT: {    
                        vertex.position.x = vertices[index];
                        vertex.position.y = vertices[index + 1];
                        vertex.position.z = vertices[index + 2];

                        vertex.normal.x = vertices[index + 3];
                        vertex.normal.y = vertices[index + 4];
                        vertex.normal.z = vertices[index + 5];

                        vertex.uv.x = vertices[index + 6];
                        vertex.uv.y = vertices[index + 7];
                    }
                    break;

                default: 
                    Debug.Assert(false);
                    break;
            }

            return vertex;
        }
    }

    public class ModelAsset : EngineAsset
    {
        public string name;
        public string path;
        public AlignedBox alignedBox;
        public List<MeshAsset> meshes;
    }
}
