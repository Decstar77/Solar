using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolarSharp
{
    public struct Basis
    {
        public Vector3 forward;
        public Vector3 right;
        public Vector3 up;

        public Basis(Vector3 right, Vector3 up, Vector3 forward)
        {
            this.forward = forward;
            this.right = right;
            this.up = up;
        }
    }
}
