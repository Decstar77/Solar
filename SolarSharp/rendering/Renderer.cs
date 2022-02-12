using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SolarSharp.Rendering
{
    internal class RasterState
    {
        public enum FillMode
        {
            WIREFRAME = 2,
            SOLID = 3
        }

        public enum CullMode
        {
            NONE = 1,
            FRONT = 2,
            BACK = 3
        }

        public RasterState(FillMode fillMode, CullMode cullMode)
        {
            this.fillMode = fillMode;
            this.cullMode = cullMode;
            obj = EngineAPI.RendererCreateRasterState((int)fillMode, (int)cullMode);
        }

        public int State { get { return obj; } }

        private FillMode fillMode;
        private CullMode cullMode;
        private int obj;        
    }


    internal class DepthState
    {
        public enum Comparison
        {
            NEVER = 1,
            LESS = 2,
            EQUAL = 3,
            LESS_EQUAL = 4,
            GREATER = 5,
            NOT_EQUAL = 6,
            GREATER_EQUAL = 7,
            ALWAYS = 8
        }

        public DepthState(bool enabled, bool write, Comparison comparison)
        {
            this.enabled = enabled;
            this.write = write; 
            this.comparison = comparison;
            obj = EngineAPI.RendererCreateDepthStencilState(enabled, write, (int)comparison);
        }

        public int State { get { return obj; } }

        private bool enabled;
        private bool write;
        private Comparison comparison;
        private int obj;
    }

    internal enum Topology
    {
        UNDEFINED = 0,
        POINTLIST = 1,
        LINELIST = 2,
        LINESTRIP = 3,
        TRIANGLELIST = 4,
        TRIANGLESTRIP = 5,
        LINELIST_ADJ = 10,
        LINESTRIP_ADJ = 11,
        TRIANGLELIST_ADJ = 12,
        TRIANGLESTRIP_ADJ = 13,
    }

    internal class SamplerState
    {
    }

    internal class BlendState
    {
    }

    internal class StaticProgram
    {
        public StaticProgram(string srcCode)
        {            
            obj = EngineAPI.RendererCreateStaticProgram(srcCode, 3);
        }

        public int State { get { return obj; } }
        private int obj;
    }

    internal class StaticMesh
    {
        public StaticMesh(float[] vertices, uint[] indices, VertexLayout layout)
        {
            obj = EngineAPI.RendererCreateStaticMesh(vertices, vertices.Length, indices, indices.Length, (int)layout);
        }

        public int State { get { return obj; } }
        private int obj;
    }

    internal class ConstBuffer
    {
        public ConstBuffer(int floatCount)
        {           
            obj = EngineAPI.RendererCreateConstBuffer(floatCount * sizeof(float));
            buffer = new float[floatCount];
            Reset();
        }

        public void Reset()
        {
            index = 0;
        }

        public void Prepare(Matrix4 m)
        {             
            buffer[index++] = m.m11; buffer[index++] = m.m12; buffer[index++] = m.m13; buffer[index++] = m.m14;
            buffer[index++] = m.m21; buffer[index++] = m.m22; buffer[index++] = m.m23; buffer[index++] = m.m24;
            buffer[index++] = m.m31; buffer[index++] = m.m32; buffer[index++] = m.m33; buffer[index++] = m.m34;
            buffer[index++] = m.m41; buffer[index++] = m.m42; buffer[index++] = m.m43; buffer[index++] = m.m44;
        }

        public float[] GetData() { return buffer; }

        public int State { get { return obj; } }
        private int obj;
        private float[] buffer;
        private int index;
    }


    internal static class RenderCommands
    {
        public static void SetViewportState(int width, int height) 
        {
            EngineAPI.RendererSetViewportState(width, height);
        }

        public static void SetTopologyState(Topology topo)
        {
            EngineAPI.RendererSetTopologyState((int)topo);
        }

        public static void SetRasterState(RasterState rasterState) 
        {
            EngineAPI.RendererSetRasterState(rasterState.State);
        }

        public static void SetDepthState(DepthState depthState) 
        {
            EngineAPI.RendererSetDepthState(depthState.State);
        }

        public static void SetVertexConstBuffer(ConstBuffer buffer, int slot)
        {
            EngineAPI.RendererSetVertexConstBuffer(buffer.State, slot);
        }

        public static void SetConstBufferData(ConstBuffer buffer)
        {
            EngineAPI.RendererSetConstBufferData(buffer.State, buffer.GetData());
        }

        public static void RendererSetStaticProgram(StaticProgram program)
        {
            EngineAPI.RendererSetStaticProgram(program.State);  
        }

        public static void RendererDrawStaticMesh(StaticMesh mesh)
        {
            EngineAPI.RendererDrawStaticMesh(mesh.State);
        }


    }

    internal class Renderer
    {
        private static RasterState rasterState;
        private static DepthState depthState;
        private static int samplerState;
        private static int blendState;
        private static StaticProgram program;
        private static ConstBuffer cbuf;
        
        private static StaticMesh unitQuad;
        private static StaticMesh unitCube;

        public static StaticMesh testMesh;

        public static bool Create()
        {
            rasterState = new RasterState(RasterState.FillMode.SOLID, RasterState.CullMode.BACK);
            depthState = new DepthState(true, true, DepthState.Comparison.LESS_EQUAL);

            program = new StaticProgram(new StreamReader("F:/codes/Learning/SolarSharp/SolarSharp/Programs/FirstShader.hlsl").ReadToEnd());

            MeshResource quadResource = Tools.MeshFactory.CreateQuad(-1, 1, 2, 2, 0);
            MeshResource cubeResource = Tools.MeshFactory.CreateBox(2, 2, 2, 1);
            
            unitQuad = new StaticMesh(quadResource.vertices.ToArray(), quadResource.indices.ToArray(), VertexLayout.PNT);
            unitCube = new StaticMesh(cubeResource.vertices.ToArray(), cubeResource.indices.ToArray(), VertexLayout.PNT);

            cbuf = new ConstBuffer(16 * 3);
            RenderCommands.SetVertexConstBuffer(cbuf, 0);



            //samplerState = EngineAPI.RendererCreateSamplerState(20, 1);
            blendState = EngineAPI.RendererCreateBlendState();
    
            return true;
        }

        public static void Render(RenderPacket renderPacket)
        {
            EngineAPI.RendererBeginFrame();

            EventSystem.Fire(EventType.RENDER_START, null);

            RenderCommands.SetRasterState(rasterState);
            RenderCommands.SetDepthState(depthState);
            EngineAPI.RendererSetBlendState(blendState);

            RenderCommands.SetTopologyState(Topology.TRIANGLELIST);
            RenderCommands.SetViewportState(Application.SurfaceWidth, Application.SurfaceHeight);

            RenderCommands.RendererSetStaticProgram(program);
#if false
            Matrix4 proj = Matrix4.CreatePerspectiveLH((MathF.PI / 180) * 45.0f, Application.WindowAspect, 0.1f, 100.0f);
            Matrix4 cam = Matrix4.CreateLookAtLH(new Vector3(3, 3, 3), Vector3.Zero, Vector3.UnitY).Inverse;
            Console.WriteLine(cam);

            Matrix4 m = Matrix4.Identity;

            Matrix4 mvp = (m * cam * proj).Transpose;

            cbuf.Reset();
            cbuf.Prepare(mvp);
            RenderCommands.SetConstBufferData(cbuf);
            RenderCommands.RendererDrawStaticMesh(unitCube);

            m.m42 = 4;
            mvp = (m * cam * proj).Transpose;

            cbuf.Reset();
            cbuf.Prepare(mvp);
            RenderCommands.SetConstBufferData(cbuf);
            RenderCommands.RendererDrawStaticMesh(unitCube);
#else
            Matrix4 proj = Matrix4.CreatePerspectiveRH((MathF.PI / 180) * 45.0f, Application.WindowAspect, 0.1f, 100.0f);
            Matrix4 cam = Matrix4.CreateLookAtRH(new Vector3(8, 8, 8), Vector3.Zero, Vector3.UnitY).Inverse;

            
            foreach (RenderEntry entry in renderPacket.renderEntries)
            {
                Matrix4 mvp = (proj * cam * entry.ComputeTransformMatrix());

                cbuf.Reset();
                cbuf.Prepare(mvp);
                RenderCommands.SetConstBufferData(cbuf);
                RenderCommands.RendererDrawStaticMesh(unitCube);
            }


            

            //m = Matrix4.Identity; m.m14 = 0; m.m24 = 4; m.m34 = 0;
            //mvp = (proj * cam * m);

            //cbuf.Reset();
            //cbuf.Prepare(mvp);
            //RenderCommands.SetConstBufferData(cbuf);
            //RenderCommands.RendererDrawStaticMesh(unitCube);
#endif


            EventSystem.Fire(EventType.RENDER_END, null);

            EngineAPI.RendererEndFrame(1);
        }

    }
}
