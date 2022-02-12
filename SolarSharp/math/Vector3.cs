using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
    public struct Vector3
    {
        public static readonly Vector3 Zero = new Vector3();
        public static readonly Vector3 UnitX = new Vector3(1.0f, 0.0f, 0.0f);
        public static readonly Vector3 UnitY = new Vector3(0.0f, 1.0f, 0.0f);
        public static readonly Vector3 UnitZ = new Vector3(0.0f, 0.0f, 1.0f);

        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return string.Format("x={0}, y={1}, z={2}", x, y, z);
        }

        public float Mag { get { return MathF.Sqrt(x * x + y * y + z * z); } }
        public float MagSqrd { get { return (x * x + y * y + z * z); } }

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

        public static Vector3 Transform(Vector3 position, Matrix4 matrix)
        {
            return new Vector3(
                position.x * matrix.m11 + position.y * matrix.m21 + position.z * matrix.m31 + matrix.m41,
                position.x * matrix.m12 + position.y * matrix.m22 + position.z * matrix.m32 + matrix.m42,
                position.x * matrix.m13 + position.y * matrix.m23 + position.z * matrix.m33 + matrix.m43);
        }

        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return (left.x == right.x && left.y == right.y && left.z == right.z);
        }

        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return (left.x != right.x || left.y != right.y || left.z != right.z);
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

        public static Vector3 operator *(Vector3 left, float right)
        {
            return new Vector3(left.x * right, left.y * right, left.z * right);
        }

        public static Vector3 operator *(float right, Vector3 left)
        {
            return new Vector3(left.x * right, left.y * right, left.z * right);
        }
    }
}
