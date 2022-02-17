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
        public bool Plane(Ray ray, Plane plane, out RaycastInfo info)
        {
            info = new RaycastInfo();

            return false;
        }

    }
}
