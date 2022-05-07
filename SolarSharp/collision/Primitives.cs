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
