using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.EngineAPI
{
	public static class ImGuiAPI
	{
		const string DLLName = "SolarWindows";

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiInitialzie();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiShutdown();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiBeginFrame();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiEndFrame();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiShowDemoWindow();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiBeginMainMenuBar();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiEndMainMenuBar();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiBeginMenuBar();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiEndMenuBar();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiBeginMenu([MarshalAs(UnmanagedType.LPStr)] string label, [MarshalAs(UnmanagedType.Bool)] bool enabled = true);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiMenuItem([MarshalAs(UnmanagedType.LPStr)] string label, [MarshalAs(UnmanagedType.LPStr)] string shortcut = null, [MarshalAs(UnmanagedType.Bool)] bool selected = false, [MarshalAs(UnmanagedType.Bool)] bool enabled = true);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiEndMenu_();       // @NOTE: Win32 has EndMenu and confuse compiler

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiSeparator();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiBegin([MarshalAs(UnmanagedType.LPStr)] string name, ref int open, int flags = 0);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiEnd();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiCollapsingHeader([MarshalAs(UnmanagedType.LPStr)] string label, int flags);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiText([MarshalAs(UnmanagedType.LPStr)] string text);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiInputText([MarshalAs(UnmanagedType.LPStr)] string label, byte[] buf, int bufSize, int flags);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiSameLine(float offset_from_start_x = 0.0f, float spacing_w = 1.0f);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiButton([MarshalAs(UnmanagedType.LPStr)] string label, float sizeX = 0, float sizeY = 0);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiDragFloat3([MarshalAs(UnmanagedType.LPStr)] string label, ref float x, ref float y, ref float z, float v_speed = 1.0f, float v_min = 0.0f, float v_max = 0.0f);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiTreeNode([MarshalAs(UnmanagedType.LPStr)] string title);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiTreePop();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool ImGuiBeginTabBar([MarshalAs(UnmanagedType.LPStr)] string title, int flags);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiEndTabBar();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiBeginTabItem([MarshalAs(UnmanagedType.LPStr)] string label, [MarshalAs(UnmanagedType.U4)] ref int open, int flags);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiEndTabItem();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiColumns(int number);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiColumnsEx(int number, string title, [MarshalAs(UnmanagedType.Bool)] bool border);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiNextColumn();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiSepartor();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiSelectable(string label, [MarshalAs(UnmanagedType.Bool)] bool selected, int flags, float xSize, float ySize);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiOpenPopup([MarshalAs(UnmanagedType.LPStr)] string id, int flags);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiBeginPopupModal([MarshalAs(UnmanagedType.LPStr)] string id);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiBeginPopupModalEx([MarshalAs(UnmanagedType.LPStr)] string id, ref int open, int flags);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiCloseCurrentPopup();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiEndPopup();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiBeginPopup([MarshalAs(UnmanagedType.LPStr)] string id, int flags);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool ImGuiCheckBox([MarshalAs(UnmanagedType.LPStr)] string label, [MarshalAs(UnmanagedType.Bool)] ref bool b);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiComboStringArr([MarshalAs(UnmanagedType.LPStr)] string label, ref int currentItem, string[] items, int count, int heightInItems);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiPushItemWidth(float width);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiPopItemWidth();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		[return: MarshalAs(UnmanagedType.I1)]
		public static extern bool ImGuiInputInt([MarshalAs(UnmanagedType.LPStr)] string label, ref int v, int step, int step_fast, int flags);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiPushId(int id);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImGuiPopId();
	}
}
