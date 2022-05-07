using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
    public struct RaycastInfo
    {
        public Vector3 point;
        public Vector3 normal;
        public float t;
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

        public static bool AlignedBox(Ray ray, AlignedBox box, out RaycastInfo info)
        {
            Vector3 tmin = (box.min - ray.origin) / ray.direction;
            Vector3 tmax = (box.max - ray.origin) / ray.direction;

            Vector3 sc = Vector3.Min(tmin, tmax);
            Vector3 sf = Vector3.Max(tmin, tmax);

            float t0 = MathF.Max(MathF.Max(sc.x, sc.y), sc.z);
            float t1 = MathF.Min(MathF.Min(sf.x, sf.y), sf.z);

            info = new RaycastInfo();

            if (t0 <= t1 && t1 > 0.0f)
            {
                info.t = t0;
                info.point = ray.Travel(t0);

                Vector3 a = Vector3.Abs(info.point - box.min);
                Vector3 b = Vector3.Abs(info.point - box.max);

                // @TODO: Make fast
                float min = a.x;
                info.normal = new Vector3(-1, 0, 0);
                if (a.y < min) { info.normal = new Vector3(0, -1, 0); min = a.y; }
                if (a.z < min) { info.normal = new Vector3(0, 0, -1); min = a.z; }
                if (b.x < min) { info.normal = new Vector3(1, 0, 0); min = b.x; }
                if (b.y < min) { info.normal = new Vector3(0, 1, 0); min = b.y; }
                if (b.z < min) { info.normal = new Vector3(0, 0, 1); min = b.z; }

                return true;
            }

            return false;
        }

        public static bool BoundingBox(Ray ray, BoundingBox box, out RaycastInfo info)
        {
            info = new RaycastInfo();
            Matrix3 rotation = Quaternion.ToMatrix3(box.orientation);

            Vector3 origin = ray.origin - box.origin;
            return false;

            //            Vector3 origin = mul(ray.origin - center, rotation);
            //            Vector3 dir = mul(ray.dir, rotation);

            //            float3 sgn = -sign(ray.dir);

            //            float3 distanceToPlane = radius * sgn - ray.origin;
            //            distanceToPlane /= ray.dir;

            //#   define TEST(U, VW)\
            //         /* Is there a hit on this axis in front of the origin? Use multiplication instead of && for a small speedup */\
            //         (distanceToPlane.U >= 0.0) && \
            //         /* Is that hit within the face of the box? */\
            //         all(LessThan(abs(ray.origin.VW + ray.dir.VW * distanceToPlane.U), radius.VW))


            //    bool3 test = bool3(TEST(x, yz), TEST(y, zx), TEST(z, xy));
            //#undef TEST

            //            sgn = test.x ? float3(sgn.x, 0.0, 0.0) : (test.y ? float3(0.0, sgn.y, 0.0) : float3(0.0, 0.0, test.z ? sgn.z : 0.0));

            //            dist = (sgn.x != 0.0) ? distanceToPlane.x : ((sgn.y != 0.0) ? distanceToPlane.y : distanceToPlane.z);

            //            return (sgn.x != 0) || (sgn.y != 0) || (sgn.z != 0);
            //            return false;
        }

    }
}
