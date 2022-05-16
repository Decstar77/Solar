using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{    public struct Ray
    {
        public Vector3 origin;
        public Vector3 direction;

        public override string ToString()
        {
            return $"Ray, origin={origin}, direction={direction}";
        }

        public Vector3 Travel(float dist)
        {
            return origin + (direction * dist);
        }
    }

    public struct Plane
    {
        public Vector3 origin;
        public Vector3 normal;

        public Plane()
        {
            origin = Vector3.Zero;
            normal = Vector3.Zero;
        }

        public Plane(Vector3 origin, Vector3 normal)
        {
            this.origin = origin;
            this.normal = normal;
        }

        public Plane(Triangle triangle)
        {
            normal = Vector3.Normalize(Vector3.Cross(triangle.b - triangle.a, triangle.c - triangle.a));
            origin = triangle.a;
        }
    }

    public struct Triangle
    {
        public Vector3 a = Vector3.Zero;
        public Vector3 b = Vector3.Zero;
        public Vector3 c = Vector3.Zero;

        public Triangle()
        {

        }

        public static Triangle Transform(Triangle triangle, Matrix4 transformMatrix)
        {
            Triangle t = new Triangle();
            t.a = Matrix4.Transform(transformMatrix, triangle.a);
            t.b = Matrix4.Transform(transformMatrix, triangle.b);
            t.c = Matrix4.Transform(transformMatrix, triangle.c);

            return t;
        }

        public static Vector3 GetBarycentric(Triangle triangle, Vector3 point)
        {
            Vector3 ap = point - triangle.a;
            Vector3 bp = point - triangle.b;
            Vector3 cp = point - triangle.c;

            Vector3 ab = triangle.b - triangle.a;
            Vector3 ac = triangle.c - triangle.a;
            Vector3 bc = triangle.c - triangle.b;
            Vector3 cb = triangle.b - triangle.c;
            Vector3 ca = triangle.a - triangle.c;

            Vector3 v;
            v = ab - Vector3.Project(ab, cb);
            float a = 1.0f - (Vector3.Dot(v, ap) / Vector3.Dot(v, ab));

            v = bc - Vector3.Project(bc, ac);
            float b = 1.0f - (Vector3.Dot(v, bp) / Vector3.Dot(v, bc));

            v = ca - Vector3.Project(ca, ab);
            float c = 1.0f - (Vector3.Dot(v, cp) / Vector3.Dot(v, ca));

            return new Vector3(a, b, c);
        }
    }

    public struct AlignedBox
    {
        public Vector3 min;
        public Vector3 max;

        public Vector3 Center => (min + max) / 2.0f;
        public Vector3 Extent => (min - max) / 2.0f;

        public AlignedBox()
        {
            min = Vector3.Zero;
            max = Vector3.Zero;
        }

        public AlignedBox(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }

        public AlignedBox(Vector3 min, Vector3 max, Quaternion orienation)
        {
            this.min = min;
            this.max = max;
        }

        public static AlignedBox CreateFromCenterExtent(Vector3 center, Vector3 extent)
        {
            AlignedBox result;

            result.min = center - extent;
            result.max = center + extent;

            return result;
        }

        public static AlignedBox Combine(AlignedBox b1, AlignedBox b2)
        {
            return new AlignedBox(Vector3.Min(b1.min, b2.min), Vector3.Max(b1.max, b2.max)); 
        }

        public static AlignedBox Transform(AlignedBox a, Vector3 translation, Quaternion orientation)
        {            
            Matrix3 rotation = Quaternion.ToMatrix3(orientation);

            Vector3 oldCenter = a.Center;
            Vector3 oldExtents = a.Extent;

            Vector3 newCenter = translation;
            Vector3 newExtents = Vector3.Zero;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    newCenter[i] += rotation[j][i] * oldCenter[j];
                    newExtents[i] += MathF.Abs(rotation[j][i]) * oldExtents[j];
                }
            }

            return CreateFromCenterExtent(newCenter, newExtents);
        }
    }

    public struct BoundingBox
    {
        public Vector3 origin;
        public Vector3 extents;
        public Quaternion orientation;

        public BoundingBox()
        {
            origin = Vector3.Zero;
            extents = Vector3.Zero;
            orientation = Quaternion.Identity;
        }

        public BoundingBox(Vector3 origin, Vector3 extents, Quaternion orientation)
        {
            this.origin = origin;
            this.extents = extents;
            this.orientation = orientation;
        }
    }
}
