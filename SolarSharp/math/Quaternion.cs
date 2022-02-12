using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
    public struct Quaternion
    {
        public float w;
        public float x;
        public float y;
        public float z;

        public Quaternion(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public Quaternion(Vector3 direction, float angleRads)
        {
            float halfAngleRadians = 0.5f * angleRads;

            w = MathF.Cos(halfAngleRadians);

            float halfSine = MathF.Sin(halfAngleRadians);
            direction = Vector3.Normalize(direction);
            x = direction.x * halfSine;
            y = direction.y * halfSine;
            z = direction.z * halfSine;
        }

        public static Quaternion Identity
        {
            get
            {
                return new Quaternion(0, 0, 0, 1);
            }
        }

        public override string ToString()
        {
            return String.Format("x={0}, y={1}, z={2}, w={3}", x, y, z, w);
        }

        public float Mag { get { return MathF.Sqrt(x*x + y*y + z*z + w*w);} }
        public float MagSqrd { get { return x * x + y * y + z * z + w * w; } }

        public static Vector3 RotatePoint(Vector3 point, Quaternion delta)
        {
            Quaternion vec = new Quaternion(point.x, point.y, point.z, 0.0f );
            Quaternion final = delta * vec * Inverse(delta);
            return new Vector3(final.x, final.y, final.z);
        }

        public static Quaternion Normalize(Quaternion value)
        {
            float invMag = 1.0f / value.Mag;

            if (0.0f * invMag == 0.0f * invMag)
            {
                return new Quaternion(value.x * invMag, value.y * invMag, value.z * invMag, value.w * invMag);
            }

            return Identity;
        }

        public static Quaternion Inverse(Quaternion value)
        {
            Quaternion res = value *  (1.0f / value.MagSqrd);
            res.x = -res.x;
            res.y = -res.y;
            res.z = -res.z;

            return res;
        }

        public static Matrix3 ToMatrix3(Quaternion value)
        {
            Matrix3 matrix = Matrix3.Identity;
            
            matrix.row1 = RotatePoint(matrix.row1, value);
            matrix.row2 = RotatePoint(matrix.row2, value);
            matrix.row3 = RotatePoint(matrix.row3, value);

            return matrix;
        }

        public static Matrix4 ToMatrix4(Quaternion value)
        {
            return new Matrix4(ToMatrix3(value));
        }

        public static Quaternion RotateLocalX(Quaternion value, float angleRads)
        {
            float hangle = angleRads * 0.5f;            
            float s = MathF.Sin(hangle);
            float c = MathF.Cos(hangle);
            
            return new Quaternion(
                     c * value.x + s * value.w,
                     c * value.y - s * value.z,
                     c * value.z + s * value.y,
                     c * value.w - s * value.x);
        }

        public static Quaternion RotateLocalY(Quaternion value, float angleRads)
        {
            float hangle = angleRads * 0.5f;
            float s = MathF.Sin(hangle);
            float c = MathF.Cos(hangle);

            return new Quaternion(
                     c * value.x + s * value.z,
                     c * value.y + s * value.w,
                     c * value.z - s * value.x,
                     c * value.w - s * value.y);
        }

        public static Quaternion RotateLocalZ(Quaternion value, float angleRads)
        {
            float hangle = angleRads * 0.5f;
            float s = MathF.Sin(hangle);
            float c = MathF.Cos(hangle);
            
            return new Quaternion(
                     c * value.x - s * value.y,
                     c * value.y + s * value.x,
                     c * value.z + s * value.w,
                     c * value.w - s * value.z);
        }

        public static Quaternion operator *(Quaternion left, float right)
        {
            Quaternion res = left;
            res.x *= right;
            res.y *= right;
            res.z *= right;
            res.w *= right;

            return res;
        }

        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            Quaternion res = new Quaternion();

            res.w = (left.w * right.w) - (left.x * right.x) - (left.y * right.y) - (left.z * right.z);
            res.x = (left.x * right.w) + (left.w * right.x) + (left.y * right.z) - (left.z * right.y);
            res.y = (left.y * right.w) + (left.w * right.y) + (left.z * right.x) - (left.x * right.z);
            res.z = (left.z * right.w) + (left.w * right.z) + (left.x * right.y) - (left.y * right.x);

            return res;
        }
    }
}
