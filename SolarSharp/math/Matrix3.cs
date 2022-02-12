using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
    public struct Matrix3
    {
        public float m11;
        public float m12;
        public float m13;
        public float m21;
        public float m22;
        public float m23;
        public float m31;
        public float m32;
        public float m33;

        public Matrix3(
            float m11 = 0.0f, float m12 = 0.0f, float m13 = 0.0f,
            float m21 = 0.0f, float m22 = 0.0f, float m23 = 0.0f,
            float m31 = 0.0f, float m32 = 0.0f, float m33 = 0.0f)
        {
            this.m11 = m11;
            this.m12 = m12;
            this.m13 = m13;
            this.m21 = m21;
            this.m22 = m22;
            this.m23 = m23;
            this.m31 = m31;
            this.m32 = m32;
            this.m33 = m33;
        }

        public Matrix3(Matrix4 matrix)
        {
            m11 = matrix.m11;
            m12 = matrix.m12;
            m13 = matrix.m13;
            m21 = matrix.m21;
            m22 = matrix.m22;
            m23 = matrix.m23;
            m31 = matrix.m31;
            m32 = matrix.m32;
            m33 = matrix.m33;
        }

        public Vector3 row1 { get { return new Vector3(m11, m12, m13); } set { m11 = value.x; m12 = value.y; m13 = value.z; } }
        public Vector3 row2 { get { return new Vector3(m21, m22, m23); } set { m21 = value.x; m22 = value.y; m23 = value.z; } }
        public Vector3 row3 { get { return new Vector3(m31, m32, m33); } set { m31 = value.x; m32 = value.y; m33 = value.z; } }


        public Vector3 col1 { get { return new Vector3(m11, m21, m31); } set { m11 = value.x; m21 = value.y; m31 = value.z; } }
        public Vector3 col2 { get { return new Vector3(m12, m22, m32); } set { m12 = value.x; m22 = value.y; m32 = value.z; } }
        public Vector3 col3 { get { return new Vector3(m13, m23, m33); } set { m13 = value.x; m23 = value.y; m33 = value.z; } }

        public static Matrix3 Identity
        {
            get
            {
                return new Matrix3(
                    1.0f, 0.0f, 0.0f,
                    0.0f, 1.0f, 0.0f,
                    0.0f, 0.0f, 1.0f);
            }
        }

        public static Matrix3 Zero
        {
            get
            {
                return new Matrix3(
                    0.0f, 0.0f, 0.0f,
                    0.0f, 0.0f, 0.0f,
                    0.0f, 0.0f, 0.0f);
            }
        }
                
        public override string ToString()
        {
            string pattern = @"Matrix3(
                {0,10:0.000}, {1,10:0.000}, {2,10:0.000},
                {3,10:0.000}, {4,10:0.000}, {5,10:0.000},
                {6,10:0.000}, {7,10:0.000}, {8,10:0.000}  )";

            return string.Format(pattern,
                m11, m12, m13,
                m21, m22, m23,
                m31, m32, m33);
        }

        public float Determinant
        {
            get
            {
                return m11 * m22 * m33 +
                       m12 * m23 * m31 +
                       m13 * m21 * m32 -
                       m11 * m23 * m32 -
                       m12 * m21 * m33 -
                       m13 * m22 * m31;
            }
        }

        public Matrix3 Inverse
        {
            get
            {
                float d = this.Determinant;
                if (d == 0.0)
                    return Matrix3.Identity;

                Matrix3 tmp = new Matrix3();
                tmp.m11 = (m22 * m33 - m23 * m32) / d;
                tmp.m12 = (m13 * m32 - m12 * m33) / d;
                tmp.m13 = (m12 * m23 - m13 * m22) / d;
                tmp.m21 = (m23 * m31 - m21 * m33) / d;
                tmp.m22 = (m11 * m33 - m13 * m31) / d;
                tmp.m23 = (m13 * m21 - m11 * m23) / d;
                tmp.m31 = (m21 * m32 - m22 * m31) / d;
                tmp.m32 = (m12 * m31 - m11 * m32) / d;
                tmp.m33 = (m11 * m22 - m12 * m21) / d;
                return tmp;
            }
        }

        public Matrix3 Transpose
        {
            get
            {
                return new Matrix3(
                    m11, m21, m31,
                    m12, m22, m32,
                    m13, m23, m33);
            }
        }

        public Basis Basis
        {
            get 
            {
                return new Basis(col1, col2, col3);
            }
        }


        public static Quaternion ToQuaternion(Matrix3 matrix)
        {   
            Quaternion q = new Quaternion();
                        
            float t;
            float tr = matrix.m11 + matrix.m22 + matrix.m33;

            if (tr >= 0.0f)
            {
                t = MathF.Sqrt(tr + 1.0f);
                q.w = t * 0.5f;
                t = 0.5f / t;
                q.x = (matrix.m23 - matrix.m32) * t;
                q.y = (matrix.m31 - matrix.m13) * t;
                q.z = (matrix.m12 - matrix.m21) * t;
            }
            else
            {
                if (matrix.m11 >= matrix.m22 && matrix.m11 >= matrix.m33)
                {
                    t = MathF.Sqrt(matrix.m11 - (matrix.m22 + matrix.m33) + 1.0f);
                    q.x = t * 0.5f;
                    t = 0.5f / t;
                    q.y = (matrix.m21 + matrix.m12) * t;
                    q.z = (matrix.m13 + matrix.m31) * t;
                    q.w = (matrix.m23 - matrix.m32) * t;
                }
                else if (matrix.m22 > matrix.m11)
                {
                    t = MathF.Sqrt(matrix.m22 - (matrix.m33 + matrix.m11) + 1.0f);
                    q.y = t * 0.5f;
                    t = 0.5f / t;
                    q.z = (matrix.m32 + matrix.m23) * t;
                    q.x = (matrix.m21 + matrix.m12) * t;
                    q.w = (matrix.m31 - matrix.m13) * t;
                }
                else
                {
                    t = MathF.Sqrt(matrix.m33 - (matrix.m11 + matrix.m22) + 1.0f);
                    q.z = t * 0.5f;
                    t = 0.5f / t;
                    q.x = (matrix.m13 + matrix.m31) * t;
                    q.y = (matrix.m32 + matrix.m23) * t;
                    q.w = (matrix.m12 - matrix.m21) * t;
                }
            }

            return q;
        }

        public static Matrix3 operator *(Matrix3 a, Matrix3 b)
        {
            Matrix3 M = new Matrix3();
            M.m11 = a.m11 * b.m11 + a.m12 * b.m21 + a.m13 * b.m31;
            M.m12 = a.m11 * b.m12 + a.m12 * b.m22 + a.m13 * b.m32;
            M.m13 = a.m11 * b.m13 + a.m12 * b.m23 + a.m13 * b.m33;
            M.m21 = a.m21 * b.m11 + a.m22 * b.m21 + a.m23 * b.m31;
            M.m22 = a.m21 * b.m12 + a.m22 * b.m22 + a.m23 * b.m32;
            M.m23 = a.m21 * b.m13 + a.m22 * b.m23 + a.m23 * b.m33;
            M.m31 = a.m31 * b.m11 + a.m32 * b.m21 + a.m33 * b.m31;
            M.m32 = a.m31 * b.m12 + a.m32 * b.m22 + a.m33 * b.m32;
            M.m33 = a.m31 * b.m13 + a.m32 * b.m23 + a.m33 * b.m33;
            return M;
        }
    }
}
