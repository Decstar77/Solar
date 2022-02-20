using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SolarSharp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4
    {
        public static readonly Vector4 Zero = new Vector4();
        public static readonly Vector4 UnitX = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
        public static readonly Vector4 UnitY = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);
        public static readonly Vector4 UnitZ = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);

        public float x;
        public float y;
        public float z;
        public float w;

        public Vector4(float value)
        {
            this.x = value;
            this.y = value;
            this.z = value;
            this.w = value;
        }

        public Vector4(Vector3 v3, float w)
        {
            x = v3.x;
            y = v3.y;
            z = v3.z;
            this.w = w;
        }

        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public float Mag { get { return MathF.Sqrt(x * x + y * y + z * z + w * w); } }
        public float MagSqrd { get { return (x * x + y * y + z * z + w * w); } }

        public override string ToString()
        {
            return string.Format("x={0}, y={1}, z={2}, w={3}", x, y, z, w);
        }

        public static float Dot(Vector4 a, Vector4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        public static Vector4 operator +(Vector4 left, Vector4 right)
        {
            return new Vector4(left.x + right.x, left.y + right.y, left.z + right.z, left.w + right.w);
        }

        public static Vector4 operator -(Vector4 left, Vector4 right)
        {
            return new Vector4(left.x - right.x, left.y - right.y, left.z - right.z, left.w - right.w);
        }

        public static Vector4 operator -(Vector4 v)
        {
            return new Vector4(-v.x, -v.y, -v.z, -v.w);
        }

        public static Vector4 operator *(Vector4 left, Vector4 right)
        {
            return new Vector4(left.x * right.x, left.y * right.y, left.z * right.z, left.w * right.w);
        }

        public static Vector4 operator *(Vector4 left, float right)
        {
            return new Vector4(left.x * right, left.y * right, left.z * right, left.w * right);
        }

        public static Vector4 operator *(float right, Vector4 left)
        {
            return new Vector4(left.x * right, left.y * right, left.z * right, left.w * right);
        }

        public static Vector4 operator /(Vector4 left, float right)
        {
            return new Vector4(left.x / right, left.y / right, left.z / right, left.w / right);
        }

    }
}
