using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SolarSharp.EngineAPI
{
    public class ImNodesAPI
    {
        const string DLLName = "SolarWindows";

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ImNodesInitialzie();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImNodesShutdown();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImNodesBeginNodeEditor();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImNodesEndNodeEditor();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImNodesBeginNode(int id);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImNodesEndNode();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImNodesBeginNodeTitleBar();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImNodesEndNodeTitleBar();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImNodesBeginInputAttribute(int id, int shape);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImNodesEndInputAttribute();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImNodesBeginOutputAttribute(int id, int shape);
				
		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImNodesEndOutputAttribute();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImNodesLink(int id, int startAttrId, int endAttrId);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool ImNodesIsLinkCreated(ref int startedAtPinId, ref int endedAtPinId, ref int createdFromSnap);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool ImNodesIsLinkDropped(ref int started_at_pin_id, [MarshalAs(UnmanagedType.Bool)] bool including_detached_links);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImNodesPushColorStyle(int item, uint col);

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern void ImNodesPopColorStyle();

		[DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
		public static extern bool ImNodesIsEditorHovered();

	}
}
