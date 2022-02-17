using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
    public struct Ray
    {
        public Vector3 origin;
        public Vector3 direction;

        public override string ToString()
        {
            return string.Format("Ray, origin={0}, direction={1}", origin, direction);
        }
    }

    public struct Plane
    {
        public Vector3 origin;
        public Vector3 normal;
    }


}
