using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp.EngineAPI
{
    public class ImGizmoAPI
    {
        const string DLLName = "SolarWindows";

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void GizmoEnable(bool enable);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        public static extern void GizmoSetRect(float x, float y, float width, float height);

        [DllImport(DLLName, CallingConvention = CallingConvention.StdCall)]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool GizmoManipulate(Matrix4 proj, Matrix4 view, ref Matrix4 world, int operation, int mode);

    }
}
