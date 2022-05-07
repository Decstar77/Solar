using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.Tools
{
	struct Vertex
	{
		public Vector3 position;
		public Vector3 normal;
		public Vector3 tanget;
		public Vector2 uv;

		public Vertex(
			float px, float py, float pz,
			float nx, float ny, float nz,
			float tx, float ty, float tz,
			float u, float v)
		{
			position	= new Vector3(px, py, pz);
			normal		= new Vector3(nx, ny, nz);
			tanget		= new  Vector3(tx, ty, tz);
			uv			= new Vector2(u, v);
        }
    }

    class MeshData
    {
		public List<Vertex> vertices;
		public List<uint> indices;
	}

    public static class MeshFactory
    {
		public static MeshAsset CreateQuad(float x, float y, float w, float h, float depth)
        {
			MeshData meshData = new MeshData();
			meshData.vertices = new List<Vertex>(4);
			meshData.indices = new List<uint>(6);

			meshData.vertices.Add(new Vertex(x, y - h, depth, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f));
			meshData.vertices.Add(new Vertex(x, y, depth, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f));
			meshData.vertices.Add(new Vertex(x + w, y, depth, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f));
			meshData.vertices.Add(new Vertex(x + w, y - h, depth, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f));

			meshData.indices.Add(0);
			meshData.indices.Add(1);
			meshData.indices.Add(2);
			meshData.indices.Add(0);
			meshData.indices.Add(2);
			meshData.indices.Add(3);

			return ConvertMeshDataIntoMeshResource(meshData, VertexLayout.PNT);
		}

		public static MeshAsset CreateBox(float width, float height, float depth, float numSubdivisions)
        {
			MeshData meshData = new MeshData();
			meshData.vertices = new List<Vertex>();
			meshData.indices = new List<uint>();

			float w2 = 0.5f * width;
			float h2 = 0.5f * height;
			float d2 = 0.5f * depth;

			meshData.vertices.Add(new Vertex(-w2, -h2, -d2, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f));
			meshData.vertices.Add(new Vertex(-w2, +h2, -d2, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f));
			meshData.vertices.Add(new Vertex(+w2, +h2, -d2, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f));
			meshData.vertices.Add(new Vertex(+w2, -h2, -d2, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f));
			meshData.vertices.Add(new Vertex(-w2, -h2, +d2, 0.0f, 0.0f, 1.0f, -1.0f, 0.0f, 0.0f, 1.0f, 1.0f));
			meshData.vertices.Add(new Vertex(+w2, -h2, +d2, 0.0f, 0.0f, 1.0f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f));
			meshData.vertices.Add(new Vertex(+w2, +h2, +d2, 0.0f, 0.0f, 1.0f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f));
			meshData.vertices.Add(new Vertex(-w2, +h2, +d2, 0.0f, 0.0f, 1.0f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f));
			meshData.vertices.Add(new Vertex(-w2, +h2, -d2, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f));
			meshData.vertices.Add(new Vertex(-w2, +h2, +d2, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f));
			meshData.vertices.Add(new Vertex(+w2, +h2, +d2, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 0.0f));
			meshData.vertices.Add(new Vertex(+w2, +h2, -d2, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 1.0f, 1.0f));
			meshData.vertices.Add(new Vertex(-w2, -h2, -d2, 0.0f, -1.0f, 0.0f, -1.0f, 0.0f, 0.0f, 1.0f, 1.0f));
			meshData.vertices.Add(new Vertex(+w2, -h2, -d2, 0.0f, -1.0f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f, 1.0f));
			meshData.vertices.Add(new Vertex(+w2, -h2, +d2, 0.0f, -1.0f, 0.0f, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f));
			meshData.vertices.Add(new Vertex(-w2, -h2, +d2, 0.0f, -1.0f, 0.0f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f));
			meshData.vertices.Add(new Vertex(-w2, -h2, +d2, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 1.0f));
			meshData.vertices.Add(new Vertex(-w2, +h2, +d2, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, 0.0f, 0.0f));
			meshData.vertices.Add(new Vertex(-w2, +h2, -d2, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, 1.0f, 0.0f));
			meshData.vertices.Add(new Vertex(-w2, -h2, -d2, -1.0f, 0.0f, 0.0f, 0.0f, 0.0f, -1.0f, 1.0f, 1.0f));
			meshData.vertices.Add(new Vertex(+w2, -h2, -d2, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f));
			meshData.vertices.Add(new Vertex(+w2, +h2, -d2, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f));
			meshData.vertices.Add(new Vertex(+w2, +h2, +d2, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 0.0f));
			meshData.vertices.Add(new Vertex(+w2, -h2, +d2, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f));

			meshData.indices.Add(0);  meshData.indices.Add(1);  meshData.indices.Add(2);
			meshData.indices.Add(0);  meshData.indices.Add(2);  meshData.indices.Add(3);
			meshData.indices.Add(4);  meshData.indices.Add(5);  meshData.indices.Add(6);
			meshData.indices.Add(4);  meshData.indices.Add(6);  meshData.indices.Add(7);			
			meshData.indices.Add(8);  meshData.indices.Add(9);  meshData.indices.Add(10);
			meshData.indices.Add(8);  meshData.indices.Add(10); meshData.indices.Add(11);
			meshData.indices.Add(12); meshData.indices.Add(13); meshData.indices.Add(14);
			meshData.indices.Add(12); meshData.indices.Add(14); meshData.indices.Add(15);
			meshData.indices.Add(16); meshData.indices.Add(17); meshData.indices.Add(18);
			meshData.indices.Add(16); meshData.indices.Add(18); meshData.indices.Add(19);
			meshData.indices.Add(20); meshData.indices.Add(21); meshData.indices.Add(22);
			meshData.indices.Add(20); meshData.indices.Add(22); meshData.indices.Add(23);

			for (int i = 0; i < numSubdivisions; i++)
			{
				meshData = Subdivide(meshData);
			}

			return ConvertMeshDataIntoMeshResource(meshData, VertexLayout.PNT);
		}

		private static MeshAsset ConvertMeshDataIntoMeshResource(MeshData meshData, VertexLayout layout)
        {
			MeshAsset meshResource = new MeshAsset();
			meshResource.vertices = new List<float>();
			meshResource.indices = new List<uint>();

			switch (layout)
            {
                case VertexLayout.INVALID:
                    break;
                case VertexLayout.P:
                    break;
                case VertexLayout.P_PAD:
                    break;
                case VertexLayout.PNT:
                    {
						meshResource.indices = meshData.indices;						

						for (int i = 0; i < meshData.vertices.Count; i++)
						{
							meshResource.vertices.Add(meshData.vertices[i].position.x);
							meshResource.vertices.Add(meshData.vertices[i].position.y);
							meshResource.vertices.Add(meshData.vertices[i].position.z);
							
							meshResource.vertices.Add(meshData.vertices[i].normal.x);
							meshResource.vertices.Add(meshData.vertices[i].normal.y);
							meshResource.vertices.Add(meshData.vertices[i].normal.z);

							meshResource.vertices.Add(meshData.vertices[i].uv.x);
							meshResource.vertices.Add(meshData.vertices[i].uv.y);
						}
					}
                    break;
                case VertexLayout.PNTC:
                    break;
                case VertexLayout.PNTM:
                    break;
                case VertexLayout.TEXT:
                    break;
                case VertexLayout.PC:
                    break;
                case VertexLayout.COUNT:
                    break;
            }

			return meshResource;

		}

        private static Vertex ComputeMidPoint(Vertex a, Vertex b)
		{
			Vertex result = new Vertex();
			result.position = 0.5f * (a.position + b.position);
			result.normal = Vector3.Normalize(0.5f * (a.normal + b.normal));
			result.tanget = Vector3.Normalize(0.5f * (a.tanget + b.tanget));
			result.uv = 0.5f * (a.uv + b.uv);

			return result;
		}

		private static MeshData Subdivide(MeshData mesh)
		{
			int numTris = mesh.indices.Count / 3;

			MeshData result = new MeshData();
			result.vertices = new List<Vertex>();
			result.indices = new List<uint>();

			//       v1
			//       *
			//      / \
			//     /   \
			//  m0*-----*m1
			//   / \   / \
			//  /   \ /   \
			// *-----*-----*
			// v0    m2     v2

			for (int i = 0; i < numTris; ++i)
			{
				Vertex v0 = mesh.vertices[(int)mesh.indices[i * 3 + 0]];
				Vertex v1 = mesh.vertices[(int)mesh.indices[i * 3 + 1]];
				Vertex v2 = mesh.vertices[(int)mesh.indices[i * 3 + 2]];

				Vertex m0 = ComputeMidPoint(v0, v1);
				Vertex m1 = ComputeMidPoint(v1, v2);
				Vertex m2 = ComputeMidPoint(v0, v2);

				result.vertices.Add(v0); // 0
				result.vertices.Add(v1); // 1
				result.vertices.Add(v2); // 2
				result.vertices.Add(m0); // 3
				result.vertices.Add(m1); // 4
				result.vertices.Add(m2); // 5

				result.indices.Add((uint)(i * 6 + 0));
				result.indices.Add((uint)(i * 6 + 3));
				result.indices.Add((uint)(i * 6 + 5));

				result.indices.Add((uint)(i * 6 + 3));
				result.indices.Add((uint)(i * 6 + 4));
				result.indices.Add((uint)(i * 6 + 5));

				result.indices.Add((uint)(i * 6 + 5));
				result.indices.Add((uint)(i * 6 + 4));
				result.indices.Add((uint)(i * 6 + 2));

				result.indices.Add((uint)(i * 6 + 3));
				result.indices.Add((uint)(i * 6 + 1));
				result.indices.Add((uint)(i * 6 + 4));
			}

			return result;
		}
    }
}
