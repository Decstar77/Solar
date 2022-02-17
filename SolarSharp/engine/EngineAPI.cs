using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SolarSharp
{
    public class EngineAPI
    {
        const string DLLName = "SolarWindows";

        [DllImport(DLLName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
		internal static extern bool Win32CreateWindow([MarshalAs(UnmanagedType.LPStr)] string title, int width, int height, int xPos, int yPos);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool Win32PumpMessages(int[] keys, ref MouseIput mouseInput);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void Win32PostQuitMessage();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void Win32DestroyWindow();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool Win32CreateRenderer();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void Win32DestroyRenderer();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void RendererBeginFrame();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void RendererEndFrame(int vsync);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern int RendererCreateConstBuffer(int sizeBytes);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern int RendererCreateRasterState(int fillMode, int cullMode);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern int RendererCreateDepthStencilState([MarshalAs(UnmanagedType.Bool)] bool enabled, [MarshalAs(UnmanagedType.Bool)] bool write, int comparison);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern int RendererCreateSamplerState(int filter, int wrapMode);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern int RendererCreateBlendState();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern int RendererCreateStaticMesh(float[] vertices, int vertexCount, uint[] indices, int indexCount, int layout);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern int RendererCreateDynamicMesh(int vertexCount, int layout);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern int RendererCreateStaticTexture(byte[] data, int width, int height, int format);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern int RendererCreateStaticProgram([MarshalAs(UnmanagedType.LPStr)] string shaderCode, int layout);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void RendererSetViewportState(int width, int height);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void RendererSetTopologyState(int topo);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void RendererSetRasterState(int id);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void RendererSetDepthState(int id);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void RendererSetBlendState(int id);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void RendererSetSamplerState(int id, int slot);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void RendererSetStaticProgram(int id);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void RendererSetVertexConstBuffer(int id, int slot);
		
		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void RendererSetConstBufferData(int id, float[] data);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void RendererSetDynamicMeshData(int id, float[] data, int count);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void RendererClearRenderTarget(int id);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void RendererDrawStaticMesh(int id);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void RendererDrawDynamicMesh(int id, int count, uint offset);
	}
}
