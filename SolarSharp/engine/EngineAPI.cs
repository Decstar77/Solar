using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SolarSharp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Input
    {
		[MarshalAs(UnmanagedType.Bool)] public bool mb1;
        [MarshalAs(UnmanagedType.Bool)] public bool mb2;
        [MarshalAs(UnmanagedType.Bool)] public bool mb3;
        [MarshalAs(UnmanagedType.Bool)] public bool alt;
        [MarshalAs(UnmanagedType.Bool)] public bool shift;
        [MarshalAs(UnmanagedType.Bool)] public bool ctrl;
		[MarshalAs(UnmanagedType.Bool)] public bool w;
		[MarshalAs(UnmanagedType.Bool)] public bool s;
		[MarshalAs(UnmanagedType.Bool)] public bool a;
		[MarshalAs(UnmanagedType.Bool)] public bool d;
		[MarshalAs(UnmanagedType.Bool)] public bool q;
		[MarshalAs(UnmanagedType.Bool)] public bool e;
		[MarshalAs(UnmanagedType.Bool)] public bool r;
		[MarshalAs(UnmanagedType.Bool)] public bool t;
		[MarshalAs(UnmanagedType.Bool)] public bool z;
		[MarshalAs(UnmanagedType.Bool)] public bool x;
		[MarshalAs(UnmanagedType.Bool)] public bool c;
		[MarshalAs(UnmanagedType.Bool)] public bool v;
		[MarshalAs(UnmanagedType.Bool)] public bool b;
		[MarshalAs(UnmanagedType.Bool)] public bool del;
		[MarshalAs(UnmanagedType.Bool)] public bool tlda;
		[MarshalAs(UnmanagedType.Bool)] public bool K1;
		[MarshalAs(UnmanagedType.Bool)] public bool K2;
		[MarshalAs(UnmanagedType.Bool)] public bool K3;
		[MarshalAs(UnmanagedType.Bool)] public bool K4;
		[MarshalAs(UnmanagedType.Bool)] public bool K5;
		[MarshalAs(UnmanagedType.Bool)] public bool K6;
		[MarshalAs(UnmanagedType.Bool)] public bool K7;
		[MarshalAs(UnmanagedType.Bool)] public bool K8;
		[MarshalAs(UnmanagedType.Bool)] public bool K9;
		[MarshalAs(UnmanagedType.Bool)] public bool K0;
		[MarshalAs(UnmanagedType.Bool)] public bool f1;
		[MarshalAs(UnmanagedType.Bool)] public bool f2;
		[MarshalAs(UnmanagedType.Bool)] public bool f3;
		[MarshalAs(UnmanagedType.Bool)] public bool f4;
		[MarshalAs(UnmanagedType.Bool)] public bool f5;
		[MarshalAs(UnmanagedType.Bool)] public bool f6;
		[MarshalAs(UnmanagedType.Bool)] public bool f7;
		[MarshalAs(UnmanagedType.Bool)] public bool f8;
		[MarshalAs(UnmanagedType.Bool)] public bool f9;
		[MarshalAs(UnmanagedType.Bool)] public bool f10;
		[MarshalAs(UnmanagedType.Bool)] public bool f11;
		[MarshalAs(UnmanagedType.Bool)] public bool f12;
		[MarshalAs(UnmanagedType.Bool)] public bool escape;
		[MarshalAs(UnmanagedType.Bool)] public bool space;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerUp;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerDown;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerLeft;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerRight;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerStart;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerBack;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerLeftThumb;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerRightThumb;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerLeftShoulder;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerRightShoulder;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerA;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerB;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerX;
		[MarshalAs(UnmanagedType.Bool)] public bool controllerY;
	}

    public class EngineAPI
    {
        const string DLLName = "SolarWindows";

        [DllImport(DLLName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Win32CreateWindow([MarshalAs(UnmanagedType.LPStr)] string title, int width, int height, int xPos, int yPos);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern bool Win32PumpMessages(ref Input input);

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
		public static extern void RendererClearRenderTarget(int id);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void RendererDrawStaticMesh(int id);

	
	}
}
