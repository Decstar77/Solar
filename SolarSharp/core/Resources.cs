using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
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

    public class MeshResource
    {
        public List<float> vertices;
        public List<uint> indices;
        public VertexLayout layout;
    }

    public class ModelResource
    {
        public List<MeshResource> meshes;
    }


    public class Resources
    {


    }
}
