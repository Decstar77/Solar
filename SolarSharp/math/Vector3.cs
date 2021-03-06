using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace SolarSharp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3
    {
        public static readonly Vector3 Zero = new Vector3();
        public static readonly Vector3 UnitX = new Vector3(1.0f, 0.0f, 0.0f);
        public static readonly Vector3 UnitY = new Vector3(0.0f, 1.0f, 0.0f);
        public static readonly Vector3 UnitZ = new Vector3(0.0f, 0.0f, 1.0f);

        [JsonInclude] public float x;
        [JsonInclude] public float y;
        [JsonInclude] public float z;

        //public float X { get { return x; } set { x = value; } }

        public Vector3(float a)
        {
            x = a; 
            y = a;
            z = a;
        }

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(Vector4 v4)
        {
            this.x = v4.x;
            this.y = v4.y;
            this.z = v4.z;
        }

        public override string ToString()
        {
            return string.Format("x={0} y={1} z={2}", x, y, z);
        }

        [JsonIgnore]
        public float Mag { get { return MathF.Sqrt(x * x + y * y + z * z); } }
        [JsonIgnore]
        public float MagSqrd { get { return (x * x + y * y + z * z); } }

        public float this[int key]
        {
            get { 
                if (key == 0) return x; 
                if (key == 1) return y;
                if (key == 2) return z;
                return 0;
            }

            set {
                if (key == 0) x = value;
                else if (key == 1) y = value;
                else if (key == 2) z = value;                
            }
        }

        public static Vector3 Normalize(Vector3 value)
        {
            var length = (float)MathF.Sqrt(value.x * value.x + value.y * value.y + value.z * value.z);
            return new Vector3(value.x / length, value.y / length, value.z / length);
        }

        public static float Dot(Vector3 vector1, Vector3 vector2)
        {
            return vector1.x * vector2.x +
                   vector1.y * vector2.y +
                   vector1.z * vector2.z;
        }

        public static Vector3 Cross(Vector3 vector1, Vector3 vector2)
        {
            return new Vector3(
                vector1.y * vector2.z - vector1.z * vector2.y,
                vector1.z * vector2.x - vector1.x * vector2.z,
                vector1.x * vector2.y - vector1.y * vector2.x);
        }

        public static Vector3 Project(Vector3 a, Vector3 b)
        {
            float nume = Dot(a, b);
            float demon = b.MagSqrd;

            float s = nume / demon;

            Vector3 result = s * b;

            return result;
        }

        public static Vector3 Transform(Vector3 position, Matrix4 matrix)
        {
            return new Vector3(
                position.x * matrix.m11 + position.y * matrix.m21 + position.z * matrix.m31 + matrix.m41,
                position.x * matrix.m12 + position.y * matrix.m22 + position.z * matrix.m32 + matrix.m42,
                position.x * matrix.m13 + position.y * matrix.m23 + position.z * matrix.m33 + matrix.m43);
        }

        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            return new Vector3(MathF.Min(a.x, b.x), MathF.Min(a.y, b.y), MathF.Min(a.z, b.z));
        }

        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            return new Vector3(MathF.Max(a.x, b.x), MathF.Max(a.y, b.y), MathF.Max(a.z, b.z));
        }
        public static Vector3 Abs(Vector3 a)
        {
            return new Vector3(MathF.Abs(a.x), MathF.Abs(a.y), MathF.Abs(a.z));
        }

        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return (left.x == right.x && left.y == right.y && left.z == right.z);
        }

        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return (left.x != right.x || left.y != right.y || left.z != right.z);
        }

        public override bool Equals([NotNullWhen(true)] object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is Vector3)
            {
                return this == (Vector3)obj;
            }

            return false;
        }

        public static Vector3 operator +(Vector3 left, Vector3 right)
        {
            return new Vector3(left.x + right.x, left.y + right.y, left.z + right.z);
        }

        public static Vector3 operator -(Vector3 left, Vector3 right)
        {
            return new Vector3(left.x - right.x, left.y - right.y, left.z - right.z);
        }

        public static Vector3 operator -(Vector3 v)
        {
            return new Vector3(-v.x, -v.y, -v.z);
        }

        public static Vector3 operator *(Vector3 left, Vector3 right)
        {
            return new Vector3(left.x * right.x, left.y * right.y, left.z * right.z);
        }

        public static Vector3 operator /(Vector3 left, Vector3 right)
        {
            return new Vector3(left.x / right.x, left.y / right.y, left.z / right.z);
        }

        public static Vector3 operator *(Vector3 left, float right)
        {
            return new Vector3(left.x * right, left.y * right, left.z * right);
        }

        public static Vector3 operator *(float right, Vector3 left)
        {
            return new Vector3(left.x * right, left.y * right, left.z * right);
        }

        public static Vector3 operator /(Vector3 left, float right)
        {
            return new Vector3(left.x / right, left.y / right, left.z / right);
        }

    }
}
