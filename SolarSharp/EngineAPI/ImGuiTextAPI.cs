using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SolarSharp.EngineAPI
{
    public class ImGuiTextAPI
    {
        const string DLLName = "SolarWindows";

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ImGuiTextInitialize();

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ImGuiTextRender( [MarshalAs(UnmanagedType.LPStr)] string title);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ImGuiTextSetText([MarshalAs(UnmanagedType.LPStr)] string text);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ImGuiTextGetText(byte[] buf, int bufSize);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ImGuiTextIsShowingWhitespaces();
        
        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ImGuiTextSetShowWhitespaces([MarshalAs(UnmanagedType.Bool)] bool v);
        
        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool ImGuiTextIsReadOnly();
        
        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void ImGuiTextSetReadOnly([MarshalAs(UnmanagedType.Bool)] bool v);


    }
}
