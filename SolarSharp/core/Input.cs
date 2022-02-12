using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SolarSharp
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MouseIput
    {
        [MarshalAs(UnmanagedType.Bool)] internal bool mouseLocked;
        internal double mouseXPositionPixelCoords;
        internal double mouseYPositionPixelCoords;
        internal double mouseXPositionNormalCoords;
        internal double mouseYPositionNormalCoords;
        [MarshalAs(UnmanagedType.Bool)] internal bool mb1;
        [MarshalAs(UnmanagedType.Bool)] internal bool mb2;
        [MarshalAs(UnmanagedType.Bool)] internal bool mb3;
    }


    public class Input
    {
        internal int[] keys = new int[ushort.MaxValue];
        internal MouseIput mouseIput = new MouseIput();

        internal void Copy(Input input)
        {
            Array.Copy(input.keys, keys, keys.Length);
            mouseIput = input.mouseIput;
        }
    }
}
