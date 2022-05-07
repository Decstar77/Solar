﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Rendering
{
    public class StaticMesh
    {
        public uint StrideBytes { get; set; }
        public uint IndexCount { get; set; }
        public uint VertexCount { get; set; }
        public DXBuffer VertexBuffer { get; set; }
        public DXBuffer IndexBuffer { get; set; }

        public StaticMesh(DXDevice device, float[] vertices, uint[] indices, VertexLayout layout)
        {
            uint vertexStrideBytes = layout.GetStrideBytes();
            uint indicesStrideBytes = sizeof(uint);

            BufferDesc vertexDesc = new BufferDesc();
            vertexDesc.BindFlags = (uint)(BufferBindFlag.VERTEX_BUFFER);
            vertexDesc.Usage = BufferUsage.DEFAULT;
            vertexDesc.CPUAccessFlags = 0;
            vertexDesc.MiscFlags = 0;
            vertexDesc.ByteWidth = (uint)(vertices.Length * sizeof(float));
            vertexDesc.StructureByteStride = vertexStrideBytes;

            SubResourceData vertexSubResourceData = new SubResourceData();
            GCHandle vertexHandle = GCHandle.Alloc(vertices, GCHandleType.Pinned);
            vertexSubResourceData.pSysMem = vertexHandle.AddrOfPinnedObject();

            BufferDesc indexDesc = new BufferDesc();
            indexDesc.BindFlags = (uint)(BufferBindFlag.INDEX_BUFFER);
            indexDesc.Usage = BufferUsage.DEFAULT;
            indexDesc.CPUAccessFlags = 0;
            indexDesc.MiscFlags = 0;
            indexDesc.ByteWidth = (uint)(indices.Length * sizeof(float));
            indexDesc.StructureByteStride = indicesStrideBytes;

            SubResourceData indexSubResourceData = new SubResourceData();
            GCHandle indexHandle = GCHandle.Alloc(indices, GCHandleType.Pinned);
            indexSubResourceData.pSysMem = indexHandle.AddrOfPinnedObject();

            VertexBuffer = device.CreateBuffer(vertexDesc, vertexSubResourceData);
            IndexBuffer = device.CreateBuffer(indexDesc, indexSubResourceData);
            StrideBytes = vertexStrideBytes;
            VertexCount = (uint)vertices.Length / layout.GetStride();
            IndexCount = (uint)indices.Length;

            vertexHandle.Free();
            indexHandle.Free();
        }

        public static StaticMesh CreateScreenSpaceQuad(DXDevice device)
        {
            float[] vertexData = {
            -1, 1, 0,   0, 0, -1,   0, 0,
            1, -1, 0,   0, 0, -1,   1, 1,
            -1, -1, 0,  0, 0, -1,   0, 1,
            1, 1, 0,    0, 0, -1,   1, 0
            };

            uint[] indexData = {
            0, 1, 2, 0, 3, 1
            };

            return new StaticMesh(device, vertexData, indexData, VertexLayout.PNT);
        }
    }
}