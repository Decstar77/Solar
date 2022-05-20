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
    }

    public static class VertexLayoutExtensions
    {
        public static uint GetStride(this VertexLayout layout)
        {
            switch (layout)
            {
                case VertexLayout.P:    return 3;
                case VertexLayout.PNT:  return (3 + 3 + 2);
                default: Debug.Assert(false); return 0;
            }

            return 0;
        }

        public static int GetStrideBytes(this VertexLayout vertexLayout)
        {
            switch (vertexLayout)
            {
                case VertexLayout.P:    return 4 * 3;
                case VertexLayout.PNT:  return 4 * (3 + 3 + 2);
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
        //public List<float> vertices;
        public List<Vector3> positions;
        public List<Vector3> normals;
        public List<Vector2> uvs;
        
        public List<uint> indices;
        public AlignedBox alignedBox;
        public string materialName;  
        
        public List<float> PackedVertices(VertexLayout layout)
        {
            List<float> vertices = new List<float>();
            switch (layout)
            {
                case VertexLayout.PNT: {
                        Debug.Assert(positions.Count == normals.Count && normals.Count == uvs.Count);
                        for (int i = 0; i < positions.Count; i++) {
                            vertices.Add(positions[i].x);
                            vertices.Add(positions[i].y);
                            vertices.Add(positions[i].z);
                            vertices.Add(normals[i].x);
                            vertices.Add(normals[i].y);
                            vertices.Add(normals[i].z);
                            vertices.Add(uvs[i].x);
                            vertices.Add(uvs[i].y);
                        }
                        return vertices;
                    }
                default: Debug.Assert(false); return vertices;
            }
        }

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
                triangle.a = positions[index1];
                triangle.b = positions[index2];
                triangle.c = positions[index3];

                triangles.Add(triangle);
            }

            return triangles;
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
