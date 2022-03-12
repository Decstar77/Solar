using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolarSharp.Rendering;

namespace SolarSharp
{
    public class Debug
    {
        internal static int debugMeshVertexCount;
        internal static DynamicMesh debugMesh;
        internal static StaticProgram debugProgram;

        internal static void Initialize()
        {
            debugMeshVertexCount = 0; 
            debugMesh = new DynamicMesh(4096 * 2, VertexLayout.P);
            debugProgram = new StaticProgram(new StreamReader("F:/codes/Solar/SolarSharp/Programs/Debug.hlsl").ReadToEnd(), VertexLayout.P);
        }

        internal static void Shutdown()
        {

        }

        internal static void Flush()
        {
            EngineAPI.RendererSetDynamicMeshData(debugMesh.obj, debugMesh.vertices, debugMeshVertexCount);
            
            RenderCommands.RendererSetStaticProgram(debugProgram);
            RenderCommands.SetTopologyState(Topology.LINELIST);            
            
            EngineAPI.RendererDrawDynamicMesh(debugMesh.obj, debugMeshVertexCount, 0);

            RenderCommands.SetTopologyState(Topology.TRIANGLELIST);
            debugMeshVertexCount = 0;
        }

        public static void DrawLine(Vector3 p1, Vector3 p2)
        {
            debugMesh.vertices[debugMeshVertexCount++] = p1.x;
            debugMesh.vertices[debugMeshVertexCount++] = p1.y;
            debugMesh.vertices[debugMeshVertexCount++] = p1.z;

            debugMesh.vertices[debugMeshVertexCount++] = p2.x;
            debugMesh.vertices[debugMeshVertexCount++] = p2.y;
            debugMesh.vertices[debugMeshVertexCount++] = p2.z;
        }

        public static void DrawBasis(Vector3 position, Basis basis)
        {
            DrawLine(position, position + basis.forward * 10);
            DrawLine(position, position + basis.up * 10);
            DrawLine(position, position + basis.right * 10);
        }

        public static void DrawAlignedBox(AlignedBox box)
        {
            Vector3 min = box.min;
            Vector3 max = box.max;

            Vector3 v2 = new Vector3(max.x, min.y, min.z);
            Vector3 v3 = new Vector3(max.x, max.y, min.z);
            Vector3 v4 = new Vector3(min.x, max.y, min.z);

            Vector3 v6 = new Vector3(max.x, min.y, max.z);
            Vector3 v7 = new Vector3(min.x, min.y, max.z);
            Vector3 v8 = new Vector3(min.x, max.y, max.z);

            DrawLine(min, v2);
            DrawLine(min, v4);
            DrawLine(min, v7);
            DrawLine(max, v6);
            DrawLine(max, v8);
            DrawLine(max, v3);
            DrawLine(v3, v2);
            DrawLine(v3, v4);
            DrawLine(v2, v6);
            DrawLine(v6, v7);
            DrawLine(v8, v7);
            DrawLine(v8, v4);
        }

        public static void DrawBoundingBox(BoundingBox box)
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
                    
            DrawLine(v0, v2);
            DrawLine(v0, v4);
            DrawLine(v0, v3);
            DrawLine(v1, v5);
            DrawLine(v1, v7);
            DrawLine(v1, v6);
            DrawLine(v3, v6);
            DrawLine(v3, v5);
            DrawLine(v2, v5);
            DrawLine(v2, v7);
            DrawLine(v4, v7);
            DrawLine(v4, v6);
        }

        public static void DrawRay(Ray ray, float dist = 100.0f)
        {
            DrawLine(ray.origin, ray.origin + ray.direction * dist);
        }

        public static void DrawPoint(Vector3 position)
        {
            DrawLine(position - Vector3.UnitX, position + Vector3.UnitX);
            DrawLine(position - Vector3.UnitY, position + Vector3.UnitY);
            DrawLine(position - Vector3.UnitZ, position + Vector3.UnitZ);
        }
    }
}
