using System;
using System.Collections.Generic;
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
            }
            return -1;
        }
    }


    public class MeshAsset : EngineAsset
    {
        public List<float> vertices;
        public List<uint> indices;
        public VertexLayout layout;
    }

    public class ModelAsset : EngineAsset
    {
        public string name;
        public string path;
        public List<MeshAsset> meshes;
    }
}
