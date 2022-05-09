using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SolarSharp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Matrix4
    {
        public float m11;
        public float m12;
        public float m13;
        public float m14;
        public float m21;
        public float m22;
        public float m23;
        public float m24;
        public float m31;
        public float m32;
        public float m33;
        public float m34;
        public float m41;
        public float m42;
        public float m43;
        public float m44;

        public Matrix4(
            float m11 = 0.0f, float m12 = 0.0f, float m13 = 0.0f, float m14 = 0.0f,
            float m21 = 0.0f, float m22 = 0.0f, float m23 = 0.0f, float m24 = 0.0f,
            float m31 = 0.0f, float m32 = 0.0f, float m33 = 0.0f, float m34 = 0.0f,
            float m41 = 0.0f, float m42 = 0.0f, float m43 = 0.0f, float m44 = 0.0f)
        {
            this.m11 = m11;
            this.m12 = m12;
            this.m13 = m13;
            this.m14 = m14;

            this.m21 = m21;
            this.m22 = m22;
            this.m23 = m23;
            this.m24 = m24;

            this.m31 = m31;
            this.m32 = m32;
            this.m33 = m33;
            this.m34 = m34;

            this.m41 = m41;
            this.m42 = m42;
            this.m43 = m43;
            this.m44 = m44;
        }

        public Matrix4(Matrix3 matrix3)
        {
            m11 = matrix3.m11;
            m12 = matrix3.m12;
            m13 = matrix3.m13;
            m14 = 0;

            m21 = matrix3.m21;
            m22 = matrix3.m22;
            m23 = matrix3.m23;
            m24 = 0;

            m31 = matrix3.m31;
            m32 = matrix3.m32;
            m33 = matrix3.m33;
            m34 = 0;

            m41 = 0;
            m42 = 0;
            m43 = 0;
            m44 = 1;
        }


        public Vector4 col1 { get { return new Vector4(m11, m21, m31, m41); } set { m11 = value.x; m21 = value.y; m31 = value.z; m41 = value.w; } }
        public Vector4 col2 { get { return new Vector4(m12, m22, m32, m42); } set { m12 = value.x; m22 = value.y; m32 = value.z; m42 = value.w; } }
        public Vector4 col3 { get { return new Vector4(m13, m23, m33, m43); } set { m13 = value.x; m23 = value.y; m33 = value.z; m43 = value.w; } }
        public Vector4 col4 { get { return new Vector4(m14, m24, m34, m44); } set { m14 = value.x; m24 = value.y; m34 = value.z; m44 = value.w; } }

        public static Matrix4 Identity
        {
            get
            {
                return new Matrix4(
                    1.0f, 0.0f, 0.0f, 0.0f,
                    0.0f, 1.0f, 0.0f, 0.0f,
                    0.0f, 0.0f, 1.0f, 0.0f,
                    0.0f, 0.0f, 0.0f, 1.0f);
            }
        }

        public static Matrix4 Zero
        {
            get
            {
                return new Matrix4(
                    0.0f, 0.0f, 0.0f, 0.0f,
                    0.0f, 0.0f, 0.0f, 0.0f,
                    0.0f, 0.0f, 0.0f, 0.0f,
                    0.0f, 0.0f, 0.0f, 0.0f);
            }
        }

        public static Vector3 Transform(Matrix4 a, Vector3 b)
        {
            return new Vector3(a * new Vector4(b.x, b.y, b.z, 1.0f));
        }

        public static Matrix4 CreatePerspectiveRH(float fovYRads, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            var yScale = 1.0f / (float)MathF.Tan(fovYRads * 0.5f);
            var xScale = yScale / aspectRatio;

            Matrix4 result;
            result.m11 = xScale;
            result.m12 = result.m13 = result.m14 = 0.0f;
            result.m22 = yScale;
            result.m21 = result.m23 = result.m24 = 0.0f;
            result.m31 = result.m32 = 0.0f;
            result.m33 = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.m43 = -1.0f;
            result.m41 = result.m42 = result.m44 = 0.0f;
            result.m34 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);

            return result;
        }

        public static Matrix4 CreatePerspectiveLH(float fovYRads, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            var yScale = 1.0f / (float)MathF.Tan(fovYRads * 0.5f);
            var xScale = yScale / aspectRatio;

            Matrix4 result;
            result.m11 = xScale;
            result.m12 = result.m13 = result.m14 = 0.0f;
            result.m22 = yScale;
            result.m21 = result.m23 = result.m24 = 0.0f;
            result.m31 = result.m32 = 0.0f;
            result.m33 = -farPlaneDistance / (nearPlaneDistance - farPlaneDistance);
            result.m34 = 1.0f;
            result.m41 = result.m42 = result.m44 = 0.0f;
            result.m43 = nearPlaneDistance * farPlaneDistance / (nearPlaneDistance - farPlaneDistance);

            return result;
        }

        public static Matrix4 CreateLookAtRH(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
        {
            Vector3 cameraReverseDirection = Vector3.Normalize(cameraTarget - cameraPosition);

            Vector3 right = Vector3.Normalize(Vector3.Cross(cameraReverseDirection, cameraUpVector));
            Vector3 up = Vector3.Cross(right, cameraReverseDirection);
            Vector3 forward = cameraReverseDirection * -1.0f;

            Matrix4 result;

            result.m11 = right.x;
            result.m12 = right.y;
            result.m13 = right.z;
            result.m14 = 0;

            result.m21 = up.x;
            result.m22 = up.y;
            result.m23 = up.z;
            result.m24 = 0;

            result.m31 = forward.x;
            result.m32 = forward.y;
            result.m33 = forward.z;
            result.m34 = 0;

            result.m41 = cameraPosition.x;
            result.m42 = cameraPosition.y;
            result.m43 = cameraPosition.z;
            result.m44 = 1;

            return result.Transpose;
        }

        public static Matrix4 CreateLookAtLH(Vector3 cameraPosition, Vector3 cameraTarget, Vector3 cameraUpVector)
        {
            Vector3 cameraReverseDirection = Vector3.Normalize(cameraPosition - cameraTarget);

            Vector3 right = Vector3.Normalize(Vector3.Cross(cameraReverseDirection, cameraUpVector));
            Vector3 up = Vector3.Cross(right, cameraReverseDirection);
            Vector3 forward = cameraReverseDirection * -1.0f;

            Matrix4 result;

            result.m11 = right.x;
            result.m12 = right.y;
            result.m13 = right.z;
            result.m14 = 0;

            result.m21 = up.x;
            result.m22 = up.y;
            result.m23 = up.z;
            result.m24 = 0;

            result.m31 = forward.x;
            result.m32 = forward.y;
            result.m33 = forward.z;
            result.m34 = 0;

            result.m41 = cameraPosition.x;
            result.m42 = cameraPosition.y;
            result.m43 = cameraPosition.z;
            result.m44 = 1;

            return result;
        }

        public static Matrix4 TranslateRH(Matrix4 matrix, Vector3 translation)
        {
            matrix.m14 += translation.x;
            matrix.m24 += translation.y;
            matrix.m34 += translation.z;
            return matrix;
        }

        public static Matrix4 TranslateLH(Matrix4 matrix, Vector3 translation)
        {
            matrix.m41 += translation.x;
            matrix.m42 += translation.y;
            matrix.m43 += translation.z;
            return matrix;
        }

        public static Matrix4 ScaleCardinal(Matrix4 matrix, Vector3 scale)
        {
            matrix.col1 *= scale.x;
            matrix.col2 *= scale.y;
            matrix.col3 *= scale.z;

            return matrix;
        }

        public static Quaternion ToQuaternion(Matrix4 matrix)
        {
            Quaternion result = Matrix3.ToQuaternion(new Matrix3(matrix));

            return result;
        }

        public static void Decompose(Matrix4 transform, out Vector3 position, out Quaternion orientation, out Vector3 scale)
        {
            position = new Vector3(transform.col4);

            float m1 = transform.col1.Mag;
            float m2 = transform.col2.Mag;
            float m3 = transform.col3.Mag;

            scale = new Vector3(m1, m2, m3);

            transform.col1 /= m1;
            transform.col2 /= m2;
            transform.col3 /= m3;

            orientation = Matrix4.ToQuaternion(transform);
        }

        public static Matrix4 CreateFromYawPitchRoll(float yaw, float pitch, float roll)
        {
            //  Roll first, about axis the object is facing, then
            //  pitch upward, then yaw to face into the new heading
            float halfRoll = roll * 0.5f;
            float sr = (float)MathF.Sin(halfRoll);
            float cr = (float)MathF.Cos(halfRoll);

            float halfPitch = pitch * 0.5f;
            float sp = (float)MathF.Sin(halfPitch);
            float cp = (float)MathF.Cos(halfPitch);

            float halfYaw = yaw * 0.5f;
            float sy = (float)MathF.Sin(halfYaw);
            float cy = (float)MathF.Cos(halfYaw);

            float x = cy * sp * cr + sy * cp * sr;
            float y = sy * cp * cr - cy * sp * sr;
            float z = cy * cp * sr - sy * sp * cr;
            float w = cy * cp * cr + sy * sp * sr;

            float xx = x * x;
            float yy = y * y;
            float zz = z * z;

            float xy = x * y;
            float wz = z * w;
            float xz = z * x;
            float wy = y * w;
            float yz = y * z;
            float wx = x * w;

            Matrix4 result = Matrix4.Identity;
            result.m11 = 1.0f - 2.0f * (yy + zz);
            result.m12 = 2.0f * (xy + wz);
            result.m13 = 2.0f * (xz - wy);
            result.m21 = 2.0f * (xy - wz);
            result.m22 = 1.0f - 2.0f * (zz + xx);
            result.m23 = 2.0f * (yz + wx);
            result.m31 = 2.0f * (xz + wy);
            result.m32 = 2.0f * (yz - wx);
            result.m33 = 1.0f - 2.0f * (yy + xx);

            return result;
        }

        public override string ToString()
        {
            return string.Format(@"Matrix4(
                {0,10:0.00}, {1,10:0.00}, {2,10:0.00}, {3,10:0.00},
                {4,10:0.00}, {5,10:0.00}, {6,10:0.00}, {7,10:0.00},
                {8,10:0.00}, {9,10:0.00}, {10,10:0.00}, {11,10:0.00},
                {12,10:0.00}, {13,10:0.00}, {14,10:0.00}, {15,10:0.00}  )",
            m11, m12, m13, m14,
            m21, m22, m23, m24,
            m31, m32, m33, m34,
            m41, m42, m43, m44);
        }

        public float Determinant
        {
            get
            {
                float A0 = m11 * m22 - m12 * m21;
                float A1 = m11 * m23 - m13 * m21;
                float A2 = m11 * m24 - m14 * m21;
                float A3 = m12 * m23 - m13 * m22;
                float A4 = m12 * m24 - m14 * m22;
                float A5 = m13 * m24 - m14 * m23;
                float B0 = m31 * m42 - m32 * m41;
                float B1 = m31 * m43 - m33 * m41;
                float B2 = m31 * m44 - m34 * m41;
                float B3 = m32 * m43 - m33 * m42;
                float B4 = m32 * m44 - m34 * m42;
                float B5 = m33 * m44 - m34 * m43;

                return A0 * B5 - A1 * B4 + A2 * B3 + A3 * B2 - A4 * B1 + A5 * B0;
            }
        }

        public Matrix4 Inverse
        {
            get
            {
                float d = this.Determinant;
                if (d == 0.0)
                    return Matrix4.Identity;

                Matrix4 M = new Matrix4();
                M.m11 = (m23 * m34 * m42 - m24 * m33 * m42 + m24 * m32 * m43 -
                         m22 * m34 * m43 - m23 * m32 * m44 + m22 * m33 * m44) / d;
                M.m12 = (m14 * m33 * m42 - m13 * m34 * m42 - m14 * m32 * m43 +
                         m12 * m34 * m43 + m13 * m32 * m44 - m12 * m33 * m44) / d;
                M.m13 = (m13 * m24 * m42 - m14 * m23 * m42 + m14 * m22 * m43 -
                         m12 * m24 * m43 - m13 * m22 * m44 + m12 * m23 * m44) / d;
                M.m14 = (m14 * m23 * m32 - m13 * m24 * m32 - m14 * m22 * m33 +
                         m12 * m24 * m33 + m13 * m22 * m34 - m12 * m23 * m34) / d;
                M.m21 = (m24 * m33 * m41 - m23 * m34 * m41 - m24 * m31 * m43 +
                         m21 * m34 * m43 + m23 * m31 * m44 - m21 * m33 * m44) / d;
                M.m22 = (m13 * m34 * m41 - m14 * m33 * m41 + m14 * m31 * m43 -
                         m11 * m34 * m43 - m13 * m31 * m44 + m11 * m33 * m44) / d;
                M.m23 = (m14 * m23 * m41 - m13 * m24 * m41 - m14 * m21 * m43 +
                         m11 * m24 * m43 + m13 * m21 * m44 - m11 * m23 * m44) / d;
                M.m24 = (m13 * m24 * m31 - m14 * m23 * m31 + m14 * m21 * m33 -
                         m11 * m24 * m33 - m13 * m21 * m34 + m11 * m23 * m34) / d;
                M.m31 = (m22 * m34 * m41 - m24 * m32 * m41 + m24 * m31 * m42 -
                         m21 * m34 * m42 - m22 * m31 * m44 + m21 * m32 * m44) / d;
                M.m32 = (m14 * m32 * m41 - m12 * m34 * m41 - m14 * m31 * m42 +
                         m11 * m34 * m42 + m12 * m31 * m44 - m11 * m32 * m44) / d;
                M.m33 = (m12 * m24 * m41 - m14 * m22 * m41 + m14 * m21 * m42 -
                         m11 * m24 * m42 - m12 * m21 * m44 + m11 * m22 * m44) / d;
                M.m34 = (m14 * m22 * m31 - m12 * m24 * m31 - m14 * m21 * m32 +
                         m11 * m24 * m32 + m12 * m21 * m34 - m11 * m22 * m34) / d;
                M.m41 = (m23 * m32 * m41 - m22 * m33 * m41 - m23 * m31 * m42 +
                         m21 * m33 * m42 + m22 * m31 * m43 - m21 * m32 * m43) / d;
                M.m42 = (m12 * m33 * m41 - m13 * m32 * m41 + m13 * m31 * m42 -
                         m11 * m33 * m42 - m12 * m31 * m43 + m11 * m32 * m43) / d;
                M.m43 = (m13 * m22 * m41 - m12 * m23 * m41 - m13 * m21 * m42 +
                         m11 * m23 * m42 + m12 * m21 * m43 - m11 * m22 * m43) / d;
                M.m44 = (m12 * m23 * m31 - m13 * m22 * m31 + m13 * m21 * m32 -
                         m11 * m23 * m32 - m12 * m21 * m33 + m11 * m22 * m33) / d;
                return M;
            }
        }

        public Matrix4 Transpose
        {
            get
            {
                return new Matrix4(
                    m11, m21, m31, m41,
                    m12, m22, m32, m42,
                    m13, m23, m33, m43,
                    m14, m24, m34, m44);
            }
        }

        public static Matrix4 operator *(Matrix4 value1, Matrix4 value2)
        {
            Matrix4 m;

            // First row
            m.m11 = value1.m11 * value2.m11 + value1.m12 * value2.m21 + value1.m13 * value2.m31 + value1.m14 * value2.m41;
            m.m12 = value1.m11 * value2.m12 + value1.m12 * value2.m22 + value1.m13 * value2.m32 + value1.m14 * value2.m42;
            m.m13 = value1.m11 * value2.m13 + value1.m12 * value2.m23 + value1.m13 * value2.m33 + value1.m14 * value2.m43;
            m.m14 = value1.m11 * value2.m14 + value1.m12 * value2.m24 + value1.m13 * value2.m34 + value1.m14 * value2.m44;

            // Second row
            m.m21 = value1.m21 * value2.m11 + value1.m22 * value2.m21 + value1.m23 * value2.m31 + value1.m24 * value2.m41;
            m.m22 = value1.m21 * value2.m12 + value1.m22 * value2.m22 + value1.m23 * value2.m32 + value1.m24 * value2.m42;
            m.m23 = value1.m21 * value2.m13 + value1.m22 * value2.m23 + value1.m23 * value2.m33 + value1.m24 * value2.m43;
            m.m24 = value1.m21 * value2.m14 + value1.m22 * value2.m24 + value1.m23 * value2.m34 + value1.m24 * value2.m44;

            // Third row
            m.m31 = value1.m31 * value2.m11 + value1.m32 * value2.m21 + value1.m33 * value2.m31 + value1.m34 * value2.m41;
            m.m32 = value1.m31 * value2.m12 + value1.m32 * value2.m22 + value1.m33 * value2.m32 + value1.m34 * value2.m42;
            m.m33 = value1.m31 * value2.m13 + value1.m32 * value2.m23 + value1.m33 * value2.m33 + value1.m34 * value2.m43;
            m.m34 = value1.m31 * value2.m14 + value1.m32 * value2.m24 + value1.m33 * value2.m34 + value1.m34 * value2.m44;

            // Fourth row
            m.m41 = value1.m41 * value2.m11 + value1.m42 * value2.m21 + value1.m43 * value2.m31 + value1.m44 * value2.m41;
            m.m42 = value1.m41 * value2.m12 + value1.m42 * value2.m22 + value1.m43 * value2.m32 + value1.m44 * value2.m42;
            m.m43 = value1.m41 * value2.m13 + value1.m42 * value2.m23 + value1.m43 * value2.m33 + value1.m44 * value2.m43;
            m.m44 = value1.m41 * value2.m14 + value1.m42 * value2.m24 + value1.m43 * value2.m34 + value1.m44 * value2.m44;

            return m;
        }

        public static Vector4 operator *(Matrix4 value1, Vector4 value2)
        {
            float x = Vector4.Dot(value1.col1, value2);
            float y = Vector4.Dot(value1.col2, value2);
            float z = Vector4.Dot(value1.col3, value2);
            float w = Vector4.Dot(value1.col4, value2);

            return new Vector4(x, y, z, w);
        }
    }

}
