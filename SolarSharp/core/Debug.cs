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
