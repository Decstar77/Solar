using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
	public static class ImGui
	{
		const string DLLName = "SolarWindows";

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool Initialzie();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool Shutdown();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool BeginFrame();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool EndFrame();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool ShowDemoWindow();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool BeginMainMenuBar();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void EndMainMenuBar();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool BeginMenu([MarshalAs(UnmanagedType.LPStr)] string label, [MarshalAs(UnmanagedType.Bool)] bool enabled = true);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool MenuItem([MarshalAs(UnmanagedType.LPStr)] string label, [MarshalAs(UnmanagedType.LPStr)] string shortcut = null, [MarshalAs(UnmanagedType.Bool)] bool selected = false, [MarshalAs(UnmanagedType.Bool)] bool enabled = true);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		private static extern void EndMenu_();
		public static void EndMenu() => EndMenu_(); // @NOTE: Win32 has EndMenu and confuse compiler

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void Separator();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		private static extern bool Begin([MarshalAs(UnmanagedType.LPStr)] string name, ref int open, int flags = 0);

		public static bool Begin(string name)
		{
			int open = -1;
			return Begin(name, ref open);
		}

		public static bool Begin(string name, ref bool open, int flags = 0)
		{
			int op = open ? 1 : 0;
			bool result = Begin(name, ref op, flags);
			open = (op == 1 ? true : false);
			return result;
		}

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void End();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool CollapsingHeader([MarshalAs(UnmanagedType.LPStr)] string label, int flags);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void Text([MarshalAs(UnmanagedType.LPStr)] string text);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		private static extern bool InputText([MarshalAs(UnmanagedType.LPStr)] string label, byte[] buf, int bufSize, int flags);

		private static bool InputText([MarshalAs(UnmanagedType.LPStr)] string label, byte[] buf, int flags = 0)
		{
			return InputText(label, buf, buf.Length, flags);
		}

		public static bool InputText([MarshalAs(UnmanagedType.LPStr)] string label, ref string input, int flags = 0)
		{
			byte[] buf = new byte[256];
			byte[] bytes = Encoding.ASCII.GetBytes(input);
			Array.Copy(bytes, buf, bytes.Length);
			
			bool result = InputText(label, buf, buf.Length, flags);					
			string temp = Encoding.ASCII.GetString(buf);
			input = "";
			for (int i = 0 ; i < temp.Length && temp[i] != '\0'; i++) { input += temp[i]; }

			return true;	
		}
		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void SameLine(float offset_from_start_x = 0.0f, float spacing_w = 1.0f);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool Button([MarshalAs(UnmanagedType.LPStr)] string label, float sizeX = 0, float sizeY = 0);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool DragFloat3([MarshalAs(UnmanagedType.LPStr)] string label, ref float x, ref float y, ref float z, float v_speed = 1.0f, float v_min = 0.0f, float v_max = 0.0f);

		public static bool DragFloat3([MarshalAs(UnmanagedType.LPStr)] string label, ref Vector3 v, float v_speed = 1.0f, float v_min = 0.0f, float v_max = 0.0f)
        {
			return DragFloat3(label, ref v.x, ref v.y, ref v.z, v_speed, v_min, v_max);
		}

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void GizmoEnable(bool enable);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void GizmoSetRect(float x, float y, float width, float height);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		private static extern bool GizmoManipulate(Matrix4 proj, Matrix4 viewLH, ref Matrix4 world, int operation, int mode);

		public static bool GizmoManipulate(Camera camera, ref Matrix4 world, int operation, int mode)
        {
			Matrix4 view = camera.GetViewMatrix();
			Matrix4 proj = camera.GetProjectionMatrix();

			return GizmoManipulate(proj.Transpose, view.Transpose, ref world, operation, mode);
        }
	}
}
