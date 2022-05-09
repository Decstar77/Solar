using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using SolarSharp.Rendering;
using SolarSharp.Assets;

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
        public static extern IntPtr DeviceCreateTexture2D(ref DXTexture2DDesc desc, ref SubResourceData resourceData);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr DeviceCreateTexture2DNoSub(ref DXTexture2DDesc desc);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr DeviceCreateShaderResourceView(IntPtr resource, ref DXShaderResourceViewDesc desc);

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
        public static extern void ContextSetIndexBuffer(IntPtr indexBuffer, TextureFormat format, uint offset);

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

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetPSSampler(IntPtr smp, uint slot);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetCSSampler(IntPtr smp, uint slot);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetPSShaderResources(IntPtr srv, uint slot);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextSetCSShaderResources(IntPtr srv, uint slot);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextMap(IntPtr res, uint subRes, uint mapType, uint mapFlags, ref DXMappedSubresource map);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextUnmap(IntPtr res, uint subRes);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ContextDraw(uint vertexCount, uint vertexStartLocation);

    }
}
