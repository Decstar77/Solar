using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y) 
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return string.Format("x={0}, y={1}", x, y);
        }

        public static Vector2 operator +(Vector2 left, Vector2 right)
        {
            return new Vector2(left.x + right.x, left.y + right.y);
        }

        public static Vector2 operator *(float right, Vector2 left)
        {
            return new Vector2(left.x * right, left.y * right);
        }
    }
}
