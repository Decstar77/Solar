using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
    public static class Util
    {
        public static float DegToRad(float deg)
        {
            return (MathF.PI / 180) * deg;
        }

        public static float RadToDeg(float rad)
        {
            return (180 / MathF.PI) * rad;
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
    }
}
