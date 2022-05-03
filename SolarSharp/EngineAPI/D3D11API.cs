using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using SolarSharp.Rendering;

namespace SolarSharp.EngineAPI
{
    public class D3D11API
    {
        const string DLLName = "SolarWindows";

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool CreateDeviceAndContext(ref IntPtr device, ref IntPtr context);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr CreateSwapchain();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr CreateSwapchainRenderTargetView();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr CreateSwapchainDepthStencilView();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void SwapchainPresent(int vsync);
        
        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr DeviceCreateDepthStencilState(ref DepthStencilDesc desc);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr DeviceCreateRasterizerState(ref RasterizerDesc desc);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr DeviceCreateBlendState(ref BlendDesc desc);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr DeviceCreateSamplerState(ref SamplerDesc desc);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr DeviceCreateBuffer(ref BufferDesc desc, ref SubResourceData resourceData);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr DeviceCreateBufferNoSub(ref BufferDesc desc);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr DeviceCompileShader([MarshalAs(UnmanagedType.LPStr)] string code, [MarshalAs(UnmanagedType.LPStr)] string entry, [MarshalAs(UnmanagedType.LPStr)] string target);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr DeviceCreateVertexShader(IntPtr vertexBlob);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern IntPtr DeviceCreatePixelShader(IntPtr pixelBlob);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr DeviceCreateInputLayout(InputElementDesc[] layouts, int count, IntPtr vertexBlob);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void Release(IntPtr d3d11Object);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextClearRenderTargetView(IntPtr rt, float r, float g, float b, float a);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextClearDepthStencilView(IntPtr rt, uint clearFlags, float depth, byte stencil);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetRenderTargets(IntPtr[] renderTargetViews, uint count, IntPtr depthStencilView);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetViewPortState(float width, float height, float minDepth, float maxDepth, float topLeftX, float topLeftY);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetPrimitiveTopology([MarshalAs(UnmanagedType.I4)] PrimitiveTopology primitiveTopology);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetDepthStencilState(IntPtr depthStencilState, uint stencilRef);
        
        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetRasterizerState(IntPtr rasterizerState);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetBlendState(IntPtr blendState, float blendFactor0, float blendFactor1, float blendFactor2, float blendFactor3, uint sampleMask);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetInputLayout(IntPtr inputLayout);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetVertexShader(IntPtr vertexShader);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetPixelShader(IntPtr pixelShader);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetVertexBuffers(IntPtr vertexBuffer, uint stride);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetIndexBuffer(IntPtr indexBuffer, DXGIFormat format, uint offset);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextDrawIndexed(uint indexCount, uint startLocation, uint baseVertexLocation);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetVSConstBuffer(IntPtr buffer, uint slot);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetPSConstBuffer(IntPtr buffer, uint slot);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetCSConstBuffer(IntPtr buffer, uint slot);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextUpdateSubresource(IntPtr buffer, uint subResource, IntPtr box, IntPtr data, uint rowPitch, uint depthPitch);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern void Win32DestroyRenderer();

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern void RendererBeginFrame();

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern void RendererEndFrame(int vsync);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern int RendererCreateConstBuffer(int sizeBytes);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern int RendererCreateRasterState(int fillMode, int cullMode);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern int RendererCreateDepthStencilState([MarshalAs(UnmanagedType.Bool)] bool enabled, [MarshalAs(UnmanagedType.Bool)] bool write, int comparison);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern int RendererCreateSamplerState(int filter, int wrapMode);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern int RendererCreateBlendState();

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern int RendererCreateStaticMesh(float[] vertices, int vertexCount, uint[] indices, int indexCount, int layout);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern int RendererCreateStaticTexture(byte[] data, int width, int height, int format);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern int RendererCreateStaticProgram([MarshalAs(UnmanagedType.LPStr)] string shaderCode, int layout);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern void RendererSetViewportState(int width, int height);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern void RendererSetTopologyState(int topo);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern void RendererSetRasterState(int id);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern void RendererSetDepthState(int id);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern void RendererSetBlendState(int id);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern void RendererSetSamplerState(int id, int slot);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern void RendererSetStaticProgram(int id);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern void RendererSetVertexConstBuffer(int id, int slot);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern void RendererSetConstBufferData(int id, float[] data);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern void RendererClearRenderTarget(int id);

        //[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        //public static extern void RendererDrawStaticMesh(int id);


    }
}
