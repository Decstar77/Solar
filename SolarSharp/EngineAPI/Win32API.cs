using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SolarSharp.EngineAPI
{
	public class Win32API
    {
        const string DLLName = "SolarWindows";
        [DllImport(DLLName, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool CreateWindow_([MarshalAs(UnmanagedType.LPStr)] string title, int width, int height, int xPos, int yPos);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern bool PumpMessages_(ref FrameInput input);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void PostQuitMessage_();

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void DestroyWindow_();

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void OpenNativeFileDialog(byte[] output);
    }
}
