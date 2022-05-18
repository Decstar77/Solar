using SolarSharp.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.core
{
    public static class DebugVariables
    {
        public static int DrawCalls { get; set; }
        public static int IndexCount { get; set; }
    }

    public static class DebugDraw
    {
		private static GraphicsShader shader = null;
		private static DynamicMesh mesh = null;

		private static int debugMeshVertexCount = 0;
		private static float[] meshVertices = null;

		public static bool Initialize(DXDevice device) {

			shader = new GraphicsShader().Create(device, "DebugLineShader", debugShaderCode, Assets.VertexLayout.P);

			uint vertexCount = 262144; // @NOTE: 1 MB
			mesh = new DynamicMesh(device, sizeof(float) * vertexCount, Assets.VertexLayout.P);
			meshVertices = new float[vertexCount];

            return true;
        }

		public static void Flush(DXContext context)
        {
			context.SetPrimitiveTopology(PrimitiveTopology.LINELIST);

			context.SetVertexShader(shader.vertexShader);
			context.SetPixelShader(shader.pixelShader);
			context.SetInputLayout(shader.inputLayout);

			DXMappedSubresource map = new DXMappedSubresource();

			GCHandle data = GCHandle.Alloc(meshVertices, GCHandleType.Pinned);
			context.Map(mesh.buffer, 0, DXMapType.WRITE_DISCARD, ref map);

            unsafe
			{
				Buffer.MemoryCopy(data.AddrOfPinnedObject().ToPointer(), map.pData.ToPointer(), meshVertices.Length * sizeof(float), debugMeshVertexCount * sizeof(float));
			}

			context.Unmap(mesh.buffer, 0);
			data.Free();

			context.SetVertexBuffers(mesh.buffer, mesh.StrideBytes);
			context.Draw((uint)debugMeshVertexCount / 3, 0);

            debugMeshVertexCount = 0;

            context.SetPrimitiveTopology(PrimitiveTopology.TRIANGLELIST);
		}


		public static void Line(Vector3 p1, Vector3 p2)
		{
			meshVertices[debugMeshVertexCount++] = p1.x;
			meshVertices[debugMeshVertexCount++] = p1.y;
			meshVertices[debugMeshVertexCount++] = p1.z;

			meshVertices[debugMeshVertexCount++] = p2.x;
			meshVertices[debugMeshVertexCount++] = p2.y;
			meshVertices[debugMeshVertexCount++] = p2.z;
		}

        public static void Basis(Vector3 position, Basis basis)
        {
            Line(position, position + basis.forward * 10);
            Line(position, position + basis.up * 10);
            Line(position, position + basis.right * 10);
        }

        public static void Triangle(Triangle triangle)
        {
            Line(triangle.a, triangle.b);
            Line(triangle.b, triangle.c);
            Line(triangle.c, triangle.a);
        }


        public static void AlignedBox(AlignedBox box)
        {
            Vector3 min = box.min;
            Vector3 max = box.max;

            Vector3 v2 = new Vector3(max.x, min.y, min.z);
            Vector3 v3 = new Vector3(max.x, max.y, min.z);
            Vector3 v4 = new Vector3(min.x, max.y, min.z);

            Vector3 v6 = new Vector3(max.x, min.y, max.z);
            Vector3 v7 = new Vector3(min.x, min.y, max.z);
            Vector3 v8 = new Vector3(min.x, max.y, max.z);

            Line(min, v2);
            Line(min, v4);
            Line(min, v7);
            Line(max, v6);
            Line(max, v8);
            Line(max, v3);
            Line(v3, v2);
            Line(v3, v4);
            Line(v2, v6);
            Line(v6, v7);
            Line(v8, v7);
            Line(v8, v4);
        }

        public static void BoundingBox(BoundingBox box)
        {
            Vector3 extents = box.extents;
            // @TODO: This inverse is pretty sus
            Quaternion orientation = Quaternion.Inverse(box.orientation);

            Vector3 v0 = Quaternion.RotatePoint(extents, orientation) + box.origin;
            Vector3 v1 = Quaternion.RotatePoint(-extents, orientation) + box.origin;
            Vector3 v2 = Quaternion.RotatePoint(new Vector3(-extents.x, extents.y, extents.z), orientation) + box.origin;
            Vector3 v3 = Quaternion.RotatePoint(new Vector3(extents.x, -extents.y, extents.z), orientation) + box.origin;
            Vector3 v4 = Quaternion.RotatePoint(new Vector3(extents.x, extents.y, -extents.z), orientation) + box.origin;
            Vector3 v5 = Quaternion.RotatePoint(new Vector3(-extents.x, -extents.y, extents.z), orientation) + box.origin;
            Vector3 v6 = Quaternion.RotatePoint(new Vector3(extents.x, -extents.y, -extents.z), orientation) + box.origin;
            Vector3 v7 = Quaternion.RotatePoint(new Vector3(-extents.x, extents.y, -extents.z), orientation) + box.origin;

            Line(v0, v2);
            Line(v0, v4);
            Line(v0, v3);
            Line(v1, v5);
            Line(v1, v7);
            Line(v1, v6);
            Line(v3, v6);
            Line(v3, v5);
            Line(v2, v5);
            Line(v2, v7);
            Line(v4, v7);
            Line(v4, v6);
        }

        public static void Ray(Ray ray, float dist = 100.0f)
        {
            Line(ray.origin, ray.origin + ray.direction * dist);
        }

        public static void Point(Vector3 position)
        {
            Line(position - Vector3.UnitX, position + Vector3.UnitX);
            Line(position - Vector3.UnitY, position + Vector3.UnitY);
            Line(position - Vector3.UnitZ, position + Vector3.UnitZ);
        }



        private static string debugShaderCode = @"
struct VS_INPUT
{
	float3 pos : POSITION;
};

struct VS_OUTPUT
{
	float4 pos : SV_POSITION;
};

cbuffer ViewData : register(b1)
{
	matrix persp;
	matrix view;
	matrix screenProjection;
}

VS_OUTPUT VSmain(VS_INPUT input)
{
	matrix transform = mul(view, persp);

	VS_OUTPUT output;
	output.pos = mul(float4(input.pos, 1.0f), transform);

	return output;
}

float4 PSmain(VS_OUTPUT input) : SV_TARGET
{
	return float4(0.2f, 0.8f, 0.2f, 1.0f);
}";
	}
}
