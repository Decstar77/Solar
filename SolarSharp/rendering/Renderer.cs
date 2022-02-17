﻿using System;
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
        public StaticProgram(string srcCode, VertexLayout layout)
        {            
            obj = EngineAPI.RendererCreateStaticProgram(srcCode, (int)layout);
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

    internal class DynamicMesh
    {
        internal DynamicMesh(int floatCount, VertexLayout layout)
        {
            vertices = new float[floatCount];
            obj = EngineAPI.RendererCreateDynamicMesh(floatCount, (int)layout);
        }

        internal float[] vertices;

        // @HACK @TODO: Make private !!
        internal int obj;
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


        private static ConstBuffer vertexShaderModelData;
        private static ConstBuffer vertexShaderViewData;

        private static StaticMesh unitQuad;
        private static StaticMesh unitCube;

        public static StaticMesh testMesh;

        public static DynamicMesh debugMesh;
        public static StaticProgram debugProgram;

        public static bool Create()
        {
            rasterState = new RasterState(RasterState.FillMode.SOLID, RasterState.CullMode.BACK);
            depthState = new DepthState(true, true, DepthState.Comparison.LESS_EQUAL);

            program = new StaticProgram(new StreamReader("F:/codes/Solar/SolarSharp/Programs/FirstShader.hlsl").ReadToEnd(), VertexLayout.PNT);

            MeshResource quadResource = Tools.MeshFactory.CreateQuad(-1, 1, 2, 2, 0);
            MeshResource cubeResource = Tools.MeshFactory.CreateBox(2, 2, 2, 1);
            
            unitQuad = new StaticMesh(quadResource.vertices.ToArray(), quadResource.indices.ToArray(), VertexLayout.PNT);
            unitCube = new StaticMesh(cubeResource.vertices.ToArray(), cubeResource.indices.ToArray(), VertexLayout.PNT);

            vertexShaderModelData = new ConstBuffer(16 * 3);
            RenderCommands.SetVertexConstBuffer(vertexShaderModelData, 0);

            vertexShaderViewData = new ConstBuffer(16 * 3);
            RenderCommands.SetVertexConstBuffer(vertexShaderViewData, 1);

            //samplerState = EngineAPI.RendererCreateSamplerState(20, 1);
            blendState = EngineAPI.RendererCreateBlendState();

            Debug.Initialize();
    
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
            //Matrix4 proj = Matrix4.CreatePerspectiveRH((MathF.PI / 180) * 45.0f, Application.WindowAspect, 0.1f, 100.0f);
            //Matrix4 view = Matrix4.CreateLookAtRH(new Vector3(8, 8, 8), Vector3.Zero, Vector3.UnitY).Inverse;
            Matrix4 proj = renderPacket.projectionMatrix;
            Matrix4 view = renderPacket.viewMatrix;

            vertexShaderViewData.Reset();
            vertexShaderViewData.Prepare(proj);
            vertexShaderViewData.Prepare(view);
            RenderCommands.SetConstBufferData(vertexShaderViewData);

            foreach (RenderEntry entry in renderPacket.renderEntries)
            {
                Matrix4 mvp = (proj * view * entry.ComputeTransformMatrix());

                vertexShaderModelData.Reset();
                vertexShaderModelData.Prepare(mvp);
                RenderCommands.SetConstBufferData(vertexShaderModelData);
                RenderCommands.RendererDrawStaticMesh(unitCube);
            }

#endif

#if DEBUG         
            Debug.Flush();
#endif
            EventSystem.Fire(EventType.RENDER_END, null);

            EngineAPI.RendererEndFrame(1);
        }

    }
}
