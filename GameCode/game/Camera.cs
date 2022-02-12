using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolarSharp;

namespace GameCode
{
    internal class Camera
    {
        private Vector3 position = new Vector3(7, 14, 7) * 2;
        private Quaternion orientation = Quaternion.Identity;

        private float far;
        private float near;
        private float yfov;

        internal Camera()
        {
            far = 100.0f;
            near = 0.1f;
            yfov = Util.DegToRad(45.0f);
            orientation = Matrix4.ToQuaternion(Matrix4.CreateLookAtRH(position, Vector3.Zero, Vector3.UnitY));
        }

        internal void Follow(Entity entity)
        {
            Vector3 offset = entity.Position + (new Vector3(7, 14, 7) * 2);

            position.x = Util.Lerp(position.x, offset.x, 0.27f);
            position.y = Util.Lerp(position.y, offset.y, 0.27f);
            position.z = Util.Lerp(position.z, offset.z, 0.27f);            
        }

        internal Matrix4 GetViewMatrix() { return Matrix4.TranslateRH(Quaternion.ToMatrix4(orientation), position).Inverse; }
        internal Matrix4 GetProjectionMatrix() { return Matrix4.CreatePerspectiveRH(yfov, Application.WindowAspect, near, far); }
    }
}
