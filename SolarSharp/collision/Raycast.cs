using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
    public struct RaycastInfo
    {
        public Vector3 point = Vector3.Zero;
        public Vector3 normal = Vector3.Zero;
        public float t = 0;
    };

    public class Raycast
    {

        private static bool FloatEqual(float a, float b, float ep = 0.00001f)
        {
            return MathF.Abs(a - b) < ep;
        }

        public static bool Plane(Ray ray, Plane plane, out RaycastInfo info)
        {
            info = new RaycastInfo();
            bool result = false;

            float demon = Vector3.Dot(ray.direction, plane.normal);

            if (!FloatEqual(demon, 0.0f))
            {
                Vector3 pr = plane.origin - ray.origin;

                float nume = Vector3.Dot(pr, plane.normal);

                float t = nume / demon;

                result = t >= 0.0f;

                info.t = t;
                info.normal = plane.normal;
                info.point = ray.origin + t * ray.direction;
            }

            return result;
        }

    }
}
