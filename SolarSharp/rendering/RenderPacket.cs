using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering
{
    public struct RenderPacketEntry
    {
        public Matrix4 transform;
        public Guid meshId;
        public Guid materialId;
    }

    public class RenderPacket
    {
        public Vector3 cameraPosition;
        public Matrix4 viewMatrix;
        public Matrix4 projectionMatrix;
        public List<RenderPacketEntry> renderPacketEntries = new List<RenderPacketEntry>();
    }
}
