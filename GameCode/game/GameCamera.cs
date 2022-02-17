using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SolarSharp;

namespace GameCode
{
    internal class GameCamera : Camera
    {
        internal GameCamera()
        {
            position = new Vector3(7, 14, 7);
            orientation = Matrix4.ToQuaternion(Matrix4.CreateLookAtRH(position, Vector3.Zero, Vector3.UnitY));
        }

        internal void Follow(Entity entity)
        {
            if (Application.IsKeyDown('D'))
            {
                position.x += 0.1f;
            }
            if (Application.IsKeyDown('A'))
            {
                position.x -= 0.1f;
            }
            if (Application.IsKeyDown('W'))
            {
                position.z -= 0.1f;
            }
            if (Application.IsKeyDown('S'))
            {
                position.z += 0.1f;
            }
            //Vector3 offset = entity.Position + (new Vector3(7, 14, 7) * 2);

            //position.x = Util.Lerp(position.x, offset.x, 0.27f);
            //position.y = Util.Lerp(position.y, offset.y, 0.27f);
            //position.z = Util.Lerp(position.z, offset.z, 0.27f);            
        }
    }
}
