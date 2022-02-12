using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering
{
    public class RenderEntry
    {
        public Vector3 position;
        public Quaternion orientation;
        public Vector3 scale;

        public RenderEntry()
        {
            position = Vector3.Zero;
            orientation = Quaternion.Identity;
            scale = new Vector3(1, 1, 1);
        }

        public RenderEntry(Vector3 position, Quaternion orientation, Vector3 scale)
        {
            this.position = position;
            this.orientation = orientation;
            this.scale = scale;
        }

        public Matrix4 ComputeTransformMatrix()
        {
            Matrix4 t = Matrix4.TranslateRH(Matrix4.Identity, position);
            Matrix4 o = Quaternion.ToMatrix4(orientation);
            Matrix4 s = Matrix4.ScaleCardinal(Matrix4.Identity, scale); 

            return t * o * s; 
        }
    }


    public class RenderPacket
    {
        public Matrix4 viewMatrix;
        public Matrix4 projectionMatrix;
        public List<RenderEntry> renderEntries = new List<RenderEntry>();
    }
}
