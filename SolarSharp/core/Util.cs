using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
    public static class Util
    {
        public static string AsciiBytesToString(byte[] buffer, int offset)
        {
            int end = offset;
            while (end < buffer.Length && buffer[end] != 0)
            {
                end++;
            }
            unsafe
            {
                fixed (byte* pAscii = buffer)
                {
                    return new string((sbyte*)pAscii, offset, end - offset);
                }
            }
        }

        public static Vector4 ColourVec(int r, int g, int b, int a)
        {
            return new Vector4(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);
        }

        public static uint ColourUnit(Vector4 col)
        {
            return ColourUnit(col.x, col.y, col.z, col.w);
        }

        public static uint ColourUnit(float r, float g, float b, float a)
        {
            uint rr = ((byte)(r * 255.0f));
            uint gg = ((byte)(g * 255.0f));
            uint bb = ((byte)(b * 255.0f));
            uint aa = ((byte)(a * 255.0f));

            uint col = (aa << 24) | (bb << 16) | (gg << 8) | rr;

            return col;
        }
    }
}
