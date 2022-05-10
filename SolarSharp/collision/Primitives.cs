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
