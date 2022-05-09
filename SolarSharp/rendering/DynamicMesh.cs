using SolarSharp.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering
{
    public class DynamicMesh
    {
        public uint StrideBytes { get; set; }

        public DXBuffer buffer;

        public DynamicMesh(DXDevice device, uint sizeBytes, VertexLayout vertexLayout) 
            : this(device, sizeBytes, (uint)vertexLayout.GetStrideBytes())
        {

        }

        public DynamicMesh(DXDevice device, uint sizeBytes, uint strideBytes)
        {
            BufferDesc desc = new BufferDesc();
            desc.BindFlags = DXBindFlag.VERTEX_BUFFER;
            desc.Usage = DXUsage.DYNAMIC;
            desc.CPUAccessFlags = DXCPUAccessFlag.D3D11_CPU_ACCESS_WRITE;
            desc.MiscFlags = 0;
            desc.ByteWidth = sizeBytes;
            desc.StructureByteStride = strideBytes;

            buffer = device.CreateBuffer(desc);
            StrideBytes = strideBytes;
        }



    }
}
